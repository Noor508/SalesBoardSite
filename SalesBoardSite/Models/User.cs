namespace SalesBoardSite.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }
        public string Pass { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Contacts { get; set; }
        public string Address { get; set; }

        // Navigation properties
        public ICollection<Item> ItemsForSale { get; set; }
        public ICollection<Purchase> Purchases { get; set; }

        public ICollection<CustomersWithTotalSpending> Customers { get; set; }
    }

}