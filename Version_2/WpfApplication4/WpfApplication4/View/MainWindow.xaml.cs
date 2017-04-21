
namespace WpfApplication4.View
{
    using EIV.OData.Core;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Linq;
    using System;
    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Dependency: OData.Client 6.15+
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SECTOR_DISPONIBLE_ID = "id";
        private const string SECTOR_DISPONIBLE_NOMBRE = "nombre";

        private const string SECTOR_ASIGNADO_ID = "id";
        private const string SECTOR_ASIGNADO_NOMBRE = "nombre";

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new SectoresViewModel();

            this.SetUpListas();
        }

        private void SetUpListas()
        {
            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            this.lstSectoresDisponibles.DisplayMemberPath = SECTOR_DISPONIBLE_NOMBRE;
            this.lstSectoresDisponibles.SelectedValuePath = SECTOR_DISPONIBLE_ID;

            this.lstSectoresDisponibles.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.Sectores });

            this.lstSectoresAsignados.DisplayMemberPath = SECTOR_ASIGNADO_NOMBRE;
            this.lstSectoresAsignados.SelectedValuePath = SECTOR_ASIGNADO_ID;

            this.lstSectoresAsignados.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.SectoresAsignados });

            // Testing ...
            //List<com.cairone.odataexample.PersonaSector> pSectorList = myViewModel.SectoresAsignados.ToList();
            //myViewModel.RemoveAll(pSectorList);

            // This could be in the xaml file
            this.btnAdd.Click += BtnAdd_Click;
            this.btnRemove.Click += BtnRemove_Click;

            this.btnGuardar.Click += BtnGuardar_Click;
            this.btnReset.Click += BtnReset_Click;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rst;

            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            int totalChanges = myViewModel.TotalChanges;

            if (totalChanges < 1)
            {
                System.Windows.MessageBox.Show("No se ha realizado ningun cambio.", "Sectores::Reset", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            rst = System.Windows.MessageBox.Show("Todos los cambios realizados seran perdidos. Desea proceder?", "Sectores::Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rst == MessageBoxResult.Yes)
            {
                myViewModel.RejectChanges();

                // I want to force a Reload!!!!!
                this.lstSectoresDisponibles.ItemsSource = null;
                this.lstSectoresDisponibles.Items.Clear();

                this.lstSectoresDisponibles.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.Sectores });

                this.lstSectoresAsignados.ItemsSource = null;
                this.lstSectoresAsignados.Items.Clear();

                this.lstSectoresAsignados.SetBinding(ListBox.ItemsSourceProperty, new Binding { Source = myViewModel.SectoresAsignados });

                this.statusInfo.Text = string.Empty;
            }
        }


        // Function 
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
                        tipoDocumentoId = myViewModel.UsuarioId,
                        numeroDocumento = myViewModel.UsuarioDocumento,
                        nombre = string.IsNullOrEmpty(sector.nombre) ? "Nuevo" : sector.nombre
                    };

                    // Execute Action
                    /* myViewModel.AddSectorToPersona(pSector);

                    foreach (ODataError item in myViewModel.Errors)
                    {
                        this.statusInfo.Text += string.Format("Oper {0}: {1} {2}", item.OperationNumber, item.StatusCode, item.ErrorMessage);
                    }
                    if (myViewModel.Errors.Count == 0)
                    {
                        this.statusInfo.Text += "Ok";
                    }*/

                    // this should happen if no errors!
                    myViewModel.SectoresAsignados.Add(pSector);

                    // It does not work!
                    // Testing ....
                    myViewModel.Insert(pSector);

                    /*
                    persona = myViewModel.GetPersonaById(1, "12345678");
                    if (persona != null)
                    {
                        persona.sectores.Add(pSector);

                        myViewModel.Insert(persona);

                        myViewModel.SectoresAsignados.Add(pSector);
                    }*/

                }
                else
                {
                    this.statusInfo.Text += pSector.id;
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

            // Testing ...
            myViewModel.RemoveAll(itemsToRemove);

            foreach (com.cairone.odataexample.PersonaSector psector in itemsToRemove)
            {
                myViewModel.SectoresAsignados.Remove(psector);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rst;

            SectoresViewModel myViewModel = this.DataContext as SectoresViewModel;

            int totalChanges = myViewModel.TotalChanges;

            // Testing ...
            myViewModel.TestAddingSectorToPersonaViaPersonaEntity(myViewModel.UsuarioId, myViewModel.UsuarioDocumento);
            return;

            if (totalChanges < 1)
            {
                System.Windows.MessageBox.Show("No se ha realizado ningun cambio.", "Sectores::Guardar cambios", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            rst = System.Windows.MessageBox.Show(string.Format("Se guardaran todos los cambios realizados hasta el momento: {0}. Desea proceder?", totalChanges), "Sectores::Guardar cambios", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rst == MessageBoxResult.Yes)
            {
                myViewModel.SubmitChanges();

                foreach (ODataError item in myViewModel.Errors)
                {
                    this.statusInfo.Text += string.Format("Oper {0}: Status Code: {1} Error: {2}: {3}", item.OperationNumber, item.StatusCode, item.ErrorCode, item.ErrorMessage);
                }
                if (myViewModel.Errors.Count == 0)
                {
                    this.statusInfo.Text += "Ok";
                }
            }
        }
    }
}