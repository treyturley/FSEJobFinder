using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FSEDataFeed
{
    [Serializable, XmlRoot("IcaoJobsFrom", Namespace = "https://server.fseconomy.net")]
    public class IcaoJobsFrom
    {
        [XmlElement("Assignment")]
        public List<Assignment> Assignments { get; set; }

        /// <summary>
        /// Search the list of assignments to find all of the assignment with a specific from ICAO
        /// </summary>
        /// <param name="ICAO"></param>
        /// <returns></returns>
        public List<Assignment> findAssignmentByICAO(string FromICAO)
        {
            List<Assignment> result = new List<Assignment>();

            foreach(Assignment assignment in Assignments)
            {
                if(assignment.FromIcao == FromICAO)
                {
                    result.Add(assignment);
                }
            }

            return result;
        }

        //delete this garbage
        public List<Assignment> get737Jobs()
        {

            List<Assignment> results = new List<Assignment>();

            foreach(Assignment assignment in Assignments)
            {
                //TODO: this is no longer a valid way to find 737 jobs
                if(assignment.Commodity == "Pax - Airline Pilot for Hire")
                {
                    results.Add(assignment);
                }
            }
            return results;
        }

        /// <summary>
        /// Finds assignments that match with the given list of aircraft.
        /// </summary>
        /// <param name="all737Aircraft"></param>
        /// <returns></returns>
        public List<Assignment> get737Jobs(AircraftItems all737Aircraft)
        {
            List<Assignment> result = new List<Assignment>();

            //loop over each assignment
            foreach(Assignment assignment in Assignments)
            {
                //check to see if the aicraft in the assignment is in the list of 737s
                if (all737Aircraft.AircraftList.Contains(new Aircraft(assignment.AircraftId)))
                { 
                    result.Add(assignment);
                }
            }

            return result;
        }

        //TODO:refactor, this is the same as above method
        public List<Assignment> get747Jobs(AircraftItems all747Aircraft)
        {
            List<Assignment> result = new List<Assignment>();

            //loop over each assignment
            foreach (Assignment assignment in Assignments)
            {
                //check to see if the aicraft in the assignment is in the list of 737s
                if (all747Aircraft.AircraftList.Contains(new Aircraft(assignment.AircraftId)))
                {
                    result.Add(assignment);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// An assignment is a job in FSE. Key features include a unique job id,
    /// a departure airport, arrival airport, commodity, and pay. 
    /// All-In assignments will also have a non zero AircraftId that represents the serial number of 
    /// aircraft that is tied to the assignment.
    /// </summary>
    public class Assignment : IEquatable<Assignment> , IComparable<Assignment>
    {
        [XmlElement("Id")]
        public string Id { get; set; }
        [XmlElement("Location")]
        public string Location { get; set; }
        [XmlElement("ToIcao")]
        public string ToIcao {get; set;}
        [XmlElement("FromIcao")]
        public string  FromIcao {get; set;}
        [XmlElement("Amount")]
        public string Amount {get; set;}
        [XmlElement("UnitType")]
        public string UnitType {get; set;}
        [XmlElement("Commodity")]
        public string Commodity {get; set;}
        [XmlElement("Pay")]
        public string Pay {get; set;}
        [XmlElement("Expires")]
        public string Expires {get; set;}
        [XmlElement("ExpireDateTime")]
        public string ExpireDateTime {get; set;}
        [XmlElement("Type")]
        public string jobType {get; set;}
        [XmlElement("Express")]
        public string Express {get; set;}
        [XmlElement("PtAssignment")]
        public string PtAssignment {get; set;}        
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
        /// the same place and use the same airplace.
        /// </summary>
        /// <param name="other">The assignment to compare against</param>
        /// <returns></returns>
        public bool Equals(Assignment other)
        {
            if (other == null) return false;

            //assumption: if two assignments start and end at the same airport and
            //use the same plane then they must be the same assignment.
            if(other.FromIcao == this.FromIcao && 
                other.ToIcao == this.ToIcao && 
                other.AircraftId == this.AircraftId)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            //TODO: should probably get handle the exception here?
            return int.Parse(Id);
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}