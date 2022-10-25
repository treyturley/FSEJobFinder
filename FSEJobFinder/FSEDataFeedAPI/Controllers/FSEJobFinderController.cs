using FSEDataFeed;
using FSEDataFeedAPI.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
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

        //TODO: start logging requests
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
        /// Gets available assignments for the specified aircraft.
        /// </summary>
        /// <param name="aircraft">The commercial aircraft to find assignments for.</param>
        /// <param name="limit">The number of assignments to get back or -1 to get all available assignments.</param>
        /// <returns>A JsonResult containing the available assignments.</returns>
        /// <response code="200">Json body describing the available assignments. 
        /// assignments are sorted by payout in descending order.</response>
        /// <response code="400">Error message describing the request error.</response>
        [HttpGet("v1/assignments/{aircraft}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Assignment> GetCommercialAssignments(AircraftMakeModel.MakeModel aircraft, int limit = -1)
        {
            //TODO: consider adding additional params to allow filtering/sorting/pagination
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
            return new JsonResult(fseDataService.GetService(userKey).getCommercialAssignments(makeModelStr, limit));
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
        public ActionResult<Assignment> GetUSCommercialAssignments(AircraftMakeModel.MakeModel aircraft)
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