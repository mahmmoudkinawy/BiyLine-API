namespace BiyLineApi.Controllers;

[ApiController]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/expenses")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
public sealed class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetExpensesFeature.Response>>> GetExpense(
        [FromQuery] ExpenseParams expenseParams)
    {
        var response = await _mediator.Send(new GetExpensesFeature.Request
        {
            PageNumber = expenseParams.PageNumber,
            PageSize = expenseParams.PageSize,
            Wallet = expenseParams.Wallet,
            Date = expenseParams.Date
        });

        return response.Match<ActionResult<IReadOnlyList<GetExpensesFeature.Response>>>(
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

    [HttpGet("{expenseId}")]
    public async Task<ActionResult<GetExpenseByIdFeature.Response>> GetExpenseById(
        [FromRoute] int expenseId)
    {
        var response = await _mediator.Send(new GetExpenseByIdFeature.Request
        {
            ExpenseId = expenseId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{expenseId}")]
    public async Task<IActionResult> UpdateExpense(
        [FromRoute] int expenseId,
        [FromBody] UpdateExpenseFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{expenseId}")]
    public async Task<IActionResult> DeleteExpense(
        [FromRoute] int expenseId)
    {
        var response = await _mediator.Send(new DeleteExpenseFeature.Request
        {
            ExpenseId = expenseId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
