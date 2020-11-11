using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace blazorclient.Services
{
	public interface ICalculator
	{
		ParseResult ParseCalculator(string previous, string value);
		Task RunCalculatorDapr(OperationData operationData, Guid operationKey);
		Task<decimal> GetCalculatorStatusDapr(Guid operationKey);
	}

	public class CalculatorService : ICalculator
	{
		private readonly char[] Operators = new char[] { '+', '-', 'x', '/' };
		public CalculatorService(HttpClient http)
		{
			Http = http;
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

		public async Task RunCalculatorDapr(OperationData operationData, Guid operationKey)
		{
			switch (operationData.Operand)
			{
				case '+':
					var a = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					var responseAdd = await Http.PostAsJsonAsync("/v1.0/invoke/add-app/method/add", a);
					if (!responseAdd.IsSuccessStatusCode)
						throw new Exception(await responseAdd.Content.ReadAsStringAsync());
					break;
				case '-':
					var s = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					var responseSub = await Http.PostAsJsonAsync("/v1.0/invoke/sub-app/method/sub", s);
					if (!responseSub.IsSuccessStatusCode)
						throw new Exception(await responseSub.Content.ReadAsStringAsync());
					break;
				default:
					break;
			}
		}

		public async Task<decimal> GetCalculatorStatusDapr(Guid operationKey)
		{
			var currentTotal = await Http.GetFromJsonAsync<decimal>($"/v1.0/state/operations-store/{operationKey}");
			return currentTotal;
		}

		public async Task<decimal> GetHistoryCalculatorStatusDapr(Guid operationKey)
		{
			var operationHistory = await Http.GetFromJsonAsync<OperationHistory>($"/v1.0/state/operations-history-store/{operationKey}");
			return 0m;
		}
	}
}
