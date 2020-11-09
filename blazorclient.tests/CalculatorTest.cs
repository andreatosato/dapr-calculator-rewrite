using System;
using Xunit;
using blazorclient.Services;

namespace blazorclient.tests
{
	public class CalculatorTest
	{
		private readonly CalculatorService calculator = new CalculatorService();

		[Fact]
		public void CannotAppendNumber()
		{
			Assert.Throws<InvalidOperationException>(() => calculator.ParseCalculator("0", "1"));
		}

		[Fact]
		public void AppendOperator()
		{
			var result = calculator.ParseCalculator("0", "+");
			Assert.Equal("0+", result.NextValue);
			Assert.False(result.CanCalculate);
			Assert.Equal(OperationData.None, result.Operation);
		}

		[Fact]
		public void AppendOperatorDecimal()
		{
			var result = calculator.ParseCalculator("0.12", "+");
			Assert.Equal("0.12+", result.NextValue);
			Assert.False(result.CanCalculate);
			Assert.Equal(OperationData.None, result.Operation);
		}

		[Fact]
		public void CanCalculate()
		{
			var result = calculator.ParseCalculator("0.3635+", "2.23552215");
			Assert.Equal("0.3635+2.23552215", result.NextValue);
			Assert.True(result.CanCalculate);
			Assert.Equal(0.3635m, result.Operation.FirstOperand);
			Assert.Equal(2.23552215m, result.Operation.SecondOperand);
			Assert.Equal('+', result.Operation.Operand);
		}

		[Fact]
		public void CanCalculateAppend()
		{
			var result = calculator.ParseCalculator("0.3635+2.3", "5");
			Assert.Equal("0.3635+2.35", result.NextValue);
			Assert.True(result.CanCalculate);
			Assert.Equal(0.3635m, result.Operation.FirstOperand);
			Assert.Equal(2.35m, result.Operation.SecondOperand);
			Assert.Equal('+', result.Operation.Operand);
		}
	}
}
