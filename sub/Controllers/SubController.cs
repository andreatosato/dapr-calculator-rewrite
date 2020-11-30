using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sub.Controllers
{
    [ApiController]
    public class SubController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "operations-store";

        [HttpPost("{sub}")]
        public async Task<IActionResult> MulOperation(
            GenericOperation genericOperation,
            [FromServices] DaprClient daprClient)
        {
            Console.WriteLine("Sub Request");
            decimal resultOperation = genericOperation.FirstOperand - genericOperation.SecondOperand;

            // Add event to eventSource
            var currentOperation = new Operation()
            {
                OperationData = new GenericOperation()
                {
                    FirstOperand = genericOperation.FirstOperand,
                    SecondOperand = genericOperation.SecondOperand,
                    Id = genericOperation.Id
                },
                OperationType = Operation.OperandType.Subtracti
            };
            await daprClient.PublishEventAsync("Calculator", "CalculatorOperation", currentOperation);

            // Set current state
            //var state = await daprClient.GetStateEntryAsync<decimal>(StoreName, genericOperation.Id);
            //state.Value = resultOperation;
            //await state.SaveAsync();

            // Return Ok to client
            return Ok(resultOperation);
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
