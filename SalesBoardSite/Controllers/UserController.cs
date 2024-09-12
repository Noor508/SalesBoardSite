
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Runtime.InteropServices.ComTypes;
using SalesBoardSite.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;

namespace SalesBoardSite.Controllers
{
    public class UserController : Controller
    {
        public SqlConnection cn = new SqlConnection("Data Source=DESKTOP-MHUVI83\\SQLEXPRESS;Database=BoardStoreApp;Trusted_Connection=True");
        public SqlCommand cmd;
        public SqlDataReader dr;
        public static int activeUser = 0;

        public UserController()
        {

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(IFormCollection Collection)
        {
            User usr = new User();
            string str = "";
            str = "SELECT * FROM Users WHERE Username = @username and Pass = @password";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@username", Collection["Username"].ToString());
            cmd.Parameters.AddWithValue("@password", Collection["Pass"].ToString());
            dr = cmd.ExecuteReader();
            dr.Read();
            int userId = 0;
            string passd = "";
            if (dr.HasRows)
            {
                usr.UserId = Int32.Parse(dr["UserId"].ToString());
                usr.Username = dr["Username"].ToString();
                usr.Name = dr["Name"].ToString();
                usr.Pass = dr["Pass"].ToString();
                usr.Email = dr["Email"].ToString();
                usr.UserId = Int32.Parse(dr["UserId"].ToString());
                userId = Convert.ToInt32(dr["UserId"].ToString());
                

                activeUser = userId;
                passd = dr["Pass"].ToString();

                Boolean b = Convert.ToBoolean(dr["Visited"]);
                if (passd == Collection["Pass"].ToString())
                {
                    if (b)
                    {
                        cn.Close();
                        dr.Close();
                        return RedirectToAction("UserHome", new { userId });
                    }
                    else
                    {
                        cn.Close();
                        dr.Close();
                        str = "UPDATE Users SET Visited = 1 " +
                              "WHERE Username = @username and Pass = @password ;";
                        cn.Open();
                        cmd = new SqlCommand(str, cn);
                        cmd.Parameters.AddWithValue("@username", Collection["Username"].ToString());
                        cmd.Parameters.AddWithValue("@password", Collection["Pass"].ToString());
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        return RedirectToAction("SaveUserDetails", new { User });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credentials.");
                    return View();
                }
            }
            else
            {
                    cn.Close();
                    dr.Close();
                    // Authentication failed, return an error message or redirect back to the login page
                    ModelState.AddModelError("", "Invalid credentials.");
                    return View();
            }
            
            // Authentication succeeded, redirect to the user's home page or another appropriate page
            return View();
        }



        public ActionResult UserDetails(User Usr)
        {
            string str = "";
            str = "SELECT * FROM Users WHERE Userid = @id ;";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@id", activeUser.ToString());
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                Usr.Username = dr["Username"].ToString();
                Usr.Pass = dr["Pass"].ToString();
                Usr.Email = dr["Email"].ToString();
                Usr.UserId = Int32.Parse(dr["UserId"].ToString());
                Usr.Name = dr["Name"].ToString();
                Usr.Contacts = dr["Contacts"].ToString();
                Usr.Address = dr["Address"].ToString();
                if (!string.IsNullOrEmpty(dr["Age"].ToString()))
                {
                    int.TryParse(dr["Age"].ToString(), out int age);
                    Usr.Age = age;
                }
                else
                {
                    Usr.Age = 0; // Or assign a default value or handle accordingly
                }
            }
            return View(Usr);
        }

        [HttpGet]
        public IActionResult UserHome(int userId)
        {
            CustomersWithTotalSpending[] cust = new CustomersWithTotalSpending[1000];
            // Item[] items = new Item[1000];
            Purchase[] p = new Purchase[1000];
            User Usr = new User();
            cn.Open();
            cmd = new SqlCommand("SELECT * FROM Items WHERE SellerId = @userId;", cn);
            cmd.Parameters.AddWithValue("@userId", activeUser.ToString());
            dr = cmd.ExecuteReader();
            var items = new List<Item>();
            while (dr.Read())
            {
                var item = new Item
                {
                    SellerId = (int)Convert.ToDecimal(dr["SellerId"].ToString()),
                    ItemId = (int)Convert.ToDecimal(dr["ItemId"].ToString()),
                    Price = Convert.ToDecimal(dr["Price"].ToString()),
                    Name = dr["Name"].ToString(),
                    AvailableStock = Convert.ToInt32(dr["AvailableStock"].ToString())
                };
                items.Add(item);
            }

            Usr.ItemsForSale = items.ToArray();
            dr.Close();
            cn.Close();

            cn.Open();
            cmd = new SqlCommand("SELECT U.Username, SUM(P.Amount) AS TotalSpending " +
                                 "FROM Purchases P " +
                                 "LEFT JOIN Items I ON P.ItemId = I.ItemId " +
                                 "LEFT JOIN Users U ON P.UserId = U.UserId " +
                                 "WHERE P.UserId != @userId AND " +
                                 "I.SellerID = @userId2 " +
                                 "GROUP BY U.Username ;", cn);
            cmd.Parameters.AddWithValue("@userId", activeUser);
            cmd.Parameters.AddWithValue("@userId2", activeUser);
            dr = cmd.ExecuteReader();
            var customers = new List<CustomersWithTotalSpending>();
            while (dr.Read())
            {
                var customer = new CustomersWithTotalSpending
                {
                    CustomerName = dr["Username"].ToString(),
                    TotalSpending = Convert.ToDecimal(dr["TotalSpending"].ToString())
                };
                customers.Add(customer);
            }

            Usr.Customers = customers.ToArray();
            dr.Close();
            dr.Close();
            cn.Close();

            return View(Usr);
    }

        public IActionResult AddItem(int Userid)
        {
            // Redirect to UserHome after adding the item
            Item itm = new Item();
            itm.SellerId = activeUser;
            return View(itm);
        }

        [HttpPost]
        public IActionResult AddItem(Item item)
        {
            // Redirect to UserHome after adding the item
            int ids = item.SellerId;
            int stk = item.AvailableStock;
            decimal prc = item.Price;
            Console.WriteLine(ids);
            Console.WriteLine(stk);
            Console.WriteLine(prc);
            cn.Open();
            cmd = new SqlCommand("INSERT INTO Items (Name , Price , AvailableStock , SellerId) " +
                                 "VALUES (@name , @price , @stock , @id) ;", cn);
            cmd.Parameters.AddWithValue("@name",item.Name);
            cmd.Parameters.AddWithValue("@price", item.Price.ToString());
            cmd.Parameters.AddWithValue("@stock", item.AvailableStock.ToString());
            cmd.Parameters.AddWithValue("@id", item.SellerId.ToString());
            cmd.ExecuteNonQuery();
            cn.Close();
            return RedirectToAction("UserHome", new { userId = item.SellerId});
        }

        public IActionResult EditItem(int UserId, int ItemID, string name , decimal price , int stock)
        {
            Item item = new Item();
            item.SellerId = UserId;
            item.ItemId = ItemID;
            item.Name = name;
            item.Price = price;
            item.AvailableStock = stock;
            return View(item);
        }
        [HttpPost]
        public IActionResult EditItem(Item item)
        {
            if (item.Name != "")
            {
                cn.Open();
                cmd = new SqlCommand("Update Items  " +
                                     "SET Name = @name " +
                                     "WHERE SellerId = @seller AND " +
                                     "ItemId = @id ;", cn);
                cmd.Parameters.AddWithValue("@name", item.Name);
                cmd.Parameters.AddWithValue("@seller", item.SellerId.ToString());
                cmd.Parameters.AddWithValue("@id", item.ItemId.ToString());
                cmd.ExecuteNonQuery();
                cn.Close();
            }

            if (item.Price != 0)
            {
                cn.Open();
                cmd = new SqlCommand("Update Items  " +
                                     "SET Price = @price " +
                                     "WHERE SellerId = @seller AND " +
                                     "ItemId = @id ;", cn);
                cmd.Parameters.AddWithValue("@price", item.Price.ToString());
                cmd.Parameters.AddWithValue("@seller", item.SellerId.ToString());
                cmd.Parameters.AddWithValue("@id", item.ItemId.ToString());
                cmd.ExecuteNonQuery();
                cn.Close();
            }

            if (item.AvailableStock != 0)
            {
                cn.Open();
                cmd = new SqlCommand("Update Items  " +
                                     "SET AvailableStock = @stock " +
                                     "WHERE SellerId = @seller AND " +
                                     "ItemId = @id ;", cn);
                cmd.Parameters.AddWithValue("@stock", item.AvailableStock.ToString());
                cmd.Parameters.AddWithValue("@seller", item.SellerId.ToString());
                cmd.Parameters.AddWithValue("@id", item.ItemId.ToString());
                cmd.ExecuteNonQuery();
                cn.Close();
            }

            return RedirectToAction("UserHome", new { userId = item.SellerId });
        }

        [HttpPost]
        public IActionResult DeleteItem(int UserId, int ItemId)
        {
            string str = "DELETE FROM Items WHERE ItemId = @itm ;";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@itm", ItemId);
            cmd.ExecuteNonQuery();
            cn.Close();
            return RedirectToAction("UserHome", new { UserId});
        }


        /*public IActionResult ShoppingItems(int UserId)
        {
        }*/

        [HttpGet]
        public IActionResult ShoppingItems(string searchTerm, int UserId)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                cn.Open();
                cmd = new SqlCommand("SELECT * FROM Items WHERE Name LIKE @nm AND SellerId != @User ;", cn);
                cmd.Parameters.AddWithValue("@nm", $"%{searchTerm}%"); // Add "%" to both sides of the search term
                cmd.Parameters.AddWithValue("@User", activeUser.ToString());
                dr = cmd.ExecuteReader();
                var items = new List<Item>();
                while (dr.Read())
                {
                    var item = new Item
                    {
                        SellerId = (int)Convert.ToDecimal(dr["SellerId"].ToString()),
                        ItemId = (int)Convert.ToDecimal(dr["ItemId"].ToString()),
                        Price = Convert.ToDecimal(dr["Price"].ToString()),
                        Name = dr["Name"].ToString(),
                        AvailableStock = Convert.ToInt32(dr["AvailableStock"].ToString()),
                    };
                    items.Add(item);
                }

                dr.Close();
                cn.Close();
                return View(items);
            }
            else
            {
                cn.Open();
                cmd = new SqlCommand("SELECT * FROM Items WHERE SellerId != @userId;", cn);
                cmd.Parameters.AddWithValue("@userId", activeUser.ToString());
                dr = cmd.ExecuteReader();
                var items = new List<Item>();
                while (dr.Read())
                {
                    var item = new Item
                    {
                        SellerId = (int)Convert.ToDecimal(dr["SellerId"].ToString()),
                        ItemId = (int)Convert.ToDecimal(dr["ItemId"].ToString()),
                        Price = Convert.ToDecimal(dr["Price"].ToString()),
                        Name = dr["Name"].ToString(),
                        AvailableStock = Convert.ToInt32(dr["AvailableStock"].ToString())
                    };
                    items.Add(item);
                }

                dr.Close();
                cn.Close();
                return View(items);
            }

