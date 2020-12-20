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
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        
        public AccountController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>Gets Account</summary>
        /// <remarks>Gets an account based on query.</remarks>
        /// <param name="accountId">Account Identifier</param>
        /// <param name="customerId">Client Identifier</param>
        /// <returns>The Account</returns>
        /// <response code="200">Returns the Account</response>
        /// <response code="400">If client id or account id is not specified</response>
        /// <response code="404">If account is not found</response>
        /// <response code="409">If a business exception has been encountered</response>
        [HttpGet]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(AccountViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountViewModel>> Get([FromQuery, Required] int accountId, [FromQuery, Required] int customerId)
        {
            try
            {
                var account = await _mediator.Send(new AccountGetRequest { CustomerId = customerId, AccountId = accountId });

                var result = _mapper.Map<AccountViewModel>(account);

                return result;
            }
            catch (BusinessException businessException)
            {
                return Conflict(new { message = businessException.Message });
            }
            catch (NotFoundException notFoundException)
            {
                return NotFound(new { message = notFoundException.Message });
            }
        }

        /// <summary>Creates a new Account</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account
        ///     {
        ///         "customerId": 1,
        ///         "accountTypeId": 2
        ///     }
        ///
        /// </remarks>
        /// <param name="request">Account information</param>
        /// <returns>The newly created Account</returns>
        /// <response code="201">Returns the newly created Account</response>
        /// <response code="400">If client's name or email are not specified
        /// <response code="409">If a business exception has been encountered</response>
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(AccountModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CustomerModel>> Create([FromBody, Required] AccountCreateRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);

                return CreatedAtAction(nameof(Create), result);
            }
            catch (BusinessException businessException)
            {
                return Conflict(new { message = businessException.Message});
            }
        }
    }
}
