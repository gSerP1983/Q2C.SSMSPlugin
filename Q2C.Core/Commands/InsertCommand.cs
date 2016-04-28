using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Q2C.Core.Commands
{
    public class InsertCommand : CommandBase, ICommand
    {
        public string Id
        {
            get { return "QueryToInsert"; }
        }

        public string Name
        {
            get { return "Query To Insert..."; }
        }

        public string Description
        {
            get { return "Query To Insert..."; }
        }

        protected override string GetDataTableScript(DataTable dt)
        {
            var columns = dt.Columns.Cast<DataColumn>().Where(x => !x.ReadOnly).ToArray();
            columns = dt.PrimaryKey.Union(columns).ToArray();
            var roColumns = dt.Columns.Cast<DataColumn>().Where(x => x.ReadOnly && !dt.PrimaryKey.Contains(x)).ToArray();
            if (!columns.Any() && !roColumns.Any())
                return string.Empty;

            var result = new StringBuilder();
            if (dt.PrimaryKey.Any(x => x.AutoIncrement))
            {
                result.AppendLine("SET IDENTITY_INSERT !!!TABLE_NAME ON");
                result.AppendLine("GO ");
                result.AppendLine();
            }

            var count = 0;
            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                // header
                result.AppendLine("-- " + ++count);

                if (dt.PrimaryKey.Length > 0)
                {
                    var whereClause = string.Empty;
                    foreach (var col in dt.PrimaryKey)
                    {
                        var val = col.ColumnName + " = " + DbHelper.GetScriptValue(dr[col], col.DataType);
                        if (!Equals(col, dt.PrimaryKey.LastOrDefault()))
                            val += " AND ";

                        whereClause += val;
                    }

                    result.AppendFormat("--IF NOT EXISTS (SELECT 1 FROM !!!TABLE_NAME WHERE {0})", whereClause);
                    result.AppendLine();
                }

                result.AppendLine("INSERT INTO !!!TABLE_NAME (");
                if (columns.Any()) result.AppendLine(ColumnsToSplitedString(columns.Select(x => x.ColumnName).ToArray()));
                if (roColumns.Any()) result.AppendLine("--ReadOnly " + (columns.Any() ? "," : "") + string.Join(", ", roColumns.Select(x => x.ColumnName)));
                result.AppendLine(")");
                result.AppendLine("VALUES (");

                // set 
                GetSetter(dr, columns, result);
                GetSetter(dr, roColumns, result, columns.Any());

                result.AppendLine(")");
                result.AppendLine("GO ");
                result.AppendLine();
            }

            if (dt.PrimaryKey.Any(x => x.AutoIncrement))
            {
                result.AppendLine("SET IDENTITY_INSERT !!!TABLE_NAME OFF");
                result.AppendLine("GO ");
                result.AppendLine();
            }

            return result + Environment.NewLine;
        }

        private static string ColumnsToSplitedString(string[] cols)
        {
            var res = string.Empty;

            for (var i = 0; i < cols.Length; i++)
            {
                if (!string.IsNullOrEmpty(res))
                    res += ", ";

                res += cols[i];                
                if (i % 10 == 0)
                    res += Environment.NewLine;
            }

            return res;
        }

        private static void GetSetter(DataRow dr, DataColumn[] columns, StringBuilder result, bool addComma = false)
        {
            foreach (var col in columns)
            {
                var val = DbHelper.GetScriptValue(dr[col], col.DataType) + "\t\t-- " + col.ColumnName;
                if (dr.Table.PrimaryKey.Any(x => x.ColumnName == col.ColumnName))
                    val += ", pk";

                if (Equals(col, columns.FirstOrDefault()))
                {
                    if (addComma)
                        val = "," + val;
                }
                else
                    val = "," + val;


                if (col.ReadOnly && !dr.Table.PrimaryKey.Contains(col))
                    val = "--ReadOnly " + val;

                result.AppendLine(val);
            }
        }
    }
}
