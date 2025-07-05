namespace DMCPortal.Web.Models
{
    public class Operation
    {
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public string? OperationDescription { get; set; }
        public DateTime OperationCreatedOn { get; set; }
        public int? OperationCreatedBy { get; set; }

        public int? OperationUpdatedBy { get; set; }
        public DateTime? OperationUpdatedOn { get; set; }

        public bool OperationIsActive { get; set; }
    }
}
