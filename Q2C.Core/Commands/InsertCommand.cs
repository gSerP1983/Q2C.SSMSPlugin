namespace Q2C.Core.Commands
{
    public class InsertCommand : ICommand
    {
        public string Execute(string connectionString, string query)
        {
            return "Query To Insert...";
        }

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
    }
}
