using FSEDataFeed;
using Microsoft.AspNetCore.Mvc;

namespace FSEDataFeedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FSEJobFinderController : ControllerBase
    {
        private FSEDataAPI fseData = new();

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
        public JsonResult GetCommercialAssignments(string makeModel, int count = -1)
        {
            //TODO: eventually this will need to also get
            //      the users userkey from the params and pass that to getCommercialAssignments

            //TODO: validate makeModel and count
            return new JsonResult(fseData.getCommercialAssignments(makeModel, count));
        }

        [HttpGet("getBestCommercialAssignmentByMakeModel/{makeModel}")]
        public JsonResult GetBestCommercialAssignment(string makeModel)
        {
            //TODO: validate makeModel
            return new JsonResult(fseData.getBestCommercialAssignment(makeModel));
        }

        [HttpGet("GetUSCommercialAssignmentsByMakeModel/{makeModel}")]
        public JsonResult GetUSCommercialAssignments(string makeModel)
        {
            //TODO: validate makeModel
            return new JsonResult(fseData.GetUSAssignments(AircraftMakeModelStrEnum.Boeing747_400));
        }
    }
}