using Spring2025_Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.eCommerce.Services
{
    public class ShoppingCartServiceProxy
    {
        private static ShoppingCartServiceProxy? instance;
        private static readonly object instanceLock = new();

        public static ShoppingCartServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ShoppingCartServiceProxy();
                    }
                    return instance;
                }
            }
        }

        private Dictionary<int, int> inventoryStock;

        private List<Cart> cartItems;
        public IReadOnlyList<Cart> Items => cartItems;

        private ShoppingCartServiceProxy()
        {
            cartItems = new List<Cart>();
            inventoryStock = new Dictionary<int, int>();
        }


        public void InitializeStockFor(Product product)
        {
            if (product == null) return;

            if (!inventoryStock.ContainsKey(product.Id))
            {
                inventoryStock[product.Id] = 10;
            }
        }

        public bool AddToCart(Product product, int quantityRequested)
        {
            if (!inventoryStock.ContainsKey(product.Id))
            {
                inventoryStock[product.Id] = 10;
            }

            int stockAvailable = inventoryStock[product.Id];
            if (quantityRequested > stockAvailable)
            {
                return false;
            }

            inventoryStock[product.Id] = stockAvailable - quantityRequested;

            var existing = cartItems.FirstOrDefault(ci => ci.ProductRef?.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity += quantityRequested;
            }
            else
            {
                cartItems.Add(new Cart(product, quantityRequested));
            }

            return true;
        }

        public void ReadCart()
        {
            if (!cartItems.Any())
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            foreach (var ci in cartItems)
            {
                Console.WriteLine(ci);
            }
        }

        public bool UpdateCartItem(int productId, int newQuantity)
        {
            var cartItem = cartItems.FirstOrDefault(ci => ci.ProductRef?.Id == productId);
            if (cartItem == null) return false;

            int oldQty = cartItem.Quantity;
            if (newQuantity == oldQty)
            {

                return true;
            }
            else if (newQuantity < oldQty)
            {

                int diff = oldQty - newQuantity;
                inventoryStock[productId] = inventoryStock.ContainsKey(productId)
                    ? inventoryStock[productId] + diff
                    : diff;
                cartItem.Quantity = newQuantity;
            }
            else
            {

                int diff = newQuantity - oldQty;
                if (!inventoryStock.ContainsKey(productId))
                {
                    inventoryStock[productId] = 10;
                }
                if (inventoryStock[productId] < diff)
                {
                    return false;
                }
                inventoryStock[productId] -= diff;
                cartItem.Quantity = newQuantity;
            }

            return true;
        }

        public bool RemoveFromCart(int productId)
        {
            var cartItem = cartItems.FirstOrDefault(ci => ci.ProductRef?.Id == productId);
            if (cartItem == null) return false;

            inventoryStock[productId] = inventoryStock.ContainsKey(productId)
                ? inventoryStock[productId] + cartItem.Quantity
                : cartItem.Quantity;

            cartItems.Remove(cartItem);
            return true;
        }

        public int CartItemCount()
        {
            return cartItems.Sum(ci => ci.Quantity);
        }

        public void CheckoutAndPrintReceipt()
        {
            if (!cartItems.Any())
            {
                Console.WriteLine("Cart is empty. No purchase made.");
                return;
            }

            Console.WriteLine("\n-- ITEMIZED RECEIPT --");
            int totalUnits = 0;
            foreach (var item in cartItems)
            {
                Console.WriteLine($"- {item.ProductRef?.Name} x {item.Quantity}");
                totalUnits += item.Quantity;
            }

            decimal subtotal = totalUnits * 1.00m;
            decimal tax = subtotal * 0.07m;
            decimal totalDue = subtotal + tax;

            Console.WriteLine($"\nSubtotal: {subtotal:C}");
            Console.WriteLine($"Tax (7%): {tax:C}");
            Console.WriteLine($"Total Due: {totalDue:C}\n");

            cartItems.Clear();
            Console.WriteLine("Checkout complete. Thank you!");
        }
    }
}