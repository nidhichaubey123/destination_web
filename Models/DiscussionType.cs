using System.ComponentModel.DataAnnotations;

namespace DMCPortal.API.Entities
{
    public class DiscussionType
    {
        public int DiscussionTypeId { get; set; }

        [Required]
        public string DiscussionTypeName { get; set; }

        [MaxLength(500)]
        public string DiscussionTypeDesc { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }

    }

}
