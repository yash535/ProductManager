﻿using System.ComponentModel.DataAnnotations;

namespace ProductManageApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockAvailable { get; set; }
        
        public string Description { get; set; }
    }
}
