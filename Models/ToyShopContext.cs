using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ToyShop.Models;

public partial class ToyShopContext : DbContext
{
    public ToyShopContext()
    {
    }

    public ToyShopContext(DbContextOptions<ToyShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketProduct> BasketProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryEvent> CategoryEvents { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetailedView> ProductDetailedViews { get; set; }

    public virtual DbSet<ProductOrder> ProductOrders { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseProduct> WarehouseProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DIANAKASHINA\\SQLEXPRESS;Database=ToyShop;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.IdAddress).HasName("PK__Address__73ED14D388C3E138");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Address_User");
        });

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.IdBasket).HasName("PK__Basket__0F6F1DD1EC98B52D");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Basket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Basket_User");
        });

        modelBuilder.Entity<BasketProduct>(entity =>
        {
            entity.HasKey(e => e.IdBasketProduct).HasName("PK__Basket_P__A3AF7D4F88CD4A07");

            entity.HasOne(d => d.IdBasketNavigation).WithMany(p => p.BasketProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Basket_Product_Basket");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.BasketProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Basket_Product_Product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Category__6DB3A68AE4B89BED");
        });

        modelBuilder.Entity<CategoryEvent>(entity =>
        {
            entity.HasKey(e => e.IdCategoryEvent).HasName("PK__Category__5C75FAFD363CA85F");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.CategoryEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Category_Event_Category");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.CategoryEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Category_Event_Event");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.IdEmployee).HasName("PK__Employee__D9EE4F36649B2784");

            entity.HasOne(d => d.IdWarehouseNavigation).WithMany(p => p.Employees).HasConstraintName("FK_Employee_Warehouse");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PK__Event__12A4DF3F67CD556C");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Order__EC9FA95575BBFE6D");

            entity.ToTable("Order", tb => tb.HasTrigger("LogOrderDelete"));

            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Employee");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Product__522DE4969A59C35A");

            entity.ToTable("Product", tb => tb.HasTrigger("LogProductDelete"));

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.IdProviderNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Provider");
        });

        modelBuilder.Entity<ProductDetailedView>(entity =>
        {
            entity.ToView("ProductDetailedView");
        });

        modelBuilder.Entity<ProductOrder>(entity =>
        {
            entity.HasKey(e => e.IdProductOrder).HasName("PK__Product___9DDA7BF92A5298F4");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.ProductOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Order_Order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Order_Product");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.IdProvider).HasName("PK__Provider__935AEF88E32347D7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__ED4DE442E102BA1E");

            entity.ToTable("User", tb => tb.HasTrigger("LogNewUser"));

            entity.Property(e => e.Role).HasDefaultValue("Customer");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.IdWarehouse).HasName("PK__Warehous__6D0FABD27681CBA7");
        });

        modelBuilder.Entity<WarehouseProduct>(entity =>
        {
            entity.HasKey(e => e.IdWarehouseProduct).HasName("PK__Warehous__B62C27F954FEAF50");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.WarehouseProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warehouse_Product_Product");

            entity.HasOne(d => d.IdWarehouseNavigation).WithMany(p => p.WarehouseProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warehouse_Product_Warehouse");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
