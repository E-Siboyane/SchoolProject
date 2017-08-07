using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolProject.WebApplication.Models.Abstract;
using SchoolProject.WebApplication.Models.Interface;

namespace SchoolProject.WebApplication.Models {
    public class AppConfiguration : Audit, IStatus {
        public int AppConfigurationId { get; set; }
        [Required, StringLength(150)]
        public string ConfigSetting { get; set; }
        [Required, StringLength(256)]
        public string ConfigValue { get; set; }
        [ForeignKey("Status"), Required]
        public int StatusId { get; set; }
        public virtual AdminStatus Status { get; set; }
    }
}