            return View();
        }


        public IActionResult ViewItem(int itemId)
        {
            cn.Open();
            cmd = new SqlCommand("SELECT * FROM Items WHERE ItemId = @id ; ", cn);
            cmd.Parameters.AddWithValue("@id",itemId.ToString());
            dr = cmd.ExecuteReader();
            dr.Read();
            Item itm = new Item();
            itm.ItemId = itemId;
            itm.Name = dr["Name"].ToString();
            itm.Price = Decimal.Parse(dr["Price"].ToString());
            itm.AvailableStock = Int32.Parse(dr["AvailableStock"].ToString());
            itm.SellerId = Int32.Parse(dr["SellerId"].ToString());
            return View(itm);
        }


        [HttpPost]
        public IActionResult AddToCart(int itemId , int quantity)
        {

            cn.Open();

                string insertQuery = "INSERT INTO ShoppingCart (UserId, ItemId, Quantity) VALUES (@userId, @itemId, @quantity) ;";

                SqlCommand cmd = new SqlCommand(insertQuery, cn);
                
                    cmd.Parameters.AddWithValue("@userId", activeUser.ToString());
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Item added to cart successfully.";
                        cn.Close();
                        cn.Open();
                        cmd = new SqlCommand("UPDATE Items " +
                                     "Set AvailableStock = AvailableStock - @Qt " +
                                     "WHERE ItemId = @id ;", cn);
                        cmd.Parameters.AddWithValue("@Qt", quantity.ToString());
                        cmd.Parameters.AddWithValue("@id", itemId.ToString());
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to add item to cart.";
                    }
                
