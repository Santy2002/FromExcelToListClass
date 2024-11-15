using FromExcelToListClass.Models.DTO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;


namespace FromExcelToListClass
{
    public class FromExcelToListClass<T> where T : class
    {
        public static readonly PropertyInfo[] Properties = typeof(T).GetProperties();

        #region Queue Dict
        public static readonly Queue<Dictionary<string, string>> QueueDictionary = new Queue<Dictionary<string, string>>();
        private static Dictionary<string, string> GetDictionaryFromPool()
        {
            return QueueDictionary.Count > 0 ? QueueDictionary.Dequeue() : new Dictionary<string, string>();
        }
        private static void ReturnDictionaryToPool(Dictionary<string, string> dict)
        {
            dict.Clear();
            QueueDictionary.Enqueue(dict);
        }
        #endregion Queue Dict
         
        public List<T> ParseExcelToClass(FileInfo file)
        {
            var result = new List<T>();
            
            #region Validaciones

            #region Tipo de Archivo

            var sheetResult = GetWorkbookType(file, file.Extension);

            if (sheetResult.Error)
            {
                Console.Write(string.Format("Error de validacion.\n{0}", sheetResult.ErrorMessage));
                return result;
            }

            #endregion Tipo de Archivo

            #region Headers

            ISheet sheet = sheetResult.Sheet;
            IRow headerRow = sheet.GetRow(0);

            var requiredHeaders = GetRequiredHeaders();
            var checkHeaders = CheckHeadersName(headerRow, requiredHeaders);
            
            if (checkHeaders.Error)
            {
                Console.WriteLine(string.Format("Error de validacion en los encabezados. No se procesara el archivo: {0}\n{1}", file.Name, checkHeaders.ErrorMessage));
                return result;
            }

            #endregion Headers

            #endregion Validaciones

            //Console.WriteLine(string.Format("Procesando: {0}", file.Name));

            var msgErrorTotal = new StringBuilder().Append("Error de formato en:\n");

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null) continue;

                var cellResult = ParseRowToClass(row, headerRow, requiredHeaders.Length, headerRow.LastCellNum);

                if (cellResult.Error)
                {
                    msgErrorTotal.Append(cellResult.ErrorMessage);
                    continue;
                }

                result.Add(cellResult.Objeto);
            }

            //Console.WriteLine(msgErrorTotal);
            return result;
        }

        private static string[] GetRequiredHeaders()
        {
            return Properties
                .Where(prop => Attribute.IsDefined(prop, typeof(RequiredAttribute)))
                .Select(prop => prop.Name)
                .ToArray();
        }

        private static SheetResultDTO GetWorkbookType(FileInfo file, string extension)
        {
            var result = new SheetResultDTO() { Error = false, ErrorMessage = string.Empty };

            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                if (extension == ".xls")
                {
                    result.Sheet = new HSSFWorkbook(stream).GetSheetAt(0);
                    return result;
                }
                else if (extension == ".xlsx")
                {
                    result.Sheet = new XSSFWorkbook(stream).GetSheetAt(0);
                    return result;
                }
                
                result.Error = true;
                result.ErrorMessage = string.Format("Se esperaba archivo con formato .xls o .xlsx en: {0}", file.Name);

                return result;
            }
        }

        private static ResultDTO CheckHeadersName(IRow row, string[] requiredHeaders)
        {
            var result = new ResultDTO() { Error = false, ErrorMessage = string.Empty };
            var msgError = new StringBuilder();

            for (int i = 0; i < requiredHeaders.Length; i++)
            {
                var cellValue = row.GetCell(i)?.StringCellValue ?? string.Empty;
                if (cellValue != requiredHeaders[i])
                {
                    result.Error = true;
                    msgError.AppendFormat("Nombre inválido en columna {0}: se esperaba '{1}', pero se encontró '{2}'.\n", i + 1, requiredHeaders[i], cellValue);
                }
            }
            result.ErrorMessage = msgError.ToString();
            return result;
        }

        private static TResultDTO<T> ParseRowToClass(IRow row, IRow headerRow, int cantidadColumnasObligatorias, int cantidadColumnasRecibidas)
        {
            var result = new TResultDTO<T>() { Error = false, ErrorMessage = string.Empty };
            var GenericObj = Activator.CreateInstance<T>();
            var msgError = new StringBuilder();

            #region Columnas Obligatorias

            for (int j = 0; j < cantidadColumnasObligatorias; j++)
            {
                PropertyInfo propiedad = Properties[j];
                ICell celda = row.GetCell(j);

                var parsedCell = ParseCellToType(celda, propiedad);

                if (parsedCell.Error)
                {
                    result.Error = parsedCell.Error;
                    msgError.AppendFormat(parsedCell.ErrorMessage, celda?.ColumnIndex + 1 ?? j + 1, celda?.RowIndex + 1 ?? row.RowNum + 1);
                    continue;
                }

                propiedad.SetValue(GenericObj, parsedCell.Property);             
            }
            result.ErrorMessage = msgError.ToString();

            #endregion Columnas Obligatoias

            #region Columnas Opcionales
            if (cantidadColumnasObligatorias < cantidadColumnasRecibidas)
            {
                var dict = GetDictionaryFromPool();
                var propiedadDict = Properties.First(prop => 
                    prop.PropertyType == typeof(Dictionary<string, string>) ||
                    prop.PropertyType == typeof(IDictionary<string, string>));

                for (int i = cantidadColumnasObligatorias; i < cantidadColumnasRecibidas; i++)
                {
                    ICell header = headerRow.GetCell(i);
                    ICell celda = row.GetCell(i);

                    dict.Add(header.StringCellValue, celda?.ToString() ?? string.Empty);
                }

                propiedadDict.SetValue(GenericObj, dict);

                ReturnDictionaryToPool(dict);
            }
            #endregion Columnas Opcionales

            result.Objeto = GenericObj;
            return result;
        }

        private static PropertyDTO ParseCellToType(ICell celda, PropertyInfo property)
        {
            var result = new PropertyDTO() { Error = false, ErrorMessage = string.Empty };
            object? parsedValue = null;
            string? cellStringValue = celda?.ToString();

            switch (Type.GetTypeCode(property.PropertyType))
            {
                case TypeCode.DateTime:
                    if (!string.IsNullOrEmpty(cellStringValue) && DateTime.TryParse(cellStringValue, out DateTime date))
                    {
                        parsedValue = date;
                    }
                    break;

                case TypeCode.Int32:
                    if (int.TryParse(cellStringValue, out int num))
                    {
                        parsedValue = num;
                    }
                    break;

                default:
                    parsedValue = Convert.ChangeType(cellStringValue, property.PropertyType);
                    break;
            }

            if (parsedValue == null || (cellStringValue == null && cellStringValue == string.Empty))
            {
                result.Error = true;
                result.ErrorMessage = "No se pudo procesar la celda OBLIGATORIA COL: {0} FILA: {1}\n";
                return result;
            }

            result.Property = parsedValue;
            return result;
        }

    }
}
