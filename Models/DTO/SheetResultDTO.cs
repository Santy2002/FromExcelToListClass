using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromExcelToListClass.Models.DTO
{
    public  class SheetResultDTO : ResultDTO
    {
        public ISheet Sheet { get; set; }
    }
}
