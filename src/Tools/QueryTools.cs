using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using MigratedProject.Services;
using MigratedProject.Utils;

namespace MigratedProject.Tools
{
    /// <summary>
    /// Implementation of query-related tools
    /// </summary>
    [McpServerToolType]
    public static class QueryTools
    {
        /// <summary>
        /// Execute a read-only SQL query
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <returns>Query results</returns>
        [McpServerTool]
        [Description("Execute SELECT queries to read data from the database")]
        public static async Task<Dictionary<string, object>> ReadQuery(
            [Description("SQL SELECT query to execute")] string query)
        {
            try
            {
                if (!query.Trim().ToLowerInvariant().StartsWith("select"))
                {
                    throw new ArgumentException("Only SELECT queries are allowed with read_query");
                }

                var result = await DatabaseService.QueryAsync(query);
                return FormatUtils.FormatSuccessResponse(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute a write SQL query (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <returns>Operation results</returns>
        [McpServerTool]
        [Description("Execute INSERT, UPDATE, or DELETE queries to modify data in the database")]
        public static async Task<Dictionary<string, object>> WriteQuery(
            [Description("SQL query to execute (INSERT, UPDATE, DELETE)")] string query)
        {
            try
            {
                string queryLower = query.Trim().ToLowerInvariant();
                if (!(queryLower.StartsWith("insert") || queryLower.StartsWith("update") || queryLower.StartsWith("delete")))
                {
                    throw new ArgumentException("Only INSERT, UPDATE, or DELETE queries are allowed with write_query");
                }

                int affectedRows = await DatabaseService.ExecuteAsync(query);
                return FormatUtils.FormatSuccessResponse(new { affectedRows });
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute a SELECT query and export the results as CSV
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <returns>CSV formatted data</returns>
        [McpServerTool]
        [Description("Execute a SELECT query and export the results as CSV")]
        public static async Task<string> ExportQuery(
            [Description("SQL SELECT query to execute")] string query)
        {
            try
            {
                if (!query.Trim().ToLowerInvariant().StartsWith("select"))
                {
                    throw new ArgumentException("Only SELECT queries are allowed with export_query");
                }

                var result = await DatabaseService.QueryAsync(query);
                return FormatUtils.ConvertToCSV(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }
    }
}
