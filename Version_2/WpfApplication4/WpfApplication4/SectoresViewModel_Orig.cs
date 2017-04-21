
namespace WpfApplication4
{
    using EIV.Demo.Model;
    using EIV.OData.Core;
    using System;
    //using EIV.OData.Core.ITrackable;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public sealed class SectoresViewModel
    {
        // EIV.Demo.WebService.Container
        private const string ODATA_SERVICE_URL_NET = "http://localhost:1860/odata/";

        // com.cairone.odataexample.ODataExample
        // Use app.config or web.config or ..... settings.
        private const string ODATA_SERVICE_URL_JAVA = "http://localhost:8080/odata/appexample.svc/";

        private EIV.OData.Core.ODataServiceContext serviceContext = null;

        //private TrackableEntities entityList = null;

        private ObservableCollection<com.cairone.odataexample.Sector> sectores = null;
        private ObservableCollection<com.cairone.odataexample.PersonaSector> sectoreAsignados = null;

        private EIV.UI.Telerik.CustomViewModel customViewModel = null;

        public SectoresViewModel()
        {
            // we could put all these lines into CustomViewModel
            serviceContext = EIV.OData.Core.ODataServiceContext.Instance;

            bool rst = serviceContext.Connect<EIV.Demo.WebService.Container>(ODATA_SERVICE_URL_NET);
            if (!rst)
            {
                return;
            }

            // Testing POST ----
            EIV.Demo.Model.Country newPais = new EIV.Demo.Model.Country()
            {
                Id = 9,
                Name = "Testing Pais from .Net Client (2)"
            };

            var stateList = this.serviceContext.GetEntities<Client>("People", null);
            if (stateList != null)
            {
                if (stateList.Count > 0)
                {
                    stateList[0].FirstName = "John (Modified)";
                }
            }

            Client deleteBoy = new Client()
            {
                Id = 3
            };

            customViewModel = new EIV.UI.Telerik.CustomViewModel();

            // POST
            this.customViewModel.InsertEntity<EIV.Demo.Model.Country>(newPais);
            // PUT
            this.customViewModel.UpdateEntity<EIV.Demo.Model.Client>(stateList[0]);
            // DELETE
            IList<Client> delPeople = new List<Client>();
            delPeople.Add(stateList[2]);

            this.customViewModel.RemoveEntities<EIV.Demo.Model.Client>(delPeople);

            // false
            //rst = this.serviceContext.IsValidEntity<Client>(delPeople);
            rst = this.serviceContext.IsValidEntity(stateList[0]);

            var totalChanges = this.customViewModel.TotalChanges;
            this.serviceContext.ProcessOperations(this.customViewModel.Items);
            var operList = this.serviceContext.SubmitChanges(false);

            if (operList != null)
            {
                int operCount = operList.Count;
            }
            // ----- EOF POST ---------------------

            com.cairone.odataexample.PersonaSector pSector;

            pSector = new com.cairone.odataexample.PersonaSector() { id = 1234 };

            IDictionary<string, object> queryParams = new Dictionary<string, object>();

            //queryParams.Add("count", "");

            IDictionary<string, object> functionParams = new Dictionary<string, object>();

            functionParams.Add("sectorId", 3);

            string fechaIngreso = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            functionParams.Add("fechaIngreso", fechaIngreso);

            IList<State> listState = new List<State>();
            listState.Add(new State() { Id = 4 });
            listState.Add(new State() { Id = 23 });
            listState.Add(new State() { Id = 18 });
            listState.Add(new State() { Id = 556 });

            functionParams.Add("StateIDs", listState);

            var query1 = this.serviceContext.CreateQueryFromEntity<EIV.Demo.Model.Country>("Country", functionParams);
            var query2 = this.serviceContext.CreateQueryFromEntity<EIV.Demo.Model.Client>("People", null);
            //var query3 = this.serviceContext.CreateFunctionQuery<Country>(null, "EIV.Demo.WebService.TotalPaises", null);
            var query3 = this.serviceContext.CreateFunctionQuery<int>("Country", "EIV.Demo.WebService.TotalPaises", null);

            var dsReq1 = this.serviceContext.GenerateBatchParam<EIV.Demo.Model.Country>(query1);
            var dsReq2 = this.serviceContext.GenerateBatchParam<EIV.Demo.Model.Client>(query2);
            var dsReq3 = this.serviceContext.GenerateBatchParam<int>(query3);

            object[] testOne = null;

            var batchResponses = this.serviceContext.ExecuteBatch(dsReq1, dsReq2, dsReq3);

            foreach (ODataBatchResponse item in batchResponses)
            {
                int statusCode = item.StatusCode;

                Type responseType = item.EntityType;
                if (responseType == typeof(Country))
                {
                    var listPais = item.ItemList.OfType<Country>().ToList();
                }
                if (responseType == typeof(Client))
                {
                    var listClient = item.ItemList.OfType<Client>().ToList();
                }
                if (responseType == typeof(Int32))
                {
                    var listInt32 = item.ItemList.OfType<Int32>().ToList();
                }
                /*
                var myArray = this.serviceContext.ToArrayList(item.Items);
                if (myArray != null)
                {
                    var totalCount = myArray.Count;
                }*/
            }

            //this.entityList = new TrackableEntities();

            this.sectores = this.LoadAllSectores();
            this.sectoreAsignados = this.LoadAllSectoresByPersona(1, "12345678");

            this.customViewModel = new EIV.UI.Telerik.CustomViewModel();
        }

        public ObservableCollection<com.cairone.odataexample.Sector> Sectores
        {
            get
            {
                return this.sectores;
            }
        }

        public ObservableCollection<com.cairone.odataexample.PersonaSector> SectoresAsignados
        {
            get
            {
                return this.sectoreAsignados;
            }
        }

        public int TotalChanges
        {
            get
            {
                return this.customViewModel.TotalChanges;
            }
        }

        public IList<ODataError> Errors
        {
            get
            {
                return this.serviceContext.Errors.Items;
            }
        }

        // com.cairone.odataexample.PersonaSector pSector
        public void Insert(com.cairone.odataexample.Persona persona)
        {
            if (persona == null)
            {
                return;
            }

            //this.gridDataSource.Add(pais);

            this.customViewModel.InsertEntity<com.cairone.odataexample.Persona>(persona);

            // not needed any longer
            //this.newItems.Add(pais);
        }

        public void AddSectorToPersona(com.cairone.odataexample.PersonaSector pSector)
        {
            var thisParams = new Dictionary<string, object>();

            if (pSector == null)
            {
                return;
            }
            thisParams.Add("sectorId", pSector.id);
            thisParams.Add("fechaIngreso", pSector.fechaIngreso);

            var rst = this.serviceContext.ExecuteAction<com.cairone.odataexample.PersonaSector>("Personas(tipoDocumentoId=1,numeroDocumento='12345678')", "Sectores.Agregar", thisParams);
        }
        public void RemoveAll(IList<com.cairone.odataexample.PersonaSector> pSector)
        {
            if (pSector == null)
            {
                return;
            }

            this.customViewModel.RemoveEntities<com.cairone.odataexample.PersonaSector>(pSector);

            // No need for search
            // param is coming off the grid view
            /*
            Pais thisPais = this.gridDataSource.Where(p => p.Id == pais.Id).SingleOrDefault();
            if (thisPais != null)
            {
                this.gridDataSource.Remove(thisPais);
            }*/

            /*foreach (com.cairone.odataexample.PersonaSector item in pSector)
            {
                //this.gridDataSource.Remove(item);

                this.entityList.ProcessEntity<com.cairone.odataexample.PersonaSector>(item, TrackableEntity.Operation.Delete);

                // not needed any longer
                //this.deleteItems.Add(item);
            }*/
        }

        public void SubmitChanges()
        {
            // This needs refactoring, but for now ....
            //this.serviceContext.SavePaises(this.newItems);
            //this.serviceContext.UpdatePaises(this.updateItems);
            //this.serviceContext.DeletePaises(this.deleteItems);

            int count = this.customViewModel.TotalChanges;

            this.serviceContext.ProcessOperations(this.customViewModel.Items);

            // false: Java Service
            this.serviceContext.SubmitChanges(false);

            //this.entityList.Clear();

            //this.newItems.Clear();
            //this.updateItems.Clear();
            //this.deleteItems.Clear();
        }

        public com.cairone.odataexample.Persona GetPersonaById(int tipoDoc, string noDoc)
        {
            ObservableCollection<com.cairone.odataexample.Persona> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(tipoDocumentoId eq {0} and numeroDocumento eq '{1}')", tipoDoc, noDoc);

            filters.Add("$filter", searchCriteria);

            rst = serviceContext.GetEntities<com.cairone.odataexample.Persona>("Personas", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }
        private ObservableCollection<com.cairone.odataexample.PersonaSector> LoadAllSectoresByPersona(int tipoDoc, string noDoc)
        {
            ObservableCollection<com.cairone.odataexample.PersonaSector> rst = null;

            ObservableCollection<com.cairone.odataexample.Persona> response = null;

            var filters = new Dictionary<string, object>();

            //Personas(tipoDocumentoId = 1, numeroDocumento = '12345678') ?$expand = sectores

            string searchCriteria = string.Format("(tipoDocumentoId eq {0} and numeroDocumento eq '{1}')", tipoDoc, noDoc);

            filters.Add("$filter", searchCriteria);

            // Java complains!
            //filters.Add("$expand", "sectores($orderby=nombre)");
            filters.Add("$expand", "sectores($orderby=nombre)");
            //filters.Add("$expand", "sectores");
            //filters.Add("$orderby", "sectores/nombre");

            IList<com.cairone.odataexample.Sector> testOne = null;
            IList<com.cairone.odataexample.PersonaSector> testTwo = null;
            response = serviceContext.GetEntities<com.cairone.odataexample.Persona>("Personas", filters);
            if (response != null)
            {
                var list = response.SelectMany(x => x.sectores).ToList();  // ToList<com.cairone.odataexample.Sector>(); // OfType<com.cairone.odataexample.Sector>().ToList();
                if (list != null)
                {
                    rst = new ObservableCollection<com.cairone.odataexample.PersonaSector>(list);
                }
            }

            return rst;
        }

        private ObservableCollection<com.cairone.odataexample.Sector> LoadAllSectores()
        {
            ObservableCollection<com.cairone.odataexample.Sector> rst = null;

            rst = serviceContext.GetEntities<com.cairone.odataexample.Sector>("Sectores", null);

            return rst;
        }
    }
}