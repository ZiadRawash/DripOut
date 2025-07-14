using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
namespace DripOut.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<AppUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Image> Images { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ProductVariant> ProductVariants { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Favourite> Favourites { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Favourite>().HasKey(f => new { f.AppUserId, f.ProductId });
			builder.Entity<Favourite>()
				.HasOne(f => f.AppUser)
				.WithMany(u => u.Favourites)
				.HasForeignKey(f => f.AppUserId);
            builder.Entity<Favourite>()
                .HasOne(f => f.Product)
                .WithMany(u => u.Favourites)
                .HasForeignKey(f => f.ProductId);

            List<IdentityRole> roles = new List<IdentityRole>
			{
				new IdentityRole
				{
					Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					Name = "Admin",
					NormalizedName = "ADMIN"
				},
				new IdentityRole
				{
					 Id = "b2c3d4e5-f678-90ab-cdef-1234567890ab",
					Name = "User",
					NormalizedName = "USER"
				},
			};
			builder.Entity<IdentityRole>().HasData(roles);

			builder.Entity<AppUser>()
				.HasMany(u => u.Reviews)
				.WithOne(r => r.User)
				.HasForeignKey(r => r.AppUserId)
				.OnDelete(DeleteBehavior.Cascade);
			builder.Entity<AppUser>()
			.HasOne(u => u.Image)
			.WithOne(i => i.AppUser)
			.HasForeignKey<Image>(i => i.AppUserId)
			.OnDelete(DeleteBehavior.Cascade);
		}


    }
}
