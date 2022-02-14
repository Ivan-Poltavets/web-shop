#nullable disable
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ProductsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _environment;


        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product,IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if(uploadFile != null)
                {
                    string extension = Path.GetExtension(uploadFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    string pathToSave = Path.Combine("wwwroot", "Upload", fileName);
                    using(var stream = new FileStream(pathToSave, FileMode.Create))
                    {
                        await uploadFile.CopyToAsync(stream);
                    }
                    product.ImageName = fileName;
                }
                else
                {
                    product.ImageName = "null";
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            string pathFile = Path.Combine("wwwroot", "Upload", product.ImageName);
            FileInfo file = new FileInfo(pathFile);
            file.Delete();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Upload(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        [HttpPost,ActionName("Upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id,IFormFile uploadFile)
        {
            if(uploadFile != null)
            {
                var product = await _context.Products.FindAsync(id);
                string directory = Path.Combine("wwwroot", "Upload");
                FileInfo deleteFile = new FileInfo(Path.Combine(directory, product.ImageName));
                deleteFile.Delete();
                string extension = Path.GetExtension(uploadFile.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                var pathToSave = Path.Combine(directory, fileName);
                using(var stream = new FileStream(pathToSave, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }
                product.ImageName = fileName;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
