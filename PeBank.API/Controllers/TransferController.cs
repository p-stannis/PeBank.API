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
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransferController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Makes a transfer</summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/transfer
        ///     {
        ///         "customerId": 1,
        ///         "accountId": 1,
        ///         "ammount": 1000,
        ///         "details": "Depositing $1000 to my friends account",
        ///         "recipientCustomerId": 2,
        ///         "recipientAccountId": 2,
        ///     }
        /// </remarks>
        /// <param name="request">Transfer request</param>
        /// <returns>The transfer transactions</returns>
        /// <response code="201">Returns the transfer transactios</response>
        /// <response code="400">If client id or account id is not specified or an ammount is not specified or if account does not belong to customer </response>
        /// <response code="404">If account is not found </response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> Create([FromBody, Required] TransferCreateRequest request)
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
