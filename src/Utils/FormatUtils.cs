using System;
using System.Collections.Generic;
using System.Text;

namespace MigratedProject.Utils
{
    /// <summary>
    /// Utility methods for formatting data
    /// </summary>
    public static class FormatUtils
    {
        /// <summary>
        /// Convert data to CSV format
        /// </summary>
        /// <param name="data">Data to convert to CSV</param>
        /// <returns>CSV formatted string</returns>
        public static string ConvertToCSV(List<Dictionary<string, object>> data)
        {
            if (data.Count == 0) return string.Empty;
            
            // Get headers
            var headers = string.Join(",", data[0].Keys);
            
            // Create CSV header row
            var csv = new StringBuilder(headers);
            csv.AppendLine();
            
            // Add data rows
            foreach (var row in data)
            {
                var values = new List<string>();
                foreach (var key in data[0].Keys)
                {
                    var val = row[key];
                    
                    // Handle strings with commas, quotes, etc.
                    if (val is string strVal)
                    {
                        // Escape quotes and wrap in quotes
                        values.Add($"\"{strVal.Replace("\"", "\"\"")}\"");
                    }
                    else
                    {
                        values.Add(val?.ToString() ?? "");
                    }
                }
                
                csv.AppendLine(string.Join(",", values));
            }
            
            return csv.ToString();
        }

        /// <summary>
        /// Format a response object for success
        /// </summary>
        /// <param name="data">Data to include in the response</param>
        /// <returns>Formatted response object</returns>
        public static Dictionary<string, object> FormatSuccessResponse(object data)
        {
            return new Dictionary<string, object>
            {
                ["success"] = true,
                ["data"] = data
            };
        }

        /// <summary>
        /// Format a response object for error
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Formatted error response object</returns>
        public static Dictionary<string, object> FormatErrorResponse(string message)
        {
            return new Dictionary<string, object>
            {
                ["success"] = false,
                ["error"] = message
            };
        }
    }
}
