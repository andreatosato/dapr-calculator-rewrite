using System.Collections.Generic;

namespace AddSample
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
}