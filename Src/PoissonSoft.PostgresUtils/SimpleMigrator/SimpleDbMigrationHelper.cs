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
        private readonly Type baseMigrationType = typeof(SimpleDbMigrationBase);
        Func<string, IDbConnection> getDbConnection;

        /// <inheritdoc />
        public SimpleDbMigrationHelper(PostgresConnectionSettings connectionSettings, 
            IPostgresHelper postgresHelper)
        {
            this.connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
            this.postgresHelper = postgresHelper ?? throw new ArgumentNullException(nameof(postgresHelper));
            getDbConnection = (s =>
            {
                var conn = new Npgsql.NpgsqlConnection(s);
                conn.Open();
                return conn;
            });
        }

        /// <inheritdoc />
        public void CreateDatabaseIfNotExists()
        {
            postgresHelper.CreateDatabaseIfNotExists(connectionSettings);
        }

        /// <inheritdoc />
        public int GetCurrentDbVersion()
        {
            using var conn = getDbConnection(connectionSettings.GetConnectionStringBuilder().ConnectionString);
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
        public IReadOnlyCollection<IDbConnection> GetDbConnection(Type type)
        {
            if (type.IsSubclassOf(baseMigrationType) == true)
            {
                return new[] { getDbConnection(connectionSettings.GetConnectionStringBuilder().ConnectionString) };
            }
            else
            {
                throw new InvalidOperationException("Unknown migration type");
            }
        }

        /// <inheritdoc />
        public void SaveMigrationData(int version, bool complete, DateTimeOffset dateTime)
        {
            using var conn = getDbConnection(connectionSettings.GetConnectionStringBuilder().ConnectionString);
            conn.CreateTableIfNotExists<SimpleMigrationInfo>();
            if (complete == false)
            {
                conn.Save(new SimpleMigrationInfo
                {
                    Id = 1,
                    Version = version,
                    StartMigrationTimestamp = dateTime
                });
            }
            else
            {
                conn.UpdateAdd(() => new SimpleMigrationInfo { FinishMigrationTimestamp = dateTime, Complete = complete },
                    conn.From<SimpleMigrationInfo>().Where(s => s.Id == 1));
            }
        }
    }
}
