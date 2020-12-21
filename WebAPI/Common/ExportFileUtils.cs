using OfficeOpenXml;

namespace WebAPI.Common
{
    public class ExportFileUtils
    {
        public ExcelWorksheet SheetTemPlate(ExcelPackage excelPkg, string sheetName)
        {
            ExcelWorksheet oSheet = excelPkg.Workbook.Worksheets[sheetName];
            return oSheet;
        }
    }
}
