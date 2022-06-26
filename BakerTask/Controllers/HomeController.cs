using BakerTask.DAL;
using BakerTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BakerTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _sql;

        public HomeController(AppDbContext sql)
        {
            _sql = sql;
        }

        public IActionResult Index()
        {
            List<Product> products = _sql.Products.Include(p => p.Images).ToList();
            return View(products);
        }

    }
}
