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
            result.AppendFormat("IF OBJECT_ID('tempdb..#{0}') IS NOT NULL DROP TABLE #{0}", dt.TableName);
            result.AppendLine();
            result.AppendFormat("CREATE TABLE #{0} (", dt.TableName);
            result.AppendLine();

            var firstTime = true;
            foreach (DataColumn column in dt.Columns)
            {
                if (firstTime) firstTime = false;
                else
                    result.Append("   ,");

                result.AppendFormat("[{0}] {1} {2} {3}",
                    column.ColumnName, // 0
                    GetSqlTypeAsString(column), // 1
                    column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                    Environment.NewLine // 3
                );
            }
            result.AppendFormat("){0}GO{0}{0}", Environment.NewLine);

            return result.ToString();
        }

        /// <summary>
        /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
        /// </summary>
        private static string GetSqlTypeAsString(DataColumn column)
        {
            switch (column.DataType.Name)
            {
                case "Boolean": return "BIT";
                case "Char": return "CHAR";
                case "SByte": return "TINYINT";
                case "Int16": return "SMALLINT";
                case "Int32": return "INT";
                case "Int64": return "BIGINT";
                case "Byte": return "TINYINT";
                case "UInt16": return "SMALLINT";
                case "UInt32": return "INT";
                case "UInt64": return "BIGINT";
                case "Single": return "FLOAT";
                case "Double": return "DOUBLE";
                case "Decimal": return "DECIMAL";
                case "DateTime": return "DATETIME";
                case "Guid": return "UNIQUEIDENTIFIER";
                case "Object": return "VARIANT";
                case "String": return "NVARCHAR(250)";
                default: return "NVARCHAR(MAX)";
            }
        }
    }
}
