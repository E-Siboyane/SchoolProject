using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DTO {
    public interface IDtoCommonMethod {
        dynamic ConvertToModel(List<string> fileListContents, int jobId, string fileContentDelimeter);
        string ConvertListToString<T>(List<T> itemsToConvertToString) where T : class;
    }
}
