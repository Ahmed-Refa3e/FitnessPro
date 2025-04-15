using Core.DTOs.ShopDTO;
using Core.DTOs.ShopDTO.ProductDTO;
using Core.Interfaces.Repositories.ShopRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shop
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpGet("{id:int}")]
        public ActionResult GetById(int id)
        {
            var result = _productRepository.GetProductById(id);
            if (result is null)
            {
                return BadRequest("No product has this Id.");
            }
            return Ok(result);
        }
        [HttpGet("ProductsNoCategory")]
        public ActionResult GetProductsNoCategory([FromQuery] int page, [FromQuery] int size)
        {
            var result = _productRepository.GetProductsInPaginationsNoCategory(page, size);
            return Ok(result);
        }
        [HttpGet("ProductsInCategory")]
        public ActionResult GetProductsInCategory([FromQuery] int page, [FromQuery] int size, [FromQuery] int category)
        {
            var result = _productRepository.GetProductsInPaginationsWithCategory(page, size, category);
            return Ok(result);
        }
        [HttpGet("ShopProductsNoCategory")]
        public ActionResult GetShopProductsNoCategory([FromQuery] int shopId, [FromQuery] int page, [FromQuery] int size)
        {
            var result = _productRepository.GetProductsInPaginationsOnShopNoCategory(shopId, page, size);
            return Ok(result);
        }
        [HttpGet("ShopProductsInCategory")]
        public ActionResult GetShopProductsInCategory([FromQuery] int shopId, [FromQuery] int page, [FromQuery] int size, [FromQuery] int category)
        {
            var result = _productRepository.GetProductsInPaginationsOnShopWithCategory(shopId, page, size, category);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Add([FromQuery] AddProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var result = await _productRepository.Add(product);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                var url = Url.Action(nameof(GetById), new { id = result.Id });
                return Created(url, _productRepository.GetProductById(result.Id));
            }
            return BadRequest(ModelState);
        }
        [HttpDelete]
        public ActionResult Delete([FromQuery] int id)
        {
            var result = _productRepository.Delete(id);
            if (result.Id == 0)
            {
                return BadRequest(result.Massage);
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpPut("Details")]
        public async Task<ActionResult> Update([FromQuery] EditProductDTO product, [FromQuery] int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _productRepository.Update(product, id);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
        [HttpPut("UpdateCategoriesIOfProduct")]
        public ActionResult UpdateCategoriesInProduct([FromQuery] ModifyCategoriesInProductDTO categories)
        {
            if (ModelState.IsValid)
            {
                var result = _productRepository.UpdateCategoriesOfProduct(categories);
                if (result.Id == 0)
                {
                    return BadRequest(result.Massage);
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            return BadRequest(ModelState);
        }
    }
}
