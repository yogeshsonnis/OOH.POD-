using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOH.POD.Models
{
    public class Compartment
    {
        public string CompartmentName { get; set; }
        public string CompartmentSize { get; set; }
        public string CompartmentType { get; set; }
        public bool IsClosed { get; set; }
        public bool IsLocked { get; set; }
        public bool IsOutOfService { get; set; }

        public ObservableCollection<Shipment> Shipments { get; set; }
            = new ObservableCollection<Shipment>();
    }

    public class Shipment
    {
        public string TrackingNumber { get; set; }
        public DateTime TimeStored { get; set; }
        public DateTime TimeOverdue { get; set; }
        public string TimeStayed { get; set; }
    }
}
