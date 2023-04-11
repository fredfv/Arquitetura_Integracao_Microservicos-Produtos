namespace GeekBurguer.Products.Service.Dto
{
    public class ProductToGetDto
    {
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ItemToGetDto> Items { get; set; }
        public decimal Price { get; set; }

    }

    public class ItemToGetDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
    }
}
