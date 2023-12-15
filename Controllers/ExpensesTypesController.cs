namespace BiyLineApi.Controllers;

[ApiController]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/expensesTypes")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
public sealed class ExpensesTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesTypesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetExpensesTypeFeature.Response>>> GetExpenseTypes(
        [FromQuery] ExpenseTypeParams expenseTypeParams)
    {
        var response = await _mediator.Send(new GetExpensesTypeFeature.Request
        {
            PageNumber = expenseTypeParams.PageNumber,
            PageSize = expenseTypeParams.PageSize,
            Wallet = expenseTypeParams.Wallet,
            Type = expenseTypeParams.Type,
            Date = expenseTypeParams.Date
        });

        return response.Match<ActionResult<IReadOnlyList<GetExpensesTypeFeature.Response>>>(
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

    [HttpGet("current-store-expenses-types")]
    public async Task<ActionResult<IReadOnlyList<GetCurrentStoreExpensesTypesFeature.Response>>> GetCurrentStoreExpenseTypes(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetCurrentStoreExpensesTypesFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetCurrentStoreExpensesTypesFeature.Response>>>(
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

    [HttpPost("{storeWalletId}")]
    public async Task<IActionResult> CreateExpenseType(
        [FromRoute] int storeWalletId,
        [FromBody] CreateExpenseTypeFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{storeWalletId}")]
    public async Task<IActionResult> DeleteExpense(
        [FromRoute] int storeWalletId,
        [FromBody] UpdateExpenseTypeFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{expenseTypeId}")]
    public async Task<IActionResult> DeleteExpenseType(
        [FromRoute] int expenseTypeId)
    {
        var response = await _mediator.Send(new DeleteExpenseTypeFeature.Request
        {
            ExpenseTypeId = expenseTypeId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

}
