using System.Collections.Generic;
using ArtShop.Data.Entities;

namespace ArtShop.Data
{
    /*Interfata e pentru testing basically
     Sa lucrezi mai usor cu queries.*/

    /*Ce metode ai creat in Repository trebuie sa le mentionezi in IRepository (in interfata)*/
    public interface IArtShopRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string category);
       
        IEnumerable<Order> GetAllOrders(bool includeItems);
        IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems);
        Order GetOrderById(string username, int id);

        bool SaveAll();// SaveChanges()?
        void AddEntity(object model);//accept object in loc de Order, ca orice tip de entitate sa se poata adauga
    }
}