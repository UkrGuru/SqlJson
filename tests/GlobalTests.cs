global using Xunit;

namespace UkrGuru.SqlJson;

public class GlobalTests
{
    public const string DbName = "SqlJsonTest";

    public const string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True";

    public static bool DbOk { get; set; }

    public static string CommandTest = "SELECT 1; /* need more text for CommandText type */";
}