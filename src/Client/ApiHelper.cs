// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Web;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// Provides helper methods for working with an API.
/// </summary>
internal class ApiHelper
{
    /// <summary>
    /// Builds a request URI for an API call.
    /// </summary>
    /// <param name="apiHoleUri">The base URI of the API.</param>
    /// <param name="proc">The name of the stored procedure to call.</param>
    /// <param name="data">The data to pass to the stored procedure.</param>
    /// <returns>The request URI for the API call.</returns>
    public static string BuildRequestUri(string? apiHoleUri, string proc, object? data = null)
    {
        var result = proc;

        if (!string.IsNullOrEmpty(apiHoleUri)) result = $"{apiHoleUri}/{result}";

        if (data != null) result += $"?Data={HttpUtility.UrlPathEncode(Convert.ToString(DbHelper.Normalize(data)))}"; // or Uri.EscapeDataString

        return result;
    }

    /// <summary>
    /// Validates the name of a stored procedure.
    /// </summary>
    /// <param name="proc">The name of the stored procedure to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the stored procedure name is invalid.</exception>
    public static void ValidateProcName(string proc)
    {
        if (!DbHelper.IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }
}