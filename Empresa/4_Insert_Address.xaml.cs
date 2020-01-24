using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data.SqlClient;
using Windows.UI.Xaml;
using System.Linq;
using System.Data;

namespace Empresa
{
    public sealed partial class Scenario4 : Page
    {
        private MainPage rootPage;

        static class Globals
        {
            // global FilterYear
            public static string Clicked;
        }
        public Scenario4()
        {
            this.InitializeComponent();
            AddressList.ItemsSource = GetAddresses((App.Current as App).ConnectionString);
            //comTax.ItemsSource = GetTaxes((App.Current as App).ConnectionString, "all");
        }
        private ObservableCollection<Address> GetAddresses(string connectionString)
        {
            string GetAddressQuery =
                "SELECT [AddressId],[AddressType],[Name],[StreetName],[StreetNumber],[ZipCode],[City],[Country],[Created],[Updated]" +
                "FROM [dbo].[address]";

            var addresses = new ObservableCollection<Address>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetAddressQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var address = new Address();
                                    address.AddressId = reader.GetInt32(0);
                                    address.AddressType = reader.GetString(1);
                                    address.Name = reader.GetString(2);
                                    address.StreetName = reader.GetString(3);
                                    address.StreetNumber = reader.GetString(4);
                                    address.ZipCode = reader.GetString(5);
                                    address.City = reader.GetString(6);
                                    address.Country = reader.GetString(7);
                                    address.Created = reader.GetDateTime(8);
                                    address.Updated = reader.GetDateTime(9);
                                    addresses.Add(address);
                                }
                            }
                        }
                    }
                }
                return addresses;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }

        private void SubmitNewAddress_Click(object sender, RoutedEventArgs e)
        {
            string insertServiceQuery =
                "INSERT INTO [dbo].[address]([AddressType],[Name],[StreetName],[StreetNumber],[ZipCode],[City],[Country],[Created],[Updated])" +
                "VALUES(@type, @name, @street, @streetnbr, @zipcode, @city, @country, getdate(), getdate())";

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
                            cmd.Parameters.AddWithValue("type", txtAddressType.Text);
                            cmd.Parameters.AddWithValue("name", txtAddressName.Text);
                            cmd.Parameters.AddWithValue("street", txtAddressStreetName.Text);
                            cmd.Parameters.AddWithValue("streetnbr", txtAddressStreetNumber.Text);
                            cmd.Parameters.AddWithValue("zipcode", txtAddressZipCode.Text);
                            cmd.Parameters.AddWithValue("city", txtAddressCity.Text);
                            cmd.Parameters.AddWithValue("country", txtAddressCountry.Text);
                            cmd.CommandText = insertServiceQuery;
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            txtAddressType.Text = "";
                            txtAddressName.Text = "";
                            txtAddressStreetName.Text = "";
                            txtAddressStreetNumber.Text = "";
                            txtAddressZipCode.Text = "";
                            txtAddressCity.Text = "";
                            txtAddressCountry.Text = "";
                            AddressList.ItemsSource = GetAddresses((App.Current as App).ConnectionString);
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
        }

        private void AddressList_ItemClick(object sender, ItemClickEventArgs e)
        {
            rootPage = MainPage.Current;
            var clickedItem = (Address)e.ClickedItem;
            Globals.Clicked = clickedItem.Name.ToString();

            txtUpdateAddressType.Text = clickedItem.AddressType.ToString();
            txtUpdateAddressName.Text = clickedItem.Name.ToString();
            txtUpdateAddressStreetName.Text = clickedItem.StreetName.ToString();
            txtUpdateAddressStreetNumber.Text = clickedItem.StreetNumber.ToString();
            txtUpdateAddressZipCode.Text = clickedItem.ZipCode.ToString();
            txtUpdateAddressCity.Text = clickedItem.City.ToString();
            txtUpdateAddressCountry.Text = clickedItem.Country.ToString();
            //comUpdateTax.Text = clickedItem.TaxId.ToString();
            txtAddressCreated.Text = clickedItem.Created.ToShortDateString() + ' ' + clickedItem.Created.ToShortTimeString();
            txtAddressUpdated.Text = clickedItem.Updated.ToShortDateString() + ' ' + clickedItem.Updated.ToShortTimeString();

            rootPage.SetStatus("Selected : " + clickedItem.Name.ToString());

        }

        private void DeleteSelectedAddress_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;

            Address SelectedAddress = (Address)AddressList.SelectedItem;
            var SelectedAddressId = SelectedAddress.AddressId;

            string DeleteAddressQuery =
                "DELETE FROM [dbo].[address]" +
                "WHERE [AddressID] = @id";

            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("id", SelectedAddressId);
                        cmd.CommandText = DeleteAddressQuery;
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        txtUpdateAddressType.Text = "";
                        txtUpdateAddressName.Text = "";
                        txtUpdateAddressStreetName.Text = "";
                        txtUpdateAddressStreetNumber.Text = "";
                        txtUpdateAddressZipCode.Text = "";
                        txtUpdateAddressCity.Text = "";
                        txtUpdateAddressCountry.Text = "";
                        AddressList.ItemsSource = GetAddresses((App.Current as App).ConnectionString);
                        rootPage.SetStatus("Deleted Service : " + SelectedAddressId);
                    }
                }
            }
        }

        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            rootPage = MainPage.Current;

            string UpdateServiceQuery =
                "UPDATE [dbo].[address] " +
                "SET [AddressType] = @type,[Name] = @name,[StreetName] = @streetname,[StreetNumber] = @streetnumber,[ZipCode] = @zipcode,[City] = @city,[Country] = @country, Updated = getdate() " +
                "WHERE NAME = @where";

            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("type", txtUpdateAddressType.Text);
                        cmd.Parameters.AddWithValue("name", txtUpdateAddressName.Text);
                        cmd.Parameters.AddWithValue("streetname", txtUpdateAddressStreetName.Text);
                        cmd.Parameters.AddWithValue("streetnumber", txtUpdateAddressStreetNumber.Text);
                        cmd.Parameters.AddWithValue("zipcode", txtUpdateAddressZipCode.Text);
                        cmd.Parameters.AddWithValue("city", txtUpdateAddressCity.Text);
                        cmd.Parameters.AddWithValue("country", txtUpdateAddressCountry.Text);
                        cmd.Parameters.AddWithValue("where", Globals.Clicked);
                        cmd.CommandText = UpdateServiceQuery;
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        AddressList.ItemsSource = GetAddresses((App.Current as App).ConnectionString);
                        rootPage.SetStatus("Updated Service :" + txtUpdateAddressName.Text);
                    }
                }
            }
        }
    }
}