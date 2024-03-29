using FSEDataFeed;
using FSEDataFeedAPI.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using static FSEDataFeed.AircraftMakeModel;
using FSEDataFeedAPI.Models;


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

        // TODO: start logging requests
        private readonly ILogger<FSEJobFinderController> _logger;

        private readonly MongoDBService _mongoDBService;

        /// <summary>
        /// Constructor for the FSEJobFinderController.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mongoDBService"></param>
        public FSEJobFinderController(ILogger<FSEJobFinderController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _mongoDBService = mongoDBService;
        }

        /// <summary>
        /// Gets a dictionary containing the MakeModel to string mappings.
        /// </summary>
        /// <returns>A JsonResult with a dictionary which maps makemodel enums to readable strings.</returns>
        /// <response code="200">Json body containing the dictionar which maps the makemodel enum to readable strings.</response>
        [HttpGet("v2/makemodels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Dictionary<MakeModel, string>> GetMakeModels_V2()
        {
            return new JsonResult(AircraftMakeModel.GetMakeModelDictionary());
        }

        /// <summary>
        /// Gets available assignments for the specified aircraft.
        /// </summary>
        /// <param name="aircraft">The commercial aircraft to find assignments for.</param>
        /// <param name="limit">The number of assignments to get back or -1 to get all available assignments.</param>
        /// <returns>A JsonResult containing the latest available assignments.</returns>
        /// <response code="200">Json body describing the available assignments. 
        /// assignments are sorted by payout in descending order.</response>
        /// <response code="400">Error message describing the request error.</response>
        [HttpGet("v1/assignments/{aircraft}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Assignment>>> GetCommercialAssignments(AircraftMakeModel.MakeModel aircraft, int limit = -1)
        {
            // TODO: consider adding additional params to allow filtering/sorting/pagination
            string makeModelStr = AircraftMakeModel.GetMakeModelString(aircraft);
            string userKey = Request.Headers["fse-access-key"];

            // validate userKey
            if (!fseDataService.userKeyIsValid(userKey))
            {
                return InvalidUserKeyResponse();
            }

            // Validate limit
            if (limit < 1 && limit != -1)
            {
                return BadRequest(new
                {
                    errorMsg = "Limit must be positive or not included at all"
                });
            }
            List<Assignment> assignmentList = fseDataService.GetService(userKey).getCommercialAssignments(makeModelStr, limit);

            // only replace the existing recent jobs if some jobs were retrieved from FSE
            if (assignmentList.Count > 0)
            {
                await _mongoDBService.UpdateAssignmentByMakeModelAsync(makeModelStr, new Assignments { aircraft = makeModelStr, jobs = assignmentList, updatedAt = DateTime.Now });
            }

            return new JsonResult(assignmentList);
        }

        /// <summary>
        /// Gets assignments from the DB that were retrieved in a previous assignments request.
        /// </summary>
        /// <param name="aircraft">The commercial aircraft to find assignments for.</param>
        /// <returns>A JsonResult containing previously retrieved assignments.</returns>
        [HttpGet("v1/getRecentAssignments/{aircraft}")]
        public async Task<Assignments> GetRecentCommercialAssignments(AircraftMakeModel.MakeModel aircraft)
        {
            string makeModelStr = AircraftMakeModel.GetMakeModelString(aircraft);
            return await _mongoDBService.GetAssignmentByMakeModelAsync(makeModelStr);
        }

        /// <summary>
        /// Adds a new assignment entry to the database.
        /// </summary>
        /// <param name="aircraft">The aircraft makemodel associated to the assignments.</param>
        /// <param name="assignments">The assignments to be added to the DB.</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("v1/addRecentAssignments/{aircraft}")]
        public async Task<IActionResult> AddRecentAssignmentsByMakeModel(AircraftMakeModel.MakeModel aircraft, [FromBody] Assignments assignments)
        {
            string makeModelStr = AircraftMakeModel.GetMakeModelString(aircraft);
            await _mongoDBService.CreateAsync(assignments);
            return CreatedAtAction(nameof(GetRecentCommercialAssignments), new { assignments.aircraft }, assignments);
        }

        /// <summary>
        /// Gets the highest paying assignment for the specified aircraft.
        /// </summary>
        /// <param name="aircraft">The aircraft to find the best assignment for.</param>
        /// <returns>A JsonResult containing the best available assignment for the aircraft.</returns>
        /// <response code="200">Json body describing the assignment.</response>
        /// <response code="400">Error message describing the request error.</response>
        [HttpGet("v1/bestAssignment/{aircraft}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Assignment> GetBestCommercialAssignment(AircraftMakeModel.MakeModel aircraft)
        {
            string makeModelStr = AircraftMakeModel.GetMakeModelString(aircraft);
            string userKey = Request.Headers["fse-access-key"];

            // validate userKey
            if (!fseDataService.userKeyIsValid(userKey))
            {
                return InvalidUserKeyResponse();
            }
            return new JsonResult(fseDataService.GetService(userKey).getBestCommercialAssignment(makeModelStr));
        }

        /// <summary>
        /// Gets all assignments that either start or end in the US for the specified aircraft.
        /// </summary>
        /// <param name="aircraft">The aircraft to find assignments for.</param>
        /// <returns>A JsonResult containing the assignments that start or end in the US for the aircraft.</returns>
        /// <response code="200">Json body describing the assignments.</response>
        /// <response code="400">Error message describing the request error.</response>
        [HttpGet("v1/assignmentsFromOrToUS/{aircraft}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Assignment>> GetUSCommercialAssignments(AircraftMakeModel.MakeModel aircraft)
        {
            string makeModelStr = AircraftMakeModel.GetMakeModelString(aircraft);
            string userKey = Request.Headers["fse-access-key"];

            // validate userKey
            if (!fseDataService.userKeyIsValid(userKey))
            {
                return InvalidUserKeyResponse();
            }
            return new JsonResult(fseDataService.GetService(userKey).GetUSAssignments(makeModelStr));
        }

        /// <summary>
        /// In Prod env, this handles errors that resulted in excpetions.
        /// Instead of sending the user the exception details like when in dev mode,
        /// this just sends a ProblemDetails response to the client.
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HandleError()
        {
            var excpetionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
            var requestFeature = HttpContext.Features.Get<IHttpRequestFeature>()!;
            using (StreamWriter w = System.IO.File.AppendText("FSEData.log"))
            {
                string errorMsg = string.Empty;
                errorMsg += DateTime.Now + " - Error - Request: " + requestFeature.RawTarget + ", ErrorMsg: " + excpetionHandlerFeature.Error;

                Console.Error.WriteLine(errorMsg);
                w.WriteLine(errorMsg);
            }
            return Problem();
        }

        /// <summary>
        /// Returns a 400 bad request response with details describing the error caused by
        /// a missing or invalid userkey.
        /// </summary>
        /// <returns>A BadRequestObjectResult with the error details.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public BadRequestObjectResult InvalidUserKeyResponse()
        {
            return BadRequest(new
            {
                error = "fse-access-key header missing or invalid",
                headerExample = "fse-access-key:<user-fse-access-key>",
                note = "Access key can be found by logging into the " +
                                "FSE Game World and selecting Data Feeds from the " +
                                "Home menu dropdown. See https://www.fseconomy.net/welcome"
            });
        }
    }
}