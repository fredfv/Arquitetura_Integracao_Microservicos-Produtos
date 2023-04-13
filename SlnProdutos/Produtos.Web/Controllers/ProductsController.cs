using GeekBurguer.Products.Service.Dto;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Produtos.Web.Controllers
{
	[Route("api/[controller]")]
	public class ProductsController : Controller
	{
		private readonly IProductService _productService;

        public ProductsController(IProductService productService)
		{
            _productService = productService;
        }

		[HttpGet("store/{{storeName}}")]
		public async Task<IActionResult> GetProductsByStoreName(string storeName)
		{
			var productsByStore = await _productService.GetProductsByStoreNameAsync(storeName);

			if (productsByStore.Count <= 0)
				return NotFound();

			return Ok(productsByStore);			
		}

        [HttpPost()]
        public async Task<IActionResult> AddProduct([FromBody] ProductToUpSert productToAdd)
        {
            try
            {
                if (productToAdd == null)
                    return BadRequest();

                bool inserted = await _productService.Add(productToAdd);

                if (!inserted)
                    return new UnprocessableEntityResult();

                var productToGet = await _productService.GetProductsByStoreNameAsync(productToAdd.StoreName);

                return CreatedAtRoute("GetProduct",
                    new { id = productToGet.First().ProductId },
                    productToGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet("product/{{id}}", Name = "GetProduct")]
        public async Task<IActionResult> GetProductAsync(Guid id)
        {
            var productToGet = await _productService.GetProductById(id);

            return Ok(productToGet);
        }

    }
}
