﻿namespace Pustok_BackEndProject.Models
{
    public class Basket:BaseEntity
    {
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public int Count { get; set; }
        public double? Price { get; set; }
        public double? Extax { get; set; }
    }
}
