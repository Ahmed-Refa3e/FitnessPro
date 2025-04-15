using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.OrderDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly FitnessContext _context;
        public OrderRepository(IOrderItemRepository orderItemRepository, FitnessContext context)
        {
            _orderItemRepository = orderItemRepository;
            _context = context;
        }
        public IntResult Add(AddOrderDTO order)
        {
            if (_context.Users.Find(order.UserId) is null)
            {
                return new IntResult() { Massage = "No User has this Id" };
            }
            var newOrder = new Order
            {
                UserId = order.UserId,
            };
            _context.orders.Add(newOrder);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    foreach (var item in order.OrderItems)
                    {
                        var result = _orderItemRepository.AddOrderItemInSpacificOrder(new AddOrderItemDTO()
                        {
                            OrderId = newOrder.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                        if (result.Id == 0)
                        {
                            return new IntResult { Massage = result.Massage };
                        }
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
            }
            return new IntResult() { Id = newOrder.Id };
        }
        public IntResult Delete(int id)
        {
            var order = _context.orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            if (order is null)
            {
                return new IntResult() { Massage = "No Order has this Id" };
            }
            if (order.IsRecieved)
            {
                return new IntResult() { Massage = "The Order Is already Reseaved we can not delete" };
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in order.OrderItems.ToList())
                    {
                        var result = _orderItemRepository.DeleteOrderItem(item.Id);
                        if (result.Id == 0)
                        {
                            return new IntResult() { Massage = result.Massage };
                        }
                    }
                    _context.orders.Remove(Get(id));
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
            }
            return new IntResult() { Id = 1 };
        }
        public ShowOrderDTO GetOrder(int id)
        {
            var result = _context.orders.Where(x => x.Id == id).Select(o => new ShowOrderDTO
            {
                Id = o.Id,
                IsRecieved = o.IsRecieved,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                UserName = o.User != null ? o.User.FirstName + " " + o.User.LastName : "UnKnown",
                UserId = o.UserId,
                ShopId = o.ShopId ?? 0,
                ShopName = (o.Shop == null) ? "Deleted Shop" : o.Shop.Name,
                OrderItems = o.OrderItems.Select(x => new ShowOrderItemDTO()
                {
                    Id = x.Id,
                    Price = x.Price,
                    ProductName = x.Product != null ? x.Product.Name : "Unknown",
                    Quantity = x.Quantity
                }).ToList()
            }).FirstOrDefault();
            return result;
        }

        public List<ShowUserOrderDTO> GetOrdersForUser(string userId)
        {
            return _context.orders.Where(x => x.UserId == userId).Select(o => new ShowUserOrderDTO()
            {
                Id = o.Id,
                IsRecieved = o.IsRecieved,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                ShopId = o.ShopId ?? 0,
                ShopName = (o.Shop == null) ? "Deleted Shop" : o.Shop.Name,
                IsPayment = o.IsPayment,
                OrderItems = o.OrderItems.Select(x => new ShowOrderItemDTO()
                {
                    Id = x.Id,
                    Price = x.Price,
                    ProductName = x.Product != null ? x.Product.Name : "Unknown",
                    Quantity = x.Quantity
                }).ToList()
            }).ToList();
        }
        public List<ShowShopOrderDTO> GetOrdersForShop(int shopId)
        {
            return _context.orders.Where(x => x.ShopId == shopId).Select(o => new ShowShopOrderDTO()
            {
                Id = o.Id,
                IsRecieved = o.IsRecieved,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                UserId = o.UserId ?? "",
                UserName = o.User != null ? o.User.FirstName + " " + o.User.LastName : "UnKnown",
                IsPayment = o.IsPayment,
                OrderItems = o.OrderItems.Select(x => new ShowOrderItemDTO()
                {
                    Id = x.Id,
                    Price = x.Price,
                    ProductName = x.Product != null ? x.Product.Name : "Unknown",
                    Quantity = x.Quantity
                }).ToList()
            }).ToList();
        }
        public IntResult MakeItReseved(int id)
        {
            var order = Get(id);
            if (order == null)
            {
                return new IntResult() { Massage = "there is no Order has this Id" };
            }
            order.IsRecieved = true;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult() { Id = id };
        }
        Order Get(int id) => _context.orders.Find(id);
    }
}
