using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa
{
    public class Product : INotifyPropertyChanged
    {
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
