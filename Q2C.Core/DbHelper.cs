using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Q2C.Core
{
    public static class DbHelper
    {
        public static DataSet TryGetDataSet(string connectionString, string query)
        {
            try
            {
                var dataSet = new DataSet("Q2C");
                using (var da = new SqlDataAdapter(query, connectionString))
                {
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    da.Fill(dataSet);
                }
                return dataSet;
            }
            catch(Exception x)
            {
                Log.Write(x);
                return null;
            }            
        }

        public static string GetScriptValue(object value, Type type)
        {
            if (value == null || Convert.IsDBNull(value))
                return "NULL";

            if (type == typeof(string))
                return "'" + value.ToString().Replace("'", "''") + "'";

            if (type == typeof(Guid))
                return "'" + value + "'";

            if (type == typeof(bool))
                return Convert.ToInt32(value).ToString(CultureInfo.InvariantCulture);

            if (type == typeof (float) || type == typeof (double) || type == typeof (decimal))
                return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);

            if (type == typeof (byte[]))
            {
                var data = (byte[]) value;
                if (data.Length == 0)
                    return "NULL";

                return data.Aggregate("0x", (current, t) => current + t.ToString("h2"));
            }

            if (type == typeof (DateTime))
            {
                var dateTime = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture);
                return string.Format(CultureInfo.InvariantCulture, "convert(datetime,'{0}', 121)", dateTime);
            }

            return value.ToString();
        }
    }
}