using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Infrastructure
{
    //NOT: DOMAİN HANGİ ORTAMDA HANGİ ORM ARACIYLA ÇALŞTIĞINI BİLMEMELİ
    //FreeCourse.Services.Order.Domain  dependencies den AddReferance dan eklemeyi unutma
    //Nuget packeges den Microsoft.EntityFrameworkCore - Microsoft.EntityFrameworkCore.Tools - Microsoft.EntityFrameworkCore.SqlServer kur
    public class OrderDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "ordering"; //

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.OrderAggregate.Order> Orders { get; set; } //veritabanı tablo ismini belirli
        public DbSet<Domain.OrderAggregate.OrderItem> OrderItems { get; set; }//veritabanı tablo ismi 2 yukarıdakiyle bire çok ilişki var.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA); //
            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);

            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner();

            base.OnModelCreating(modelBuilder);
        }
    }
}