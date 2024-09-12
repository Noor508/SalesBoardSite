using System.Collections;

namespace SalesBoardSite.Models
{
    public class Item 
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }

        // Foreign key
        public int SellerId { get; set; }

        // Navigation property
        public User Seller { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
        
    }

}
