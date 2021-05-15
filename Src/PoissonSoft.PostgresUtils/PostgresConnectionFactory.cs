using PoissonSoft.CommonUtils.Data;
using PoissonSoft.PostgresUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace PoissonSoft.PostgresUtils
{
    /// <summary>
    /// Simple postgres connection factory
    /// </summary>
    public class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly DbConnectionStringBuilder connectionString;
        readonly Func<IDbConnection> connectionFactory;

        /// <summary>
        /// Create simple postgres connection factory 
        /// </summary>
        /// <param name="dbSettings">Postgres connection settings</param>
        public PostgresConnectionFactory(PostgresConnectionSettings dbSettings)
        {
            this.connectionString = dbSettings.GetConnectionStringBuilder();
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.connectionFactory = () =>
            {
                var conn = new Npgsql.NpgsqlConnection(connectionString.ConnectionString);
                conn.Open();
                return conn;
            };
        }

        /// <inheritdoc />
        public IDbConnection GetConnection()
        {
            var conn = connectionFactory();
            return conn;
        }
    }
}
