using System;
using System.Collections.Generic;
using System.Data;

namespace PoissonSoft.Data.Migrator
{
    /// <summary>
    /// Database migration helper interface
    /// </summary>
    public interface IMigrationHelper
    {
        /// <summary>
        /// Current version of the database
        /// </summary>
        /// <returns></returns>
        int GetCurrentDbVersion();

        /// <summary>
        /// Save data about current migration in database
        /// </summary>
        /// <param name="version">Version of database</param>
        /// <param name="startMigrationTime">Start time of the migration</param>
        /// <param name="finishMigrationTime">End time of the migration</param>
        void SaveMigrationData(int version, DateTimeOffset startMigrationTime, DateTimeOffset finishMigrationTime);

        /// <summary>
        /// Returns connections to database servers 
        /// </summary>
        /// <param name="type">Type of the migration to determine which database connections we want to recieve</param>
        /// <returns></returns>
        IReadOnlyCollection<IDbConnection> GetDbConnection(Type type);

        /// <summary>
        /// Returns using migrations types
        /// </summary>
        /// <returns>Migrations types</returns>
        IReadOnlyCollection<Type> GetMigrationTypes();

        /// <summary>
        /// Create database if it is not exists
        /// </summary>
        void CreateDatabaseIfNotExists();
    }
}
