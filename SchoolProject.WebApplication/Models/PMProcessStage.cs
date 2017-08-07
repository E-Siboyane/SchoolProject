using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;

namespace SchoolProject.WebApplication.Models {
    public class PMProcessStage : Audit {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProcessStageId { get; set; }
        [Required, StringLength(100)]
        public string ProcessStageName { get; set; }
        [Required, StringLength(50)]
        public string ProcessStageCode { get; set; }
        [Required]
        public int ProcessingOrderNumber { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }        
    }
}
