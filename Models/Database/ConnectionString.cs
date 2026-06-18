using EBookStore.Models.Filters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace EBookStore.Models.Database
{
    public class ConnectionString : DbContext
    {
        public ConnectionString(DbContextOptions<ConnectionString> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }     
        public DbSet<ActionRoute> ActionRoutes { get; set; }     
        public DbSet<RolePermission> RolePermissions { get; set; }     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region OrderProduct
            modelBuilder.Entity<OrderProduct>().HasKey(k => new { k.OrderID, k.ProductID });

            modelBuilder.Entity<OrderProduct>().HasOne(m => m.Order).WithMany(w => w.OrderProducts).HasForeignKey(k => k.OrderID);
            modelBuilder.Entity<OrderProduct>().HasOne(m => m.Product).WithMany(w => w.OrderProducts).HasForeignKey(k => k.ProductID);
            #endregion

            #region Cart
            modelBuilder.Entity<Cart>().HasOne(m => m.User).WithMany(w => w.Carts).HasForeignKey(k => k.UserID);
            modelBuilder.Entity<Cart>().HasOne(m => m.Product).WithMany(w => w.Carts).HasForeignKey(k => k.ProductID);
            #endregion

            #region Authorization
                modelBuilder.Entity<ActionRoute>().HasIndex(e => new { e.Controller, e.Action }).IsUnique();  //Controller and Action together Unique
                modelBuilder.Entity<RolePermission>().HasIndex(e => new { e.ActionRouteID, e.RoleID }).IsUnique();  //ActionRouteID and RoleID together Unique
            #endregion

            modelBuilder.Entity<Author>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<Category>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<Cover>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<PaymentMethod>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<Product>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<Publisher>().HasIndex(h => h.Name).IsUnique();

            modelBuilder.Entity<User>().HasIndex(h => h.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(h => h.Phone).IsUnique();

            modelBuilder.Entity<UserType>().HasIndex(h => h.Name).IsUnique();
            
            #region Cascade
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion

            base.OnModelCreating(modelBuilder);
        }

    }
}
