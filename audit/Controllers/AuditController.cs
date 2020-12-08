using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace audit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "operations-history-store";

        [HttpGet("{audit}/{operationId}")]
        public async Task<IActionResult> Audit(Guid operationId,
            [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<OperationHistory>(StoreName, operationId.ToString());
            if (state.Value == null)
                return Ok(OperationHistory.None);

            return Ok(state.Value);
        }

        [Topic("calculator", "CalculatorOperation")]
        public async Task<ActionResult<OperationHistory>> PushElement(Operation operation, [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<OperationHistory>(StoreName, operation.OperationData.Id);

            if(state.Value == null)
                state.Value = new OperationHistory(new Collection<Operation>
                {
                    operation
                });
            else
                state.Value.AddOperation(operation);
            await state.SaveAsync();
            return state.Value;            
        }
    }
}
