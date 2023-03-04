using System.Collections.Generic;

namespace Ntech.NetStandard.Utilities.Excel
{
    public interface IExcelData
    {
        ExcelSheets Sheets { get; }

        IExcelSheet FirstSheet { get; }

        IList<string> WorksheetNames { get; }

        void UpdateCell(string worksheet, int column, int row, string value);

        object ReadCellValue(string worksheet, int column, int row);

        void SaveWorkBook(string path);
    }
}
