namespace SalesBoardSite.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        public string CustomerUserName { get; set; }

        public string? Amount { get; set; }
        // Navigation properties
        public User User { get; set; }
        public Item Item { get; set; }
    }

}