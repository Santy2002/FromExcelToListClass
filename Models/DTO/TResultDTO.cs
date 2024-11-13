using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromExcelToListClass.Models.DTO
{
    public class TResultDTO<T> : ResultDTO where T : class
    {
        private T _valor;

        public TResultDTO()
        {
        }

        public TResultDTO(T valorInicial)
        {
            _valor = valorInicial;
        }
        public T Objeto
        {
            get { return _valor; }
            set { _valor = value; }
        }
    }
}
