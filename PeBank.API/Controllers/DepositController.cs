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
    [Route("api/deposit")]
    public class DepositController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepositController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Makes a deposit</summary>
        /// <param name="request">Deposit request</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/deposit
        ///     {
        ///         "customerId": 1,
        ///         "accountId": 1,
        ///         "ammount": 1000,
        ///         "details": "Depositing $1000 to my account"
        ///     }
        /// </remarks>
        /// <returns>The deposit transaction</returns>
        /// <response code="201">Returns the deposit transaction</response>
        /// <response code="400">If client id or account id is not specified or an ammount is not specified or if account does not belong to customer </response>
        /// <response code="404">If account is not found </response>    
        /// <response code="409">If a business exception has been encountered</response>
        [HttpPost]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(TransactionModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> Create([FromBody, Required] DepositCreateRequest request)
        {
            try
            {
                var result = await _mediator.Send(request);

                return CreatedAtAction(nameof(Create), result);
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
    }
}
