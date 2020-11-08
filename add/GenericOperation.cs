namespace AddSample
{
    using System.ComponentModel.DataAnnotations;
    public class GenericOperation
    {
        [Required]
        public string Id { get; set; }
        public decimal FirstOperand { get; set; }
        public decimal SecondOperand { get; set; }
    }
}