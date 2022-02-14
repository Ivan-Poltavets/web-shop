﻿namespace OnlineShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? ImageName { get; set; }
        public decimal Price { get; set; }
    }
}
