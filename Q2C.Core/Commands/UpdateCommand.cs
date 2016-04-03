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
            var result = new StringBuilder();
            var count = 0;

            foreach (var dr in dt.Rows.Cast<DataRow>())
            {
                // header
                result.AppendLine("-- " + ++count);
                result.AppendLine("UPDATE !!!TABLE_NAME");
                result.AppendLine("SET ");

                // set
                var columns = dt.Columns.Cast<DataColumn>().Where(x => !x.ReadOnly).ToArray();
                foreach (var col in columns)
                {
                    if (dt.PrimaryKey.Any(x => x.ColumnName == col.ColumnName))
                        continue;

                    var val = col.ColumnName + " = " + DbHelper.GetScriptValue(dr[col], col.DataType);
                    if (!Equals(col, columns.FirstOrDefault()))
                        val = "," + val;

                    result.AppendLine(val);
                }

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
    }
}