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
        public IntResult Add(AddOrderDTO order,string userId)
        {
            if (_context.Users.Find(userId) is null)
            {
                return new IntResult() { Massage = "No User has this Id" };
            }
            var newOrder = new Order
            {
                UserId = userId,
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
        public IntResult Delete(int id,string userId)
        {
            var order = _context.orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == id);
            if (order is null||(order.UserId!=userId&&_context.Shops.Find(order.ShopId)?.OwnerID!=userId))
            {
                return new IntResult() { Massage = "No Order has this Id" };
            }
            if (order.IsRecieved||order.IsPayment)
            {
                return new IntResult() { Massage = "The Order Is already Reseaved Or Payment we can not delete" };
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
        public ShowOrderDTO GetOrder(int id,string userId)
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
            if(result is not null)
            {
                if (result.UserId != userId && _context.Shops.Find(result.ShopId).OwnerID != userId)
                {
                    return null;
                }
            }
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
        public List<ShowShopOrderDTO> GetOrdersForShop(int shopId, string userId)
        {
            if (_context.Shops.Find(shopId).OwnerID != userId)
            {
                return null;
            }
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
        public IntResult MakeItReseved(int id,string userId)
        {
            var order = _context.orders.Include(x => x.Shop).FirstOrDefault(x => x.Id == id);
            if (order == null|| order.Shop.OwnerID != userId)
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
        public IntResult MakeItPaymented(int id, string userId)
        {
            var order = _context.orders.FirstOrDefault(x => x.Id == id);
            if (order == null || order.UserId != userId)
            {
                return new IntResult() { Massage = "there is no Order has this Id" };
            }
            order.IsPayment = true;
            try
            {
                //payment code -> stripe code
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult() { Id = id };
        }
    }
}
