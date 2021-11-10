using System;

namespace PoissonSoft.Data.Migrator
{
    /// <summary>
    /// Interface of the db migrator
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// Check if db scheme is outdated.
        /// </summary>
        /// <returns></returns>
        bool IsOwnDbSchemeOutdated();

        /// <summary>
        /// Apply the migration to db
        /// </summary>
        /// <returns>Version after applying the migrations</returns>
        int Migrate();
    }
}