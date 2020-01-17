using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtShop.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArtShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            //BuildWebHost(args).Run();

            //pt seeding:
            var host = BuildWebHost(args);
            //instantiam si built up Seeder:

            //RunSeeding(host);
            SeedDb(host);

            host.Run();
        }

        private static void SeedDb(IWebHost host)
        {
            /*Part 2 (Dupa services din StartUp):
             * ca seeder sa fie bulletproof: ArtShopSeeder contine o scoped dependency;ffindca in Startup, AddDbCOntext defapt la creat ca scoped service si din cauza asta
             * avem nevoie de o noua clasa numita scopeFactory
             * un mod pe langa web server sa creezi un scope*/

            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope()) //folosim using ca sa se inchida scope-ul dupa ce si-a terminat treaba
            {
                //Part 1 :obtinem obiectul seeder:
                var seeder = scope.ServiceProvider.GetService<ArtShopSeeder>();
                //scope.ServiceProvider. = asa luam serviciile din inauntrul scope-ului
                seeder.SeedAsync().Wait();//aici creaza o instanta care incearca sa proceseze toate dependentele de la : host.Services.GetService<ArtShopSeeder>();
                              //Dupa seeder.Seed() dute in Startup sa faci register
            }
        }

        /* CreateDefaultBuilder = creeaza un fisier default configuration pe care putem sa-l folosim
         * Contruim configuratia manual, sa vedem benefiicile:
         * Configuram configuratia, sa ne permita sa facem apel dupa ce cream default builder numit ConfigureAppConfiguration
         si asta ia un delegat care i se paseaza un conf bui sa putem sa adaugam propriile optiuni de configuratie
         si fac asta printr-o met separata = SetupConfiguration
         ii cer lui V Studio sa genereze metoda asta jos
             */
        public static IWebHost BuildWebHost(string[] args) => //CreateWebHostBuider
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(SetupConfiguration)//in () i se paseaza un configuration builder ca sa putem sa ne facem propriile optiuni de configuratie
            .UseStartup<Startup>()
            .Build();

        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.Sources.Clear(); // scoate optiunile de configurare default

            builder.AddJsonFile("config.json", false, true)//avem nevoie de un fis config.json;false-fis nu e optional;reload on change = true-daca ruleaza aplicatie, reporneste fis json cand o schimbare e facuta
            //daca primesc configuratii din mai multe locuri:(jos)
                //.AddXmlFile("config.xml", true) - exemplu, nu-l folosesc
                .AddEnvironmentVariables();
            
            /*ca sa nu existe conflict sist trateaza configuratiile de mai sus ca ierarhii:
             daca o config e salvata in 2 :AddJsonFile si AddEnvironmnetVariables(most trust worthy) ,AddEnvironmnetVariables castiga fiindca e ultima */
        }

        /*public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });*/

    }
}
