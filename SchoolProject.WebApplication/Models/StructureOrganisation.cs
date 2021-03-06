﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class StructureOrganisation : Audit, IStatus {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StructureOrganisationId { get; set; }
        [Required, StringLength(150)]
        public string OrganisationName { get; set; }
        [Required, StringLength(150)]
        public string OrganisationCode { get; set; }
        [Required, ForeignKey("Status")]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
