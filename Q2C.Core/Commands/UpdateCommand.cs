namespace Q2C.Core.Commands
{
    public class UpdateCommand : ICommand
    {
        public string Execute(string connectionString, string query)
        {
            return "Query To Update...";
        }

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
    }
}
