using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ArtShop.Data;
using ArtShop.Data.Entities;
using ArtShop.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ArtShop
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            //Configuration = configuration;
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<StoreUser, IdentityRole>( cfg => 
            {
                cfg.User.RequireUniqueEmail = true;//userul are email unic
                //cfg.Password. - cat de puternica sa fie parola
            })
                .AddEntityFrameworkStores<ArtShopContext>();//luam data din context

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer( cfg => //json web token
                {
                    {
                        cfg.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = _configuration["Token:Issuer"],
                            ValidAudience = _configuration["Token:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
                        };
                    }
                }
                );

            services.AddTransient<IMailService, NullMailService>();
            //suport pt service mail adevarat
            //n-au date cu sine insusi, doar metode care fac lucruri; 
            //<numele serv sau in cazul nsotru numele interfetei, implementarea vrem sa fie NullMailService>
            // services.AddScoped   //pt adaug. serv care sunt mai scumpe de creat
            //  services.AddSingleton//pt serv care sunt create o singura data 

            services.AddControllersWithViews();//lipsa D

            // services.AddDbContext<ArtShopContext>();//spune sa faca dbContext parte din service collection sa o injecteze in diferite servicii de care am nevoie, de exp un controller
            services.AddDbContext<ArtShopContext>(cfg =>
           {
               cfg.UseSqlServer(_configuration.GetConnectionString("ArtShopConnectionString")); //ia ArtShopConnectionString din config.json
            });

            services.AddTransient<ArtShopSeeder>();//ArtShopSeeder=un tip care poate fi creat prin depency injection la nivelul Service

            services.AddScoped<IArtShopRepository, ArtShopRepository>(); //fiindca vreau ca Repository sa fie intr-un singur scope;ca sa nu il mai construiesti de mai multe ori
                                                                               /*Spune asa: adauga IArtShopRepository ca un serviciu pe care 
                                                                                * userii il pot folosi, dar foloseste implementarea din ArtShopRepository*/

            services.AddAutoMapper(Assembly.GetExecutingAssembly());//ii spune lui automapper sa caut profiles

            services.AddMvc();
            //.AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
           // .AddNewtonsoftJson(); // in loc de AddJsonOptions


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication(); //sa fie inainte de routing si endpoints; identifica user
            app.UseAuthorization();   //la ce are acces user

            app.UseRouting();//porneste generic routing in asp.net core 

            app.UseNodeModules();

            app.UseEndpoints(endpoints => //pasam lambda sa specificam cum folosim si ascultam diferite endpoints
            {
                endpoints.MapControllerRoute(//= fiindca vrem sa cream un endpoint care zice sa cautam controllers folosim semantica asta:
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"); //ii spui la routing system cum sa cauti un controller specific
             // cream un obiect anonim care are defaults:
             // new { controller = "Home", action = "Index" });
             //, dupa action ne duce in public IActionResult Index()
            });
        }
    }
}
