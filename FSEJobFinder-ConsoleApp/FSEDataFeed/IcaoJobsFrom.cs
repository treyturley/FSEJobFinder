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

        public List<Assignment> getCommercialJobs(AircraftItems allAircraft)
        {
            //TODO: input validation to make sure we only got aircraft that can have commercial (aka All-In Reserved) flights
            List<Assignment> result = new List<Assignment>();

            //loop over each assignment
            foreach (Assignment assignment in Assignments)
            {
                //if the assignment aicraft id is zero then its not an all in assignment and we can short circuit this so no obj has to be created
                //check to see if the aicraft in the assignment is in the list of 737s
                if (allAircraft.AircraftList.Contains(new Aircraft(assignment.AircraftId)))
                {
                    result.Add(assignment);
                }
            }
            return result;
        }
    }
}