                cn.Close();

                return RedirectToAction("ViewItem", new { itemId });
        }

        public IActionResult ViewCart()
        {
            var  shps = new List<ShoppingCartItem>();
            cn.Open();
            cmd = new SqlCommand("SELECT * FROM ShoppingCart C " +
                                 "LEFT JOIN Items I ON C.ItemId = I.ItemId " +
                                 "WHERE C.UserId = @id ; ", cn);
            cmd.Parameters.AddWithValue("@id", activeUser.ToString());
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var shp = new ShoppingCartItem();
                shp.UserId = activeUser;
                Console.WriteLine(shp.UserId);
                shp.Quantity = Int32.Parse(dr["Quantity"].ToString());
                shp.ItemId = Int32.Parse(dr["ItemId"].ToString());
                shp.ShoppingCartItemId = Int32.Parse(dr["ShoppingCartItemId"].ToString());

                Item itm = new Item();
                itm.SellerId = Int32.Parse(dr["SellerId"].ToString());
                itm.Name = dr["Name"].ToString();
                itm.AvailableStock = Int32.Parse(dr["AvailableStock"].ToString());
                itm.ItemId = Int32.Parse(dr["ItemId"].ToString());
                itm.Price = Decimal.Parse(dr["Price"].ToString());

                shp.Items = itm;
                shps.Add(shp);
            }
            dr.Close();
            cn.Close();
            return View(shps);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int itemId , int cartid)
        {
            string str = "SELECT * FROM ShoppingCart " +
                         "WHERE ItemId = @itmid AND " +
                         "ShoppingCartItemId = @crtid ;";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@itmid",itemId);
            cmd.Parameters.AddWithValue("@crtid", cartid);
            dr = cmd.ExecuteReader();
            dr.Read();
            int Qt = Int32.Parse(dr["Quantity"].ToString());
            cn.Close();
            dr.Close();
            str = "UPDATE Items " +
                  "SET AvailableStock = AvailableStock + @quantity " +
                  "WHERE ItemId = @id ; ";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@quantity", Qt.ToString());
            cmd.Parameters.AddWithValue("@id", itemId.ToString());
            cmd.ExecuteNonQuery();
            cn.Close();
            cn.Open();

            // Create a SQL command to remove the item from the shopping cart
            str = "DELETE FROM ShoppingCart " +
                  "WHERE UserId = @usrid AND " +
                  "ItemId = @itmid AND " +
                  "ShoppingCartItemId = @cartid ;";
            using (SqlCommand command = new SqlCommand(str, cn))
            { 
                command.Parameters.AddWithValue("@itmid", itemId);
                command.Parameters.AddWithValue("@cartid", cartid);
                command.Parameters.AddWithValue("@usrid", activeUser.ToString());
                command.ExecuteNonQuery();
            }
            cn.Close();

            return RedirectToAction("ViewCart");
        }

        [HttpGet]
        public IActionResult Purchases()
        {
            string str = "";
            var cartItems = new List<ShoppingCartItem>();
            cn.Open();
            cmd = new SqlCommand("SELECT * FROM ShoppingCart C " +
                                 "LEFT JOIN Items I ON C.ItemId = I.ItemId " +
                                 "WHERE C.UserId = @id ; ", cn);
            cmd.Parameters.AddWithValue("@id", activeUser.ToString());
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var shp = new ShoppingCartItem();
                shp.UserId = activeUser;
                Console.WriteLine(shp.UserId);
                shp.Quantity = Int32.Parse(dr["Quantity"].ToString());
                shp.ItemId = Int32.Parse(dr["ItemId"].ToString());
                shp.ShoppingCartItemId = Int32.Parse(dr["ShoppingCartItemId"].ToString());

                Item itm = new Item();
                itm.SellerId = Int32.Parse(dr["SellerId"].ToString());
                itm.Name = dr["Name"].ToString();
                itm.AvailableStock = Int32.Parse(dr["AvailableStock"].ToString());
                itm.ItemId = Int32.Parse(dr["ItemId"].ToString());
                itm.Price = Decimal.Parse(dr["Price"].ToString());

                shp.Items = itm;
                cartItems.Add(shp);
            }
            dr.Close();
            cn.Close();


            for (int i = 0 ; i < cartItems.Count ; i++)
            {
                var id = cartItems[i].ItemId;
                var userid = cartItems[i].UserId;
                var Quantity = cartItems[i].Quantity;
                var cartid = cartItems[i].ShoppingCartItemId;

                cn.Open();
                str = "Update Items " +
                      "SET AvailableStock = AvailableStock + @QT " +
                      "WHERE ItemId = @itmid ; ";
                cmd = new SqlCommand(str, cn);
                cmd.Parameters.AddWithValue("@QT", Quantity);
                cmd.Parameters.AddWithValue("@itmid", id);
                cmd.ExecuteNonQuery();
                cn.Close();

                cn.Open();
                str = "DELETE FROM ShoppingCart " +
                      "WHERE UserId = @usrid AND " +
                      "ItemId = @itmid AND " +
                      "ShoppingCartItemId = @cartid ;";
                cmd = new SqlCommand(str, cn);
                cmd.Parameters.AddWithValue("@usrid", userid);
                cmd.Parameters.AddWithValue("@itmid", id);
                cmd.Parameters.AddWithValue("@cartid", cartid);
                cmd.ExecuteNonQuery();
                cn.Close();

                cn.Open();
                str = "INSERT INTO Purchases (UserId , ItemId , Quantity) " +
                      "VALUES (@usrid , @itmid , @QT) ;";
                cmd = new SqlCommand(str, cn);
                cmd.Parameters.AddWithValue("@usrid", userid);
                cmd.Parameters.AddWithValue("@itmid", id);
                cmd.Parameters.AddWithValue("@QT", Quantity);
                cmd.ExecuteNonQuery();
                cn.Close();
            }

            return RedirectToAction("ViewCart");
        }
 



        public IActionResult SaveUserDetails()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveUserDetails(User model)
        {
            string str = "";
            str = "SELECT * FROM Users WHERE Userid = @id ;";
            cn.Open();
            cmd = new SqlCommand(str, cn);
            cmd.Parameters.AddWithValue("@id",activeUser.ToString());
            dr = cmd.ExecuteReader();
            dr.Read();
            model.UserId = Int32.Parse(dr["UserId"].ToString());
            model.Name = dr["Name"].ToString();
            model.Username = dr["Username"].ToString();
            model.Email = dr["Email"].ToString();
            model.Pass = dr["Pass"].ToString();
            cn.Close();
            dr.Close();
            cn.Open();

            cmd = new SqlCommand("UPDATE Users SET Contacts = @contact, " +
                       "Address = @addr, Age = @age, " +
                       "Visited = 1, Name = @name " +
                       "WHERE UserId = @userid;", cn);


            cmd.Parameters.AddWithValue("@contact" , model.Contacts);
            cmd.Parameters.AddWithValue("@addr", model.Address); 
            cmd.Parameters.AddWithValue("@age", model.Age); 
            cmd.Parameters.AddWithValue("@userid", model.UserId);
            cmd.Parameters.AddWithValue("@name",model.Name);

            cmd.ExecuteNonQuery();
            
            cn.Close();
                

                // Redirect to a success page or register page
            return RedirectToAction("UserDetails");

                // If registration fails, return to the registration page with the model
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }

     
         [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-MHUVI83\\SQLEXPRESS;Initial Catalog=BoardStoreApp;Integrated Security=True"))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Users (Name, Username , Email, Pass) VALUES (@Name, @Username, @Email, @Password)", connection))
                    {
                        command.Parameters.AddWithValue("@Name", model.Name);
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Password", model.Password);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                // Set a success message in TempData
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";

                // Redirect to login page
                return RedirectToAction("Login");
            }

            // If registration fails, return to the registration page with the model
            return View(model);
        }



    }
}
