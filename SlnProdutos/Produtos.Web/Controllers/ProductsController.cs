using AutoMapper;
using GeekBurguer.Products.Contract.Dto;
using GeekBurguer.Products.Service.Dto;
using GeekBurguer.Products.Service.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using System.ComponentModel.DataAnnotations;

namespace Produtos.Web.Controllers
{
	[Route("api/[controller]")]
	public class ProductsController : Controller
	{
		private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductsController(IProductService productService, IMapper mapper)
		{
            _productService = productService;
            _mapper = mapper;
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateProduct(Guid id, [Required] [FromBody] ProductToUpSert productToUpdate)
        {            
            var product = await _productService.GetProductById(id);

            if (id == null || product == null)
            {
                return NotFound("Produto não encontrado para atualização");
            }
                       
            if (product.StoreId == Guid.Empty)
                return NotFound("SoreId não encontrado");

            await _productService.Update(id, productToUpdate);

            var productUpdated = await _productService.GetProductById(id);

            return CreatedAtRoute("GetProduct",
                new { id = productUpdated.ProductId },
                productUpdated);
        }
    }
}
