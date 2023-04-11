using GeekBurguer.Products.Service.Dto;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Produtos.Web.Controllers
{
	[Route("api/products")]
	public class ProductsController : Controller
	{
		private readonly IProductService _productService;

        public ProductsController(IProductService productService)
		{
            _productService = productService;
        }

		[HttpGet("{storeName}")]
		public async Task<IActionResult> GetProductsByStoreName(string storeName)
		{
			var productsByStore = await _productService.GetProductsByStoreNameAsync(storeName);

			if (productsByStore.Count <= 0)
				return NotFound();

			return Ok(productsByStore);			
		}

        [HttpPost()]
        public IActionResult AddProduct([FromBody] ProductToUpSert productToAdd)
        {
            if (productToAdd == null)
                return BadRequest();

            var product = _mapper.Map<Product>(productToAdd);

            if (product.StoreId == Guid.Empty)
                return new
                    Helper.UnprocessableEntityResult(ModelState);

            _productsRepository.Add(product);
            _productsRepository.Save();
            var productToGet = _mapper.Map<ProductToGet>(product);

            return CreatedAtRoute("GetProduct",
                new { id = productToGet.ProductId },
                productToGet);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(Guid id)
        {
            var product = _productsRepository.GetProductById(id);
            var productToGet = _mapper.Map<ProductToGet>(product);

            return Ok(productToGet);
        }

    }
}
