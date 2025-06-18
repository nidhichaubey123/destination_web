namespace DMCPortal.Web.Models
{
    public class Operation
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string? OperationDescription { get; set; }
        public DateTime OperationCreatedOn { get; set; }

        public bool OperationIsActive { get; set; }
    }
}
