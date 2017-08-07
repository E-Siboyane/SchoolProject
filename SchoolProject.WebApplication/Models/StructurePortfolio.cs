using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
   [Table("Portfolio")]
    public class StructurePortfolio : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PortfolioId { get; set; }
        [Required, StringLength(50)]
        public string PortfolioCode { get; set; }
        [Required, StringLength(256)]
        public string PortfolioName { get; set; }
        [Required, ForeignKey("Organisation")]
        public int StructureOrganisationId { get; set; }
        public virtual StructureOrganisation Organisation { get; set; }
        [Required, ForeignKey("Status")]       
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}