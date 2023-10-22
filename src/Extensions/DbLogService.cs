// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Service for logging to the database.
/// </summary>
public class DbLogService : DbService, IDbLogService
{
    /// <summary>
    /// Minimum allowed logging level.
    /// </summary>
    private readonly DbLogLevel? _minDbLogLevel;

    /// <summary>
    /// Initializes a new instance of the DbLogService class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration to use</param>
    public DbLogService(IConfiguration configuration) : base(configuration)
        => _minDbLogLevel = configuration.GetValue<DbLogLevel?>(MinDbLogLevelPath);

    /// <summary>
    /// Gets the minimum allowed logging level.
    /// </summary>
    public DbLogLevel MinDbLogLevel => _minDbLogLevel ?? DbLogLevel.Information;

    /// <summary>
    /// Gets the path for the MinDbLogLevel property in the configuration.
    /// </summary>
    public virtual string MinDbLogLevelPath => DbLogHelper.DbLogLevelPathDefault;

    /// <inheritdoc/>
    public async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if ((byte)logLevel >= (byte)MinDbLogLevel)
            {
                _ = await ExecAsync(DbLogHelper.WJbLogs_Ins, DbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
            }
        }
        finally { await Task.CompletedTask; }
    }
}