using EconomicCapital.RF.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.BusinessLogic {
   public interface IRetailPoolingManager {


      #region RETAIL_POOLING_LOGIC      
      Guid ReadRetailPoolingFiles(RetailInputFileParam retailInputFileParam);
      bool ProcessRetailPoolingRanks(DbContextType dbContextType);
      bool ProcessSingleProductRetailPoolingRank(RetailPoolingFileType productType, DbContextType dbContextType);
      bool PDRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      bool LGDRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      bool TMRank(RetailPoolingFileType productName, DbContextType dbContextType, Guid rowId);
      #endregion RETAIL_POOLING_LOGIC
   }
}
