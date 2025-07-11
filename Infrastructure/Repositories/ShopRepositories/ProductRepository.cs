﻿using Core.DTOs.GeneralDTO;
using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.ShopRepositories;
using Core.Interfaces.Services;
using Core.Utilities;
using FuzzySharp;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.ShopRepositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly FitnessContext _context;
        private readonly IBlobService _blobService;
        public ProductRepository(FitnessContext context, ICategoryRepository categoryRepository, IBlobService blobService)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _blobService = blobService;
        }
        public async Task<IntResult> Add(AddProductDTO product, string userId)
        {
            var shop = await _context.Shops.FindAsync(product.ShopId);
            if (shop is null || shop.OwnerID != userId)
            {
                return new IntResult { Massage = "Id of shop not valid" };
            }
            if (product.OfferPrice is null || product.OfferPrice > product.Price)
            {
                product.OfferPrice = product.Price;
            }
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ShopId = product.ShopId,
                OfferPrice = (decimal)product.OfferPrice,
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
                if (product.Image is not null)
                {
                    var result = AddImageHelper.CheckImage(product.Image);
                    if (result.Id == 0)
                    {
                        return result;
                    }
                    newProduct.ImagePath = await _blobService.UploadImageAsync(product.Image);
                    await _context.SaveChangesAsync();
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

            try
            {
                await _context.SaveChangesAsync();
                await _blobService.DeleteImageAsync(product.ImagePath);
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
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
            query = query.Where(x => x.Price >= searchDTO.MinimumPrice);
        }

        if (searchDTO.MaximumPrice is not null && searchDTO.MaximumPrice > 0)
        {
            query = query.Where(x => x.Price <= searchDTO.MaximumPrice);
        }

        var products = await query.ToListAsync();

        var productWithSimilarity = products
            .Select(p => new
            {
                Product = p,
                Similarity = !string.IsNullOrWhiteSpace(searchDTO.Name)
                    ? Fuzz.PartialRatio(p.Name, searchDTO.Name)
                    : 0
            })
            .Where(p => string.IsNullOrWhiteSpace(searchDTO.Name) || p.Similarity >= 60)
            .ToList();

        var ordered = productWithSimilarity.OrderByDescending(p => p.Similarity);

        if (searchDTO.SearchByBiggetDiscount)
        {
            if (searchDTO.SearchByPriceAscending)
            {
                ordered = ordered
                    .ThenByDescending(p => 100 - (p.Product.OfferPrice / p.Product.Price) * 100)
                    .ThenBy(p => p.Product.OfferPrice);
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                ordered = ordered
                    .ThenByDescending(p => 100 - (p.Product.OfferPrice / p.Product.Price) * 100)
                    .ThenByDescending(p => p.Product.OfferPrice);
            }
            else
            {
                ordered = ordered
                    .ThenByDescending(p => 100 - (p.Product.OfferPrice / p.Product.Price) * 100);
            }
        }
        else
        {
            if (searchDTO.SearchByPriceAscending)
            {
                ordered = ordered
                    .ThenBy(p => p.Product.OfferPrice)
                    .ThenByDescending(p => 100 - (p.Product.OfferPrice / p.Product.Price) * 100);
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                ordered = ordered
                    .ThenByDescending(p => p.Product.OfferPrice)
                    .ThenByDescending(p => 100 - (p.Product.OfferPrice / p.Product.Price) * 100);
            }
        }

        var finalProducts = ordered.Select(p => p.Product).ToList();

        (searchDTO.PageNumber, searchDTO.PageSize) =await PaginationHelper.NormalizePaginationWithCountAsync(
            finalProducts.Count(),
            searchDTO.PageNumber,
            searchDTO.PageSize
        );

        return finalProducts
            .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
            .Take(searchDTO.PageSize)
            .Select(x => new ShowProductDTO
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath,
                OfferPrice = x.OfferPrice,
                Discount = 100 - (x.OfferPrice / x.Price) * 100
            })
            .ToList();
    }

    /*public async Task<List<ShowProductDTO>> GetProducts(ProductSearchDTO searchDTO)
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
            query = query.Where(x => x.Price >= searchDTO.MinimumPrice);
        }

        if (searchDTO.MaximumPrice is not null && searchDTO.MaximumPrice > 0)
        {
            query = query.Where(x => x.Price <= searchDTO.MaximumPrice);
        }

        var products = await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchDTO.Name))
        {
            products = products
                .Select(p => new
                {
                    Product = p,
                    Similarity = Fuzz.PartialRatio(p.Name, searchDTO.Name)
                })
                .Where(p => p.Similarity >= 60) 
                .OrderByDescending(p => p.Similarity)
                .Select(p => p.Product)
                .ToList();
        }

        if (searchDTO.SearchByBiggetDiscount)
        {
            if (searchDTO.SearchByPriceAscending)
            {
                products = products
                    .OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100)
                    .ThenBy(x => x.OfferPrice)
                    .ToList();
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                products = products
                    .OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100)
                    .ThenByDescending(x => x.OfferPrice)
                    .ToList();
            }
            else
            {
                products = products
                    .OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100)
                    .ToList();
            }
        }
        else if (!searchDTO.SearchByPriceAscending && !searchDTO.SearchByPriceDescending && !searchDTO.SearchByBiggetDiscount && !string.IsNullOrWhiteSpace(searchDTO.Name))
        {
        }
        else
        {
            if (searchDTO.SearchByPriceAscending)
            {
                products = products
                    .OrderBy(x => x.OfferPrice)
                    .ThenByDescending(x => 100 - (x.OfferPrice / x.Price) * 100)
                    .ToList();
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                products = products
                    .OrderByDescending(x => x.OfferPrice)
                    .ThenByDescending(x => 100 - (x.OfferPrice / x.Price) * 100)
                    .ToList();
            }
        }

        (searchDTO.PageNumber, searchDTO.PageSize) =await PaginationHelper.NormalizePaginationWithCountAsync(
            products.Count,
            searchDTO.PageNumber,
            searchDTO.PageSize
        );

        return products
            .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
            .Take(searchDTO.PageSize)
            .Select(x => new ShowProductDTO
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                Price = x.Price,
                ImagePath = x.ImagePath,
                OfferPrice = x.OfferPrice,
                Discount = 100 - (x.OfferPrice / x.Price) * 100
            })
            .ToList();
    }*/
    /*public async Task<List<ShowProductDTO>> GetProducts(ProductSearchDTO searchDTO)
    {
        var query = _context.products.AsQueryable();

        if(!string.IsNullOrEmpty(searchDTO.Name))
        {
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{searchDTO.Name}%"));
        }

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
            query = query.Where(x => x.Price >= searchDTO.MinimumPrice);
        }

        if (searchDTO.MaximumPrice is not null && searchDTO.MaximumPrice > 0)
        {
            query = query.Where(x => x.Price <= searchDTO.MaximumPrice);
        }
        if (searchDTO.SearchByBiggetDiscount)
        {
            if (searchDTO.SearchByPriceAscending)
            {
                query = query.OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100).ThenBy(x => x.OfferPrice);
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                query = query.OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100).ThenByDescending(x => x.OfferPrice);
            }
            else
            {
                query = query.OrderByDescending(x => 100 - (x.OfferPrice / x.Price) * 100);
            }
        }
        else
        {
            if (searchDTO.SearchByPriceAscending)
            {
                query = query.OrderBy(x => x.OfferPrice).ThenByDescending(x => 100 - (x.OfferPrice / x.Price) * 100);
            }
            else if (searchDTO.SearchByPriceDescending)
            {
                query = query.OrderByDescending(x => x.OfferPrice).ThenByDescending(x => 100 - (x.OfferPrice / x.Price) * 100);
            }
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
                ImagePath = x.ImagePath,
                OfferPrice = x.OfferPrice,
                Discount = 100 - (x.OfferPrice / x.Price) * 100
            })
            .ToListAsync();
    }*/
    public async Task<IntResult> Update(EditProductDTO product, int id, string userId)
        {
            var productDB = await _context.products
                .Include(x => x.Shop)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (productDB is null || productDB.Shop.OwnerID != userId)
            {
                return new IntResult() { Massage = "No product has this Id" };
            }
            if (product.OfferPrice is null || product.OfferPrice > product.Price)
            {
                product.OfferPrice = product.Price;
            }
            productDB.Price = product.Price;
            productDB.Description = product.Description;
            productDB.Name = product.Name;
            productDB.Quantity = product.Quantity;
            productDB.OfferPrice = (decimal)product.OfferPrice;

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
        public async Task<IntResult> UpdateImage(UpdateImageDTO imageDTO, int productId, string userId)
        {
            var productDB = await _context.products
                            .Include(x => x.Shop)
                            .FirstOrDefaultAsync(x => x.Id == productId);
            if (productDB is null || productDB.Shop.OwnerID != userId)
            {
                return new IntResult() { Massage = "No product has this Id" };
            }
            var oldPath = productDB.ImagePath;
            if (imageDTO.Image is null)
            {
                productDB.ImagePath = null;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
                await _blobService.DeleteImageAsync(oldPath);
            }
            else
            {
                var result = AddImageHelper.CheckImage(imageDTO.Image);
                if (result.Id == 0)
                {
                    return result;
                }
                productDB.ImagePath = await _blobService.UploadImageAsync(imageDTO.Image);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await _blobService.DeleteImageAsync(productDB.ImagePath);
                    return new IntResult { Massage = ex.Message };
                }
                await _blobService.DeleteImageAsync(oldPath);
            }
            return new IntResult { Id = productId };
        }

    }
}
