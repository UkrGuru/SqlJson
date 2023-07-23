// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Interface for a service that logs to the database. 
/// </summary>
public interface IDbLogService
{
    /// <summary>
    /// Gets the minimum allowed logging level.
    /// </summary>
    DbLogLevel MinDbLogLevel { get; }

    /// <summary>
    /// Gets the path for the MinDbLogLevel property in the configuration.
    /// </summary>
    string MinDbLogLevelPath { get; }

    /// <summary>
    /// Asynchronous method that writes a log of any type to the database.
    /// </summary>
    /// <param name="logLevel">The level of the log to write</param>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronous method that writes a critical log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a debug log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an error log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an information log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a trace log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a warning log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Warning, title, more, cancellationToken);
}

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
                await CreateAsync<int?>(DbLogHelper.WJbLogs_Ins, DbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
            }
        }
        finally { await Task.CompletedTask; }
    }
}