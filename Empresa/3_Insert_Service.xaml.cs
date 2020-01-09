using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data.SqlClient;
using Windows.UI.Xaml;
using System.Linq;

namespace Empresa
{
    public sealed partial class Scenario3 : Page
    {
        private MainPage rootPage;

        static class Globals
        {
            // global FilterYear
            public static string Clicked;
        }
        public Scenario3()
        {
            this.InitializeComponent();
            ServiceList.ItemsSource = GetServices((App.Current as App).ConnectionString);
            comTax.ItemsSource = GetTaxes((App.Current as App).ConnectionString, "all");
        }
        private ObservableCollection<Service> GetServices(string connectionString)
        {
            string GetServicesQuery =
                "SELECT [ServiceId],[Name],[UnitPrice],[Version],[TaxId],[Created],[Updated]" +
                "FROM [dbo].[service]";

            var services = new ObservableCollection<Service>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetServicesQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var service = new Service();
                                    service.ServiceId = reader.GetInt32(0);
                                    service.Name = reader.GetString(1);
                                    service.UnitPrice = reader.GetDecimal(2);
                                    service.Version = reader.GetString(3);
                                    service.TaxId = reader.GetInt32(4);
                                    service.Created = reader.GetDateTime(5);
                                    service.Updated = reader.GetDateTime(6);
                                    services.Add(service);
                                }
                            }
                        }
                    }
                }
                return services;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        private ObservableCollection<Tax> GetTaxes(string connectionString, string id)
        {
            string GetTaxesQuery =
                "SELECT [TaxId],[Name]" +
                "FROM [dbo].[tax] ";
            if (id != "all") {
                GetTaxesQuery += "where [TaxId] = 1" ;
            }

            var taxes = new ObservableCollection<Tax>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetTaxesQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var tax = new Tax();
                                    tax.TaxId = reader.GetInt32(0);
                                    tax.Name = reader.GetString(1);
                                    taxes.Add(tax);
                                }
                            }
                        }
                    }
                }
                return taxes;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        private void SubmitNewService_Click(object sender, RoutedEventArgs e)
        {
            string insertServiceQuery =
                "INSERT INTO [dbo].[service]([TaxID],[UnitID],[Name],[UnitPrice],[Version],[Created],[Updated])" +
                "VALUES(@tax, 1, @name, @price, @version, getdate(), getdate())";

            try
            {
                var taxes = new ObservableCollection<Tax>();
                using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Parameters.AddWithValue("tax", comTax.SelectedValue);
                            cmd.Parameters.AddWithValue("name", txtServiceName.Text);
                            cmd.Parameters.AddWithValue("price", txtServiceUnitPrice.Text);
                            cmd.Parameters.AddWithValue("version", txtServiceVersion.Text);
                            cmd.CommandText = insertServiceQuery;
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            txtServiceName.Text = "";
                            txtServiceUnitPrice.Text = "";
                            txtServiceVersion.Text = "";
                            ServiceList.ItemsSource = GetServices((App.Current as App).ConnectionString);
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }

        private void ServiceList_ItemClick(object sender, ItemClickEventArgs e)
        {
            rootPage = MainPage.Current;
            var clickedItem = (Service)e.ClickedItem;
            Globals.Clicked = clickedItem.Name.ToString();

            txtServiceUpdateName.Text = clickedItem.Name.ToString();
            txtServiceUpdateUnitPrice.Text = clickedItem.UnitPrice.ToString();
            txtServiceUpdateVersion.Text = clickedItem.Version.ToString();
            //comUpdateTax.Text = clickedItem.TaxId.ToString();
            txtServiceCreated.Text = clickedItem.Created.ToShortDateString() + ' ' + clickedItem.Created.ToShortTimeString();
            txtServiceUpdated.Text = clickedItem.Updated.ToShortDateString() + ' ' + clickedItem.Updated.ToShortTimeString();

            rootPage.SetStatus("Selected : " + clickedItem.Name.ToString());
            comUpdateTax.ItemsSource = GetTaxes((App.Current as App).ConnectionString,"all");
            var selectedItems = GetTaxes((App.Current as App).ConnectionString, "all");

            int index = -1;
            foreach (Tax item in selectedItems)
            {
                if (item.TaxId == clickedItem.TaxId)
                {
                    index = selectedItems.IndexOf(item);
                    break;
                }
            }
            comUpdateTax.ItemsSource = selectedItems;
            comUpdateTax.SelectedIndex = index;



        }

        private void DeleteSelectedService_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;



        }

        private void UpdateService_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;



        }
    }
}