using AutoMapper;
using ArtShop.Data;
using ArtShop.Data.Entities;
using ArtShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ArtShop.Controllers
{
    [Route("/api/orders/{orderid}/items")]//subcontroller a lui order
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]//limitam orderItem la token
    public class OrderItemsController : Controller
    {
        private readonly IArtShopRepository _repository;
        private readonly ILogger<OrderItemsController> _logger;
        private readonly IMapper _mapper;

        public OrderItemsController(IArtShopRepository repository, ILogger<OrderItemsController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
      
        [HttpGet]
        public IActionResult Get(int orderId) 
        { //sau cu get repository call pt get items sau cu:
            var order = _repository.GetOrderById(User.Identity.Name, orderId); //orderId = e mapat aici fiindca e in url de mai sus de la Route
            if (order != null) //return Ok(order.Item);
                return Ok(_mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemModel>>(order.Items));//return thr colletion of data
            return NotFound();
        }


        [HttpGet("{id}")]//sa returnam un item individual, dupa id
        public IActionResult Get(int orderId, int id)//orderItemId
        {
            var order = _repository.GetOrderById(User.Identity.Name, orderId);
            if (order != null)
            {//daca avem order
                var item = order.Items.Where(i => i.Id == id).FirstOrDefault();//unde item id este id pe care il cerem
                //ne garanteaza ca cautam un id in contextul unei comenzi(sus)
                if (item != null)
                {
                    return Ok(_mapper.Map <OrderItem,OrderItemModel>(item));
                }
            }
            return NotFound();
        }
    }
}
