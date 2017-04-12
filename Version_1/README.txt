

Servicio OData (.NET)
---------------------

Version : 4.0

Date    : 2017-04-12

URL     : http://localhost:1860/odata/

Metadata: http://localhost:1860/odata/$metadata

Features
--------

- Actions  : PaisAction, StateListAction
- Functions: TotalPaises
- Methods  : GET / POST / PATCH / DELETE
- Filters  : All (?)
- Batch Mode


EIV Libraries (or Wrappers)

- Version: 1.0.0 (Development)

- EIV.OData.Proxy

    * Dependencies: Microsoft.OData.Client (6.15+)
	
	* Contiene clases proxy provenientes de servicios OData
	* Puede contener mas de una clase proxy y de diferentes lenguages (e.g. Java) o librerias
	* No contiene codigo de 'usuario'
	* Se actualiza facilmente (OneClick)
	* Tests: N/A
	* Deployment: Un solo assembly
	
- EIV.OData.Core

    * Dependencies: Microsoft.OData.Client (6.15+)
	
	* Contiene codigo generico para administracion de servicio OData
	* Singleton
	* Una misma interfaz, multiple servicios OData
	
	      bool rst = serviceContext.Connect<EIV.Demo.WebService.Container>(ODATA_SERVICE_URL_NET);
          bool rst = serviceContext.Connect<com.cairone.odataexample.ODataExample>(ODATA_SERVICE_URL_JAVA);
		  
	* Soporta Queries, Actions, Functions, Batch processing, Filters [1]
	* Authentication
	* Trackable entities (link con UI) [2,3]
	* Errores standarizados
	* Tests: TO DO
	* Deployment: Un solo assembly

- EIV.UI.Telerik

    * Dependencies: Telerik controls [Telerik.Windows.Controls v2017.1.222.45]
	                EIV.OData.Core

    * Todo codigo repetitivo relacionado a controles Telerik, el cual se pueda reutilizar
	* Funcionalidad no provista por Telerik o funcionalidad especifica segun requerimientos
	* Interface simple y generica para comunicarse con servicio OData
	* Tests: TO DO
	* Deployment: Un solo assembly


Aplicaciones Cliente (WPF)

    * Dependencies: Telerik controls (especifico segun lo que se necesite implementar)
                    EIV.UI.Telerik   (trackeo de entidades: INS / MOD / DEL)
					                  cuantas entidades fueron modificadas y cuales, etc)
	                EIV.OData.Core   (conexion al servicio OData, queries, etc)
					EIV.OData.Proxy  (contiene el modelo de datos, el cual suele utilizarse en algun
					                  control Telerik)
									 
	* Architecture: MVVM
	
	
    * Notas / Sugerencias / Ideas
	
	     - <nombre>.xaml.cs
		       - Constructor: this.DataContext = new SectoresViewModel();
			   - Todo el resto del codigo en dicho archivo deberia hacer uso del DataContext
			   
	     - <ViewModel>.cs
		 
		        - Crear una instacia de 'CustomViewModel' (Constructor)
				- Preparar collections de datos que requiera la vista (.xaml.cs)
				
		 - Se podria crear una capa de servicio (e.g. EIV.OData.Service) que contenga o implemente los queries mas comunes
		   (e.g. provincias por pais, sectores por persona, etc) consumidos por UI para evitar de 
		   'clutter' UI con dicho queries. Si en un futuro se requiere cambiar un query existente o agregar uno nuevo ....
           Esta capa de servicio tambien podria incluir validaciones, etc. para evitar que UI envie 'rubbish'
           a la capa de datos.

		 - Case 1: Se actualiza el servicio OData (e.g. tenia un bug) y no hay impacto en UI
		           Actiones: recompilar y deploy 'EIV.OData.Proxy'
				   
		 - Case 2: Se actualiza el servicio OData (e.g. agrego un property a una entidad) y se requiere mostrar dicha
		           property en un Grid.
				   Acciones: recompilar y deploy 'EIV.OData.Proxy' y actualizar la aplication cliente (WPF) correspondiente

* Ejecucion

      1.- Levantar y ejecturar el servicio OData (Projecto ABMDemoService) (.NET)
	      (o el servicio OData en Java)
	  2.- Levatar y ejecutar la aplicacion cliente (Projecto WpfApplication4)
	  3.- El proyecto cliente utiliza el servicio OData en Java. Para cambiar al servicio OData .NET, modificar esta linea 
	      en SectoresViewModel.cs:
	  
	                bool rst = serviceContext.Connect<EIV.Demo.WebService.Container>(ODATA_SERVICE_URL_NET);
	  
	  
References
----------

[1] http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part2-url-conventions/odata-v4.0-errata03-os-part2-url-conventions-complete.html			
[2] https://blog.tonysneed.com/2013/11/18/trackable-entities-versus-self-tracking-entities/
[3] https://www.codeproject.com/Articles/35066/Generic-implementation-of-IEditableObject-via-Type
