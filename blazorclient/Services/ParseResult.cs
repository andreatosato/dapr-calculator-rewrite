namespace blazorclient.Services
{
	public class ParseResult
	{
		public static ParseResult None = new ParseResult();
		protected ParseResult() { CanCalculate = false; }
		public ParseResult(string nextValue)
		{
			NextValue = nextValue;
		}
		public string NextValue { get; }
		public bool CanCalculate { get; set; }
		public OperationData Operation { get; set; }
	}

	public class OperationData
	{
		public static OperationData None = new OperationData();
		protected OperationData() { }
		public OperationData(string firsOperand, string secondOperand, char operand)
		{
			FirstOperand = decimal.Parse(firsOperand);
			SecondOperand = decimal.Parse(secondOperand);
			Operand = operand;
		}
		public decimal FirstOperand { get; }
		public char Operand { get; }
		public decimal SecondOperand { get; }
	}
}
