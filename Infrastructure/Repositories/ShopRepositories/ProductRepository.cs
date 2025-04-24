using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly FitnessContext _context;
        public ProductRepository(FitnessContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }
        public async Task<IntResult> Add(AddProductDTO product, string userId)
        {
            var shop = await _context.Shops.FindAsync(product.ShopId);
            if (shop is null || shop.OwnerID != userId)
            {
                return new IntResult { Massage = "Id of shop not valid" };
            }

            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImagePath = product.ImageUrl,
                Quantity = product.Quantity,
                ShopId = product.ShopId,
                Shop = shop
            };

            await _context.products.AddAsync(newProduct);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

                foreach (var name in product.CategoriesName)
                {
                    var category = await _categoryRepository.CheckIfItexistingAndGet(name);
                    newProduct.Categories.Add(category);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new IntResult { Id = newProduct.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new IntResult { Massage = ex.Message };
            }
        }

        public async Task<ItemPriceDTO> Decrease(int productId, int quantityNeeded)
        {
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return new ItemPriceDTO { Massage = "No Product has this Id" };
            }

            if (product.Quantity - quantityNeeded < 0)
            {
                return new ItemPriceDTO { Massage = $"There just {product.Quantity} of {product.Name}" };
            }

            product.Quantity -= quantityNeeded;

            try
            {
                await _context.SaveChangesAsync();
                return new ItemPriceDTO { price = product.Price * quantityNeeded };
            }
            catch (Exception ex)
            {
                return new ItemPriceDTO { Massage = ex.Message };
            }
        }

        public async Task<IntResult> Delete(int productId, string userId)
        {
            var product = await _context.products
                .Include(x => x.Shop)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product is null || product.Shop.OwnerID != userId)
            {
                return new IntResult { Massage = "No product with this id" };
            }

            _context.products.Remove(product);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new IntResult { Massage = ex.Message };
            }
        }
        public async Task<ShowOneProductDTO> GetProductById(int productId)
        {
            var product = await _context.products
                .Where(x => x.Id == productId)
                .Select(x => new ShowOneProductDTO
                {
                    Id = x.Id,
                    ImagePath = x.ImagePath,
                    Description = x.Description,
                    Name = x.Name,
                    Price = x.Price,
                    CategoriesName = x.Categories.Select(c => c.Name).ToList(),
                    ShopName = x.Shop.Name,
                    ShopImage = x.Shop.PictureUrl
                })
                .FirstOrDefaultAsync();

            return product;
        }
        public async Task<List<ShowProductDTO>> GetProducts(ProductSearchDTO searchDTO)
        {
            var query = _context.products.AsQueryable();

            if (searchDTO.CategoryID is not null && searchDTO.CategoryID != 0)
            {
                query = query.Where(x => x.Categories.Any(c => c.Id == searchDTO.CategoryID));
            }

            if (searchDTO.ShopID is not null && searchDTO.ShopID != 0)
            {
                query = query.Where(x => x.ShopId == searchDTO.ShopID);
            }

            if (searchDTO.MinimumPrice is not null && searchDTO.MinimumPrice > 0)
            {
                query = query.Where(x => x.Price>= searchDTO.MinimumPrice);
            }

            if (searchDTO.MaximumPrice is not null && searchDTO.MaximumPrice > 0)
            {
                query = query.Where(x => x.Price <= searchDTO.MaximumPrice);
            }
            (searchDTO.PageNumber, searchDTO.PageSize) = await PaginationHelper.NormalizePaginationAsync(
                query,
                searchDTO.PageNumber,
                searchDTO.PageSize
            );

            return await query
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(x => new ShowProductDTO
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    Price = x.Price,
                    ImagePath = x.ImagePath
                })
                .ToListAsync();
        }
        public async Task<IntResult> Update(EditProductDTO product, int id, string userId)
        {
            var productDB = await _context.products
                .Include(x => x.Shop)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (productDB is null || productDB.Shop.OwnerID != userId)
            {
                return new IntResult() { Massage = "No product has this Id" };
            }

            productDB.Price = product.Price;
            productDB.Description = product.Description;
            productDB.Name = product.Name;
            productDB.Quantity = product.Quantity;
            productDB.ImagePath = product.ImageUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }

            return new IntResult() { Id = productDB.Id };
        }

        public async Task<IntResult> UpdateCategoriesOfProduct(ModifyCategoriesInProductDTO modifyCategories, string userId)
        {
            var product = await _context.products
                .Include(x => x.Categories)
                .Include(x => x.Shop)
                .FirstOrDefaultAsync(x => x.Id == modifyCategories.ProductId);

            if (product is null || product.Shop.OwnerID != userId)
            {
                return new IntResult { Massage = "No product has this Id." };
            }

            product.Categories.Clear();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (string categoryName in modifyCategories.CategoriesName)
                {
                    var category = await _categoryRepository.CheckIfItexistingAndGet(categoryName);
                    if (category is not null)
                    {
                        product.Categories.Add(category);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult { Id = product.Id };
        }

    }
}
