using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CornerStore.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Cashier")]
    public int CashierId { get; set; }


    [NotMapped]  // Not mapped to the database
    public decimal Total
    {
        get
            {
                decimal total = 0;
                
                if (OrderProducts != null)
                {
                    foreach (var orderProduct in OrderProducts)
                    {
                        // Ensure Product is not null before calculating total
                        if (orderProduct.Product != null)
                        {
                            total += orderProduct.Quantity * orderProduct.Product.Price;
                        }
                    }
                }
                
                return total;
            }
    }

    public DateTime? PaidOnDate { get; set; }

    // Explicitly initialize the collection to avoid null references
    public virtual List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();  // Initialization here

    public virtual List<Product> Products {get; set;} = new List<Product>();


    public virtual Cashier Cashier { get; set; }
}