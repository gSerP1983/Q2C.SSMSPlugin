using System;
using System.Data;
using System.Text;

namespace Q2C.Core.Commands
{
    public class CreateTableCommand : CommandBase, ICommand
    {
        public string Id
        {
            get { return "QueryToCreateTempTable"; }
        }

        public string Name
        {
            get { return "Query To Create Temp Table..."; }
        }

        public string Description
        {
            get { return "Query To Create Temp Table..."; }
        }

        protected override string GetDataTableScript(DataTable dt)
        {
            var result = new StringBuilder();
            result.AppendFormat("CREATE TABLE #{1} ({0}   ", Environment.NewLine, dt.TableName);

            var firstTime = true;
            foreach (DataColumn column in dt.Columns)
            {
                if (firstTime) firstTime = false;
                else
                    result.Append("   ,");

                result.AppendFormat("[{0}] {1} {2} {3}",
                    column.ColumnName, // 0
                    GetSqlTypeAsString(column.DataType), // 1
                    column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                    Environment.NewLine // 3
                );
            }
            result.AppendFormat(") ON [PRIMARY]{0}GO{0}{0}", Environment.NewLine);

            return result.ToString();
        }

        /// <summary>
        /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
        /// </summary>
        private static string GetSqlTypeAsString(Type dataType)
        {
            switch (dataType.Name)
            {
                case "Boolean": return "[bit]";
                case "Char": return "[char]";
                case "SByte": return "[tinyint]";
                case "Int16": return "[smallint]";
                case "Int32": return "[int]";
                case "Int64": return "[bigint]";
                case "Byte": return "[tinyint]";
                case "UInt16": return "[smallint]";
                case "UInt32": return "[int]";
                case "UInt64": return "[bigint]";
                case "Single": return "[float]";
                case "Double": return "[double]";
                case "Decimal": return "[decimal]";
                case "DateTime": return "[datetime]";
                case "Guid": return "[uniqueidentifier]";
                case "Object": return "[variant]";
                case "String": return "[nvarchar](250)";
                default: return "[nvarchar](MAX)";
            }
        }
    }
}
