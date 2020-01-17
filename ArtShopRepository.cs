using ArtShop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Data
{
    //In aceasta clasa voi injecta ArtShopContext.
    public class ArtShopRepository : IArtShopRepository
    {
        private readonly ArtShopContext _ctx;
        private readonly ILogger<ArtShopRepository> _logger;
        public ArtShopRepository(ArtShopContext ctx, ILogger<ArtShopRepository> logger) //logger va fi legat de acest tip, iar atunci cand ma emita data vom putea vedea de un sa facut logarea
        {
            _ctx = ctx;
            _logger = logger;
        }

        //apeluri care returneaza data
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts was called");// cand cheman GetAllProducts folosim logger pt a loga informatii

                //returnez produsele folosindu-ma de context
                return _ctx.Products
                           .OrderBy(p => p.Title)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }
        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
           if (includeItems)
             {
            return _ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();//cu Include ii spunem sa includa item cand intoarce datele din metoda
            } 
            else 
            { //altfel intoarcem o lista fara items
                return _ctx.Orders
                   .ToList();
            }
        }
        public Order GetOrderById(string username, int id)
        {
            //return _ctx.Orders.Find(id)//ar merge dar nu ar ok fiindca la celalta metoda am inclus Includ
             return _ctx.Orders
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Product) //nu luam doar item ci si product information pt fiecare din item
                        .Where(o => o.Id == id && o.User.UserName == username) // &&...=garanteaza c cineva care cere un id pt un order care nu ii apartine nu o va putea primi
                        .FirstOrDefault();//=firstOrDefault va face intrega query,o va gasi pe cea din id,si va intoarce
            /*primul rezultat sau null daca nu a gasit*/

            //ToList intorcea o colectie, de aia nu poti include mai sus
        }

        //userul paseaza categoria 
        //simple API for getting data
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return _ctx.Products
                       .Where(p => p.Category == category)
                       .ToList();
        }

        public bool SaveAll()
        {
            //SaveChanges returneaza numarul de randuri afectate
            /* => SaveAll functioneaza daca nr rand afectate este > 0*/
            return _ctx.SaveChanges() > 0;
        }

        public bool SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
        {
            if (includeItems)
            {
                return _ctx.Orders
                        .Where(o => o.User.UserName == username)
                        .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                        .ToList();
            }
            else
            { 
                return _ctx.Orders
                   .Where(o => o.User.UserName == username)
                   .ToList();
            }
        }
    }
}
