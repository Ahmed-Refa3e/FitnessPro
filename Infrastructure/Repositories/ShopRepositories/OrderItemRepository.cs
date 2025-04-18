using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly FitnessContext _context;
        private readonly IProductRepository _productRepository;
        public OrderItemRepository(FitnessContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;

        }
        public IntResult AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId)
        {
            var user = _context.Users.Include(x => x.Orders).FirstOrDefault(x => x.Id == userId);
            if (user is null)
            {
                return new IntResult() { Massage = "No User has this Id" };
            }
            var product = _context.products.Find(item.ProductId);
            if (product is null)
            {
                return new IntResult() { Massage = "No Product has this Id" };
            }
            var order = user.Orders.Where(x => !x.IsRecieved && x.ShopId == product.ShopId).FirstOrDefault();
            if (order is null)
            {
                order = new Order { UserId = userId };
                _context.orders.Add(order);
            }
            var resultId = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    var result = AddOrderItemInSpacificOrder(new AddOrderItemDTO()
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                    if (result.Id == 0)
                    {
                        return new IntResult() { Massage = result.Massage };
                    }
                    resultId = result.Id;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return new IntResult() { Massage = ex.Message };
                }
            }
            return new IntResult() { Id = resultId };
        }
        public IntResult AddOrderItemInSpacificOrder(AddOrderItemDTO item)
        {
            var newItem = new OrderItem
            {
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
            var order = _context.orders.Include(x => x.OrderItems).FirstOrDefault(x => x.Id == item.OrderId);
            var product = _context.products.Find(item.ProductId);
            if (order is null)
            {
                return new IntResult() { Massage = "No Order has this Id" };
            }
            if (order.IsRecieved)
            {
                return new IntResult() { Massage = "The Order has reseived ,we could not change it" };
            }
            if (product is null)
            {
                return new IntResult { Massage = "No product has this Id : " + item.ProductId };
            }
            if (!order.OrderItems.Any())
            {
                order.ShopId = product.ShopId;
            }
            else if (product.ShopId != order.ShopId)
            {
                return new IntResult()
                {
                    Massage = "The products not from the same shop : the order asked from shop " +
                    _context.Shops.Find(order.ShopId)?.Name + " put this item " + product.Name + " did not from this shop"
                };
            }
            try
            {
                var result = _productRepository.Decrease(item.ProductId, item.Quantity);
                if (!string.IsNullOrEmpty(result.Massage))
                {
                    return new IntResult() { Massage = result.Massage };
                }
                newItem.Price = (decimal)result.price;
                _context.ordersItems.Add(newItem);
                order.TotalPrice += newItem.Price;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = newItem.Id };
        }
        public IntResult AddOrderItemInSpacificOrderWithCheckUserID(AddOrderItemDTO item, string userId)
        {
            if (_context.orders.Find(item.OrderId).UserId != userId)
            {
                return new IntResult { Massage = "No Order has this Id." };
            }
            return AddOrderItemInSpacificOrder(item);
        }
        public IntResult DeleteOrderItemWithUserId(int id, string userId)
        {
            var item = GetWithOrder(id);
            if (item == null || item.Order.UserId != userId)
            {
                return new IntResult() { Massage = "No Order Item has this Id" };
            }
            return DeleteItem(item);
        }
        public IntResult DeleteOrderItem(int id)
        {
            var item = GetWithOrder(id);
            if (item == null)
            {
                return new IntResult() { Massage = "No Order Item has this Id" };
            }
            return DeleteItem(item);
        }
        IntResult DeleteItem(OrderItem item)
        {
            if (item.Order.IsRecieved)
            {
                return new IntResult() { Massage = "The Order Is already Reseaved we can not delete this item" };
            }
            _context.ordersItems.Remove(item);
            try
            {
                var result = _productRepository.Decrease((int)item.ProductId, -1 * item.Quantity);
                if (!string.IsNullOrEmpty(result.Massage) && result.Massage != "No Product has this Id")
                {
                    return new IntResult() { Massage = result.Massage };
                }
                var order = item.Order;
                if (order is not null)
                {
                    order.TotalPrice -= item.Price;
                    if (order.OrderItems.IsNullOrEmpty())
                    {
                        _context.orders.Remove(order);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult() { Id = 1 };
        }
        public ShowOrderItemDTO GetById(int id, string userId)
        {
            var item = _context.ordersItems.Include(x=>x.Product).Include(x => x.Order).ThenInclude(x=>x.Shop).FirstOrDefault(x => x.Id == id);
            if (item == null || (item.Order.UserId != userId && item.Order.Shop?.OwnerID != userId))
            {
                return null;
            }
            return new ShowOrderItemDTO()
            {
                Id = item.Id,
                Price = item.Price,
                ProductName = item.Product.Name,
                Quantity = item.Quantity
            };
        }
        public decimal GetPrice(int id)
        {
            return Get(id).Price;
        }
        public IntResult UpdateOrderItem(EditOrderItemDTO item, string userId)
        {
            var oldItem = GetWithOrder(item.Id);
            if (oldItem == null||oldItem.Order.UserId!=userId)
            {
                return new IntResult { Massage = "No Order Item has this Id" };
            }
            if (oldItem.Order.IsRecieved)
            {
                return new IntResult { Massage = "The Order is already Reseaved" };
            }
            if (item.Quantity == 0)
            {
                return DeleteItem(oldItem);
            }
            var changedQuantity = item.Quantity - oldItem.Quantity;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = _productRepository.Decrease((int)oldItem.ProductId, changedQuantity);
                    if (!string.IsNullOrEmpty(result.Massage))
                    {
                        return new IntResult() { Massage = result.Massage };
                    }
                    oldItem.Quantity += changedQuantity;
                    oldItem.Price += (decimal)result.price;                    
                    oldItem.Order.TotalPrice += (decimal)result.price;
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
            }
            return new IntResult { Id = oldItem.Id };
        }
        OrderItem Get(int id) => _context.ordersItems.Find(id);
        OrderItem GetWithOrder(int id) => _context.ordersItems.Include(x => x.Order).ThenInclude(x=>x.OrderItems).FirstOrDefault(x => x.Id == id);
    }
}
