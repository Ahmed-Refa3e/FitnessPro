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
        public async Task<IntResult> AddOrderItemInOrderDidnotReseived(AddOrderItemInOrderDTO item, string userId)
        {
            var user = await _context.Users.Include(x => x.Orders).FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return new IntResult() { Massage = "No User has this Id" };

            var product = await _context.products.FindAsync(item.ProductId);
            if (product is null)
                return new IntResult() { Massage = "No Product has this Id" };

            var order = user.Orders.FirstOrDefault(x => !x.IsRecieved && x.ShopId == product.ShopId);
            if (order is null)
            {
                order = new Order { UserId = userId };
                await _context.orders.AddAsync(order);
            }

            var resultId = 0;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                var result = await AddOrderItemInSpacificOrder(new AddOrderItemDTO
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });

                if (result.Id == 0)
                    return new IntResult() { Massage = result.Massage };

                resultId = result.Id;
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult() { Id = resultId };
        }
        public async Task<IntResult> AddOrderItemInSpacificOrder(AddOrderItemDTO item)
        {
            var order = await _context.orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == item.OrderId);
            var product = await _context.products.FindAsync(item.ProductId);

            if (order is null)
                return new IntResult() { Massage = "No Order has this Id" };

            if (order.IsRecieved)
                return new IntResult() { Massage = "The Order has reseived, we could not change it" };

            if (product is null)
                return new IntResult { Massage = $"No product has this Id: {item.ProductId}" };

            if (!order.OrderItems.Any())
                order.ShopId = product.ShopId;
            else if (product.ShopId != order.ShopId)
            {
                var shopName = (await _context.Shops.FindAsync(order.ShopId))?.Name;
                return new IntResult()
                {
                    Massage = $"The products not from the same shop: the order asked from shop {shopName}, but this item {product.Name} is not from this shop"
                };
            }

            try
            {
                var result = await _productRepository.Decrease(item.ProductId, item.Quantity);
                if (!string.IsNullOrEmpty(result.Massage))
                    return new IntResult() { Massage = result.Massage };

                var newItem = new OrderItem
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = (decimal)result.price
                };

                await _context.ordersItems.AddAsync(newItem);
                order.TotalPrice += newItem.Price;
                await _context.SaveChangesAsync();

                return new IntResult { Id = newItem.Id };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }
        public async Task<IntResult> AddOrderItemInSpacificOrderWithCheckUserID(AddOrderItemDTO item, string userId)
        {
            var order = await _context.orders.FindAsync(item.OrderId);
            if (order?.UserId != userId)
                return new IntResult { Massage = "No Order has this Id." };

            return await AddOrderItemInSpacificOrder(item);
        }
        public async Task<IntResult> DeleteOrderItemWithUserId(int id, string userId)
        {
            var item = await GetWithOrder(id);
            if (item == null || item.Order.UserId != userId)
                return new IntResult() { Massage = "No Order Item has this Id" };

            return await DeleteItem(item);
        }
        public async Task<IntResult> DeleteOrderItem(int id)
        {
            var item = await GetWithOrder(id);
            if (item == null)
                return new IntResult() { Massage = "No Order Item has this Id" };

            return await DeleteItem(item);
        }
        private async Task<IntResult> DeleteItem(OrderItem item)
        {
            if (item.Order.IsRecieved)
                return new IntResult() { Massage = "The Order is already received. We cannot delete this item." };

            _context.ordersItems.Remove(item);
            try
            {
                var result = await _productRepository.Decrease((int)item.ProductId, -1 * item.Quantity);
                if (!string.IsNullOrEmpty(result.Massage) && result.Massage != "No Product has this Id")
                    return new IntResult() { Massage = result.Massage };

                var order = item.Order;
                if (order is not null)
                {
                    order.TotalPrice -= item.Price;
                    if (!order.OrderItems.Any())
                        _context.orders.Remove(order);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult() { Id = 1 };
        }
        public async Task<ShowOrderItemDTO> GetById(int id, string userId)
        {
            var item = await _context.ordersItems
                .Include(x => x.Product)
                .Include(x => x.Order).ThenInclude(x => x.Shop)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null || (item.Order.UserId != userId && item.Order.Shop?.OwnerID != userId))
                return null;

            return new ShowOrderItemDTO()
            {
                Id = item.Id,
                Price = item.Price,
                ProductName = item.Product.Name,
                Quantity = item.Quantity
            };
        }
        public async Task<IntResult> UpdateOrderItem(EditOrderItemDTO item, string userId)
        {
            var oldItem = await GetWithOrder(item.Id);
            if (oldItem == null || oldItem.Order.UserId != userId)
                return new IntResult { Massage = "No Order Item has this Id" };

            if (oldItem.Order.IsRecieved)
                return new IntResult { Massage = "The Order is already received" };

            if (item.Quantity == 0)
                return await DeleteItem(oldItem);

            var changedQuantity = item.Quantity - oldItem.Quantity;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await _productRepository.Decrease((int)oldItem.ProductId, changedQuantity);
                if (!string.IsNullOrEmpty(result.Massage))
                    return new IntResult() { Massage = result.Massage };

                oldItem.Quantity += changedQuantity;
                oldItem.Price += (decimal)result.price;
                oldItem.Order.TotalPrice += (decimal)result.price;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = oldItem.Id };
        }
        private async Task<OrderItem> GetWithOrder(int id)
        {
            return await _context.ordersItems
                .Include(x => x.Order).ThenInclude(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
