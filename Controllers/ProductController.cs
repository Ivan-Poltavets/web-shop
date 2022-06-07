#nullable disable
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using OnlineShop.Repository;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ProductController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<Product> _repository;

        public ProductController(ApplicationContext context, IRepository<Product> repository)
        {
            _context = context;
            _repository = repository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());

        [HttpGet]
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

        [HttpGet]
        public IActionResult Create() => View();

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> Upload(int? id)
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
                        await uploadFile.CopyToAsync(stream);
                    product.ImageName = fileName;
                }
                else
                {
                    product.ImageName = "null";
                }
                await _repository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(product);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            string pathFile = Path.Combine("wwwroot", "Upload", product.ImageName);
            FileInfo file = new FileInfo(pathFile);
            file.Delete();
            await _repository.DeleteAsync(product.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost,ActionName("Upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id,IFormFile uploadFile)
        {
            if(uploadFile != null)
            {
                var product = await _repository.GetByIdAsync(id);
                string directory = Path.Combine("wwwroot", "Upload");
                
                if (product.ImageName != null)
                {
                    FileInfo deleteFile = new FileInfo(Path.Combine(directory, product.ImageName));
                    deleteFile.Delete();
                } 
                string extension = Path.GetExtension(uploadFile.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                var pathToSave = Path.Combine(directory, fileName);
                using(var stream = new FileStream(pathToSave, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }
                product.ImageName = fileName;
                await _repository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return BadRequest();
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
