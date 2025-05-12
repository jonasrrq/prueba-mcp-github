using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace MigratedProject.Adapters
{
    /// <summary>
    /// SQLite database adapter implementation
    /// </summary>
    public class SqliteAdapter : IDbAdapter
    {
        private SqliteConnection? _connection;
        private readonly string _dbPath;

        /// <summary>
        /// Get database type
        /// </summary>
        public string DatabaseType => "sqlite";

        /// <summary>
        /// Get database connection information
        /// </summary>
        public object ConnectionInfo => _dbPath;

        /// <summary>
        /// Initialize with the path to the SQLite database file
        /// </summary>
        /// <param name="dbPath">Path to SQLite database file</param>
        public SqliteAdapter(string dbPath)
        {
            _dbPath = dbPath ?? throw new ArgumentNullException(nameof(dbPath));
        }

        /// <summary>
        /// Initialize the SQLite database connection
        /// </summary>
        public async Task InitializeAsync()
        {
            Console.WriteLine($"[INFO] Opening SQLite database at: {_dbPath}");
            _connection = new SqliteConnection($"Data Source={_dbPath}");
            await _connection.OpenAsync();
            
            // Create insights table if it doesn't exist
            await ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS mcp_insights (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    insight TEXT NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )
            ");
        }

        /// <summary>
        /// Close the SQLite database connection
        /// </summary>
        public async Task CloseAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        /// <summary>
        /// Execute a query and return all results
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Optional query parameters</param>
        /// <returns>Result set as a list of dictionaries</returns>
        public async Task<List<Dictionary<string, object>>> QueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            EnsureConnectionOpen();

            using var command = _connection!.CreateCommand();
            command.CommandText = query;
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var sqliteParam = command.CreateParameter();
                    sqliteParam.ParameterName = param.Key;
                    sqliteParam.Value = param.Value;
                    command.Parameters.Add(sqliteParam);
                }
            }

            using var reader = await command.ExecuteReaderAsync();
            var results = new List<Dictionary<string, object>>();
            
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }
            
            return results;
        }

        /// <summary>
        /// Execute a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="parameters">Optional command parameters</param>
        /// <returns>Number of rows affected</returns>
        public async Task<int> ExecuteAsync(string command, Dictionary<string, object>? parameters = null)
        {
            EnsureConnectionOpen();

            using var cmd = _connection!.CreateCommand();
            cmd.CommandText = command;
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var sqliteParam = cmd.CreateParameter();
                    sqliteParam.ParameterName = param.Key;
                    sqliteParam.Value = param.Value;
                    cmd.Parameters.Add(sqliteParam);
                }
            }

            return await cmd.ExecuteNonQueryAsync();
        }

        private void EnsureConnectionOpen()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Database connection is not open. Call InitializeAsync first.");
            }
        }
    }
}
