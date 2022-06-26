using BakerTask.DAL;
using BakerTask.Extentions;
using BakerTask.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BakerTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _sql;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext sql, IWebHostEnvironment env)
        {
            _sql = sql;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _sql.Products.Include(p => p.Images).ToListAsync();
            return View(products);
        }
        public IActionResult Create()
        {
            //ViewBag.Images = _sql.Products.Include(p => p.Images).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View();
            if (_sql.Products.Any(p => p.Name == product.Name))
            {
                ModelState.AddModelError("Name", "Bu adda mehsul movcuddur!");
                return View();
            }
            if (product.ImagesFile != null)
            {
                if (CheckImage(product.ImagesFile) != "")
                {
                    ModelState.AddModelError("ImagesFile", CheckImage(product.ImagesFile));
                    return View();
                }
                product.Images = new List<ProductImage>();
                foreach (var itemFile in product.ImagesFile)
                {
                    ProductImage productImage = new ProductImage
                    {
                        Image = itemFile.SaveImage(_env.WebRootPath, "assets/images"),
                        IsMain = false,
                        ProductId = product.Id
                    };
                    product.Images.Add(productImage);
                }
                if (product.MainImage != null)
                {
                    if (!product.MainImage.IsImage())
                    {
                        ModelState.AddModelError("MainImage", "Sekilin formati duzgun deyil!");
                        return View();
                    }
                    if (!product.MainImage.IsSizeOk(5))
                    {
                        ModelState.AddModelError("MainImage", "Sekil max 5 mb ola biler!");
                        return View();
                    }
                    ProductImage productImage = new ProductImage
                    {
                        Image = product.MainImage.SaveImage(_env.WebRootPath, "assets/images"),
                        IsMain = true,
                        ProductId = product.Id
                    };
                    product.Images.Add(productImage);
                }
            }
            await _sql.Products.AddAsync(product);
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            Product product = _sql.Products.Include(p=>p.Images).FirstOrDefault(p=>p.Id==id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Product product)
        {
            if (!ModelState.IsValid) return View();
            Product productExist = await _sql.Products.Include(p=>p.Images).FirstOrDefaultAsync(p=>p.Id==id);
            if (productExist == null) return NotFound();
            //productExist.Images = await _sql.Products.Include(p => p.Images==productExist.Images).ToListAsync();
            if (product.ImagesFile != null)
            {
                if (CheckImage(product.ImagesFile) != "")
                {
                    ModelState.AddModelError("ImagesFile", CheckImage(product.ImagesFile));
                    return View();
                }
                foreach (var item in productExist.Images)
                {
                    Helpers.Helper.DeleteImg(_env.WebRootPath,"assets/images",item.Image);
                }
                foreach (var item in product.ImagesFile)
                {
                    item.SaveImage(_env.WebRootPath, "assets/images");
                    ProductImage productImage = new ProductImage
                    {
                        Image=item.FileName,
                        IsMain=false,
                        ProductId=productExist.Id
                    };
                    productExist.Images.Add(productImage);
                }
            }
            if (product.MainImage!=null)
            {
                if (product.MainImage.IsImage())
                {
                    ModelState.AddModelError("MainImage","Sekilin formati duzgun deyil!");
                    return View();
                }
                if (product.MainImage.IsSizeOk(5))
                {
                    ModelState.AddModelError("MainImage", "Sekil 5 mb-dan boyuk ola bilmez!");
                    return View();
                }
                Helpers.Helper.DeleteImg(_env.WebRootPath,"assets/images",productExist.Images.Where(i=>i.IsMain).FirstOrDefault().Image);
                ProductImage mainImage = new ProductImage
                {
                    Image =product.MainImage.SaveImage(_env.WebRootPath,"assets/images"),
                    IsMain = true,
                    ProductId=productExist.Id
                };
                productExist.Images.Add(mainImage);
            }

            if (product.IsDelete == false) productExist.IsDelete = false;
            productExist.Name = product.Name;
            productExist.Description = product.Description;
            productExist.Price = product.Price;
            productExist.Rating = product.Rating;

            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            Product product = _sql.Products.Find(id);
            if (product == null) return View();
            _sql.Products.Remove(product);
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
        public static string CheckImage(IFormFileCollection imagesFile)
        {
            foreach (var item in imagesFile)
            {
                if (!item.IsImage())
                {
                    return "Sekilin formati duzgun deyil!";
                }
                if (!item.IsSizeOk(5))
                {
                    return "Sekil 5 mb-dan boyuk ola bilmez!";
                }
            }
            return "";
        }
    }
}
