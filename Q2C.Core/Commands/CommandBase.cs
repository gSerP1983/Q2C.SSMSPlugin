using System.Data;
using System.Linq;

namespace Q2C.Core.Commands
{
    public abstract class CommandBase
    {
        public string Execute(string connectionString, string query)
        {
            var ds = DbHelper.TryGetDataSet(connectionString, query);
            if (ds == null)
                return "";

            return ds.Tables.Cast<DataTable>()
                .Select(GetDataTableScript)
                .Aggregate((s, result) => result + s);
        }   
     
        public bool IsRequireQuery { get { return true; } }

        protected abstract string GetDataTableScript(DataTable dt);
    }
}