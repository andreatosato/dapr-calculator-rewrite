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

        [Topic("Calculator", "CalculatorOperation")]
        //[HttpPost("withdraw")]
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
