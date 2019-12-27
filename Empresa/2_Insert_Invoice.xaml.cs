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
    public sealed partial class Scenario2 : Page
    {
        static class Globals
        {
            // global FilterYear
            public static string FilterYear;
        }
        public Scenario2()
        {
            this.InitializeComponent();
            TaxList.ItemsSource = GetTaxes((App.Current as App).ConnectionString);
        }
        public ObservableCollection<Tax> GetTaxes(string connectionString)
        {
            string GetTaxesQuery =
                "SELECT [Name],[Value]" +
                "FROM [dbo].[tax]";


            var products = new ObservableCollection<Tax>();
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
                                    tax.Name = reader.GetString(0);
                                    tax.Value = reader.GetDecimal(1);
                                    products.Add(tax);
                                }
                            }
                        }
                    }
                }
                return products;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
        private void SubmitNewTax_Click(object sender, RoutedEventArgs e)
        {
            string insertTaxQuery =
                "INSERT INTO [dbo].[tax]([Name],[Value],[Created],[Updated])" +
                "VALUES(@name, @value, getdate(), getdate())";

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
                            cmd.Parameters.AddWithValue("name", txtInvoiceName.Text);
                            cmd.Parameters.AddWithValue("value", txtInvoiceValue.Text);
                            cmd.CommandText = insertTaxQuery;
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            txtInvoiceName.Text = "";
                            txtInvoiceValue.Text = "";
                            TaxList.ItemsSource = GetTaxes((App.Current as App).ConnectionString);
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }

        private void DeleteSelectedTax_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}