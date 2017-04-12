
namespace WpfApplication4
{
    //using EIV.Demo.Model;
    using EIV.OData.Core;
    //using EIV.OData.Core.ITrackable;
    //using EIV.UI.Telerik;
    //using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    //using Telerik.Windows.Controls;
    using System.Linq;
    using System;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Dependency: OData.Client 6.15+
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new SectoresViewModel();

            this.SetUpListas();
        }

        private void SetUpListas()
        {
            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            this.lstSectoresDisponibles.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.Sectores });
            //this.lstSectoresDisponibles.ItemsSource = myViewModel.Sectores;

            this.lstSectoresDisponibles.DisplayMemberPath = "nombre";
            this.lstSectoresDisponibles.SelectedValuePath = "id";

            //var list = myViewModel.LoadAllSectoresByPersona(1, "12345678");

            this.lstSectoresAsignados.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.SectoresAsignados });

            this.lstSectoresAsignados.DisplayMemberPath = "nombre";
            this.lstSectoresAsignados.SelectedValuePath = "id";

            this.btnAdd.Click += BtnAdd_Click;
            this.btnRemove.Click += BtnRemove_Click;

            this.btnGuardar.Click += BtnGuardar_Click;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rst;

            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            int totalChanges = myViewModel.TotalChanges;

            if (totalChanges < 1)
            {
                System.Windows.MessageBox.Show("No se ha realizado ningun cambio.", "Sectores::Guardar cambios", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }
            // it is read-only, remember?
            //this.gridView.BeginInsert();
            rst = System.Windows.MessageBox.Show(string.Format("Se guardaran todos los cambios realizados hasta el momento: {0}. Desea proceder?", totalChanges), "Sectores::Guardar cambios", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rst == MessageBoxResult.Yes)
            {
                myViewModel.SubmitChanges();

                foreach (ODataError item in myViewModel.Errors)
                {
                    this.statusInfo.Text += string.Format("Oper {0}: {1} {2}", item.OperationNumber, item.StatusCode, item.ErrorMessage);
                }
                if (myViewModel.Errors.Count == 0)
                {
                    this.statusInfo.Text += "Ok";
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            IList<com.cairone.odataexample.PersonaSector> itemsToRemove = new List<com.cairone.odataexample.PersonaSector>();

            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            if (this.lstSectoresAsignados.SelectedItems.Count == 0)
            {
                return;
            }

            int totalCount = 0;
            foreach (object item in this.lstSectoresAsignados.SelectedItems)
            {
                if (item is com.cairone.odataexample.PersonaSector)
                {
                    totalCount++;
                    com.cairone.odataexample.PersonaSector thisSector = item as com.cairone.odataexample.PersonaSector;

                    itemsToRemove.Add(thisSector);
                }
            }

            myViewModel.RemoveAll(itemsToRemove);

            foreach (com.cairone.odataexample.PersonaSector psector in itemsToRemove)
            {
                myViewModel.SectoresAsignados.Remove(psector);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            IList<com.cairone.odataexample.Sector> itemsToAdd = new List<com.cairone.odataexample.Sector>();

            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            if (this.lstSectoresDisponibles.SelectedItems.Count == 0)
            {
                return;
            }

            int totalCount = 0;
            foreach (object item in this.lstSectoresDisponibles.SelectedItems)
            {
                if (item is com.cairone.odataexample.Sector)
                {
                    totalCount++;
                    com.cairone.odataexample.Sector thisSector = item as com.cairone.odataexample.Sector;

                    itemsToAdd.Add(thisSector);
                }
            }

            com.cairone.odataexample.PersonaSector pSector = null;

            com.cairone.odataexample.Persona persona = null;

            foreach (com.cairone.odataexample.Sector sector in itemsToAdd)
            {
                pSector = myViewModel.SectoresAsignados.Where(x => x.id == sector.id).SingleOrDefault();
                if (pSector == null)
                {
                    pSector = new com.cairone.odataexample.PersonaSector()
                    {
                        id = sector.id,
                        fechaIngreso = DateTime.Today,
                        //numeroDocumento = "12345678",
                        //tipoDocumentoId = 1
                        nombre = string.IsNullOrEmpty(sector.nombre) ? "Nuevo" : sector.nombre
                    };

                    myViewModel.AddSectorToPersona(pSector);

                    foreach (ODataError item in myViewModel.Errors)
                    {
                        this.statusInfo.Text += string.Format("Oper {0}: {1} {2}", item.OperationNumber, item.StatusCode, item.ErrorMessage);
                    }
                    if (myViewModel.Errors.Count == 0)
                    {
                        this.statusInfo.Text += "Ok";
                    }

                    // this should happen if no errors!
                    myViewModel.SectoresAsignados.Add(pSector);

                    // It does not work!
                    //myViewModel.Insert(pSector);

                    /*
                    persona = myViewModel.GetPersonaById(1, "12345678");
                    if (persona != null)
                    {
                        persona.sectores.Add(pSector);

                        myViewModel.Insert(persona);

                        myViewModel.SectoresAsignados.Add(pSector);
                    }*/

                }
            }
        }
    }
}