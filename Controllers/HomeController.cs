using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using TakipApp.Models;

namespace TakipApp.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = "";

        public bool CheckLogin()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("email"));
        }


        public IActionResult Index()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Login", "Login");
            }

            var userId = HttpContext.Session.GetInt32("Id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            using var connection = new SqlConnection(connectionString);
            var trans = connection.Query<Transaction>("SELECT Transactions.*, Categories.Name AS CategoryName FROM Transactions LEFT JOIN Categories ON Transactions.CategoryId = Categories.CategoryId WHERE Transactions.UserId = @UserId", new { UserId = userId }).ToList();

            return View(trans);
        }


        public IActionResult CategoryAdd()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Login", "Login");
            }

            using var connection = new SqlConnection(connectionString);
            var categories = connection.Query<Category>("SELECT c.CategoryId, c.Name as CategoryName FROM Categories c LEFT JOIN Transactions t ON c.CategoryId = t.CategoryId WHERE t.UserId = @UserId", new { UserId = ViewData["Id"] }).ToList();
            return View(categories);
        }
        [HttpPost]
        public IActionResult CategoryAdd(Category model)
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Login", "Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalý iþlem yaptýnýz.";
                return View("Message");
            }

            using var connection = new SqlConnection(connectionString);
            var transactionQuery = "INSERT INTO Categories (Name, UserId) VALUES (@Name, @UserId)";

            var data = new
            {
                model.Name,
                UserId = HttpContext.Session.GetInt32("Id") 
            };

            try
            {
                var rowsAffected = connection.Execute(transactionQuery, data);
                ViewBag.MessageCssClass = "alert-success";
                ViewBag.Message = "Ýþlem baþarýyla eklendi.";
            }
            catch (Exception ex)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = $"Bir hata oluþtu: {ex.Message}";
            }

            return View("Message");
        }


       
        public IActionResult Add()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Login", "Login");
            }

            using var connection = new SqlConnection(connectionString);
            var userId = HttpContext.Session.GetInt32("Id"); 
            var categories = connection.Query<Category>("SELECT c.CategoryId, c.Name as CategoryName FROM Categories c WHERE c.UserId = @UserId", new { UserId = userId }).ToList();

            return View(categories);
        }
        [HttpPost]
        public IActionResult Add(Transaction model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalý iþlem yaptýnýz.";
                return View("Message");
            }

            using var connection = new SqlConnection(connectionString);
            var transactionQuery = "INSERT INTO Transactions (UserId, CategoryId, Amount, DateCreated, Type) VALUES (@UserId, @CategoryId, @Amount, @DateCreated, @Type)";

            var data = new
            {
                UserId = HttpContext.Session.GetInt32("Id"),
                model.CategoryId,
                model.Amount,
                model.Type,
                DateCreated = DateTime.Now
            };

            var rowsAffected = connection.Execute(transactionQuery, data);
            if (rowsAffected > 0)
            {
                return RedirectToAction("Index"); 
            }

            ViewBag.MessageCssClass = "alert-danger";
            ViewBag.Message = "Bir hata oluþtu.";
            return View("Message");
        }

        public IActionResult Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM Transactions WHERE TransactionsId = @TransactionsId AND UserId = @UserId";

            var parameters = new
            {
                TransactionsId = id, 
                UserId = HttpContext.Session.GetInt32("Id") 
            };

            var rowsAffected = connection.Execute(sql, parameters);

            return RedirectToAction("Index");
        }


        public IActionResult UppDate(int id)
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Login", "Login");
            }

            using var connection = new SqlConnection(connectionString);
            var userId = HttpContext.Session.GetInt32("Id");

            // Kullanýcýnýn kategorilerini al
            var categories = connection.Query<Category>(
                "SELECT c.CategoryId, c.Name as CategoryName FROM Categories c WHERE c.UserId = @UserId",
                new { UserId = userId }
            ).ToList();

            // Ýþlem bilgilerini al
            var transaction = connection.QuerySingleOrDefault<Transaction>(
                "SELECT t.TransactionsId, t.UserId, t.CategoryId, t.Amount, t.DateCreated, t.Type, c.Name as CategoryName " +
                "FROM Transactions t JOIN Categories c ON t.CategoryId = c.CategoryId " +
                "WHERE t.TransactionsId = @TransactionsId AND t.UserId = @UserId",
                new { TransactionsId = id, UserId = userId }
            );

            if (transaction == null)
            {
                // Eðer iþlem bulunamazsa hata mesajý göster
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Ýþlem bulunamadý.";
                return View("Message");
            }

            ViewBag.Categories = categories;
            return View(transaction);
        }

        [HttpPost]
        public IActionResult UppDate(Transaction model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalý iþlem yaptýnýz.";
                return View("Message");
            }

            using var connection = new SqlConnection(connectionString);
            var userId = HttpContext.Session.GetInt32("Id");

            var sql = "UPDATE Transactions SET UserId = @UserId, CategoryId = @CategoryId, Amount = @Amount, DateCreated = @DateCreated, Type = @Type " +
                      "WHERE TransactionsId = @TransactionsId AND UserId = @UserId";

            var parameters = new
            {
                UserId = userId,
                model.CategoryId,
                model.Amount,
                model.DateCreated,
                model.Type,
                model.TransactionsId,
            };

            var affectedRows = connection.Execute(sql, parameters);

            if (affectedRows > 0)
            {
                ViewBag.MessageCssClass = "alert-success";
                ViewBag.Message = "Güncellendi.";
                return View("Message");
            }

            ViewBag.MessageCssClass = "alert-danger";
            ViewBag.Message = "Güncelleme sýrasýnda bir hata oluþtu.";
            return View(model);
        }


    }
}
