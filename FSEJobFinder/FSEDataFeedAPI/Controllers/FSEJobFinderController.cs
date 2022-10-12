using FSEDataFeed;
using FSEDataFeedAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FSEDataFeedAPI.Controllers
{

    /// <summary>
    /// Handles requests to the FSEJobFinder API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FSEJobFinderController : ControllerBase
    {
        private FSEDataService fseDataService = new();

        private readonly ILogger<FSEJobFinderController> _logger;

        /// <summary>
        /// Constructor for the FSEJobFinderController.
        /// </summary>
        /// <param name="logger"></param>
        public FSEJobFinderController(ILogger<FSEJobFinderController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets available jobs for the specified aircraft.
        /// </summary>
        /// <param name="aircraft">The commercial aircraft to find jobs for.</param>
        /// <param name="count">The number of jobs to get back or -1 to get all available jobs.</param>
        /// <returns>A JsonResult containing the available jobs.</returns>
        /// <response code="200">Json body describing the available jobs. Jobs are sorted by payout in descending order.</response>
        /// /// <response code="400">Error code describing the request error.</response>
        [HttpGet("/api/v1/availableJobs/{aircraft}")]
        public ActionResult GetCommercialAssignments(AircraftMakeModel.MakeModel aircraft, int count = -1)
        {
            string makeModelStr = "";
            string userKey = Request.Headers["fse-access-key"];

            // validate makeModel
            if (Enum.IsDefined(typeof(AircraftMakeModel.MakeModel), aircraft))
            {
                makeModelStr = AircraftMakeModel.MakeModelAsString(aircraft);
            }
            else
            {
                // When make model not found we can just return an empty json obj and a 404?

                return BadRequest(new object());
            }

            // validate userKey
            if (userKey == null || userKey.Length == 0)
            {
                //JsonResult result = new JsonResult("UserKey header missing or invalid");
                //result.StatusCode = 400;
                //return BadRequest(result);

                // can create our own json response like this!
                return BadRequest(new
                {
                    message = "fse-access-key header missing or invalid",
                    headerExample = "fse-access-key:<user-fse-access-key>",
                    note = "Access key can be found by logging into the " +
                    "FSE Game World and selecting Data Feeds from the " +
                    "Home menu dropdown. see https://www.fseconomy.net/welcome"
                });
            }

            // Validate count
            if (count < 1 && count != -1)
            {
                return new JsonResult(BadRequest("Count must be positive or not included at all"));
            }

            //TODO: Revert to real jsonresult once testing completes
            //return new JsonResult(fseDataService.GetService(userKey).getCommercialAssignments(makeModelStr, count));
            return new JsonResult(new { success = true });
        }

        //[HttpGet("/{makeModel}/BestJob")]
        //public JsonResult GetBestCommercialAssignment(string makeModel, string userKey)
        //{
        //    //TODO: validate makeModel
        //    return new JsonResult(fseDataService.GetService(userKey).getBestCommercialAssignment(makeModel));
        //}

        //[HttpGet("/{makeModel}/AvailableUSJobs")]
        //public JsonResult GetUSCommercialAssignments(string makeModel, string userKey)
        //{
        //    //TODO: validate makeModel
        //    return new JsonResult(fseDataService.GetService(userKey).GetUSAssignments(makeModel));
        //}
    }
}