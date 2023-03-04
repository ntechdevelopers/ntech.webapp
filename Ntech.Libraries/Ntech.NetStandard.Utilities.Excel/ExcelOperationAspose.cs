using System;
using System.Collections.Generic;
using Aspose.Cells;
using System.IO;
using System.Linq;

namespace Ntech.NetStandard.Utilities.Excel
{
    public class ExcelOperationAspose : IExcelData
    {
        readonly Workbook _xBook;
        List<string> _worksheetNames;
        ExcelSheets _sheets;

        public ExcelOperationAspose(string excelFile, int columnHeaderRow = 0, int readNumberOfRows = 0, string worksheet = "", bool loadAllSheets = false)
        {
            if (!File.Exists(excelFile))
                throw new FileNotFoundException(string.Format("The file request for excel read is not found in the file system. File path {0}", excelFile));

            _xBook = new Workbook(excelFile);
            _sheets = new ExcelSheets();

            if (loadAllSheets)
            {
                foreach (Worksheet sheet in _xBook.Worksheets)
                {
                    _sheets.Set(sheet.Name, new ExcelOperationSheetAspose(sheet, columnHeaderRow, readNumberOfRows));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(worksheet))
                {
                    if (!WorksheetNames.Any())
                        throw new Exception("There is no worksheet to load");

                    _sheets.Set(_xBook.Worksheets[0].Name, new ExcelOperationSheetAspose(_xBook.Worksheets[0], columnHeaderRow, readNumberOfRows));
                }
                else
                {
                    if (!WorksheetNames.Contains(worksheet))
                        throw new Exception("The worksheet specified does not exist in the excel file");

                    _sheets.Set(worksheet, new ExcelOperationSheetAspose(_xBook.Worksheets[worksheet], columnHeaderRow, readNumberOfRows));
                }
            }
       
        }

        public IList<string> WorksheetNames
        {
            get
            {
                if (_xBook == null)
                    throw new Exception("Excel data not loaded");

                if (_worksheetNames == null)
                {
                    _worksheetNames = new List<string>();
                    foreach (Worksheet sheet in _xBook.Worksheets)
                    {
                        _worksheetNames.Add(sheet.Name);
                    }
                }

                return _worksheetNames;
            }
        }

        public ExcelSheets Sheets
        {
            get { return _sheets; }
        }

        public IExcelSheet FirstSheet
        {
            get { return _sheets[_xBook.Worksheets[0].Name]; }
        }

        public void UpdateCell(string worksheet, int column, int row, string value)
        {
            _xBook.Worksheets[worksheet].Cells[row, column].Value = value;
        }

        public object ReadCellValue(string worksheet, int column, int row)
        {
            return _xBook.Worksheets[worksheet].Cells[row, column].Value;
        }

        public void SaveWorkBook(string path)
        {
            _xBook.Save(path);
        }
    }
}
