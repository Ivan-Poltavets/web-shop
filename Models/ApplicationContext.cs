#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models
{
    public class ApplicationContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Product> Product { get; set; }
        public ApplicationContext (DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
