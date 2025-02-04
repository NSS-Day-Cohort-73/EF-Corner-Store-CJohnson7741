using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CornerStore.Models.DTOs;

namespace CornerStore.Models;
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; }

    [Required]
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    // Changed to ICollection to properly represent the one-to-many relationship
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
