using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;


        public HomeController(ILogger<HomeController> logger,ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _context.Products.ToListAsync());

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Search(string search)
        {
            if (search == null) return View();
            List<Product> products = await _context.Products.ToListAsync();
            Regex regex = new Regex(search,RegexOptions.IgnoreCase);
            List<Product> searchItems= new List<Product>();
            
            foreach(var item in products)
            {
                Match match = regex.Match(item.Name);
                if (match.Success)
                {
                    searchItems.Add(item);
                }
            }
            return View(searchItems);
        }

    }
}