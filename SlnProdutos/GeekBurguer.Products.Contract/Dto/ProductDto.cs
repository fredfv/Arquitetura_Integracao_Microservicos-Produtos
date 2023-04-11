using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GeekBurguer.Products.Contract.Dto
{
    public class Product
    {
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        public Guid StoreId { get; set; }
        [Key]
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<Item> Items { get; set; }
            = new List<Item>();
    }

    public class Store
    {
        [Key]
        public Guid StoreId { get; set; }
        public string Name { get; set; }
    }
    public class Item
    {
        [Key]
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
