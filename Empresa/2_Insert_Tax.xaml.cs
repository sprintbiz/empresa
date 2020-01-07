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
    public sealed partial class Scenario2 : Page
    {
        private MainPage rootPage;

        static class Globals
        {
            // global FilterYear
            public static string Clicked;
        }
        public Scenario2()
        {
            this.InitializeComponent();
            TaxList.ItemsSource = GetTaxes((App.Current as App).ConnectionString);
        }
        public ObservableCollection<Tax> GetTaxes(string connectionString)
        {
            string GetTaxesQuery =
                "SELECT [TaxID],[Name],[Value],[Created],[Updated]" +
                "FROM [dbo].[tax]";

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
                                    tax.Value = reader.GetDecimal(2);
                                    tax.Created = reader.GetDateTime(3);
                                    tax.Updated = reader.GetDateTime(4);
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
                            cmd.Parameters.AddWithValue("name", txtTaxName.Text);
                            cmd.Parameters.AddWithValue("value", txtTaxValue.Text);
                            cmd.CommandText = insertTaxQuery;
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            txtTaxName.Text = "";
                            txtTaxValue.Text = "";
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

        private void TaxList_ItemClick(object sender, ItemClickEventArgs e)
        {
            rootPage = MainPage.Current;
            var clickedItem = (Tax)e.ClickedItem;
            Globals.Clicked = clickedItem.Name.ToString();

            txtTaxUpdateName.Text = clickedItem.Name.ToString();
            txtTaxUpdateValue.Text = clickedItem.Value.ToString();
            txtTaxCreated.Text = clickedItem.Created.ToShortDateString() + ' ' + clickedItem.Created.ToShortTimeString();
            txtTaxUpdated.Text = clickedItem.Updated.ToShortDateString() + ' ' + clickedItem.Updated.ToShortTimeString();

            rootPage.SetStatus("Selected : " + clickedItem.Name.ToString());
        }

        private void DeleteSelectedTax_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;

            Tax SelectedTax = (Tax)TaxList.SelectedItem;
            var SelectedTaxId = SelectedTax.TaxId;

            string insertTaxQuery =
                "DELETE FROM [dbo].[tax]" +
                "WHERE [TaxID] = @name";

            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("name", SelectedTaxId);
                        cmd.CommandText = insertTaxQuery;
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        txtTaxName.Text = "";
                        txtTaxValue.Text = "";
                        TaxList.ItemsSource = GetTaxes((App.Current as App).ConnectionString);
                        rootPage.SetStatus("Deleted Tax : " + SelectedTaxId);
                    }
                }
            }
        }

        private void UpdateTax_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;

            string UpdateTaxQuery =
                "UPDATE [dbo].[tax] " +
                "SET NAME = @name, VALUE = @value, Updated = getdate() " +
                "WHERE NAME = @where";

            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("name", txtTaxUpdateName.Text);
                        cmd.Parameters.AddWithValue("value", Convert.ToDecimal(txtTaxUpdateValue.Text));
                        cmd.Parameters.AddWithValue("where", Globals.Clicked);
                        cmd.CommandText = UpdateTaxQuery;
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        TaxList.ItemsSource = GetTaxes((App.Current as App).ConnectionString);
                        rootPage.SetStatus("Deleted Tax :" + txtTaxUpdateName.Text);
                    }
                }
            }
        }
    }
}