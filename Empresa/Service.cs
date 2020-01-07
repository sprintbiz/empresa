using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empresa
{
    class Service : INotifyPropertyChanged
    {
        public int ServiceId { get; set; }
        public int TaxId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
