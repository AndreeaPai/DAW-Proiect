using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ArtShop.Models;
using ArtShop.Services;
using ArtShop.Data;
using Microsoft.AspNetCore.Authorization;

namespace ArtShop.Controllers
{
    public class HomeController : Controller //cl HomeController mosteneste clasa din MVC Controller
                                             //Controller-ul ne permite să mapăm cererea care vine la o Actions specifica
    {
        private readonly IMailService _mailService;
        private readonly IArtShopRepository _repository;

        //jos:folosim constr sa injectam serv de care avem nevoie(serv scrie ca de parametru IMailService si l-am numit mailService )
        public HomeController(IMailService mailService, ILogger<HomeController> logger, IArtShopRepository repository)//ArtShopContext ctx - am adaugat sa ma acces la Shop()
        {
            _mailService = mailService;
            _logger = logger;
            _repository = repository;
        }

        private readonly ILogger<HomeController> _logger;
        /*
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        */
        //ActionResult stie ce se intampla in metoda, o mapeaza la un View si o returneaza
        public IActionResult Index()
        {
            //var results = _ctx.Products.ToList();//sa afiseze toate produsele din baza de date = pt seeding
            return View();
            //View de aici se uita in folder-ul cu Views ( asa e conventia ) bazanduse pe numele controller-ului,care e Home.
            //Deci Home folder, din folder Views, va contine toate Views pentru Home Controller.
            //Si numele View-ului va fi numele metodei, adica care este actiunea, adica Index
            //Si View() de mai sus NU reprezinta html, ci Razor.
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // pag pt Contact 

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            //ViewData["Title"] = "Contact Us";
            return View();
        }


        //browser ne trimite inapoi informatii; metoda asta se cheama post fiindca <form> din Contact.cshtml are method de post
        //metoda asta este cand trimiti inapoi date; dupa ce a pus user date in form si sa click pe buton send

        [HttpPost("contact")]
        //public IActionResult Contact(object model) //(1) daca ne uitam la model s-a instantiat ca obiect, dar nu are date asociate cu asta
        //asta fiindca in Contact.cshtml input si textarea nu au un nume asociate cu ele

        public IActionResult Contact(Contact model)//(2) in loc de ia model,ii spunem sa ia Contact(de la Models folder)
        {//va lua si potrivi names de la field-urilor input si textarea la proprietatile de la Contact Model.

            //modalitate in care ne asiguram ca datele primite de la server urmeaza regurile impuse din Contact.cs(Models)
            if (ModelState.IsValid)
            {
                //send the email

                //mod rapid de a trimite mail via un Service
                _mailService.SendMessage("andreea@p.com", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");//harcode an email
                                                                                                                                          // sa folosim serv de mail in clasa noastra am creat:NullMailService.cs, IMailService.cs si in Startup.cs la ConfigureServices o vom adauga ca service configurat

                //sa vedem ca s-a trimis email":
                ViewBag.UserMessage = "Mail Sent";
            }
            //either case, return View

            return View();
        }
       // [HttpGet("shop")] // ???????
        [Authorize] //= cine va folosi asta va trebui sa fie logat sau sa aiba crendetials
        public IActionResult Shop()     
        {
            //doar trebuie sa arat form care va incarca componenta Angular

            return View();

            //link Query
            //ordonam date care apoi le pasam in View
            /* var results = from p in _ctx.Products
                          orderby p.Category
                          select p;*/

            // sau 
            //sintaxa fluida 
            //var results = _ctx.Products
            //    .OrderBy( p => p.Category) // le ordoneaza dupa categorie
            //    .ToList(); //();// se duce in db, ia toate produsele si le returneaza 
            //return View(); // vreau sa fac query pt toate produsele si sa le trimit in pag Shop si ma duc constructor, adaug ArtShopContext ctx
        }

        [HttpGet("rent")]
        public IActionResult Rent()     
        {
            return View();
        }
        public IActionResult About()
        {
            //ViewData["Title"] = "About us";
            return View();

        }
    }
}
