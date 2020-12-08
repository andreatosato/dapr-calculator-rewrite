using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Calculator.Frontend.Services
{
	public interface ICalculator
	{
		ParseResult ParseCalculator(string previous, string value);
		Task<decimal> RunCalculatorDapr(OperationData operationData, Guid operationKey);
		Task<decimal> GetCalculatorStatusDapr(Guid operationKey);
		Task SaveCalculatorStatusDapr(Guid operationKey, decimal currentOperation);
		Task<ICollection<Operation>> GetHistoryCalculatorStatusDapr(Guid operationKey);
	}

	public class CalculatorService : ICalculator
	{
		private readonly char[] Operators = new char[] { '+', '-', 'x', '/' };
		private ILogger logger;
		public CalculatorService(HttpClient http, ILoggerFactory factory)
		{
			Http = http;
			logger = factory.CreateLogger(nameof(CalculatorService));
		}

		public HttpClient Http { get; }

		public ParseResult ParseCalculator(string previous, string value)
		{
			string newValue = value == "=" ? previous : previous + value;
			if (!newValue.Contains(Operators))
				return ParseResult.None;
				//throw new InvalidOperationException("Nessun operatore selezionato");
			var operands = newValue.Split(Operators).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			if (operands.Length > 2)
				return ParseResult.None;
				//throw new ArgumentOutOfRangeException("Più operatori presenti");
			if (operands.Length == 2)
				return new ParseResult(newValue)
				{
					CanCalculate = true,
					Operation = new OperationData(
						operands.ElementAt(0),
						operands.ElementAt(1),
						newValue.ElementAt(operands.ElementAt(0).Length))
				};
			else
				return new ParseResult(newValue)
				{
					CanCalculate = false,
					Operation = OperationData.None
				};
		}

		public async Task<decimal> RunCalculatorDapr(OperationData operationData, Guid operationKey)
		{
			HttpResponseMessage responseMessage = null;
			switch (operationData.Operand)
			{
				case '+':
					var a = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					responseMessage = await Http.PostAsJsonAsync("/v1.0/invoke/add-app/method/add", a);
					if (!responseMessage.IsSuccessStatusCode)
						throw new Exception(await responseMessage.Content.ReadAsStringAsync());
					break;
				case '-':
					var s = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					responseMessage = await Http.PostAsJsonAsync("/v1.0/invoke/sub-app/method/sub", s);
					if (!responseMessage.IsSuccessStatusCode)
                    {
						string error = await responseMessage.Content.ReadAsStringAsync();
						Console.WriteLine(error);
						logger.LogError(error);
						throw new Exception(responseMessage.StatusCode + error);
					}
					break;
				case '/':
					var d = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					responseMessage = await Http.PostAsJsonAsync("/v1.0/invoke/div-app/method/div", d);
					if (!responseMessage.IsSuccessStatusCode)
						throw new Exception(await responseMessage.Content.ReadAsStringAsync());
					break;
				case 'x':
				case '*':
					var m = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					responseMessage = await Http.PostAsJsonAsync("/v1.0/invoke/mul-app/method/mul", m);
					if (!responseMessage.IsSuccessStatusCode)
						throw new Exception(await responseMessage.Content.ReadAsStringAsync());
					break;
				default:
					break;
			}
			string result = await responseMessage.Content.ReadAsStringAsync();
			return decimal.Parse(result);
		}

		public async Task<decimal> GetCalculatorStatusDapr(Guid operationKey)
		{
            try
            {
				var response = await Http.GetAsync($"/v1.0/state/operations-store/{operationKey}");
				if (!response.IsSuccessStatusCode)
					return 0m;
				return decimal.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
			}
            catch (Exception ex)
            {
				// log
				return 0m;
            }
			
		}

		public async Task SaveCalculatorStatusDapr(Guid operationKey, decimal currentOperation)
		{
			var message = await Http.PostAsJsonAsync($"/v1.0/state/operations-store/{operationKey}", currentOperation);
			if(!message.IsSuccessStatusCode)
            {
				// Log
            }
		}

		public async Task<ICollection<Operation>> GetHistoryCalculatorStatusDapr(Guid operationKey)
		{
			var operationHistory = await Http.GetFromJsonAsync<OperationHistory>($"/v1.0/invoke/audit-app/method/audit/{operationKey}");
			return operationHistory.Operations;
		}
	}
}
