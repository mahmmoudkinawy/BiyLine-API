using BiyLineApi.Features.CashDepostiePermission;
using BiyLineApi.Features.CashDiscountPermission;
using BiyLineApi.Features.SalaryPayment;
using BiyLineApi.Features.StoreWallet;
using BiyLineApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/salaryPayment")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTraderOrEmployee)]
[EnsureSingleStore]
//[EnsureStoreProfileCompleteness]

public class SalaryPaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    public SalaryPaymentController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("{employeeId}")]
    public async Task<IActionResult> CreateSalaryPayment([FromBody] CreateSalaryPaymentFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{storeWalletId}")]
    public async Task<ActionResult<IReadOnlyList<GetAllEmployeesWithLastSalaryPaymentFeature.Response>>> GetAllEmployeesWithLastSalaryPayment(
               [FromQuery] FilterParams filterParams
           )
    {
        var response = await _mediator.Send(new GetAllEmployeesWithLastSalaryPaymentFeature.Request
        {
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize,
            Predicate = filterParams.Predicate,
        });

        Response.AddPaginationHeader(
           response.CurrentPage,
           response.PageSize,
           response.TotalPages,
           response.TotalCount);

        return Ok(response);
    }


    [HttpGet("employees/{employeeId}")]

    public async Task <ActionResult<IReadOnlyList<GetSalaryPaymentsForEmployeeFeature.Response>>> GetSalaryPaymentsForEmployee (
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetSalaryPaymentsForEmployeeFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
        });

        return response.Match<ActionResult<IReadOnlyList<GetSalaryPaymentsForEmployeeFeature.Response>>>(
           successResponse =>
           {
               Response.AddPaginationHeader(
                   successResponse.CurrentPage,
                   successResponse.PageSize,
                   successResponse.TotalPages,
                   successResponse.TotalCount);

               return Ok(successResponse);
           },
            errorResponse =>
            {
                return NotFound(errorResponse.Errors);
            });
    }


    [HttpGet("employees/{employeeId}/last-payment-salary")]

    public async Task <ActionResult<GetLastSalaryPaymentForEmployeeFeature.Response>> GetLastSalaryPaymentForEmployeeFeature(
        [FromRoute] int employeeId)
    {
        var response = await _mediator.Send(new GetLastSalaryPaymentForEmployeeFeature.Request { EmployeeId = employeeId });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

       return Ok(response.Value); 
    }


}
