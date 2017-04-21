/// <summary>
/// 
/// </summary>
namespace EIV.Demo.WebService.Controllers
{
    using Data.Interface;
    using Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    //using System.Collections.ObjectModel;
    using System.Diagnostics;

    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using System.Web.OData;

    [EnableQuery]
    public class PeopleController : ODataController
    {
        private static IClientRepository clientRepository = null;

        public PeopleController()
        {
            if (clientRepository == null)
            {
                clientRepository = new ClientRepository();
            }

        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var rst = clientRepository.GetAll();

            var response = Request.CreateResponse(HttpStatusCode.OK, rst);
            return ResponseMessage(response);

            //return clientRepository.GetAll();
        }

        [HttpGet]
        public Client Get([FromODataUri]int key)
        {
            Client client = clientRepository.GetById(key);
            if (client == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return client;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] Client person)
        {
            if (person == null)
            {
                return BadRequest("Invalid passed data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                clientRepository.Add(person);

                var response = Request.CreateResponse(HttpStatusCode.Created, person);

                response.Headers.Location = Request.RequestUri;
                return ResponseMessage(response);
            }
            catch (ArgumentNullException e)
            {
                Debugger.Log(1, "Error", e.Message);
                return BadRequest();
            }
            catch (ArgumentException e)
            {
                Debugger.Log(1, "Error", e.Message);
                return BadRequest();
            }
            catch (InvalidOperationException e)
            {
                Debugger.Log(1, "Error", e.Message);
                return Conflict();
            }
        }

        [HttpPut]
        public IHttpActionResult Put([FromODataUri] int key, Client person)
        {
            Client thisFellow = null;

            if (key < 1)
            {
                return BadRequest("Invalid passed data");
            }
            if (person == null)
            {
                return BadRequest("Invalid passed data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            thisFellow = clientRepository.GetById(key);
            if (thisFellow != null)
            {
                clientRepository.Update(thisFellow);

                return Updated(thisFellow);
            }

            return NotFound();
        }

        [HttpPatch]
        public IHttpActionResult Patch([FromODataUri] int key, Client person) //Delta<Client>
        {
            Client thisFellow = null;

            if (key < 1)
            {
                return BadRequest("Invalid passed data");
            }
            if (person == null)
            {
                return BadRequest("Invalid passed data");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            thisFellow = clientRepository.GetById(key);
            if (thisFellow != null)
            {
                //clientRepository.Update(thisFellow);
                clientRepository.Update(person);

                return Updated(person);
            }

            return NotFound();
        }

        [HttpDelete]
        public IHttpActionResult Delete(int key)
        {
            Client thisFellow = null;

            if (key < 1)
            {
                return BadRequest("Invalid passed data");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            thisFellow = clientRepository.GetById(key);
            if (thisFellow != null)
            {
                clientRepository.Delete(thisFellow);

                var response = Request.CreateResponse(HttpStatusCode.NoContent);
                return ResponseMessage(response);
            }

            return NotFound();
        }

        [HttpPost]
        //public IHttpActionResult Transfer([FromODataUri] int key, ODataActionParameters parameters)
        // [FromODataUri]
        public IHttpActionResult Transfer(ODataActionParameters parameters)
        {
            object param1 = null;
            object param2 = null;

            int sectorId;
            DateTime fechaIngreso;

            /*if (parameters == null)
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
            }*/

            //sectorId = (int)param1;
            //fechaIngreso = (DateTime) param2;

            sectorId = 3;
            fechaIngreso = DateTime.Today;

            Client newClient_1 = new Client() { Id = 123, FirstName = sectorId.ToString(), LastName = fechaIngreso.ToString() };
            Client newClient_2 = new Client() { Id = 234, FirstName = "Galicia", LastName = fechaIngreso.ToString() };
            Client newClient_3 = new Client() { Id = 345, FirstName = "N/A", LastName = fechaIngreso.ToString() };

            IList<Client> list = new List<Client>() { newClient_1, newClient_2, newClient_3 };

            var rst = clientRepository.GetAll();

            var response = Request.CreateResponse(HttpStatusCode.OK, rst);
            return ResponseMessage(response);

            //return Ok <IList<Client>>(list);
        }
    }
}