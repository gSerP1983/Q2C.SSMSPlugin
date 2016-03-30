namespace Q2C.Core.Commands
{
    public class CreateTableCommand : ICommand
    {
        public string Execute(string connectionString, string query)
        {
            return "Query To Create Temp Table...";
        }

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
    }
}
