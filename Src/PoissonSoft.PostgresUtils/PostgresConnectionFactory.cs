using PoissonSoft.CommonUtils.Data;
using PoissonSoft.PostgresUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace PoissonSoft.PostgresUtils
{
    /// <summary>
    /// Simple postgres connection factory
    /// </summary>
    public class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly DbConnectionStringBuilder connectionString;

        /// <summary>
        /// Create simple postgres connection factory 
        /// </summary>
        /// <param name="dbSettings">Postgres connection settings</param>
        public PostgresConnectionFactory(PostgresConnectionSettings dbSettings)
        {
            this.connectionString = dbSettings.GetConnectionStringBuilder();
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <inheritdoc />
        public IDbConnection GetConnection()
        {
            var conn = new Npgsql.NpgsqlConnection(connectionString.ConnectionString);
            conn.Open();
            return conn;
        }

        /// <inheritdoc />
        public async Task<IDbConnection> GetConnectionAsync()
        {
            var conn = new Npgsql.NpgsqlConnection(connectionString.ConnectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}
