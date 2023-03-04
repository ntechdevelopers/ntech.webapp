using System;
using System.Collections.Generic;
using Aspose.Cells;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Ntech.NetStandard.Utilities.Excel
{
    public class ExcelOperationSheetAspose : ExcelData, IExcelSheet
    {
        readonly int _columnHeaderRow;
        int _readNrecords;
        IDictionary<int, string> _columnNames;
        IDictionary<int, string> _columnMapping;
        readonly Worksheet _xSheet;

        public ExcelOperationSheetAspose(Worksheet xSheet, int columnHeaderRow = 0, int readNumberOfRows = 0)
        {
            _xSheet = xSheet;
            _columnHeaderRow = columnHeaderRow != 0 ? columnHeaderRow - 1 : columnHeaderRow;
            _readNrecords = readNumberOfRows;

            GenerateData();
        }

        public int TotalRows
        {
            get { return _xSheet.Cells.MaxDataRow; }
        }

        public int TotalColumns
        {
            get { return _xSheet.Cells.MaxDataColumn; }
        }
        private void GenerateData()
        {
            InitializeColumnNames();

            var rangeCells = _xSheet.Cells;

            _readNrecords = _readNrecords == 0 ? TotalRows : _readNrecords + _columnHeaderRow;

            Parallel.For((_columnHeaderRow + 1), (_readNrecords + 1), i =>
            {
                var obj = new ExpandoObject() as IDictionary<string, Object>;
                for (var j = 0; j <= TotalColumns; j++)
                {
                    if (obj.ContainsKey(_columnNames[j]))
                    {
                        obj[_columnNames[j]] = rangeCells[i, j].StringValue;
                    }
                    else
                    {
                        obj.Add(_columnNames[j], rangeCells[i, j].StringValue);
                    }
                }
                _data.Add(obj);
            });
        }

        private void InitializeColumnNames()
        {
            var columnNameRow = _xSheet.Cells.Rows[_columnHeaderRow];

            _columnNames = new Dictionary<int, string>();
            _columnMapping = new Dictionary<int, string>();

            for (int j = 0; j <= TotalColumns; j++)
            {
                _columnMapping.Add(j, columnNameRow[j].StringValue);
                _columnNames.Add(j, columnNameRow[j].StringValue.ConvertToDbFormatColumnName());
            }
        }

        public IEnumerable<dynamic> Rows
        {
            get { return _data; }
        }

        public string GetValue(string columnName, dynamic row)
        {
            return (row as IDictionary<string, Object>)
                .Where(d => d.Key == GetColumnDynamicPropertyName(columnName)).Select(d => d.Value as string).FirstOrDefault();
        }

        public IList<string> ColumnsAsPropertyName
        {
            get { return _columnNames.Select(c => c.Value).ToList(); }
        }

        public IList<string> ColumnNames
        {
            get { return _columnMapping.Select(c => c.Value).ToList(); }
        }

        private int FindColumnNumber(string columnName)
        {
            if (_columnMapping.Any(c => c.Value == columnName))
                return _columnMapping.Where(c => c.Value == columnName).Select(c => c.Key).FirstOrDefault();
            else if (_columnNames.Any(c => c.Value == columnName))
                return _columnNames.Where(c => c.Value == columnName).Select(c => c.Key).FirstOrDefault();
            else
                throw new Exception("The specified column does not exist in the excel");
        }

        private string GetColumnDynamicPropertyName(string columnName)
        {
            return _columnNames.Where(c => c.Key == FindColumnNumber(columnName)).Select(d => d.Value).FirstOrDefault();
        }
        public string GetCellValue(int column, int row)
        {
            return (Rows.Skip(row).Take(1).FirstOrDefault() as IDictionary<string, Object>)
                .Where(d => d.Key == _columnNames.Where(c => c.Key == column).Select(c => c.Value).FirstOrDefault())
                .Select(d => d.Value as string).FirstOrDefault();
        }
    }
}
