using CsvHelper;
using NotVisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ntech.NetStandard.Utilities.Excel
{
    public class CsvHelper
    {
        public static void WriteToCsv<T>(IEnumerable<T> records, string filepath, string delimiter = ",", bool hasHeaderRecords = false)
        {
            PrepareFolder(filepath);

            using (var csvWriter = new CsvWriter(new StreamWriter(filepath)))
            {
                csvWriter.Configuration.Delimiter = delimiter;
                csvWriter.Configuration.HasHeaderRecord = hasHeaderRecords;
                csvWriter.WriteRecords(records);
            }
        }

        public static IList<string[]> ReadCsv(string filepath, string delimiter = ",", bool hasReadHeader = true)
        {
            IList<string[]> result = new List<string[]>();
            if (!File.Exists(filepath))
            {
                Console.WriteLine($"Not found file path.");
                throw new Exception("Not found file path.");
            }
            //using (TextFieldParser csvParser = new TextFieldParser(filepath))
            using (var csvReader = new StringReader(filepath))
            using (var csvParser = new CsvTextFieldParser(csvReader))
            {
                csvParser.Delimiters = new string[] { delimiter };
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    result.Add(fields);
                }
            }

            if (hasReadHeader)
            {
                // Skip the row with the column header
                result.RemoveAt(0);
            }
            return result;
        }

        public void WriteInChucks<T>(List<List<T>> records, string rootFolderPath, string filePath, int portNo, int[] portNos)
        {
            ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 20 };
            Parallel.For(0, records.Count(), parallelOptions, i =>
            {
                var dataInGroups = records[i];
                var offset = (portNos.Any() ? portNos[i] : portNo + i);
                var dataFilepath = string.Format(filePath, offset);

                Console.WriteLine($"Writing file {dataFilepath}");
                WriteToCsv(records: dataInGroups, filepath: Path.Combine(rootFolderPath.EmptyIfNull(), dataFilepath), hasHeaderRecords: true);
            });
        }

        public static void PrepareFolder(string filepath)
        {
            var directory = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

        }
    }
}
