using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Gateway.Infrastructure.Repositories;

/// <summary>
/// High-performance repository for inserting usage logs using SqlBulkCopy.
/// </summary>
public class UsageLogRepository : IUsageLogRepository
{
    private readonly string _connectionString;
    private readonly ILogger<UsageLogRepository> _logger;

    public UsageLogRepository(IConfiguration configuration, ILogger<UsageLogRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing SQL connection string");
        _logger = logger;
    }

    public async Task BulkInsertAsync(IEnumerable<UsageLog> logs, CancellationToken ct)
    {
        var dataTable = ToDataTable(logs);

        if (dataTable.Rows.Count == 0)
            return;

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            using var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "UsageLogs",
                BatchSize = Math.Min(dataTable.Rows.Count, 5000),
                BulkCopyTimeout = 60 // seconds
            };

            // Map columns explicitly to avoid mismatches
            bulk.ColumnMappings.Add("UserId", "UserId");
            bulk.ColumnMappings.Add("KeyId", "KeyId");
            bulk.ColumnMappings.Add("CompanyId", "CompanyId");
            bulk.ColumnMappings.Add("ContractId", "ContractId");
            bulk.ColumnMappings.Add("PlanId", "PlanId");
            bulk.ColumnMappings.Add("RouteId", "RouteId");
            bulk.ColumnMappings.Add("ResponseStatusCode", "ResponseStatusCode");
            bulk.ColumnMappings.Add("DurationMs", "DurationMs");

            await bulk.WriteToServerAsync(dataTable, ct);

            _logger.LogDebug("Bulk inserted {Count} usage logs successfully", dataTable.Rows.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk insert failed for {Count} usage logs", dataTable.Rows.Count);
        }
    }

    /// <summary>
    /// Converts a collection of UsageLogEvent objects to a DataTable for bulk copy.
    /// </summary>
    private static DataTable ToDataTable(IEnumerable<UsageLog> logs)
    {
        var table = new DataTable();

        table.Columns.Add("UserId", typeof(long));
        table.Columns.Add("KeyId", typeof(long));
        table.Columns.Add("CompanyId", typeof(long));
        table.Columns.Add("ContractId", typeof(long));
        table.Columns.Add("PlanId", typeof(long));
        table.Columns.Add("RouteId", typeof(long));
        table.Columns.Add("ResponseStatusCode", typeof(int));
        table.Columns.Add("DurationMs", typeof(long));

        foreach (var log in logs)
        {
            var row = table.NewRow();
            row["UserId"] = log.UserId;
            row["KeyId"] = log.KeyId;
            row["CompanyId"] = log.CompanyId;
            row["ContractId"] = log.ContractId;
            row["PlanId"] = log.PlanId;
            row["RouteId"] = log.RouteId;
            row["ResponseStatusCode"] = log.ResponseStatusCode;
            row["DurationMs"] = log.DurationMs;
            table.Rows.Add(row);
        }

        return table;
    }
}