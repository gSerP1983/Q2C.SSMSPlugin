using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Q2C.Core.Commands
{
    public class UpdateCommand : CommandBase, ICommand
    {
        public string Id
        {
            get { return "QueryToUpdate"; }
        }

        public string Name
        {
            get { return "Query To Update..."; }
        }

        public string Description
        {
            get { return "Query To Update..."; }
        }

        protected override string GetDataTableScript(DataTable dt)
        {
            var columns = dt.Columns.Cast<DataColumn>().Where(x => !x.ReadOnly).ToArray();
            var roColumns = dt.Columns.Cast<DataColumn>().Where(x => x.ReadOnly && !dt.PrimaryKey.Contains(x)).ToArray();
            if (!columns.Any() && !roColumns.Any())
                return string.Empty;

            var result = new StringBuilder();
            var count = 0;

            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                // header
                result.AppendLine("-- " + ++count);
                result.AppendLine("UPDATE !!!TABLE_NAME");
                result.AppendLine("SET ");

                // set
                GetSetter(dr, columns, result);
                GetSetter(dr, roColumns, result, columns.Any());

                // where
                if (dt.PrimaryKey.Length == 0)
                {
                    result.AppendLine("!!!WHERE");
                }
                else
                {
                    result.AppendLine("WHERE ");
                    foreach (var col in dt.PrimaryKey)
                    {
                        var val = col.ColumnName + " = " + DbHelper.GetScriptValue(dr[col], col.DataType);
                        if (!Equals(col, dt.PrimaryKey.LastOrDefault()))
                            val += " AND ";

                        result.AppendLine(val);
                    }
                }

                result.AppendLine("GO ");
                result.AppendLine();
            }

            return result + Environment.NewLine;
        }

        private static void GetSetter(DataRow dr, DataColumn[] columns, StringBuilder result, bool addComma = false)
        {
            foreach (var col in columns)
            {
                if (dr.Table.PrimaryKey.Any(x => x.ColumnName == col.ColumnName))
                    continue;

                var val = col.ColumnName + " = " + DbHelper.GetScriptValue(dr[col], col.DataType);
                if (Equals(col, columns.FirstOrDefault()))
                {
                    if (addComma)
                        val = "," + val;
                }
                else
                    val = "," + val;


                if (col.ReadOnly)
                    val = "--ReadOnly " + val;

                result.AppendLine(val);
            }
        }
    }
}