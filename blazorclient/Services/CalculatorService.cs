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
			string newValue = previous + value;
			if (!newValue.Contains(Operators))
				throw new InvalidOperationException("Nessun operatore selezionato");
			var operands = newValue.Split(Operators).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			if (operands.Length > 2)
				throw new ArgumentOutOfRangeException("Più operatori presenti");
			if (operands.Length == 2)
				return new ParseResult(newValue)
				{
					CanCalculate = true,
					Operation = new ParseResult.OperationData(
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
					break;
				case '-':
					var s = new GenericOperation()
					{
						FirstOperand = operationData.FirstOperand,
						SecondOperand = operationData.SecondOperand,
						Id = operationKey.ToString("N")
					};
					var responseSub = await Http.PostAsJsonAsync("/v1.0/invoke/sub-app/method/sub", s);
					break;
				default:
					break;
			}
		}

		public async Task<decimal> GetCalculatorStatusDapr(Guid operationKey)
		{
			var responseAdd = await Http.GetFromJsonAsync<string>("/v1.0/state/");
			return 0m;
		}

	}
}
