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
        public async Task<IntResult> Add(AddOrderDTO order, string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                return new IntResult() { Massage = "No User has this Id" };
            }

            var newOrder = new Order
            {
                UserId = userId,
            };

            await _context.orders.AddAsync(newOrder);
            using var transaction = await _context.Database.BeginTransactionAsync();
                try
            {
                await _context.SaveChangesAsync();

                foreach (var item in order.OrderItems)
                {
                    var result = await _orderItemRepository.AddOrderItemInSpacificOrder(new AddOrderItemDTO()
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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult() { Id = newOrder.Id };
        }
        public async Task<IntResult> Delete(int id, string userId)
        {
            var order = await _context.orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            var shop = order?.ShopId != null ? await _context.Shops.FindAsync(order.ShopId) : null;

            if (order is null || (order.UserId != userId && shop?.OwnerID != userId))
            {
                return new IntResult() { Massage = "No Order has this Id" };
            }

            if (order.IsRecieved || order.IsPayment)
            {
                return new IntResult() { Massage = "The Order Is already Reseaved Or Payment we can not delete" };
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in order.OrderItems.ToList())
                    {
                        var result = await _orderItemRepository.DeleteOrderItem(item.Id);
                        if (result.Id == 0)
                        {
                            return new IntResult() { Massage = result.Massage };
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
            }

            return new IntResult() { Id = 1 };
        }
        public async Task<ShowOrderDTO> GetOrder(int id, string userId)
        {
            var result = await _context.orders
                .Where(x => x.Id == id)
                .Select(o => new ShowOrderDTO
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
                }).FirstOrDefaultAsync();

            if (result is not null)
            {
                var shop = await _context.Shops.FindAsync(result.ShopId);
                if (result.UserId != userId && shop?.OwnerID != userId)
                {
                    return null;
                }
            }

            return result;
        }


        public async Task<List<ShowUserOrderDTO>> GetOrdersForUser(string userId)
        {
            return await _context.orders
                .Where(x => x.UserId == userId)
                .Select(o => new ShowUserOrderDTO()
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
                }).ToListAsync();
        }
        public async Task<List<ShowShopOrderDTO>> GetOrdersForShop(int shopId, string userId)
        {
            var shop = await _context.Shops.FindAsync(shopId);
            if (shop?.OwnerID != userId)
            {
                return null;
            }

            return await _context.orders
                .Where(x => x.ShopId == shopId)
                .Select(o => new ShowShopOrderDTO()
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
                }).ToListAsync();
        }

        public async Task<IntResult> MakeItReseved(int id, string userId)
        {
            var order = await _context.orders.Include(x => x.Shop).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null || order.Shop.OwnerID != userId)
            {
                return new IntResult() { Massage = "there is no Order has this Id" };
            }

            order.IsRecieved = true;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult() { Id = id };
        }

        public async Task<IntResult> MakeItPaymented(int id, string userId)
        {
            var order = await _context.orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null || order.UserId != userId)
            {
                return new IntResult() { Massage = "there is no Order has this Id" };
            }

            order.IsPayment = true;
            try
            {
                // payment code -> stripe code
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult() { Id = id };
        }
    }
}
