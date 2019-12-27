using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Empresa
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    /// 
    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        public static MainPage Current;

        static class Globals
        {
            // global FilterYear
            public static string FilterYear;
        }

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Invoices", ClassType=typeof(Scenario1)},
            new Scenario() { Title="New Invoice", ClassType=typeof(Scenario2)},
            //new Scenario() { Title="Device Capabilities", ClassType=typeof(Scenario3)},
            //new Scenario() { Title="XAML Manipulations", ClassType=typeof(Scenario4)},
            //new Scenario() { Title="Gesture Recognizer", ClassType=typeof(Scenario5)}
        };

        public MainPage()
        {
            Current = this;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            var itemCollection = new List<Scenario>();
            int i = 1;
            foreach (Scenario s in scenarios)
            {
                itemCollection.Add(new Scenario { Title = $"{i++}) {s.Title}", ClassType = s.ClassType });
            }
            ScenarioControl.ItemsSource = itemCollection;

            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }
    }
}
