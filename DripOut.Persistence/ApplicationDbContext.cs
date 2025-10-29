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
		public DbSet<Favourite> Favorites { get; set; }
		public DbSet<ReviewVote> ReviewVotes { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Governorate> Governorates { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ReviewVote>()
				.HasOne(rv => rv.User)
				.WithMany(u => u.ReviewVotes)
				.HasForeignKey(rv => rv.AppUserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ReviewVote>()
				.HasOne(rv => rv.Review)
				.WithMany(r => r.ReviewVotes)
				.HasForeignKey(rv => rv.ReviewId)
				.OnDelete(DeleteBehavior.Restrict); 


			builder.Entity<Favourite>().HasKey(f => new { f.AppUserId, f.ProductId });
			builder.Entity<Favourite>()
				.HasOne(f => f.AppUser)
				.WithMany(u => u.Favorites)
				.HasForeignKey(f => f.AppUserId);
            builder.Entity<Favourite>()
                .HasOne(f => f.Product)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.ProductId);





			// OrderItem relationships
			builder.Entity<OrderItem>()
				.HasOne(oi => oi.Order)
				.WithMany(o => o.OrderItems)
				.HasForeignKey(oi => oi.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<OrderItem>()
				.HasOne(oi => oi.ProductVariant)
				.WithMany(pv => pv.OrderItems)
				.HasForeignKey(oi => oi.ProductVariantId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<StockReservation>()
				.HasOne(sr => sr.OrderItem)
				.WithOne(oi => oi.StockReservation)  // Fixed: Added inverse navigation
				.HasForeignKey<StockReservation>(sr => sr.OrderItemId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<StockReservation>()
				.HasOne(sr => sr.ProductVariant)
				.WithMany(pv => pv.StockReservations)  // Fixed: Use the collection navigation
				.HasForeignKey(sr => sr.ProductVariantId)
				.OnDelete(DeleteBehavior.Cascade);








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
