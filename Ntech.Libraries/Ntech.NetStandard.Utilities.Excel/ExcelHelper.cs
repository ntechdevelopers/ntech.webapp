using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ntech.NetStandard.Utilities.Excel
{
    public class ExcelHelper
    {
        public static IExcelData LoadDataset(string filePath, int columnHeaderRow = 0, int numberOfRowsToRead = 0, string worksheet = "", bool loadAllSheets = false)
        {
            System.Console.WriteLine($"Loading the excel data into memory: {filePath}");
            return new ExcelOperationAspose(filePath, columnHeaderRow, numberOfRowsToRead, worksheet, loadAllSheets);
        }

        public static IExcelData GetExcel(string templatePath, string currentExcelPath, int columnHeaderRow = 0)
        {
            if (File.Exists(currentExcelPath))
            {
                File.Delete(currentExcelPath);
            }
            File.Copy(templatePath, currentExcelPath);
            return LoadDataset(currentExcelPath, columnHeaderRow: columnHeaderRow);
        }

        public static void UpdateExcel(IExcelData excel, string sheetName, IList<Tuple<int, int, string>> data)
        {
            if (!data.Any(true))
            {
                return;
            }
            foreach (var cell in data)
            {
                excel.UpdateCell(sheetName, cell.Item1, cell.Item2, cell.Item3);
            }
        }

    }
}

