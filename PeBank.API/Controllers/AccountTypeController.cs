using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeBank.API.Features;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PeBank.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/accounttype")]
    public class AccountTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets Account Types</summary>
        /// <returns>Account Types</returns>
        /// <response code="200">Returns the Account Types</response>
        /// <response code="400">If client id or account id is not specified</response>
        /// <response code="404">If account is not found</response>
        [HttpGet]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(IEnumerable<AccountTypeModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AccountTypeModel>>> Get()
        {
            var accountTypes = await _mediator.Send(new AccountTypeGetAllRequest { });

            return Ok(accountTypes);

        }
    }
}
