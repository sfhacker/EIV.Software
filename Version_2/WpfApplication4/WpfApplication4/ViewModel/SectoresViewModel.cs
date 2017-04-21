
namespace WpfApplication4.ViewModel
{
    using EIV.OData.Core;
    using System;
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

        // Hardoced for now
        private const int tipoDocumento = 2;
        private const string noDocumento = "11111111";        // "12345678";

        private EIV.OData.Core.ODataServiceContext serviceContext = null;

        //private TrackableEntities entityList = null;

        private ObservableCollection<com.cairone.odataexample.Sector> sectores = null;
        private ObservableCollection<com.cairone.odataexample.PersonaSector> sectoreAsignados = null;

        private ObservableCollection<com.cairone.odataexample.PersonaSector> sectoreAsignadosOrig = null;

        private EIV.UI.Telerik.CustomViewModel customViewModel = null;

        public SectoresViewModel()
        {
            // we could put all these lines into CustomViewModel
            serviceContext = EIV.OData.Core.ODataServiceContext.Instance;


            // .NET Service
            
            bool rst = serviceContext.Connect<EIV.Demo.WebService.Container>(ODATA_SERVICE_URL_NET);
            if (!rst)
            {
                return;
            }

            // Testing $expand filter here
            // Works fine in .NET but Java
            this.GetClientById(1);
            this.GetCountryById(2);
            this.GetCountryById(3);
            

            /*
            // Java Service
            bool rst = serviceContext.Connect<com.cairone.odataexample.ODataExample>(ODATA_SERVICE_URL_JAVA);
            if (!rst)
            {
                return;
            }*/

            this.sectores = this.LoadAllSectores();

            // This is hardcoded for now!
            this.sectoreAsignados = this.LoadAllSectoresByPersona(tipoDocumento, noDocumento);
            this.sectoreAsignadosOrig = this.LoadAllSectoresByPersona(tipoDocumento, noDocumento);

            this.customViewModel = new EIV.UI.Telerik.CustomViewModel();
        }

        public int UsuarioId
        {
            get
            {
                return tipoDocumento;
            }
        }

