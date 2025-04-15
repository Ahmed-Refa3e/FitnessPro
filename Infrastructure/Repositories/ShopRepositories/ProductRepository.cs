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
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProductImages");
        private readonly ICategoryRepository _categoryRepository;
        private readonly FitnessContext _context;
        public ProductRepository(FitnessContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }
        public async Task<IntResult> Add(AddProductDTO product)
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            var shop = _context.Shops.Find(product.ShopId);
            if (shop is null)
            {
                return new IntResult() { Massage = "Id of shop not valid" };
            }
            var filePath = AddImageHelper.chickImagePath(product.Image, _storagePath);
            if (!string.IsNullOrEmpty(filePath.Massage))
            {
                return new IntResult() { Massage = filePath.Massage };
            }
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImagePath = filePath.Id,
                Quantity = product.Quantity,
                ShopId = product.ShopId,
                Shop = shop
            };
            _context.products.Add(newProduct);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    foreach (var name in product.CategoriesName)
                    {
                        var category = _categoryRepository.CheckIfItexistingAndGet(name);
                        newProduct.Categories.Add(category);
                    }
                    await _context.SaveChangesAsync();
                    using (var stream = new FileStream(filePath.Id, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    return new IntResult() { Massage = ex.Message };
                }
                await transaction.CommitAsync();
            }
            return new IntResult() { Id = newProduct.Id };
        }

        public ItemPriceDTO Decrease(int productId, int quantityNeeded)
        {
            var product = GetProduct(productId);
            if (product == null)
            {
                return new ItemPriceDTO() { Massage = "No Product has this Id" };
            }
            if (product.Quantity - quantityNeeded < 0)
            {
                return new ItemPriceDTO() { Massage = "There just " + product.Quantity + " of " + product.Name };
            }
            product.Quantity -= quantityNeeded;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ItemPriceDTO() { Massage = ex.Message };
            }
            return new ItemPriceDTO() { price = product.Price * quantityNeeded };
        }

        public IntResult Delete(int productId)
        {
            var product = GetProduct(productId);
            if (product is null)
            {
                return new IntResult() { Massage = "No product with this id" };
            }
            _context.products.Remove(product);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    if (File.Exists(product.ImagePath))
                    {
                        File.Delete(product.ImagePath);
                    }
                    transaction.Commit();
                }
                catch (Exception ex) { return new IntResult() { Massage = ex.Message }; }
            }
            return new IntResult() { Id = 1 };
        }
        public List<ShowProductDTO> GetProductsInPaginationsOnShopNoCategory(int shopId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfProduct = _context.products.Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfProduct / pageSize);
            page = Math.Min(page, numOfPage);
            var products = _context.products.Where(x => x.ShopId == shopId).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).Select(x => new ShowProductDTO()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath
            }).ToList();
            return products;
        }
        public List<ShowProductDTO> GetProductsInPaginationsOnShopWithCategory(int shopId, int page, int pageSize, int categoryId)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfProduct = _context.products.Count(x => x.Categories.Any(x => x.Id == categoryId));
            var numOfPage = (int)Math.Ceiling((decimal)numOfProduct / pageSize);
            page = Math.Min(page, numOfPage);
            var products = _context.products.Where(x => x.ShopId == shopId && x.Categories.Any(x => x.Id == categoryId)).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).Select(x => new ShowProductDTO()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath
            }).ToList().ToList();
            return products;
        }
        public ShowOneProductDTO GetProductById(int productId)
        {
            var product = _context.products.Where(x => x.Id == productId).Select(x => new ShowOneProductDTO()
            {
                Id = x.Id,
                ImagePath = x.ImagePath,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                CategoriesName = x.Categories.Select(x => x.Name).ToList(),
                ShopName = x.Shop.Name,
                ShopImage = x.Shop.PictureUrl
            }).FirstOrDefault();
            return product;
        }

        public List<ShowProductDTO> GetProductsInPaginationsNoCategory(int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfProduct = _context.products.Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfProduct / pageSize);
            page = Math.Min(page, numOfPage);
            var products = _context.products.Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).Select(x => new ShowProductDTO()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath
            }).ToList();
            return products;
        }

        public List<ShowProductDTO> GetProductsInPaginationsWithCategory(int page, int pageSize, int categoryId)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfProduct = _context.products.Count(x => x.Categories.Any(x => x.Id == categoryId));
            var numOfPage = (int)Math.Ceiling((decimal)numOfProduct / pageSize);
            page = Math.Min(page, numOfPage);
            var products = _context.products.Where(x => x.Categories.Any(x => x.Id == categoryId)).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).Select(x => new ShowProductDTO()
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath
            }).ToList().ToList();
            return products;
        }

        public async Task<IntResult> Update(EditProductDTO product, int id)
        {
            var filePath = AddImageHelper.chickImagePath(product.Image, _storagePath);
            if (!string.IsNullOrEmpty(filePath.Massage))
            {
                return new IntResult() { Massage = filePath.Massage };
            }
            var productDB = GetProduct(id);
            var oldPath = productDB.ImagePath;
            productDB.Price = product.Price;
            productDB.Description = product.Description;
            productDB.Name = product.Name;
            productDB.Quantity = product.Quantity;
            try
            {
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(filePath.Id))
                {
                    productDB.ImagePath = filePath.Id;
                    using (var stream = new FileStream(filePath.Id, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult() { Massage = ex.Message };
            }
            return new IntResult() { Id = productDB.Id };
        }
        public int ShowProductShopId(int productId)
        {
            var product = GetProduct(productId);
            if (product is null)
            {
                return 0;
            }
            return product.ShopId;
        }
        public IntResult UpdateCategoriesOfProduct(ModifyCategoriesInProductDTO modifyCategories)
        {
            var product = _context.products.Include(x => x.Categories).Where(x => x.Id == modifyCategories.ProductId).FirstOrDefault();
            if (product is null)
            {
                return new IntResult { Massage = "No product has this Id." };
            }
            product.Categories.RemoveAll(x => true);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (string categoryName in modifyCategories.CategoriesName)
                    {
                        Category category = _categoryRepository.CheckIfItexistingAndGet(categoryName);
                        if (category is not null)
                        {
                            product.Categories.Add(category);
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
            return new IntResult { Id = product.Id };
        }
        Product GetProduct(int id) => _context.products.Find(id);
    }
}
