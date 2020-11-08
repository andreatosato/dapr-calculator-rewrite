namespace AddSample.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Dapr;
    using Dapr.Client;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public class AddController : ControllerBase
    {
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "operations-store";

        [HttpGet("{calculatoroperations}")]
        public ActionResult<CurrentOperationValue> Get([FromState(StoreName)]StateEntry<CurrentOperationValue> operation)
        {
            if (operation.Value is null)
            {
                return this.NotFound();
            }

            return operation.Value;
        }

        [HttpPost("{add}")]
        public async Task<IActionResult> AddOperation(
            GenericOperation genericOperation,
            [FromServices] DaprClient daprClient)
        {
            Console.WriteLine("Add Request");
            decimal resultOperation = genericOperation.FirstOperand + genericOperation.SecondOperand;

            // Add event to eventSource
            var currentOperation = new Operation() { 
                OperationData = new GenericOperation() { 
                    FirstOperand = genericOperation.FirstOperand,
                    SecondOperand = genericOperation.SecondOperand,
                    Id = genericOperation.Id
                }
            };
            await daprClient.PublishEventAsync("Calculator", "AddOperation", currentOperation);

            // Set current state
            var state = await daprClient.GetStateEntryAsync<CurrentOperationValue>(StoreName, genericOperation.Id);
            state.Value = new CurrentOperationValue(resultOperation, genericOperation.Id);
            await state.SaveAsync();

            // Return Ok to client
            return Ok();
        }

        ///// <summary>
        ///// Method for withdrawing from account as specified in transaction.
        ///// </summary>
        ///// <param name="transaction">Transaction info.</param>
        ///// <param name="daprClient">State client to interact with Dapr runtime.</param>
        ///// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /////  "pubsub", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        //[Topic("pubsub", "withdraw")]
        //[HttpPost("withdraw")]
        //public async Task<ActionResult<Account>> Withdraw(Transaction transaction, [FromServices] DaprClient daprClient)
        //{
        //    Console.WriteLine("Enter withdraw");
        //    var state = await daprClient.GetStateEntryAsync<Account>(StoreName, transaction.Id);

        //    if (state.Value == null)
        //    {
        //        return this.NotFound();
        //    }

        //    state.Value.Balance -= transaction.Amount;
        //    await state.SaveAsync();
        //    return state.Value;
        //}

        /// <summary>
        /// Method for returning a BadRequest result which will cause Dapr sidecar to throw an RpcException
        [HttpPost("throwException")]
        public async Task<IActionResult> ThrowException(GenericOperation genericOperation, [FromServices] DaprClient daprClient)
        {
            Console.WriteLine("Enter ThrowException");
            var task = Task.Delay(10);
            await task;
            return BadRequest(new { statusCode = 400, message = "bad request" });
        }
    }
}
