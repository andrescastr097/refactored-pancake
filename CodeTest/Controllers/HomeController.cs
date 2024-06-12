using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using PruebaIngreso.Models;
using Quote.Contracts;
using Quote.Models;

namespace PruebaIngreso.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQuoteEngine quote;
        private readonly IMarginProvider marginProvider;

        private static readonly HttpClient client = new HttpClient();


        public HomeController(IQuoteEngine quote, IMarginProvider marginProvider)
        {
            this.quote = quote;
            this.marginProvider = marginProvider;
            // Ensure the application uses TLS 1.2 or higher
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls12;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            var request = new TourQuoteRequest()
            {
                adults = 1,
                ArrivalDate = DateTime.Now.AddDays(1),
                DepartingDate = DateTime.Now.AddDays(2),
                getAllRates = true,
                GetQuotes = true,
                RetrieveOptions = new TourQuoteRequestOptions
                {
                    GetContracts = true,
                    GetCalculatedQuote = true,
                },

                Language = Language.Spanish,
                TourCode = "E-U10-PRVPARKTRF"
            };

            var result = this.quote.Quote(request);
            var tour = result.Tours.FirstOrDefault();
            ViewBag.Message = "Test 1 Correcto";
            return View(tour);
        }

        public ActionResult Test2()
        {
            ViewBag.Message = "Test 2 Correcto";
            return View();
        }

        public async Task<ActionResult> Test3()
        {
            var requestUrl = "https://refactored-pancake.free.beeceptor.com/margin/";

            // Arreglo de cadenas
            string[] cadenas = { "E-U10-PRVPARKTRF", "E-U10-DSCVCOVE", "E-E10-PF2SHOW", "E-U10-UNILATIN" };

            // Generar un índice aleatorio
            Random rand = new Random();
            int indice = rand.Next(0, cadenas.Length);

            // Seleccionar la cadena aleatoria y guardarla en una variable
            string cadenaAleatoria = cadenas[indice];

            requestUrl = requestUrl + cadenaAleatoria;


            try
            {
                var response = await client.GetAsync(requestUrl);
                //response.EnsureSuccessStatusCode();
                var statusCode = response.StatusCode;
                var responseBody = "";
                Margen apiResponse = new Margen();
                if (statusCode == HttpStatusCode.OK)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<Margen>(responseBody);
                }
                else
                {
                    apiResponse.Margin = 0;
                }




                ViewBag.Message = "Test 3 Correcto";
                ViewBag.Response = responseBody;



                return View(apiResponse);
            }
            catch (HttpRequestException e)
            {
                ViewBag.Message = $"Request error: {e.Message}";
                return View("Error");
            }

        }

        public ActionResult Test4()
        {
            var request = new TourQuoteRequest
            {
                adults = 1,
                ArrivalDate = DateTime.Now.AddDays(1),
                DepartingDate = DateTime.Now.AddDays(2),
                getAllRates = true,
                GetQuotes = true,
                RetrieveOptions = new TourQuoteRequestOptions
                {
                    GetContracts = true,
                    GetCalculatedQuote = true,
                },
                Language = Language.Spanish
            };

            var result = this.quote.Quote(request);
            return View(result.TourQuotes);
        }
    }
}