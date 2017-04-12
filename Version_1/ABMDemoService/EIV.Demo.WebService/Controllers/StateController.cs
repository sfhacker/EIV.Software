
namespace EIV.Demo.WebService.Controllers
{
    using Data.Interface;
    using Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.OData;

    [EnableQuery]
    public class StateController : ODataController
    {
        private static IStateRepository stateRepository = null;

        public StateController()
        {
            if (stateRepository == null)
            {
                stateRepository = new StateRepository();
            }
        }
        [HttpGet]
        public IList<State> Get()
        {
            return stateRepository.GetAll();
        }

        [HttpGet]
        public State Get([FromODataUri]int key)
        {
            State state = stateRepository.GetById(key);
            if (state == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return state;
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] State state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (state == null)
            {
                return BadRequest("Invalid argument");
            }

            // state.Country is always NULL!!!!!!
            stateRepository.Add(state);

            var response = Request.CreateResponse(HttpStatusCode.Created, state);

            response.Headers.Location = Request.RequestUri;
            return ResponseMessage(response);
        }
    }
}