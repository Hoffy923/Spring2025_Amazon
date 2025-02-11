//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Library.eCommerce.Services;
using Spring2025_Samples.Models;
using System;
using System.Xml.Serialization;

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Amazon!");

            Console.WriteLine("C. Create new inventory item");
            Console.WriteLine("R. Read all inventory items");
            Console.WriteLine("U. Update an inventory item");
            Console.WriteLine("D. Delete an inventory item");
            Console.WriteLine("S. Manage Shopping Cart");
            Console.WriteLine("Q. Quit");

            List<Product?> list = ProductServiceProxy.Current.Products;

            char choice;
            do
            {
                string? input = Console.ReadLine();
                choice = input[0];
                switch (choice)
                {
                    case 'C':
                    case 'c':
                        var newProduct = new Product
                        {
                            Name = Console.ReadLine()
                        };
                        ProductServiceProxy.Current.AddOrUpdate(newProduct);

                        ShoppingCartServiceProxy.Current.InitializeStockFor(newProduct);
                        break;

                    case 'R':
                    case 'r':
                        list.ForEach(Console.WriteLine);
                        break;

                    case 'U':
                    case 'u':
                        Console.WriteLine("Which product would you like to update?");
                        
                        int selection = int.Parse(Console.ReadLine() ?? "-1");
                        var selectedProd = list.FirstOrDefault(p => p.Id == selection);

                        if (selectedProd != null)
                        {
                            selectedProd.Name = Console.ReadLine() ?? "ERROR";
                            ProductServiceProxy.Current.AddOrUpdate(selectedProd);

                            ShoppingCartServiceProxy.Current.InitializeStockFor(selectedProd);
                        }
                        break;

                    case 'D':
                    case 'd':
                        Console.WriteLine("Which product would you like to update?");
                        selection = int.Parse(Console.ReadLine() ?? "-1");
                        ProductServiceProxy.Current.Delete(selection);
                        break;

                    case 'S':
                    case 's':
                        ManageCart();
                        break;

                    case 'Q':
                    case 'q':
                  
                        ShoppingCartServiceProxy.Current.CheckoutAndPrintReceipt();
                        break;

                    default:
                        Console.WriteLine("Error: Unknown Command");
                        break;
                }
            } while (choice != 'Q' && choice != 'q');

            Console.ReadLine();
        }

        static void ManageCart()
        {
            char cChoice = ' ';
            do
            {
                Console.WriteLine("\n-- Cart Menu --");
                Console.WriteLine("A. Add item to cart");
                Console.WriteLine("R. Read cart items");
                Console.WriteLine("U. Update cart item quantity");
                Console.WriteLine("D. Delete (remove) product from cart");
                Console.WriteLine("B. Back to main menu");

                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue; 

                cChoice = input[0]; 

                switch (cChoice)
                {
                    case 'A':
                    case 'a':
                        AddToCart();
                        break;
                    case 'R':
                    case 'r':
                        ShoppingCartServiceProxy.Current.ReadCart();
                        Console.WriteLine($"Total items in cart: {ShoppingCartServiceProxy.Current.CartItemCount()}");
                        break;
                    case 'U':
                    case 'u':
                        UpdateCartItem();
                        break;
                    case 'D':
                    case 'd':
                        RemoveFromCart();
                        break;
                    case 'B':
                    case 'b':
                        break;
                    default:
                        Console.WriteLine("Unknown Cart Command");
                        break;
                }
            }
            while (cChoice != 'B' && cChoice != 'b');
        }

        static void AddToCart()
        {
            Console.WriteLine("Enter the Product ID you want to add:");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int prodId))
            {
                Console.WriteLine("Invalid product ID—please enter a number.");
                return;
            }

            var product = ProductServiceProxy.Current.Products
                .FirstOrDefault(p => p != null && p.Id == prodId);

            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.WriteLine("How many do you want to add?");
            string? qtyInput = Console.ReadLine();

            if (!int.TryParse(qtyInput, out int qty))
            {
                Console.WriteLine("Invalid quantity—must be a number.");
                return;
            }
            bool success = ShoppingCartServiceProxy.Current.AddToCart(product, qty);
            Console.WriteLine(success ? "Added to cart." : "Not enough in stock!");
        }

        static void UpdateCartItem()
        {
            Console.WriteLine("Enter Product ID in cart to update quantity:");
            int prodId = int.Parse(Console.ReadLine() ?? "-1");

            Console.WriteLine("Enter new quantity:");
            int newQty = int.Parse(Console.ReadLine() ?? "0");

            bool success = ShoppingCartServiceProxy.Current.UpdateCartItem(prodId, newQty);
            Console.WriteLine(success ? "Cart updated." : "Could not update (not enough stock or not in cart).");
        }

        static void RemoveFromCart()
        {
            Console.WriteLine("Enter Product ID to remove from cart:");
            int prodId = int.Parse(Console.ReadLine() ?? "-1");

            bool success = ShoppingCartServiceProxy.Current.RemoveFromCart(prodId);
            Console.WriteLine(success ? "Item removed from cart." : "Item not found in cart.");
        }
    }
}
