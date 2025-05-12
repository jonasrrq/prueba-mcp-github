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
    /// Implementation of database schema tools
    /// </summary>
    [McpServerToolType]
    public static class SchemaTools
    {
        /// <summary>
        /// Create a new table in the database
        /// </summary>
        /// <param name="query">CREATE TABLE SQL statement</param>
        /// <returns>Result of the operation</returns>
        [McpServerTool]
        [Description("Create a new table in the database")]
        public static async Task<Dictionary<string, object>> CreateTable(
            [Description("CREATE TABLE SQL statement")] string query)
        {
            try
            {
                if (!query.Trim().ToLowerInvariant().StartsWith("create table"))
                {
                    throw new ArgumentException("Only CREATE TABLE statements are allowed");
                }

                await DatabaseService.ExecuteAsync(query);
                return FormatUtils.FormatSuccessResponse(new { success = true, message = "Table created successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Alter an existing table in the database
        /// </summary>
        /// <param name="query">ALTER TABLE SQL statement</param>
        /// <returns>Result of the operation</returns>
        [McpServerTool]
        [Description("Alter an existing table in the database")]
        public static async Task<Dictionary<string, object>> AlterTable(
            [Description("ALTER TABLE SQL statement")] string query)
        {
            try
            {
                if (!query.Trim().ToLowerInvariant().StartsWith("alter table"))
                {
                    throw new ArgumentException("Only ALTER TABLE statements are allowed");
                }

                await DatabaseService.ExecuteAsync(query);
                return FormatUtils.FormatSuccessResponse(new { success = true, message = "Table altered successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Drop an existing table from the database
        /// </summary>
        /// <param name="query">DROP TABLE SQL statement</param>
        /// <returns>Result of the operation</returns>
        [McpServerTool]
        [Description("Drop an existing table from the database")]
        public static async Task<Dictionary<string, object>> DropTable(
            [Description("DROP TABLE SQL statement")] string query)
        {
            try
            {
                if (!query.Trim().ToLowerInvariant().StartsWith("drop table"))
                {
                    throw new ArgumentException("Only DROP TABLE statements are allowed");
                }

                await DatabaseService.ExecuteAsync(query);
                return FormatUtils.FormatSuccessResponse(new { success = true, message = "Table dropped successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// List all tables in the database
        /// </summary>
        /// <returns>List of tables</returns>
        [McpServerTool]
        [Description("List all tables in the database")]
        public static async Task<Dictionary<string, object>> ListTables()
        {
            try
            {
                string query = DatabaseService.GetListTablesQuery();
                var results = await DatabaseService.QueryAsync(query);
                return FormatUtils.FormatSuccessResponse(results);
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Describe a table's schema
        /// </summary>
        /// <param name="tableName">Name of the table to describe</param>
        /// <returns>Table schema information</returns>
        [McpServerTool]
        [Description("Describe a table's schema")]
        public static async Task<Dictionary<string, object>> DescribeTable(
            [Description("Name of the table to describe")] string tableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    throw new ArgumentException("Table name is required");
                }

                string query = DatabaseService.GetDescribeTableQuery(tableName);
                var results = await DatabaseService.QueryAsync(query);
                return FormatUtils.FormatSuccessResponse(results);
            }
            catch (Exception ex)
            {
                throw new Exception($"SQL Error: {ex.Message}");
            }
        }
    }
}
