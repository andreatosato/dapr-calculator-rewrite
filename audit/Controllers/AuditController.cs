﻿using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace audit.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditController : ControllerBase
    {
        public AuditController(ILogger<AuditController> logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "operations-history-store";
        private readonly ILogger<AuditController> logger;

        [HttpGet("{operationId}")]
        public async Task<IActionResult> Audit(Guid operationId,
            [FromServices] DaprClient daprClient)
        {
            var state = await daprClient.GetStateEntryAsync<OperationHistory>(StoreName, operationId.ToString("N"));
            logger.LogInformation($"Read operations: {JsonSerializer.Serialize(state)}");
            if (state.Value == null)
                return Ok(OperationHistory.None);

            return Ok(state.Value);
        }

        [Topic("calculator", "CalculatorOperation")]
        public async Task PushElement(Operation operation, [FromServices] DaprClient daprClient)
        {
            logger.LogInformation($"Arrive event: {JsonSerializer.Serialize(operation)}");
            var state = await daprClient.GetStateEntryAsync<OperationHistory>(StoreName, operation.OperationData.Id);
            logger.LogInformation($"Current state: {JsonSerializer.Serialize(state)}");
            if (state.Value == null || state.Value.Operations.Count == 0)
            {
                var newHistory = new OperationHistory(new Collection<Operation>
                {
                    operation
                });
                await daprClient.SaveStateAsync(StoreName, operation.OperationData.Id, newHistory);
                logger.LogInformation($"Save new event");
            }
            else
            {
                state.Value.AddOperation(operation);
                await state.SaveAsync();
                logger.LogInformation($"Update event");
            }                          
        }
    }
}
