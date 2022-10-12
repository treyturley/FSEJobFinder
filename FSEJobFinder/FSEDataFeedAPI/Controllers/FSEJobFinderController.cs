using FSEDataFeed;
using FSEDataFeedAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FSEDataFeedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FSEJobFinderController : ControllerBase
    {
        private FSEDataService fseDataService = new();

        private readonly ILogger<FSEJobFinderController> _logger;

        public FSEJobFinderController(ILogger<FSEJobFinderController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all of the available commercial assignment for the given aircraft MakeModel. 
        /// Can be reduced to a subset by using the count paramater.
        /// </summary>
        /// <param name="makeModel"> - a AircraftMakeModelStrEnum indicating the MakeModel 
        /// of aircraft we want to find jobs for.</param>
        /// <param name="count"> - a paramater to reducde the number of jobs 
        /// returned to the specified count.</param>
        /// <returns>A json result object containing the available assignments.</returns>
        [HttpGet("GetCommercialAssignmentsByMakeModel/{makeModel}")]
        public JsonResult GetCommercialAssignments(AircraftMakeModel.MakeModel makeModel, string userKey, int count = -1)
        {
            string makeModelStr = "";

            // validate makeModel
            if (Enum.IsDefined(typeof(AircraftMakeModel.MakeModel), makeModel))
            {
                makeModelStr = AircraftMakeModel.MakeModelAsString(makeModel);
            }
            else
            {
                return new JsonResult(BadRequest("Invalid MakeModel"));
            }

            // validate userKey
            if(userKey == null || userKey.Length == 0)
            {
                return new JsonResult(BadRequest("Invalid User Key"));
            }

            // Validate count
            if(count < 1 && count != -1)
            {
                return new JsonResult(BadRequest("Count must be positive or not included at all"));
            }
            
            return new JsonResult(fseDataService.GetService(userKey).getCommercialAssignments(makeModelStr, count));
        }

        [HttpGet("getBestCommercialAssignmentByMakeModel/{makeModel}")]
        public JsonResult GetBestCommercialAssignment(string makeModel, string userKey)
        {
            //TODO: validate makeModel
            return new JsonResult(fseDataService.GetService(userKey).getBestCommercialAssignment(makeModel));
        }

        [HttpGet("GetUSCommercialAssignmentsByMakeModel/{makeModel}")]
        public JsonResult GetUSCommercialAssignments(string makeModel, string userKey)
        {
            //TODO: validate makeModel
            return new JsonResult(fseDataService.GetService(userKey).GetUSAssignments(makeModel));
        }
    }
}