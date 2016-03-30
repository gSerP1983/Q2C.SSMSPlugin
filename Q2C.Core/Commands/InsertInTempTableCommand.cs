namespace Q2C.Core.Commands
{
    public class InsertInTempTableCommand : ICommand
    {
        public string Execute(string connectionString, string query)
        {
            return "Query To Insert In Temp Table...";
        }

        public string Id
        {
            get { return "QueryToInsertInTempTable"; }
        }

        public string Name
        {
            get { return "Query To Insert In Temp Table..."; }
        }

        public string Description
        {
            get { return "Query To Insert In Temp Table..."; }
        }
    }
}
