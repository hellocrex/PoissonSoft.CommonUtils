using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ServiceStack.OrmLite;
using System.Linq;
using PoissonSoft.Data.Migrator;

namespace PoissonSoft.PostgresUtils.Migrations
{
    /// <inheritdoc />
    public class SimpleDbMigrationHelper : IMigrationHelper
    {
        private readonly PostgresConnectionSettings connectionSettings;
        private readonly IPostgresHelper postgresHelper;
        private readonly Type baseTypeOfMigrations;
        Func<string, IDbConnection> getDbConnection;

        /// <inheritdoc />
        public SimpleDbMigrationHelper(PostgresConnectionSettings connectionSettings, 
            IPostgresHelper postgresHelper, Type baseTypeOfMigrations)
        {
            this.connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            this.postgresHelper = postgresHelper ?? throw new ArgumentNullException(nameof(postgresHelper));
            this.baseTypeOfMigrations = baseTypeOfMigrations;
            getDbConnection = (s =>
            {
                var conn = new Npgsql.NpgsqlConnection(s);
                conn.Open();
                return conn;
            });
        }

        /// <inheritdoc />
        public void CreateDatabaseIfNotExists(IDbConnection conn)
        {
            postgresHelper.CreateDatabaseIfNotExists(connectionSettings);
        }

        /// <inheritdoc />
        public int GetCurrentDbVersion()
        {
            using var conn = GetDbConnection();
            if (conn.TableExists<SimpleMigrationInfo>() == false)
            {
                return 0;
            }
            var last = conn.Select(conn.From<SimpleMigrationInfo>().OrderByDescending<SimpleMigrationInfo>(m => m.Version)
                .Limit(1)).FirstOrDefault();
            if (last == null)
            {
                return 0;
            }
            return last.Version;
        }

        /// <inheritdoc />
        public IDbConnection GetDbConnection()
        {
            return getDbConnection(connectionSettings.GetConnectionStringBuilder().ConnectionString);
        }

        /// <inheritdoc />
        public void SaveMigrationData(IDbConnection conn, int version, bool complete, DateTimeOffset dateTime)
        {
            conn.CreateTableIfNotExists<SimpleMigrationInfo>();
            if (complete == false)
            {
                conn.Save(new SimpleMigrationInfo
                {
                    Version = version,
                    Complete = complete,
                    StartMigrationTimestamp = dateTime
                });
            }
            else
            {
                conn.UpdateAdd(() => new SimpleMigrationInfo { FinishMigrationTimestamp = dateTime, Complete = complete },
                    conn.From<SimpleMigrationInfo>().Where(s => s.Version == version));
            }
        }
    }
}
