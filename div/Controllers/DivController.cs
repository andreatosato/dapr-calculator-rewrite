using System;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace div.Controllers
{
    [ApiController]
    public class DivController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "operations-store";

        [HttpPost("{div}")]
        public async Task<IActionResult> DivOperation(
            GenericOperation genericOperation,
            [FromServices] DaprClient daprClient)
        {
            Console.WriteLine("Div Request");
            decimal resultOperation = genericOperation.FirstOperand / genericOperation.SecondOperand;

            // Add event to eventSource
            var currentOperation = new Operation() { 
                OperationData = new GenericOperation() { 
                    FirstOperand = genericOperation.FirstOperand,
                    SecondOperand = genericOperation.SecondOperand,
                    Id = genericOperation.Id
                },
                OperationType = Operation.OperandType.Divide
            };
            await daprClient.PublishEventAsync("Calculator", "CalculatorOperation", currentOperation);

            // Set current state
            var state = await daprClient.GetStateEntryAsync<decimal>(StoreName, genericOperation.Id);
            state.Value = resultOperation;
            await state.SaveAsync();

            // Return Ok to client
            return Ok();
        }

        /// <summary>
        /// Method for returning a BadRequest result which will cause Dapr sidecar to throw an RpcException
        [HttpPost("throwException")]
        public async Task<IActionResult> ThrowException(GenericOperation genericOperation)
        {
            Console.WriteLine("Enter ThrowException");
            var task = Task.Delay(10);
            await task;
            return BadRequest(new { statusCode = 400, message = "bad request" });
        }
    }
}
