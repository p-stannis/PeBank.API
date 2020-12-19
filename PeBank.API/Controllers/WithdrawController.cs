using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;


namespace PeBank.API.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/withdraw")]
    public class WithdrawController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WithdrawController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Makes a withdrawal from account</summary>
        /// <param name="request">Withdrawal request</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/withdraw
        ///     {
        ///         "customerId": 1,
        ///         "accountId": 1,
        ///         "ammount": 500.50,
        ///         "details": "Withdrawing $1000 from my account"
        ///     }
        /// </remarks>
        /// <returns>The Withdrawal transactions</returns>
        /// <response code="201">Returns the Withdrawal transactios</response>
        /// <response code="400">If client id or account id is not specified or an ammount is not specified 
        /// or if account does not belong to customer or if balance will be negative </response>
        /// <response code="404">If account is not found </response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> Create([FromBody, Required] WithdrawCreateRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);

                return CreatedAtAction(nameof(Create), result);
            }
            catch (BusinessException businessException)
            {
                return BadRequest(new { message = businessException.Message });
            }
            catch (NotFoundException notFoundException)
            {
                return BadRequest(new { message = notFoundException.Message });
            }

        }
    }
}
