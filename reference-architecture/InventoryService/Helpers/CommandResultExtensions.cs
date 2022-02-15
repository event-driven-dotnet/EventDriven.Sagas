using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Helpers;

public static class CommandResultExtensions
{
    public static ActionResult ToActionResult(this CommandResult result, object? entity = null)
    {
        switch (result.Outcome)
        {
            case CommandOutcome.Accepted:
                return entity != null
                    ? new OkObjectResult(entity)
                    : new OkResult();
            case CommandOutcome.Conflict:
                return result.Errors != null
                    ? new ConflictObjectResult(result.Errors)
                    : new ConflictResult();
            case CommandOutcome.NotFound:
                return result.Errors != null
                    ? new NotFoundObjectResult(result.Errors)
                    : new NotFoundResult();
            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}