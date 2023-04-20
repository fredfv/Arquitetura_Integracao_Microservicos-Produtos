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

    public class ProductChangedEvent
    {
        [Key]
        public Guid EventId { get; set; }

        public ProductState State { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public bool MessageSent { get; set; }
    }

    public class ProductChangedMessage
    {
        public ProductState State { get; set; }        
        public ProductToGetMessageDto Product { get; set; }
    }

    public class ProductToGetMessageDto
    {
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        public Guid StoreId { get; set; }
        [Key]
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<ItemToGetMessageDto> Items { get; set; }
            = new List<ItemToGetMessageDto>();

    }
    public class ItemToGetMessageDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
    }

    public enum ProductState
    {
        Deleted = 2,
        Modified = 3,
        Added = 4
    }
}
