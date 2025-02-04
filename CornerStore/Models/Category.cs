using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CornerStore.Models;

public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }