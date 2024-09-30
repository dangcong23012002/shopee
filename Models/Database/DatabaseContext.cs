﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Project.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<SellerInfo> SellerInfos { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<SliderShop> SliderShops { get; set; }
        public DbSet<ParentCategory> ParentCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Payment> PaymentTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<AddressChoose> AddressChooses { get; set; }
        public DbSet<ShippingOrder> ShippingOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasKey(e => e.PK_iUserID);
            });

            modelBuilder.Entity<UserInfo>(entity => {
                entity.HasKey(e => e.PK_iUserInfoID);
            });

            modelBuilder.Entity<Seller>(entity => {
                entity.HasKey(e => e.PK_iSellerID);
            });

            modelBuilder.Entity<SellerInfo>(entity => {
                entity.HasKey(e => e.PK_iSellerID);
            });

            modelBuilder.Entity<Store>(entity => {
                entity.HasKey(e => e.PK_iStoreID);
            });

            modelBuilder.Entity<SliderShop>(entity => {
                entity.HasKey(e => e.PK_iSliderShopID);
            });

            modelBuilder.Entity<ParentCategory>(entity => {
                entity.HasKey(e => e.PK_iParentCategoryID);
            });

            modelBuilder.Entity<Category>(entity => {
                entity.HasKey(e => e.PK_iCategoryID);
            });

            modelBuilder.Entity<Product>(entity => {
                entity.HasKey(e => e.PK_iProductID);
            });

            modelBuilder.Entity<Favorite>(entity => {
                entity.HasKey(e => e.PK_iFavoriteID);
            });

            modelBuilder.Entity<User>(entity => {
                entity.HasKey(e => e.PK_iUserID);
            });

            modelBuilder.Entity<Cart>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Payment>(entity => {
                entity.HasKey(e => e.PK_iPaymentTypeID);
            });

            modelBuilder.Entity<Order>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.Entity<CartDetail>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OrderDetail>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Address>(entity => {
                entity.HasKey(e => e.PK_iAddressID);
            });

            modelBuilder.Entity<City>(entity => {
                entity.HasKey(e => e.PK_iCityID);
            });

            modelBuilder.Entity<District>(entity => {
                entity.HasKey(e => e.PK_iDistrictID);
            });

            modelBuilder.Entity<AddressChoose>(entity => {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ShippingOrder>(entity => {
                entity.HasKey(e => e.PK_iShippingOrderID);
            });
        }
    }
}
