using ArtShop.Data.Entities;
using ArtShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArtShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<StoreUser> _signInManager;
        private readonly UserManager<StoreUser> _userManager;
        private readonly IConfiguration _config;

        /*serviciu SignInManager inainte de a imp login/logout*/
        public AccountController(ILogger<AccountController> logger, SignInManager<StoreUser> signInManager,
            UserManager<StoreUser> userManager, IConfiguration config) //ult 2 pt token
            
        /*SignInManager ca param tip user,in cazul nostru StoreUser*/
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        public IActionResult Login()
        {
            //user e prop pe controller
            if (this.User.Identity.IsAuthenticated)//daca cineva s-a logat
            {
                return RedirectToAction("Index", "Home");//isi da seama ce url este pt index la home controller si redirect
                //return RedirectToAction("Index", "App");
            }
            return View();
        }
        /*fiindca form din View, Login.cshtml odata ce e submitet il va gasi in Login din AccountController*/
        [HttpPost]
        public async Task <IActionResult> Login(LoginModel model)//trimitem date de tip LoginModel
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username,   //ne lasa sa facem sign nu user si parola si nu ne mai trebuie store object
                    model.Password,
                    model.RememberMe,
                    false); // false = te blocheaza daca user si parola sunt puse gresit
                    

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))//Keys ,adica key din perechea key-value
                    {
                        Redirect(Request.Query["ReturnUrl"].First());//first=ia prima valoare din query string
                    }
                    else
                    {
                        RedirectToAction("Shop", "Home"); //return RedirectToAction("Index", "App");
                    }
                }
            }
            //daca toate au esuat:
            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        [HttpGet]
        public async Task <IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); 
            //ne intoarce in home pag dar ne si sign out
        }
        
        [HttpPost]//fiindca nu vrem sa includ credentials in header sau query stream
        public async Task<IActionResult> CreateToken([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);//luam numele din model.Username

                if (user != null)//daca userul a fost gasit
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password, false);//vf parola

                    if (result.Succeeded) 
                    {
                        //daca a result merge
                        var claims = new[] //proprietati cu valori cunoscute ce pot fi stocate in tokens ce pot fi fol de claims sau pasate inapoi in server
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email), //nume standard fol pt token
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) //UniqueName=username lui user
                        };
                        /*deci asa toke va contine suficienta info sa lege current user in api cu store user*/

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));//key=secret cu care se incripteaza token
                                                                                                         //citesc asta din configuration
                                                                                                         //["Token:Key"] - o creez in config.json
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        //creeam token:

                        var token = new JwtSecurityToken(
                            _config["Tokens:Issuer"], //cine la creat; in config il creezi
                            _config["Tokens:Audiece"], //cine il foloseste // idem
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: creds
                            );
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo//ret un timp de expirare
                        };
                        return Created("", results);
                    }
                }
            }
            return BadRequest();
        }
    }
}
