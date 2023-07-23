// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Database service for logging to the database via ApiHole
/// </summary>
public class ApiDbLogService : ApiDbService, IDbLogService
{
    /// <summary>
    /// Minimum allowed logging level.
    /// </summary>
    private readonly DbLogLevel? _minDbLogLevel;

    /// <summary>
    /// Initializes a new instance of the ApiDbLogService class with the specified client.
    /// </summary>
    /// <param name="http">HTTP client instance</param>
    /// <param name="configuration">The configuration.</param>
    public ApiDbLogService(HttpClient http, IConfiguration configuration) : base(http)
        => _minDbLogLevel = configuration.GetValue<DbLogLevel?>(MinDbLogLevelPath);

    /// <summary>
    /// MinDbLogLevel allows to set the minimum allowed logging level.
    /// </summary>
    public DbLogLevel MinDbLogLevel => _minDbLogLevel ?? DbLogLevel.Information;

    /// <summary>
    /// MinDbLogLevelPath allows you to change the default path for the MinDbLogLevel property.
    /// </summary>
    public virtual string MinDbLogLevelPath => DbLogHelper.DbLogLevelPathDefault;

    /// <inheritdoc/>
    public async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if ((byte)logLevel >= (byte)MinDbLogLevel)
            {
                await CreateAsync<int?>(DbLogHelper.WJbLogs_Ins, DbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
            }
        }
        finally { await Task.CompletedTask; }
    }
}