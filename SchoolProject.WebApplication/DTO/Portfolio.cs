using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.WebApplication.DTO {
    public class Portfolio {
        public int PortfolioId { get; set; }
        public string PortfolioCode { get; set; }
        public string PortfolioName { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public int OrganisationStructureId { get; set; }
        public string OrganisationStructureName { get; set; }
        public string OrganisationStructureCode { get; set; }
    }
}
