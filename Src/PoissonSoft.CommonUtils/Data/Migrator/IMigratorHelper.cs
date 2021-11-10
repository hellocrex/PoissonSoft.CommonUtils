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
        /// <param name="conn"></param>
        /// <param name="version"></param>
        /// <param name="complete"></param>
        /// <param name="dateTime"></param>
        void SaveMigrationData(IDbConnection conn, int version, bool complete, DateTimeOffset dateTime);

        /// <summary>
        /// Returns connections to database servers 
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// Create database if it is not exists
        /// </summary>
        void CreateDatabaseIfNotExists(IDbConnection conn);
    }
}
