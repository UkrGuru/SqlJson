using System.Text;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// 
/// </summary>
internal static class ByteArrayExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToHexString(this byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }
}
