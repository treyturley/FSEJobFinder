using System;
using System.Xml.Serialization;

namespace FSEDataFeed
{
    /// <summary>
    /// An assignment is a job in FSE. Key features include a unique job id,
    /// a departure airport, arrival airport, commodity, and pay. 
    /// All-In assignments will also have a non zero AircraftId that represents the serial number of 
    /// aircraft that is tied to the assignment.
    /// </summary>
    public class Assignment : IEquatable<Assignment>, IComparable<Assignment>
    {
        [XmlElement("Id")]
        public string Id { get; set; }
        [XmlElement("Location")]
        public string Location { get; set; }
        [XmlElement("ToIcao")]
        public string ToIcao { get; set; }
        [XmlElement("FromIcao")]
        public string FromIcao { get; set; }
        [XmlElement("Amount")]
        public string Amount { get; set; }
        [XmlElement("UnitType")]
        public string UnitType { get; set; }
        [XmlElement("Commodity")]
        public string Commodity { get; set; }
        [XmlElement("Pay")]
        public string Pay { get; set; }
        [XmlElement("Expires")]
        public string Expires { get; set; }
        [XmlElement("ExpireDateTime")]
        public string ExpireDateTime { get; set; }
        [XmlElement("Type")]
        public string jobType { get; set; }
        [XmlElement("Express")]
        public string Express { get; set; }
        [XmlElement("PtAssignment")]
        public string PtAssignment { get; set; }
        [XmlElement("AircraftId")]
        public string AircraftId { get; set; }

        public int CompareTo(Assignment other)
        {
            if (other == null) return 1;
            else
            {
                double thisPay = double.Parse(this.Pay);
                double otherPay = double.Parse(other.Pay);
                return thisPay.CompareTo(otherPay);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Assignment objAsAssignment = obj as Assignment;
            if (objAsAssignment == null) return false;
            else return Equals(objAsAssignment);
        }

        /// <summary>
        /// We assume two assignments are equal if they start and end at 
        /// the same place and use the same airplane.
        /// </summary>
        /// <param name="other">The assignment to compare against</param>
        /// <returns></returns>
        public bool Equals(Assignment other)
        {
            if (other == null) return false;

            //assumption: if two assignments start and end at the same airport and
            //use the same plane then they must be the same assignment.
            if (other.FromIcao == this.FromIcao &&
                other.ToIcao == this.ToIcao &&
                other.AircraftId == this.AircraftId)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            //TODO: should probably handle the exception here?
            return int.Parse(Id);
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
