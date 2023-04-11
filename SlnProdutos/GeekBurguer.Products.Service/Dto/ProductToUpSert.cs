namespace GeekBurguer.Products.Service.Dto
{
    public class ProductToUpSert
    {
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ItemToUpSert> Items { get; set; }

    }

    public class ItemToUpSert
    {
        public string Name { get; set; }
    }

}
