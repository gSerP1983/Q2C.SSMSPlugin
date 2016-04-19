namespace Q2C.Core.Commands
{
    public interface ICommand
    {
        string Execute(string connectionString, string query);
        string Id { get; }
        string Name { get; }
        string Description { get; }
        bool IsRequireQuery { get; }
    }
}
