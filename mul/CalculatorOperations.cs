using System.ComponentModel.DataAnnotations;

namespace mul
{
    public class Operation
    {
        public GenericOperation OperationData {get; set; }
        public OperandType OperationType {get; set;}
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