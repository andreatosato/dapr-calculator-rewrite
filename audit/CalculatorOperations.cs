using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace audit
{
    public class OperationHistory
	{
        public static ICollection<Operation> None = System.Array.Empty<Operation>();
		public OperationHistory(ICollection<Operation> operations)
		{
            Operations = operations ?? new Collection<Operation>();
		}
        public void AddOperation(Operation o)
		{
            Operations.Add(o);
		}
        public ICollection<Operation> Operations { get; }
	}

    public class Operation
    {
        public GenericOperation OperationData { get; set; }
        public OperandType OperationType { get; set; }
        public enum OperandType
        {
            Sum,
            Subtracti,
            Divide,
            Multiply
        }
    }

    public class GenericOperation
    {
        [Required]
        public string Id { get; set; }
        public decimal FirstOperand { get; set; }
        public decimal SecondOperand { get; set; }
    }
}
