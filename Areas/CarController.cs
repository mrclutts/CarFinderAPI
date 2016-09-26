using CarFinderAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CarFinderAPI.Areas
{
    [RoutePrefix("api/CarFind")]
    public class CarController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Route("years")]
        public async Task<List<string>> GetYears()
        {
            return await db.GetYears();
        }
        [Route("makes")]
        public async Task<List<string>> GetMakes(int year)
        {
            return await db.GetMakes(year);
        }
        [Route("models")]
        public async Task<List<string>> GetModels(int year, string make)
        {
            return await db.GetModels(year, make);
        }
        [Route("trims")]
        public async Task<List<string>> GetTrims(int year, string make, string model)
        {
            return await db.GetTrims(year, make, model);
        }
        [Route("getCars")]
        public async Task<List<Car>> GetCars(int year, string make, string model, string trim)
        {
            return await db.GetCars(year, make, model, trim);
        }
        [Route("getCar")]
        public async Task<Car> GetCar(int year, string make, string model, string trim)
        {
            return await db.GetCar(year, make, model, trim);
        }

        [Route("getCarById")]
        public async Task<Car> GetCarById(int id)
        {
            return await db.GetCarById(id);
        }


        [Route("getData")]
        public async Task<IHttpActionResult> getCarData(int id)
        {
            HttpResponseMessage response;
            string content = "";
            var singleCar = await GetCarById(id);
            var car = new carViewModel
            {
                Car = singleCar,
                Recalls = new carRecall(),
                Image = ""
            };

            var result1 = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://www.nhtsa.gov/");
                try
                {
                    response = await client.GetAsync("webapi/api/Recalls/vehicle/modelyear/" + car.Car.model_year.ToString() + "/make/" + car.Car.make + "/model/" + car.Car.model_name);
                    result1 = await response.Content.ReadAsStringAsync();
                    var rc = JsonConvert.DeserializeObject<carRecall>(result1);
                    car.Recalls = rc;

                }
                catch (Exception e)
                {
                    content = null;
                    //return InternalServerError(e);
                }
            }
            ////////////////////// BING /////////////////////////
            string query = singleCar.model_year + " " + singleCar.make + " " + singleCar.model_name + " " + singleCar.model_trim;
            string rootUri = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUri));
            var accountKey = "LSwOVKbupi3r6MpCAOEDxczz7tVZgdrXMBLd9e08j9I";
            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);
            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);
            var imageResults = imageQuery.Execute().ToList().FirstOrDefault().MediaUrl;
            return Ok (car);
        }
    }
}
