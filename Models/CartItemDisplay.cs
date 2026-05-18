namespace ToyShop.Models
{
    public class CartItemDisplay
    {
        public int IdBasketProduct { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
        public string ImagePath { get; set; }
    }
}