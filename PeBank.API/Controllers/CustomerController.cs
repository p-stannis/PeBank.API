using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeBank.API.Features;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PeBank.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets Customer</summary>
        /// <param name="customerId">Client Identifier</param>
        /// <returns>Customer</returns>
        /// <response code="201">Returns the Customer</response>
        /// <response code="400">If client id is not specified </response>
        /// <response code="404">If client is not found </response>
        [HttpGet]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerModel>> Get([FromQuery, Required] int customerId)
        {
            var result = await _mediator.Send(new CustomerGetRequest { Id = customerId });

            if(result == null) 
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>Creates a new Customer</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/customer
        ///     {
        ///         "name": "John Doe",
        ///         "email": "john.doe@pebank.com"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">Client information</param>
        /// <returns>The newly created Customer</returns>
        /// <response code="201">Returns the newly created Customer</response>
        /// <response code="400">If client's name or email are not specified
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerModel>> Create([FromBody, Required] CustomerCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { message = "You must provide both the name and email from customer." });
            }
            var result = await _mediator.Send(request);

            return CreatedAtAction(nameof(Create), result);
        }
    }
}
