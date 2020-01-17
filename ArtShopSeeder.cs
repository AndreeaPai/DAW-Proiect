using ArtShop.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Data
{
    public class ArtShopSeeder
    {
        private readonly ArtShopContext _ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<StoreUser> _userManager;
        public ArtShopSeeder(ArtShopContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> userManager)
            //ia StoreUser ca parametru generic ca sa stie ce fel de views managerul le poate manage 
        {
            _ctx = ctx;
            _hosting = hosting;
            _userManager = userManager;

        }

        public async Task SeedAsync() //daca o operatie devine async o putem adauga la nume
        {
            _ctx.Database.EnsureCreated();//vf daca baza de date exista

            //dupa ce a fost creata db ne uitam dupa store user

            //user mana ne permite sa gasim dupa email
            StoreUser user = await _userManager.FindByEmailAsync("andreea@artstore.com");
            //fiindca a async trebuie sa folosim await

            if (user == null) // daca userul nu exista il creeam
            {
                user = new StoreUser()
                {
                    FirstName = "Andreea",
                    LastName = "Poclitaru",
                    Email = "andreea@artstore.com",
                    UserName = "andreea@artstore.com"
                };

                var result = await _userManager.CreateAsync(user, "par0laANDREEA!"); //dupa ce avem datele creeam user
                //prin default ne cere upper, lower ,numer
                if (result != IdentityResult.Success) //daca nu a creeat noul user
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
            }

            if (!_ctx.Products.Any()) //returneaza adev daca exista Products in DB = select count(Products);if count>0 return true
            {
                /* Daca NU(!!) exista Products in DB trebuie sa cream sample data
             Prima oara luam date din fisierul art.json:    
             */
                var filepath = Path.Combine(_hosting.ContentRootPath, "Data/art.json");//directorul pt root a proiectului = _hosting.ContentRootPath
                var json = File.ReadAllText(filepath);

                /*Luam lista produselor folosind o librarie*/

                var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json); // DeserializeObject<tipul in care vrem sa serializam> si stiu ca art.json este un IEnumerable de entitate Product
                //linia de sus ne returneaza o lista de produse 

                _ctx.Products.AddRange(products);//addRange=fiindca este o colectie
                //returneaza produsele

                //DAR daca am vrea sa adaugam orders :

                var order = _ctx.Orders.Where(o => o.Id == 1).FirstOrDefault();
                if (order != null) 
                {
                    order.User = user; //folosim user: va update comanda sa fie a user-ului
                    /*setam user ca prop a Order prin context*/
                    order.Items = new List<OrderItem>() // adaugam in order items
                    {
                        new OrderItem() //folosim o initializare sa cream un nou order item
                        {
                            Product = products.First(), //iau primul produs
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    };

                } 

                _ctx.SaveChanges();//produsele sunt adaugate in database
            }
        }
    }
}
