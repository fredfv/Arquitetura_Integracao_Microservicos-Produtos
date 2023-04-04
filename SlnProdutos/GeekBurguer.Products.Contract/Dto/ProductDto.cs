namespace GeekBurguer.Products.Contract.Dto
{
	public class ProductDto
	{
		public string StoreName { get; set; }
		public Guid ProductId { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public List<ItemDto> Items { get; set; }
		public decimal Price { get; set; }

	}

	public class ItemDto
	{
		public Guid ItemId { get; set; }
		public string Name { get; set; }
	}

	public class ProductChangedDto
	{
		public ProductState State { get; set; }
		public ProductDto Product { get; set; }
	}

	public enum ProductState
	{
		Deleted = 2,
		Modified = 3,
		Added = 4
	}

}
