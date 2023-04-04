using GeekBurguer.Products.Contract.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Produtos.Web.Controllers
{
	[Route("api/products")]
	public class ProductsController : Controller
	{
		private IList<ProductDto> Products = new List<ProductDto>();

		public ProductsController()
		{
			var paulistaStore = "Paulista";
			var morumbiStore = "Morumbi";

			var beef = new ItemDto { ItemId = Guid.NewGuid(), Name = "beef" };
			var pork = new ItemDto { ItemId = Guid.NewGuid(), Name = "pork" };
			var mustard = new ItemDto { ItemId = Guid.NewGuid(), Name = "mustard" };
			var ketchup = new ItemDto { ItemId = Guid.NewGuid(), Name = "ketchup" };
			var bread = new ItemDto { ItemId = Guid.NewGuid(), Name = "bread" };
			var wBread = new ItemDto { ItemId = Guid.NewGuid(), Name = "whole bread" };

			Products = new List<ProductDto>()
			{
				new ProductDto { ProductId = Guid.NewGuid(), Name = "Darth Bacon",
					Image = "hamb1.png", StoreName = paulistaStore,
					Items = new List<ItemDto> {beef, ketchup, bread }
				},
				new ProductDto { ProductId = Guid.NewGuid(), Name = "Cap. Spork",
					Image = "hamb2.png", StoreName = paulistaStore,
					Items = new List<ItemDto> { pork, mustard, wBread }
				},
				new ProductDto { ProductId = Guid.NewGuid(), Name = "Beef Turner",
					Image = "hamb3.png", StoreName = morumbiStore,
					Items = new List<ItemDto> {beef, mustard, bread }
				}
			};
		}

		[HttpGet("{storeName}")]
		public IActionResult GetProductsByStoreName(string storeName)
		{
			var productsByStore = Products.Where(product =>
	                  product.StoreName == storeName).ToList();

			if (productsByStore.Count <= 0)
				return NotFound();

			return Ok(productsByStore);

		}
	}
}
