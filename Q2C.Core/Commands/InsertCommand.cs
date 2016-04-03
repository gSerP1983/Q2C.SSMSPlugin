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
                var columns = dt.Columns.Cast<DataColumn>()
                    .Where(x => !x.ReadOnly)
                    .ToArray();
                columns = dt.PrimaryKey.Union(columns).ToArray();

                // header
                result.AppendLine("-- " + ++count);
                result.AppendLine("INSERT INTO !!!TABLE_NAME (" + string.Join(", ", columns.Select(x => x.ColumnName)) + ")");
                result.AppendLine("VALUES (");

                // set
                foreach (var col in columns)
                {
                    var val = DbHelper.GetScriptValue(dr[col], col.DataType) + "\t\t-- " + col.ColumnName;
                    if (dt.PrimaryKey.Any(x => x.ColumnName == col.ColumnName))
                        val += ", pk";
                    if (!Equals(col, columns.FirstOrDefault()))
                        val = "," + val;

                    result.AppendLine(val);
                }

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
    }
}
