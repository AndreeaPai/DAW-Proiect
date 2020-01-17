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
using Microsoft.AspNetCore.Identity;

namespace ArtShop.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]//specificam sa nu folosim cookies,fol JWtB
    public class OrdersController : Controller
    {
        private readonly IArtShopRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUser> _userManager;

        public OrdersController(IArtShopRepository repository,   
            ILogger<OrdersController> logger,
            IMapper mapper,
            UserManager<StoreUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)//daca parametrul includeItems nu e inclus, include-l in mod default
        {
            try
            {
                var username = User.Identity.Name;

                var results = _repository.GetAllOrdersByUser(username, includeItems);
                
                //fiidca intoarce o colectie(_repository.GetAllOrders()), am nevoie de IEnumerable
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderModel>>(results));// pt V66
                // return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderModel>>(_repository.GetAllOrders()));
                //in , out (tipuri).returnezi o colectie deasemenea: IEnumerable<OrderModel>
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders.");
            }
        }

        [HttpGet("{id:int}")]//int = de ce tip sa fie id;daca nu specifici poate sa fie si string
        public IActionResult Get(int id)    
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, id);
                //va intoarce ok doar daca order nu este null; daca puneam totul in return,intorcea automat ok

                if (order != null)
                    return Ok(_mapper.Map<Order,OrderModel>(order));//daca order nu e null, returnam order inauntrul Ok
                /*2 Folosim Map sa mapam dintr-un Order intr-un OrderModel
                 * va lua comanda pe care o pasezi ( (order) ) si va returna a mapped version of the Model(2lea paramentru)
                 fiindca intotdeauna vom vrea sa returnam Models*/

                else return NotFound();//ret not found daca order e null(daca id nu e valid)
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderModel model)//parametru=date noi care le trimitem pe post
            /*2 schimbam din Order in OrderModel*/
        {
            //adaugam la baza de date
            try
            {
                /*2*/
                if (ModelState.IsValid)//daca required si minlenght din ContactModel sunt valide
                {
                    var newOrder = _mapper.Map<OrderModel, Order>(model);
                    
                    /*3 - in loc de de partea de sus am facut cu mapping:
                     * var newOrder = new Order()//2 trebuie sa convertim model(OrderModel) la Order
                    {
                        OrderDate = model.OrderDate,
                        OrderNumber = model.OrderNumber,
                        Id = model.OrderId
                    };*/

                    //2 validare: vom forta sa puna OrderDate daca userul nu a pus fiindca OrderDate nu a fost required in OrderModel
                    if (newOrder.OrderDate == DateTime.MinValue)//= daca n-au specificat OrderDate
                    {
                        newOrder.OrderDate = DateTime.Now; //se specifica acum
                    }

                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;

                    //adaugam comanda noua la db
                    _repository.AddEntity(newOrder);//2 ca avem OrdersModel entitatea devine newOrder(in loc de model)
                    if (_repository.SaveAll())//daca e salvat,returneaza Created
                    {
                        /* 3 inlocuim cu mapping:
                        //2 convertim din newOrder inapoi in Model (inversam):
                        var m = new OrderModel()
                        {
                            OrderId = newOrder.Id,
                            OrderDate = newOrder.OrderDate,
                            OrderNumber = newOrder.OrderNumber
                        };
                        */
                        //nu ->return Ok(model);//in post daca ai creat un nou obiect obligatoriu returnezi Created (nu Ok)
                       
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order, OrderModel>(newOrder));//Created e 201,nu 200
                        //3 fara mapping era: return Created($"/api/orders/{m.OrderId}", m);//return Created($"/api/orders/{m.OrderId}",m);
                         /* {model.Id} - sa se potriveasca cu HttpGet de la Get(id)
                         2 = model devine m*/
                    }
                }
                else //de la isValid
                {
                    return BadRequest(ModelState);//erorile care vin din partea modelState trimise de la user
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order: {ex}");
            }

            return BadRequest("Failed to save new order");
        }
    }
}
