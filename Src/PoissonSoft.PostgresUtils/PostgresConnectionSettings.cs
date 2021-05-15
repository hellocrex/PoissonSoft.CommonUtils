using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;

namespace PoissonSoft.PostgresUtils
{
    /// <summary>
    /// Settings for connect to postgres
    /// </summary>
    public class PostgresConnectionSettings    
    {
        /// <summary>
        /// Hostname or ip
        /// </summary>
        public string Host { get; set; } = "localhost";
        /// <summary>
        /// Port 
        /// </summary>
        public int? Port { get; set; } = 5432;
        /// <summary>
        /// Username
        /// </summary>
        public string User { get; set; } = "postgres";
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = "1111";
        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// RequireSsl
        /// </summary>
        public bool RequireSsl { get; set; } = true;

        /// <summary>
        /// Get connection string builder to database
        /// </summary>
        /// <returns>Connection string builder</returns>
        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionString = GetConnectionStringToServer();
            if (string.IsNullOrWhiteSpace(Database) == false)
                connectionString["Database"] = Database;
            return connectionString;
        }

        /// <summary>
        /// Get connection string builder to server (without spicify database)
        /// </summary>
        /// <returns>Connection string builder</returns>
        public DbConnectionStringBuilder GetConnectionStringToServer()
        {
            var connectionString = new DbConnectionStringBuilder();
            connectionString["User ID"] = User;
            connectionString["Password"] = Password;
            if (Port.HasValue)
                connectionString["Port"] = Port.ToString();
            connectionString["Host"] = Host;
            if (RequireSsl)
            {
                connectionString["SSL Mode"] = "Require";
                connectionString["Trust Server Certificate"] = true;
            }
            return connectionString;
        }


        /// <summary>
        /// Load this config from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PostgresConnectionSettings LoadConfig(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<PostgresConnectionSettings>(json);
        }
    } 
}
