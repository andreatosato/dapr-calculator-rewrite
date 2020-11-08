namespace AddSample
{
    public class CurrentOperationValue
    {
        public CurrentOperationValue(decimal value, string operationId)
        {
            Value = value;
            OperationId = operationId;
        }
        public decimal Value { get; }
        public string OperationId { get; }
    }
}