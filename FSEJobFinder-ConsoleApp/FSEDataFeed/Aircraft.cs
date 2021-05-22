using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FSEDataFeed
{
    public class Aircraft : IEquatable<Aircraft>, IComparable<Aircraft>
    {
        [XmlElement("SerialNumber")]
        public string SerialNumber { get; set; }
        [XmlElement("MakeModel")]
        public string MakeModel { get; set; }
        [XmlElement("Registration")]
        public string Registration { get; set; }
        [XmlElement("Owner")]
        public string Owner { get; set; }
        [XmlElement("Location")]
        public string Location { get; set; }
        [XmlElement("LocationName")]
        public string LocationName { get; set; }
        [XmlElement("Home")]
        public string Home { get; set; }
        [XmlElement("SalePrice")]
        public string SalePrice { get; set; }
        [XmlElement("SellbackPrice")]
        public string SellbackPrice { get; set; }
        [XmlElement("Equipment")]
        public string Equipment { get; set; }
        [XmlElement("RentalDry")]
        public string RentalDry { get; set; }
        [XmlElement("RentalWet")]
        public string RentalWet { get; set; }
        [XmlElement("RentalType")]
        public string RentalType { get; set; }
        [XmlElement("Bonus")]
        public string Bonus { get; set; }
        [XmlElement("RentalTime")]
        public string RentalTime { get; set; }
        [XmlElement("RentedBy")]
        public string RentedBy { get; set; }
        [XmlElement("FuelPct")]
        public string FuelPct { get; set; }
        [XmlElement("NeedsRepair")]
        public string NeedsRepair { get; set; }
        [XmlElement("AirframeTime")]
        public string AirframeTime { get; set; }
        [XmlElement("EngineTime")]
        public string EngineTime { get; set; }
        [XmlElement("TimeLast100hr")]
        public string TimeLast100hr { get; set; }
        [XmlElement("MonthlyFee")]
        public string MonthlyFee { get; set; }
        [XmlElement("FeeOwed")]
        public string FeeOwed { get; set; }

        public Aircraft()
        {

        }

        public Aircraft(string serialNumber)
        {
            this.SerialNumber = serialNumber;
        }


        public int CompareTo(Aircraft other)
        {
            if (other == null) return 1;
            else return this.SerialNumber.CompareTo(other.SerialNumber);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Aircraft objAsAircraftr = obj as Aircraft;
            if (objAsAircraftr == null) return false;
            else return Equals(objAsAircraftr);
        }

        /// <summary>
        /// If two aircraft have the same serial number they must be the same aircraft.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Aircraft other)
        {
            if (other == null) return false;
            if (this.SerialNumber.Equals(other.SerialNumber)) return true;
            return false;
        }

        /// <summary>
        /// checks to see if this plane can be rented
        /// </summary>
        /// <returns></returns>
        public bool IsRentable()
        {
            //TODO: Complete full implementation
            //check for both rental amounts, make sure its not already rented, 
            //make sure its not broke (if broke can we repair it?)

            bool result = false;

            double rentalPrice = 0.0;
            if (!double.TryParse(RentalDry, out rentalPrice))
            {
                //error occured while parsing
                //TODO: handle parse error
            }

            if (rentalPrice != 0.0)
            {
                //its rentable
                if (RentedBy.CompareTo("Not rented.") == 0)
                {
                    if (int.Parse(NeedsRepair) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Auto-generated get hashcode function
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 1562504743;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SerialNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MakeModel);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Registration);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Owner);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Location);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Home);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SalePrice);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SellbackPrice);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Equipment);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RentalDry);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RentalWet);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RentalType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Bonus);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RentalTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RentedBy);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FuelPct);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NeedsRepair);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AirframeTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EngineTime);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TimeLast100hr);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MonthlyFee);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FeeOwed);
            return hashCode;
        }
    }
}