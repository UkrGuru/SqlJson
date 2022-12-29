using System.Web;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
internal class ApiHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiCrudUri"></param>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string BuildRequestUri(string? apiCrudUri, string proc, object? data = null)
    {
        var result = proc;

        if (!string.IsNullOrEmpty(apiCrudUri)) result = $"{apiCrudUri}/{result}";

        if (data != null) result += $"?data={HttpUtility.UrlPathEncode(DbHelper.Normalize(data).ToString()!)}"; // or Uri.EscapeDataString

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proc"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ValidateProcName(string proc)
    {
        if (!DbHelper.IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }
}
