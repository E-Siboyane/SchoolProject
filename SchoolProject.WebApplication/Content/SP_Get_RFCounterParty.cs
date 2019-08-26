using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
    public class SP_Get_RFCounterParty {
        public string CounterPartyId { get; set; }
        public string Pid { get; set; }
        public string CounterPartyName { get; set; }
        public string CounterPartyDescription { get; set; }
        public string CounterPartyCountryCode { get; set; }
        public int? UserVariableInt { get; set; }
        public string UserVariableString1 { get; set; }
        public string UserVariableString2 { get; set; }
        public string UserVariableString3 { get; set; }
        public string CounterpartyType { get; set; }
        public bool IsPublicFlag { get; set; }
    }
}
