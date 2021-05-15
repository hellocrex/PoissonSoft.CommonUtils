using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PoissonSoft.Data.Migrator
{
    /// <summary>
    /// Simple migrator
    /// </summary>
    public class Migrator : IMigrator
    {
        readonly IMigrationHelper migrationHelper;
        readonly IReadOnlyCollection<MigrationBase> migrations;

        /// <summary>
        /// Creating an instance of the migrator
        /// </summary>
        /// <param name="migrationHelper"></param>
        public Migrator(IMigrationHelper migrationHelper)
        {
            this.migrationHelper = migrationHelper ?? throw new ArgumentNullException(nameof(migrationHelper));
            migrations = GetMigrationsFromCurrentAppDomain();
        }

        /// <inheritdoc />
        public bool IsOwnDbSchemeOutdated()
        {
            var currentDbVersion = migrationHelper.GetCurrentDbVersion();
            var currentVersionOwnMigrations = migrations.Max(m => m.Version);
            return currentDbVersion > currentVersionOwnMigrations;
        }

        /// <summary>
        /// Run the migration
        /// </summary>
        /// <returns></returns>
        public int Migrate()
        {
            Trace.TraceInformation($"Starting db migration...");

            if (migrations is null)
            {
                throw new NullReferenceException($"Метод {nameof(GetMigrationsFromCurrentAppDomain)} вернул NULL");
            }

            // Количество миграций не соответствует количеству уникальных версий
            if (migrations.Count != migrations.Select(m => m.Version).Distinct().Count())
            {
                throw new InvalidOperationException("There were one or any migration wrong.");
            }

            migrationHelper.CreateDatabaseIfNotExists();

            var currentDbVersion = migrationHelper.GetCurrentDbVersion();

            Trace.TraceInformation($"Current version of database: {currentDbVersion}");

            var actualMigrations = migrations.Where(m => m.Version > currentDbVersion).ToArray();

            if (!actualMigrations.Any())
            {
                Trace.TraceInformation("There is no new migrations");
                return currentDbVersion;
            }

            // Prepare connections
            var connections = actualMigrations.Select(m => m.GetType()).Distinct().ToDictionary(t => t,
                t => migrationHelper.GetDbConnection(t));
            var allConnections = connections.Values.SelectMany(cc => cc).ToList();

            // Applying migrations to the servers
            foreach (var migration in actualMigrations.OrderBy(m => m.Version))
            {
                var start = DateTimeOffset.UtcNow;

                foreach (var conn in connections[migration.GetType()])
                {
                    migration.ApplyMigration(conn);
                }

                var finish = DateTimeOffset.UtcNow;
                currentDbVersion = migration.Version;
                migrationHelper.SaveMigrationData(currentDbVersion, start, finish);
                Trace.TraceInformation($"Ьigration to version {currentDbVersion} completed successfully");
            }

            // Утилизируем подключения 
            foreach (var conn in allConnections)
            {
                conn.Dispose();
            }

            return currentDbVersion;
        }

        /// <summary>
        /// Get instances for migration by given type of migration
        /// </summary>
        /// <returns>Collection of migrations to applying to db</returns>
        IReadOnlyCollection<MigrationBase> GetMigrationsFromCurrentAppDomain()
        {
            var migrationTypes = migrationHelper.GetMigrationTypes();
            var assemblies = migrationTypes.Select(t => t.Assembly).Distinct().ToArray();
            var migrations = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => assemblies.Any(a.Equals))
                .SelectMany(s => s.GetTypes())
                .Where(t => migrationTypes.Any(tt => tt.IsAssignableFrom(t)) 
                    && migrationTypes.All(tt => t != tt))
                .Select(t => (MigrationBase)Activator.CreateInstance(t))
                .ToArray();
            return migrations;
        }
    }
}
