using ArtShop.Data;
using ArtShop.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Controllers
{
    [Route("api/[Controller]")]//Setat ca toate URL-urile sa fie api/ce-o fi. Asta va fi conventia
    [ApiController]
    [Produces("application/json")]//acest api controller va returna mereu application json
    public class ProductsController : ControllerBase
    {
        private readonly IArtShopRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        //repository il folosim pt data

        public ProductsController(IArtShopRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;

        }
        
        [HttpGet]
        [ProducesResponseType(200)]//toate resp type care sunt asteptate de la metoda Get
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Product>> Get()  // NU IAction 
        {
            try
            {
                return Ok(_repository.GetAllProducts());// Ok=status code 200
            //vrei sa returnezi status code
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Failed to get products: {ex}");
                return BadRequest("Failed to get products.") ;//avem si return BadRequest fiindca met iti cere un return
                                                              //BadRequest=status 400
            }
        }
    }
}