        public string UsuarioDocumento
        {
            get
            {
                return noDocumento;
            }
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
        /*public void Insert(com.cairone.odataexample.Persona persona)
        {
            if (persona == null)
            {
                return;
            }

            //this.gridDataSource.Add(pais);

            this.customViewModel.InsertEntity<com.cairone.odataexample.Persona>(persona);

            // not needed any longer
            //this.newItems.Add(pais);
        }*/

        public void Insert(com.cairone.odataexample.PersonaSector pSector)
        {
            com.cairone.odataexample.PersonaSector tempSector = null;

            if (pSector == null)
            {
                return;
            }

            // If initially I removed a sector and later on I added it again!
            tempSector = this.sectoreAsignadosOrig.Where(x => x.id == pSector.id).SingleOrDefault();
            if (tempSector == null)
            {
                // Uncomment later on
                //this.customViewModel.InsertEntity<com.cairone.odataexample.PersonaSector>(pSector);
            } else
            {
                this.customViewModel.InsertEntity<com.cairone.odataexample.PersonaSector>(tempSector);
            }
        }

        // Execute Action
        // Not working at the moment
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
            IList<com.cairone.odataexample.PersonaSector> tempList = null;
            com.cairone.odataexample.PersonaSector tempSector = null;

            if (pSector == null)
            {
                return;
            }

            tempList = new List<com.cairone.odataexample.PersonaSector>();
            foreach (com.cairone.odataexample.PersonaSector item in pSector)
            {
                tempSector = this.sectoreAsignadosOrig.Where(x => x.id == item.id).SingleOrDefault();
                if (tempSector != null)
                {
                    tempList.Add(tempSector);
                } else
                {
                    tempList.Add(item);
                }
            }

            this.customViewModel.RemoveEntities<com.cairone.odataexample.PersonaSector>(tempList);

            //this.customViewModel.RemoveEntities<com.cairone.odataexample.PersonaSector>(pSector);

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

        // The idea is to use the 'AgregarSector' action
        // what about using the PersonaSector entity?
        // What about using Persona entity and add a sector to it
        // then issue an update?
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

        public void RejectChanges()
        {
            // Going to OData/DB again!
            this.sectores = this.LoadAllSectores();

            // This is hardcoded for now!
            this.sectoreAsignados = this.LoadAllSectoresByPersona(tipoDocumento, noDocumento);

            this.customViewModel.Items.Clear();
        }

        // Java Service
        // $expand does NOT work here
        public com.cairone.odataexample.Persona GetPersonaById(int tipoDoc, string noDoc)
        {
            ObservableCollection<com.cairone.odataexample.Persona> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(tipoDocumentoId eq {0} and numeroDocumento eq '{1}')", tipoDoc, noDoc);

            filters.Add("$filter", searchCriteria);
            filters.Add("$expand", "localidad");

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

        // .NET Service
        // $expand works fine
        public EIV.Demo.Model.Client GetClientById(int id)
        {
            ObservableCollection<EIV.Demo.Model.Client> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(Id eq {0})", id);

            filters.Add("$filter", searchCriteria);
            filters.Add("$expand", "Country");

            rst = serviceContext.GetEntities<EIV.Demo.Model.Client>("People", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        // .NET Service
        // $expand works fine
        public EIV.Demo.Model.Country GetCountryById(int id)
        {
            ObservableCollection<EIV.Demo.Model.Country> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(Id eq {0})", id);

            //filters.Add("$expand", "localidad,sectores");
            filters.Add("$filter", searchCriteria);
            filters.Add("$expand", "States");

            rst = serviceContext.GetEntities<EIV.Demo.Model.Country>("Country", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        public com.cairone.odataexample.Sector GetSectorByName(string sectorName)
        {
            ObservableCollection<com.cairone.odataexample.Sector> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(nombre eq '{0}')", sectorName);

            filters.Add("$filter", searchCriteria);

            rst = serviceContext.GetEntities<com.cairone.odataexample.Sector>("Sectores", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        // $expand does not work!
        public com.cairone.odataexample.Localidad GetLocalidadByName(string localidadName)
        {
            ObservableCollection<com.cairone.odataexample.Localidad> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(nombre eq '{0}')", localidadName);

            filters.Add("$filter", searchCriteria);
            filters.Add("$expand", "provincia");

            rst = serviceContext.GetEntities<com.cairone.odataexample.Localidad>("Localidades", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        public com.cairone.odataexample.Provincia GetProvinciaByName(string provinciaName)
        {
            ObservableCollection<com.cairone.odataexample.Provincia> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(nombre eq '{0}')", provinciaName);

            filters.Add("$filter", searchCriteria);
            filters.Add("$expand", "pais");

            rst = serviceContext.GetEntities<com.cairone.odataexample.Provincia>("Provincias", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        public com.cairone.odataexample.Pais GetPaisByName(string paisName)
        {
            ObservableCollection<com.cairone.odataexample.Pais> rst = null;

            var filters = new Dictionary<string, object>();

            //com.cairone.odataexample.Persona rst = null;
            string searchCriteria = string.Format("(nombre eq '{0}')", paisName);

            filters.Add("$filter", searchCriteria);

            rst = serviceContext.GetEntities<com.cairone.odataexample.Pais>("Paises", filters);
            if (rst != null)
            {
                if (rst.Count > 0)
                {
                    return rst[0];
                }
            }

            return null;
        }

        /*
        public void AddLocalidadToPersona()
        {
            EIV.Demo.Model.Persona usuario = new EIV.Demo.Model.Persona()
            {
                Id = 234,
                Nombres = "Nombre desde cliente WPF",
                Apellidos = "Apellido desde client WPF",
                Localidad = new EIV.Demo.Model.Localidad() { Id = 45, Nombre = "Localidad desde cliente WPF" }
            };

        }*/

        // Tres maneras diferentes de agregar un Sector a un Usuario
        // 1.- Usando la entidad Persona y agregar un sector a la collection sectores (ver $metadata)
        // 2.- Creando una instancia de la entidad PersonaSector y enviar un PUT al servicio
        // 3.- Invocando una action y pasar los parametros requeridos
        //     Nota: No estoy seguro que se puedan enviar varias actiones simultaneamente (batch)
        //           En caso afirmativo, se pueden enviar una collection como parametro
        //           de la action, y asi, se enviaria una sola action al servicio
        //           afectando multiple entidades simultaneamente
        public bool TestAddingSectorToPersonaViaPersonaEntity(int tipoDoc, string noDoc)
        {
            com.cairone.odataexample.Persona cliente = null;
            com.cairone.odataexample.Sector sector = null;

            cliente = this.GetPersonaById(tipoDoc, noDoc);
            if (cliente != null)
            {
                // $expand does not work!
                // why i have to do this by hand?????
                if (cliente.localidad == null)
                {
                    cliente.localidad = this.GetLocalidadByName("ROSARIO");

                    // $expand does not work!
                    if (cliente.localidad.provincia == null)
                    {
                        cliente.localidad.provincia = this.GetProvinciaByName("SANTA FE");

                        if (cliente.localidad.provincia.pais == null)
                        {
                            cliente.localidad.provincia.pais = this.GetPaisByName("ARGENTINA");
                        }
                    }
                }

                sector = this.GetSectorByName("NADA");

                if (sector != null)
                {
                    // Why 'PersonaSector' entity and not 'Sector'?
                    // because we need the fechaIngreso property
                    // that property could be set internally by the application
                    // unless the user could change it in the UI
                    //cliente.sectores.Add()
                    com.cairone.odataexample.PersonaSector pSector = new com.cairone.odataexample.PersonaSector()
                    {
                        id = sector.id,
                        fechaIngreso = DateTime.Today,
                        tipoDocumentoId = cliente.tipoDocumentoId,
                        numeroDocumento = cliente.numeroDocumento
                    };

                    // Will this work?
                    // Does it update all related entities? (e.g. sectores)
                    // Or OData does not work this way?
                    // Siempre el mismo error:
                    // 'HAY DATOS INVALIDOS EN LA SOLICITUD ENVIADA. NO SE HA PROPORCIONADO UN VALOR PARA ID DEL PAIS'
                    // pero este error msg parece misleading
                    cliente.sectores.Add(pSector); // esto nunca va a funcionar
                                                   // pSector.Context = null!!!
                                                   // necesito ..Add(sector) y NO pSector
                    this.customViewModel.UpdateEntity<com.cairone.odataexample.Persona>(cliente);

                    this.serviceContext.ProcessOperations(this.customViewModel.Items);

                    // false: Java Service
                    this.serviceContext.SubmitChanges(false);

                    // Or I can do it this way
                    // Not implemented
                    // this.customViewModel.InsertEntity<com.cairone.odataexample.PersonaSector>(pSector);

                    // Or I could invoke an action
                    // One at a time
                    // Perhaps send an array of parameters to the action
                    // this.serviceContext.ExecuteAction<com.cairone.odataexample.PersonaSector>("Personas(tipoDocumentoId=1,numeroDocumento='12345678')", "Sectores.Agregar", null);
                }
            }

            return true;
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