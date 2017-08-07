using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    [Table("ReportingStructure")]
    public class PMReviewReportingStructure : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewReportingStructureId { get; set; }
        [ForeignKey("Owner")]
        public int MemberId { get; set; }
        public virtual StructureEmployee Owner { get; set; }
        [ForeignKey("Manager")]
        public int ManagerId { get; set; }
        public virtual StructureEmployee Manager { get; set; }
        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public virtual PMDocumentType DocumentType { get; set; }
        [ForeignKey("Status")]
        public int StatusId{ get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
