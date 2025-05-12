using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MigratedProject.Adapters;

namespace MigratedProject.Services
{
    /// <summary>
    /// Database service for managing database connections and operations
    /// </summary>
    public class DatabaseService
    {
        private static IDbAdapter? _adapter;
        private static string _dbType = string.Empty;
        private static object _connectionInfo = new();

        /// <summary>
        /// Initialize the database connection with the specified adapter
        /// </summary>
        /// <param name="connectionInfo">Connection information or SQLite path</param>
        /// <param name="dbType">Database type ('sqlite', 'sqlserver', 'postgresql')</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public static async Task InitializeAsync(object connectionInfo, string dbType = "sqlite")
        {
            _dbType = dbType.ToLowerInvariant();
            _connectionInfo = connectionInfo;

            // Create appropriate adapter based on database type
            _adapter = _dbType switch
            {
                "sqlite" => new SqliteAdapter(connectionInfo.ToString() ?? throw new ArgumentNullException(nameof(connectionInfo))),
                //"sqlserver" => new SqlServerAdapter(connectionInfo),
                //"postgresql" => new PostgreSqlAdapter(connectionInfo),
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };

            await _adapter.InitializeAsync();
        }

        /// <summary>
        /// Close the database connection
        /// </summary>
        /// <returns>Task representing the asynchronous operation</returns>
        public static async Task CloseAsync()
        {
            if (_adapter != null)
            {
                await _adapter.CloseAsync();
                _adapter = null;
            }
        }

        /// <summary>
        /// Execute a query and return all results
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Optional query parameters</param>
        /// <returns>Result set as a list of dictionaries</returns>
        public static async Task<List<Dictionary<string, object>>> QueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            EnsureAdapterInitialized();
            return await _adapter!.QueryAsync(query, parameters);
        }

        /// <summary>
        /// Execute a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="parameters">Optional command parameters</param>
        /// <returns>Number of rows affected</returns>
        public static async Task<int> ExecuteAsync(string command, Dictionary<string, object>? parameters = null)
        {
            EnsureAdapterInitialized();
            return await _adapter!.ExecuteAsync(command, parameters);
        }

        /// <summary>
        /// Get database metadata including type and connection info
        /// </summary>
        /// <returns>Database metadata as a dictionary</returns>
        public static Dictionary<string, object> GetDatabaseMetadata()
        {
            if (_adapter == null)
            {
                return new Dictionary<string, object>
                {
                    ["type"] = "none",
                    ["name"] = "No database initialized"
                };
            }

            return new Dictionary<string, object>
            {
                ["type"] = _dbType,
                ["name"] = _dbType switch
                {
                    "sqlite" => "SQLite",
                    "sqlserver" => "SQL Server",
                    "postgresql" => "PostgreSQL",
                    _ => "Unknown"
                },
                ["connectionInfo"] = _connectionInfo
            };
        }

        /// <summary>
        /// Get the appropriate query for listing tables based on database type
        /// </summary>
        /// <returns>SQL query for listing tables</returns>
        public static string GetListTablesQuery()
        {
            EnsureAdapterInitialized();
            
            return _dbType switch
            {
                "sqlite" => "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'",
                "sqlserver" => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                "postgresql" => "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'",
                _ => throw new NotSupportedException($"Database type not supported: {_dbType}")
            };
        }

        /// <summary>
        /// Get the appropriate query for describing a table based on database type
        /// </summary>
        /// <param name="tableName">Name of the table to describe</param>
        /// <returns>SQL query for describing a table</returns>
        public static string GetDescribeTableQuery(string tableName)
        {
            EnsureAdapterInitialized();
            
            return _dbType switch
            {
                "sqlite" => $"PRAGMA table_info({tableName})",
                "sqlserver" => $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'",
                "postgresql" => $"SELECT column_name, data_type, character_maximum_length, is_nullable FROM information_schema.columns WHERE table_name = '{tableName}'",
                _ => throw new NotSupportedException($"Database type not supported: {_dbType}")
            };
        }

        private static void EnsureAdapterInitialized()
        {
            if (_adapter == null)
            {
                throw new InvalidOperationException("Database is not initialized. Call InitializeAsync first.");
            }
        }
    }
}
