﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.Models;

namespace Pustok_BackEndProject.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Basket> Baskets  { get; set; }
        public DbSet<WishList> WishLists  { get; set; }
        public DbSet<Review> Reviews { get; set; }

    }
}
