using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private Cart UserCart { get; set; }
        public  CartController(
            ApplicationContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userCart = await _context.Carts.FirstOrDefaultAsync(x=>x.UserId == user.Id);
                if (userCart == null)
                    return RedirectToAction(nameof(Empty));
                return View(_context.CartItems.Where(x=> x.CartId == userCart.Id).ToList());
            }
            return Unauthorized();
        }
        public IActionResult Empty()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userCart = await _context.Carts.FirstOrDefaultAsync(x=>x.UserId == user.Id);
                if (userCart == null)
                    return View();
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == id);
                if (cartItem == null)
                    return View();
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangeQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            return PartialView(cartItem);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeQuantity(CartItem cartItem)
        {
            _context.Update(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AddToCart(int? id)
        {
            return PartialView(id);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userCart = new Cart { UserId = user.Id };
                if (!await _context.Carts.AnyAsync(x => x.UserId == userCart.UserId))
                {
                    _context.Carts.Add(userCart);
                    await _context.SaveChangesAsync();
                }
                var cart = await _context.Carts.FirstOrDefaultAsync(x => x.UserId == userCart.UserId);
                CartItem cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = id,
                    Quantity = 1
                };
                if (await _context.CartItems.AnyAsync(x => x.ProductId == id && x.CartId == cart.Id))
                {
                    var existCartItem = _context.CartItems.FirstOrDefault(x => x.ProductId == id && x.CartId == cart.Id);
                    existCartItem.Quantity += 1;
                    _context.CartItems.Update(existCartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.CartItems.Add(cartItem);
                    await _context.SaveChangesAsync();
                }
                return LocalRedirect("/");
                ;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int? id)
        {
            if (id == null)
                return NotFound();
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
                return NotFound();
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return View();
        }
    }
}
