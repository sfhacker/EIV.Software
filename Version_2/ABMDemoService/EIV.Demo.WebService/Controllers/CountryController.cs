
namespace EIV.Demo.WebService.Controllers
{
    using Data.Interface;
    using Data.Repository;
    using EIV.Demo.Model;
    using Microsoft.OData.UriParser;
    using System;
    //using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.OData;
    //using System.Web.OData.Routing.Conventions;
    //using System.Web.OData.Extensions;
    //using Microsoft.OData;

    [EnableQuery]
    public class CountryController : ODataController
    {
        private static ICountryRepository countryRepository = null;

        public CountryController()
        {
            if (countryRepository == null)
            {
                countryRepository = new CountryRepository();
            }

        }

        [HttpGet]
        public IHttpActionResult Get()    // IList<Country>
        {
            var rst = countryRepository.GetAll();

            var response = Request.CreateResponse(HttpStatusCode.OK, rst);
            response.Headers.Add("Company-Name", "EIV.Software");

            return ResponseMessage(response);

            //return countryRepository.GetAll();
        }

        [HttpGet]
        public Country Get([FromODataUri]int key)
        {
            Country country = countryRepository.GetById(key);
            if (country == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return country;
        }

        [HttpGet]
        public IHttpActionResult TotalPaises()
        {
            var rst = countryRepository.GetAll();
            int count = rst.Count;

            var response = Request.CreateResponse(HttpStatusCode.OK, count);
            response.Headers.Add("Company-Name", "EIV.Software");
            response.Headers.Add("OData-Action-Name", "TotalPaises");

            return ResponseMessage(response);
        }

        [HttpPost]
        //public IHttpActionResult Transfer([FromODataUri] int key, ODataActionParameters parameters)
        // [FromODataUri]
        public IHttpActionResult PaisAction([FromODataUri] int key, ODataActionParameters parameters)
        {
            object param1 = null;
            object param2 = null;
            object param3 = null;

            int sectorId;
            DateTime fechaIngreso;
            IList<State> stateList = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (parameters == null)
            {
                throw new ArgumentNullException();
            }

            if (!parameters.TryGetValue("sectorId", out param1))
            {
                return NotFound();
            }

            if (!parameters.TryGetValue("fechaIngreso", out param2))
            {
                return NotFound();
            }

            if (!parameters.TryGetValue("StateIDs", out param3))
            {
                return NotFound();
            }

            sectorId = (int)param1;

            string formats = "{0:yyyy-MM-dd}";

            //fechaIngreso = DateTime.Today.AddDays(-1);

            //if (param2 is DateTime)
            //{
                bool rst = DateTime.TryParse(param2.ToString(), out fechaIngreso);
            //}

            //sectorId = 3;
            //

            Client newClient = new Client() { Id = 666, FirstName = sectorId.ToString(), LastName = fechaIngreso.ToString() };

            var response = Request.CreateResponse(HttpStatusCode.OK, newClient);
            response.Headers.Add("Company-Name", "EIV.Software");
            response.Headers.Add("OData-Action-Name", "PaisAction");

            return ResponseMessage(response);

            //return Ok<Client>(newClient);
        }

        [HttpPost]
        //public IHttpActionResult Transfer([FromODataUri] int key, ODataActionParameters parameters)
        // [FromODataUri]
        public IHttpActionResult StateListAction([FromODataUri] int key, ODataActionParameters parameters)
        {
            object param1 = null;

            IList<State> stateList = null;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (parameters == null)
            {
                throw new ArgumentNullException();
            }

            if (!parameters.TryGetValue("StateIDs", out param1))
            {
                return NotFound();
            }

            if (param1 != null)
            {
                //IEnumerable testList = param1 as IEnumerable;
                IEnumerable<State> testList = param1 as IEnumerable<State>;

                stateList = testList.ToList();  // ToList<State>();

                //IEnumerable<State> list2 = one.Cast<State>();

                // Runtime exception here
                //stateList = (IList<State>) param1;

                //stateList = new List<State>(param1); // param1 as IList<State>;
            }

            int count = stateList.Count;

            var response = Request.CreateResponse(HttpStatusCode.OK, count);
            response.Headers.Add("Company-Name", "EIV.Software");
            response.Headers.Add("OData-Action-Name", "StateListAction");

            return ResponseMessage(response);

            //return Ok<Client>(newClient);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (country == null)
            {
                return BadRequest();
            }
            countryRepository.Add(country);

            var response = Request.CreateResponse(HttpStatusCode.Created, country);

            var segments = new List<ODataPathSegment>();

            // missing reference?
            //var one = new KeyValuePathSegment(ODataUriUtils.ConvertToUriLiteral(country.Id, ODataVersion.V4));

            //var uri = new Uri(this.Url.CreateODataLink(segments));

            // http://localhost:1860/odata/Country(4)
            return Created<Country>(country);

            //response.Headers.Location = Request.RequestUri;
            //return ResponseMessage(response);
        }

        [HttpPut]
        public IHttpActionResult Put(int key, Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                country.Id = key;
                countryRepository.Update(country);

                //var response = Request.CreateResponse(HttpStatusCode.OK, country);
                var response = Request.CreateResponse(HttpStatusCode.NoContent);

                return ResponseMessage(response);
            }
            catch (ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Country country = countryRepository.GetById(key);
            if (country == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            countryRepository.Delete(country);

            //return Ok();
            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return ResponseMessage(response);
        }
    }
}