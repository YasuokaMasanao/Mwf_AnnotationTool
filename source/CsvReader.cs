using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace annotationTool;
internal class CsvReader {
    public static string? filePath { get; private set; } = null;
    public bool isExist { get; private set; } = false;
    public DataTable? dataTable = null;

    public CsvReader(string path) {
        ReadCsvFile(path);
    }
    public void ReadCsvFile(string path) {
        DataTable dt = new DataTable();
        filePath = path;
        if (File.Exists(path)) {
            isExist = true;
            using (StreamReader sr = new StreamReader(path)) {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers) {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream) {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++) {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            dataTable = dt;
        } else {
            isExist = false;
        }
    }

    public void SaveDataTableToCsv(DataTable dt) {
        StringBuilder sb = new StringBuilder();

        IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
        sb.AppendLine(string.Join(",", columnNames));

        foreach (DataRow row in dt.Rows) {
            IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
            sb.AppendLine(string.Join(",", fields));
        }

        File.WriteAllText(filePath, sb.ToString());
    }
}

