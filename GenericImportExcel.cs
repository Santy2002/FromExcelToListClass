using FromExcelToListClass.Models.DTO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace FromExcelToListClass
{
    public class GenericImportExcel
    {
        public static List<T> ParseExcelToClass<T>(FileInfo file) where T : class
        {
            var result = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();

            #region Validaciones

            #region Tipo de Archivo
            var sheetResult = GetWorkbookType(file, file.Extension);

            if (sheetResult.Error)
            {
                Console.Write(string.Format("Error de validacion.\n{0}", sheetResult.ErrorMessage));
                return result;
            }

            ISheet sheet = sheetResult.Sheet;
            #endregion Tipo de Archivo

            #region Headers
            IRow row = sheet.GetRow(0);

            var requiredHeaders = GetRequiredHeaders<T>();
            var checkHeaders = CheckHeadersName(row, requiredHeaders);
            
            if (checkHeaders.Error)
            {
                Console.WriteLine(string.Format("Error de validacion en los encabezados. No se procesara el archivo: {0}\n{1}", file.Name, checkHeaders.ErrorMessage));
                return result;
            }
            #endregion Headers

            #endregion Validaciones

            Console.WriteLine(string.Format("Procesando: {0}", file.FullName));

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);

                if (row == null) continue;

                var cellResult = ParseRowToClass<T>(row, properties);

                if (cellResult.Error)
                {
                    Console.WriteLine(string.Format("Error de formato en: {0}", cellResult.ErrorMessage));
                    continue;
                }

                result.Add(cellResult.Objeto);
            }

            return result;
        }

        private static List<string> GetRequiredHeaders<T>() where T : class
        {
            return typeof(T).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(RequiredAttribute)))
                .Select(prop => prop.Name)
                .ToList();
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

        private static ResultDTO CheckHeadersName(IRow row, List<string> requiredHeaders)
        {
            var result = new ResultDTO() { Error = false, ErrorMessage = string.Empty };

            for (int i = 0; i < requiredHeaders.Count; i++)
            {
                var cellValue = row.GetCell(i)?.StringCellValue ?? string.Empty;
                if (cellValue != requiredHeaders[i])
                {
                    result.Error = true;
                    result.ErrorMessage += string.Format("Nombre inválido en columna {0}: se esperaba '{1}', pero se encontró '{2}'.\n", i + 1, requiredHeaders[i], cellValue);
                }
            }

            return result;
        }

        private static TResultDTO<T> ParseRowToClass<T>(IRow row, PropertyInfo[] properties) where T : class
        {
            var result = new TResultDTO<T>() { Error = false, ErrorMessage = string.Empty };
            var GenericObj = Activator.CreateInstance<T>();

            for (int j = 0; j < properties.Length; j++)
            {
                PropertyInfo propiedad = properties[j];
                ICell celda = row.GetCell(j);

                if (celda != null)
                {
                    var parsedCell = ParseCellToClass(celda, propiedad);

                    if (parsedCell.Error)
                    {
                        result.Error = parsedCell.Error;
                        result.ErrorMessage += parsedCell.ErrorMessage;
                        continue;
                    }

                    propiedad.SetValue(GenericObj, parsedCell.Property);
                }
            }

            result.Objeto = GenericObj;
            return result;
        }

        private static PropertyDTO ParseCellToClass(ICell celda, PropertyInfo property)
        {
            var result = new PropertyDTO() { Error = false, ErrorMessage = string.Empty };
            string msgError = string.Format("No se pudo procesar la celda col: {0} fila: {1}.\n", celda.ColumnIndex + 1, celda.RowIndex + 1);
            object? parsedValue = null;

            if(property.PropertyType == typeof(DateTime))
            {
                if (DateTime.TryParse(celda.ToString(), out DateTime date))
                {
                    parsedValue = date;
                }
            }
            else
            {
                //TODO ver esto, corregir. Tiene que poder setear propiedades vacias. Esto tira error en todas las vacias.
                if (string.IsNullOrEmpty(celda.ToString()) && property.PropertyType != typeof(int))
                {
                    parsedValue = Convert.ChangeType(celda.ToString(), property.PropertyType);
                }
            }

            if (parsedValue == null)
            {
                result.Error = true;
                result.ErrorMessage = msgError;
                return result;
            }

            result.Property = parsedValue;
            return result;
        }

        //La idea es que sea una sobrecarga, let me cook
        public List<T> ParseExcelToClass<T>(List<FileInfo> file) where T : class
        {
            //PropertyInfo[] properties = typeof(T).GetType().GetProperties();

            //foreach (var prop in properties)
            //{

            //}

            throw new NotImplementedException();
        }

    }
}
