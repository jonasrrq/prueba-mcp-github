using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigratedProject.Adapters
{
    /// <summary>
    /// Database adapter interface that defines the contract for all database implementations
    /// </summary>
    public interface IDbAdapter
    {
        /// <summary>
        /// Initialize database connection
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Close database connection
        /// </summary>
        Task CloseAsync();

        /// <summary>
        /// Execute a query and return all results
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Optional query parameters</param>
        /// <returns>Result set as a list of dictionaries</returns>
        Task<List<Dictionary<string, object>>> QueryAsync(string query, Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Execute a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="parameters">Optional command parameters</param>
        /// <returns>Number of rows affected</returns>
        Task<int> ExecuteAsync(string command, Dictionary<string, object>? parameters = null);

        /// <summary>
        /// Get database type
        /// </summary>
        string DatabaseType { get; }

        /// <summary>
        /// Get database connection information
        /// </summary>
        object ConnectionInfo { get; }
    }
}
