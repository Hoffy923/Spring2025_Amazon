using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring2025_Samples.Models;

namespace Spring2025_Samples.Models
{
    public class Cart
    {
        public Product? ProductRef { get; set; }
        public int Quantity { get; set; }

        public Cart(Product? product, int quantity)
        {
            ProductRef = product;
            Quantity = quantity;
        }

        public override string ToString()
        {
            if (ProductRef == null)
            {
                return $"(Unknown Product) x {Quantity}";
            }
            return $"{ProductRef.Name} (Qty {Quantity})";
        }
    }
}
