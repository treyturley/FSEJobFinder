using System;
using System.Collections.Generic;

namespace FSEDataFeed
{
    //TODO: Refactor this to make as few calls to FSE DATA as possible
    //one call to get all 737, one call to get all 747
    //combine ICAOs into a single list and request the jobs

    //TODO: cache all requests locally after the data has been pulled down from FSE
    //dont query FSE if that data is less than 5 minutes old unless a force refresh is requests

    //TODO: lookup FSE limits for number of hits to their servers per minute
    public class FSEDataAPI
    {
        //TODO: begin using these lists to reduce the number of calls made to FSE Data API
        List<Aircraft> all737_800s;
        List<Aircraft> all747_400s;

        List<Assignment> All737Assignments;
        List<Assignment> All747Assignments;

        //TODO: think about tracking the last time when a pull was made to get 737 or 747 assignments.
        //If the data is less than 5 minutes old for example we should not hit the FSE Data API again.

        List<IcaoJobsFrom> ICAOWithAssignments;

        private FSEDataExport fseDataExport;

        /// <summary>
        /// Constructor for FSEDataAPI
        /// This class interacts with the FSE Data API provided by FSEconomey.net to get
        /// details on available airplanes to rent, available jobs, and other details.
        /// </summary>
        public FSEDataAPI(string userKey)
        {
            all737_800s = new List<Aircraft>();
            all747_400s = new List<Aircraft>();
            All737Assignments = new List<Assignment>();
            All747Assignments = new List<Assignment>();
            ICAOWithAssignments = new List<IcaoJobsFrom>();

            //FIXME: remove this as aircraft data gets populated the first time the user gets assignments for a makemodel 
            GetAircraftItems(AircraftMakeModelStrEnum.Boeing737_800);

            fseDataExport = new FSEDataExport(userKey);

        }

        /// <summary>
        /// Get the AircraftItems (All of the aircraft in FSE that match the Make Model)
        /// </summary>
        /// <param name="makeModel"></param>
        void GetAircraftItems(string makeModel)
        {
            //TODO: delete
        }

        /// <summary>
        /// Returns the best available flight given one of the rentable only planes(737/747).
        /// </summary>
        /// <param name="makeModel"></param>
        /// <returns></returns>
        public Assignment getBestCommercialAssignment(string makeModel)
        {
            //TODO: See if we already have a list assignment for the makeModel and return that if its not stale. otherwise get new data from FSE.
            List<Assignment> availableAssignments = getAllCommercialAssignments(makeModel);

            //sanity check that there are assignments available
            if (availableAssignments.Count == 0)
            {
                throw new Exception("No Assignments were found for makeModel: " + makeModel);
            }

            //sort the list and return the highest paying job
            availableAssignments.Sort();
            return availableAssignments[availableAssignments.Count - 1];
        }

        /// <summary>
        /// Gets the top five Commercial (All-In) assignments that are available for the makeModel.
        /// </summary>
        /// <param name="makeModel">The the airplane to get assignment for</param>
        /// <param name="numAssignments">The max number of assignements to return</param>
        /// <returns></returns>
        public List<Assignment> getCommercialAssignments(string makeModel,int numAssignments = -1)
        {
            //TODO: instead of makemodel strings use the enums
            List<Assignment> result = new List<Assignment>();

            List<Assignment> assignments = getAllCommercialAssignments(makeModel);

            if(assignments.Count == 0)
            {
                throw new Exception("No Assignments were found for makeModel: " + makeModel);
            }

            //sort it and reverse the list so that the best 5 assignments are at the front of the list
            assignments.Sort();
            assignments.Reverse();

            if (numAssignments == -1)
            {
                return assignments;
            }
            else
            {
                for (int i = 0; i < numAssignments && i < assignments.Count; i++)
                {
                    result.Add(assignments[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a list of assignments for the given make model that depart or arrive in the us
        /// </summary>
        /// <param name="makeModel"></param>
        /// <returns></returns>
        public List<Assignment> GetUSAssignments(string makeModel)
        {
            List<Assignment> result = new List<Assignment>();
            List<Assignment> assignments = getAllCommercialAssignments(makeModel);

            if (assignments.Count == 0)
            {
                throw new Exception("No Assignments were found for makeModel: " + makeModel);
            }

            assignments.Sort();
            assignments.Reverse();

            foreach(Assignment assignment in assignments)
            {
                //TODO: remove magic Letter?
                if(assignment.FromIcao.StartsWith("K") || assignment.ToIcao.StartsWith("K"))
                {
                    result.Add(assignment);
                }
            }
            return result;
        }

        private List<Assignment> getAllCommercialAssignments(string makeModel)
        {
            //TODO: we should check to see if the associated aircraft list (all737_800s, all747_400s, etc)
            //      is populated before making a call to the FSE API


            //TODO: input validation on makeModel, must only be makemodels that have "commercial flights"
            //      aka assigned All-In assignments"

            //TODO: Make this return a assignment object?                     
            List<Assignment> availableAssignments = new List<Assignment>();

            if (fseDataExport.requestTracker.remainingRequests() >= 2)
            {
                //Get the current list of all aicraft that match the Make and Model - Query FSE Data Feed'
                AircraftItems allAircraft = fseDataExport.GetAircraftByMakeModel(makeModel);

                //build the list of ICAO that have the plane we want
                List<string> AirportsWithMatchingPlane = new List<string>();
                foreach (Aircraft aircraft in allAircraft.AircraftList)
                {
                    //make sure the plane is not rented and is at a valid location
                    //TODO: if we want to filter by locations we could do that here?
                    if (aircraft.RentedBy.CompareTo("Not rented.") == 0 && aircraft.Location.Length == 4)
                    {
                        AirportsWithMatchingPlane.Add(aircraft.Location);
                    }
                    /* if a plane doest have 4 letters in the location then that plane is currently rented and/or flying
                    else
                    {
                        System.Console.WriteLine("plane has a location with only 3 letters in the icao: " + aircraft.Registration);
                    }
                    */
                }

                //TODO: consider saving this massive list of assignments off somewhere so that they can be retrived later?
                //Query FSE Data Feed for list of jobs from each ICAO that has makeModel - query = ICAO Jobs From            
                IcaoJobsFrom ICAOAssignments = fseDataExport.GetIcaoJobsFrom(AirportsWithMatchingPlane);
                
                if (ICAOAssignments == null)
                {
                    //there was some issue with getting the assignments
                    //TODO: Handle this gracefully
                    throw new Exception("Unable to retrieve assignments");
                }
                else
                {
                    availableAssignments = ICAOAssignments.getCommercialJobs(allAircraft);
                }
            }

            //TODO: refactor this so we dont need to use an if statement on the madeModel. This should be handled with inheritance             
            //List<Assignment> availableAssignments = new List<Assignment>();
            //if (ICAOAssignments == null)
            //{
            //    //there was some issue with getting the assignments
            //    throw new Exception("Unable to retrieve assignments");
            //}
            //else if (makeModel == "Boeing 737-800")
            //{   
            //    availableAssignments = ICAOAssignments.get737Jobs(allAircraft);
            //}
            //else if (makeModel == "Boeing 747-400")
            //{   
            //    availableAssignments = ICAOAssignments.get747Jobs(allAircraft);
            //}
            //else
            //{
            //    //some error has occured
            //    throw new ArgumentException("makeModel is an invalid type", makeModel);
            //}


            return availableAssignments;
        }
    }
}
