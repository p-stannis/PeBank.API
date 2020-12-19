using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;

namespace PeBank.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/statement")]
    public class StatementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public StatementController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>Gets the account's statements</summary>
        /// <remarks>Gets the bank statements based on query.</remarks>
        /// <param name="accountId">Account Identifier</param>
        /// <param name="customerId">Client Identifier</param>
        /// <returns>The Bank statement</returns>
        /// <response code="200">Returns the account's statement</response>
        /// <response code="400">If client id or account id is not specified</response>
        /// <response code="404">If account is not found</response>
        [HttpGet]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(StatementModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatementModel>> Get([FromQuery, Required] int accountId, [FromQuery, Required] int customerId)
        {
            try
            {
                var account = await _mediator.Send(new AccountGetRequest { CustomerId = customerId, AccountId = accountId });

                var statement = _mapper.Map<StatementModel>(account);

                return statement;
            }
            catch (BusinessException businessException)
            {
                return BadRequest(new { message = businessException.Message });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
