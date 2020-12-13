using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeBank.API.Features;
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

        /// <summary>New Customer</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/customer
        ///     {
        ///         
        ///     }
        ///
        /// </remarks>
        /// <param name="">Client Identifier</param>
        /// <returns>The newly created Customer</returns>
        /// <response code="201">Returns the newly created Customer</response>
        /// <response code="400">If client id is not specified, //SPECIFY OTHER TYPES OF BADREQUEST ???????????????????????????????
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerModel>> Create() 
        {
            return new CustomerModel();
        }
    }
}
