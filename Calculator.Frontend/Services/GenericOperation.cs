using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Frontend.Services
{
    public class OperationHistory
    {
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
