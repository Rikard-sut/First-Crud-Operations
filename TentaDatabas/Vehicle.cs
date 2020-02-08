using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TentaDatabas
{
    class Vehicle
    {
        private string registrationNumber;
        private int vehicleTypeID;
        public DateTime Arrival { get; set; }
        public DateTime CheckOutTime { get; set; }
        public DateTime TotalTime { get; set; }
        public int? Oldspot { get; set; }
        public int CurrentSpot { get; set; }
        public int TotalCost { get; set; }
        public Vehicle()
        {
            this.registrationNumber = RegistrationNummber;
            this.vehicleTypeID = VehicleTypeID;
        }
        public string RegistrationNummber
        {
            get { return this.registrationNumber; }
            set { this.registrationNumber = value; }
        }
        public int VehicleTypeID
        {
            get { return this.vehicleTypeID; }
            set { this.vehicleTypeID = value; }
        }
        public override string ToString()
        {
            StringBuilder sr = new StringBuilder();
            sr.Append("RegNmbr: " + this.registrationNumber);
            sr.Append(" Arrivaltime: " + this.Arrival);
            sr.Append(" CheckoutTime: " + this.CheckOutTime);
            sr.AppendLine();
            sr.Append("TotalCost: " + this.TotalCost + "Kr");
            if(this.Oldspot == null)
            {
                sr.Append(" ParkedSpot: " + this.CurrentSpot);
            }
            else
            {
                sr.Append(" Oldspot: " + this.Oldspot);
                sr.Append(" NewSpot: " + this.CurrentSpot);
            }
            if(this.vehicleTypeID == 1)
            {
                sr.Append(" Type: MC");
            }
            if(this.vehicleTypeID == 2)
            {
                sr.Append(" Type: CAR");
            }
            sr.AppendLine();
            return sr.ToString();
        }

    }
}
