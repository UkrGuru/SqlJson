// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Data.SqlTypes;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extentions for working with an API.
/// </summary>
public static class ApiDbExtensions
{
    /// <summary>
    /// Reads the content of an HTTP response as a string and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public static async Task<int> ReadAsync(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        _ = httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        content.ThrowIfError();

        int.TryParse(content, out var number);

        return number;
    }

    /// <summary>
    /// Reads the content of an HTTP response as a string, deserializes it to an object of type <typeparamref name="T"/>, and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous read operation and contains the deserialized object.</returns>
    public static async Task<T?> ReadAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        _ = httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        return ApiDbHelper.DeNormalize(content).ToObj<T?>();
    }

    /// <summary>
    /// Throws an <see cref="HttpRequestException"/> if the specified content starts with "Error:".
    /// </summary>
    /// <param name="content">The content to check for errors.</param>
    /// <exception cref="HttpRequestException">Thrown if the content starts with "Error:".</exception>
    public static void ThrowIfError(this string? content)
    {
        if (content?.StartsWith("Error:") == true) throw new HttpRequestException(content["Error:".Length..]?.TrimStart());
    }

    /// <summary>
    /// Tries to create a new record in the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="type">Result data type.</param>
    /// <returns></returns>
    public static async Task<string?> TryCreateAsync(this IDbService db, string proc, string? data = default, byte? type = default)
    {
        try
        {
            if (type == (byte)SqlDbType.VarBinary)
            {
                return ApiDbHelper.Normalize(await db.CreateAsync<byte[]>(proc, ApiDbHelper.DeNormalize(data)));
            }
            else if (type == (byte)SqlDbType.VarChar)
            {
                return ApiDbHelper.Normalize(await db.CreateAsync<char[]>(proc, ApiDbHelper.DeNormalize(data)));
            }
            else if (type == (byte)SqlDbType.Xml)
            {
                return ApiDbHelper.Normalize(await db.CreateAsync<SqlXml>(proc, ApiDbHelper.DeNormalize(data)));
            }
            else
            {
                return ApiDbHelper.Normalize(await db.CreateAsync<object?>(proc, ApiDbHelper.DeNormalize(data)));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// Tries to read a record from the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="type">Result data type.</param>
    /// <returns>The record read from the database.</returns>
    public static async Task<string?> TryReadAsync(this IDbService db, string proc, string? data = default, byte? type = default)
    {
        try
        {
            if (type == (byte)SqlDbType.VarBinary)
            {
                return ApiDbHelper.Normalize(await db.ReadAsync<byte[]>(proc, ApiDbHelper.DeNormalize(data)));
            }
            else if (type == (byte)SqlDbType.VarChar)
            {
                return ApiDbHelper.Normalize(await db.ReadAsync<char[]>(proc, data));
            }
            else if (type == (byte)SqlDbType.Xml)
            {
                return await db.ReadAsync<string?>(proc, data);
            }
            else
            {
                //return ApiDbHelper.Normalize(await db.ReadAsync<object?>(proc, ApiDbHelper.DeNormalize(data)));
                return await db.ReadAsync<string?>(proc, ApiDbHelper.DeNormalize(data));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// Tries to update a record in the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The number of records updated in the database.</returns>
    public static async Task<string?> TryUpdateAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            return Convert.ToString(await db.UpdateAsync(proc, ApiDbHelper.DeNormalize(data)));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// Tries to delete a record from the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The number of records deleted from the database.</returns>
    public static async Task<string?> TryDeleteAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            return Convert.ToString(await db.DeleteAsync(proc, ApiDbHelper.DeNormalize(data)));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }
}
