using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlJsonServiceCollectionExtensions
    {
        public static void AddSqlJson(this IServiceCollection services, string connString = null)
        {
            DbHelper.ConnString = connString;
            services.AddSqlJson();
        }
    }
}