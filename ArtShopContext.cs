using ArtShop.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


 namespace ArtShop.Data
{//are nevoie de referinta - package
    public class ArtShopContext : IdentityDbContext<StoreUser> //ia StoreUser ca identity type
    {

        public ArtShopContext(DbContextOptions<ArtShopContext> options): base(options)//nu facem nimic cu optiunile decat le pasam clasei base
        {
        }

        //proprietati de tip dbSet<tipulEntitatii> Nume(de obicei plurarul)
        //dupa asta putem sa facem queries sau sa adaugam date la db
        //optional pt OrderItem fiindca Order are o relatie cu OrderItems(unless we want to query OrderItems across orders) nu avem nevoie de ea
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //poti sa specifici aici proprietati ale Models; astea sunt folosite de migrations cand se creaza

            modelBuilder.Entity<Order>() //
                /*.Property(p => p.Title) //vrem sa spunem ca proprietatea pentru titlu Product 
                .HasMaxLength(50);     */

                .HasData(new Order() //permite sa adaugam seeded data la Model, adaugand-o la migration
                {
                    Id = 1,
                    OrderDate = DateTime.UtcNow,
                    OrderNumber = "12345"
                });    
                }
    }
}
