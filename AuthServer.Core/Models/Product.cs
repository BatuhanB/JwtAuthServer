
namespace AuthServer.Core.Models
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public string UserId { get; set; }

         public Product(int id,string name, decimal price, int stock, string userId)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            UserId = userId;
        }

    }
}
