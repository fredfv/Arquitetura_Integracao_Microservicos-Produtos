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
	}
}
