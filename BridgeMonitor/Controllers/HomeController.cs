using BridgeMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;

namespace BridgeMonitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var Ponts = GetBridgeMonitorFromApi();
            var Date_Actuelle = DateTime.Now;
            Ponts.Sort((x, y) => DateTime.Compare(x.ClosingDate, y.ClosingDate));
            foreach (var Pont in Ponts)
            {
                if (Date_Actuelle < Pont.ClosingDate)
                {
                    return View(Pont);
                }
                else{
                    continue;
                }
                
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult all_close()
        {
            
            var Pont = GetBridgeMonitorFromApi();
            Pont.Sort((x, y) => DateTime.Compare(x.ClosingDate, y.ClosingDate));
            return View(Pont);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private static List<GestionPont> GetBridgeMonitorFromApi()
        {
            //Création un HttpClient (= outil qui va permettre d'interroger une URl via une requête HTTP)
            using (var client = new HttpClient())
            {
                //Interrogation de l'URL censée me retourner les données
                var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
                //Récupération du corps de la réponse HTTP sous forme de chaîne de caractères
                var stringResult = response.Result.Content.ReadAsStringAsync();
                //Conversion de mon flux JSON (string) en une collection d'objets BikeStation
                //d'un flux de données vers des objets => Déserialisation
                //d'objets vers un flux de données => Sérialisation
                var result = JsonConvert.DeserializeObject<List<GestionPont >>(stringResult.Result);
                return result;
            }
        }
    }
}
