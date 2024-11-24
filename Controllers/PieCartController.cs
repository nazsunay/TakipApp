using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TakipApp.Models;

namespace TakipApp.Controllers
{
    public class PieCartController : Controller
    {
        string connectionString = "";
      
        public IActionResult Index()
        {
            
            var userId = HttpContext.Session.GetInt32("Id");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login"); 
            }

            using var connection = new SqlConnection(connectionString);

            
            var trans = connection.Query<Transaction>(
                "SELECT Transactions.*, Categories.Name AS CategoryName FROM Transactions " +
                "LEFT JOIN Categories ON Transactions.CategoryId = Categories.CategoryId " +
                "WHERE Transactions.UserId = @UserId",
                new { UserId = userId }
            ).ToList();

            ViewBag.gelirler = connection.Query<Transaction>(
                "SELECT Transactions.*, Categories.Name AS CategoryName FROM Transactions " +
                "LEFT JOIN Categories ON Transactions.CategoryId = Categories.CategoryId " +
                "WHERE Transactions.Type = 'Gelir' AND Transactions.UserId = @UserId",
                new { UserId = userId }
            ).ToList();

            return View(trans);
        }

    }
    
    
}
