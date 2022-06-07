using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Repository;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _itemRepository;


        public CartController(
            ApplicationContext context,
            UserManager<IdentityUser> userManager,
            IRepository<Cart> cartRepository,
            IRepository<CartItem> itemRepository
            )
        {
            _context = context;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userCart = await _context.Carts.FirstOrDefaultAsync(x=>x.UserId == user.Id);
                if(userCart == null)
                {
                    userCart = new Cart { UserId = user.Id };
                    await _cartRepository.CreateAsync(userCart);
                }
                List<CartItem> cartItems = _context.CartItems.Where(x => x.CartId == userCart.Id).ToList();

                if (cartItems.Count == 0)
                    return RedirectToAction(nameof(Empty));
                return View(cartItems);
            }
            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Empty() => View();

        [HttpGet]
        public async Task<IActionResult> ChangeQuantity(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            return PartialView(cartItem);
        }

        [HttpGet]
        public IActionResult AddToCart(int? id) => PartialView(id);

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userCart = await _context.Carts.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userCart == null)
                    return View();

                var cartItem = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == id);
                if (cartItem == null)
                    return View();

                await _itemRepository.DeleteAsync(cartItem.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeQuantity(CartItem cartItem)
        {
            await _itemRepository.UpdateAsync(cartItem);
            return RedirectToAction(nameof(Index));
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
                    await _cartRepository.CreateAsync(userCart);
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
                    await _itemRepository.UpdateAsync(existCartItem);
                }
                else
                {
                    await _itemRepository.CreateAsync(cartItem);
                }
                return LocalRedirect("/");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int? id)
        {
            if (id == null) return NotFound();

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null) return NotFound();

            await _itemRepository.DeleteAsync(cartItem.Id);
            return View();
        }
    }
}
