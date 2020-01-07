using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data.SqlClient;
using Windows.UI.Xaml;

namespace Empresa
{
    public sealed partial class Scenario1 : Page
    {
        static class Globals
        {
            // global FilterYear
            public static string FilterYear;
        }
        public Scenario1()
        {
            this.InitializeComponent();
            InventoryList.ItemsSource = GetProducts((App.Current as App).ConnectionString, Globals.FilterYear);
        }
        public ObservableCollection<InvoiceService> GetProducts(string connectionString, string year)
        {
            if (String.IsNullOrEmpty(year))
            {
                year = DateTime.Now.Year.ToString();
            }

            string GetProductsQuery =
               "SELECT [InvoiceCode]" +
               ",[CustomerName]" +
               ",[Quantity]" +
               ",[CreateDate]" +
               ",[PaymentDate]" +
               "FROM[antman].[dbo].[vw_Invoice]" +
               "WHERE [CreateYear] = " + year;


            var InvoiceServices = new ObservableCollection<InvoiceService>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetProductsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var InvoiceService = new InvoiceService();
                                    InvoiceService.InvoiceCode = reader.GetString(0);
                                    InvoiceService.CustomerName = reader.GetString(1);
                                    InvoiceService.Quantity = reader.GetDecimal(2);
                                    InvoiceService.CreateDate = reader.GetDateTime(3);
                                    InvoiceService.PaymentDate = reader.GetDateTime(4);
                                    InvoiceServices.Add(InvoiceService);
                                }
                            }
                        }
                    }
                }
                return InvoiceServices;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        private void FilterYear_Click(object sender, RoutedEventArgs e)
        {
            InventoryList.ItemsSource = GetProducts((App.Current as App).ConnectionString, txtYear.Text);
        }
    }
}