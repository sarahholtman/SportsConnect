using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportsConnect1._0.Models;

namespace SportsConnect1._0.Controllers;

// Handles navigation pages: Home, Privacy, and Error
public class HomeController : Controller
{
    // Displays the homepage of the application
    public IActionResult Index()
    {
        return View();
    }

    // Displays the privacy page
    public IActionResult Privacy()
    {
        return View();
    }

    // Handles application errors and provides a request identifier for debugging.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
