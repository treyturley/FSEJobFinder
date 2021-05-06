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
}
