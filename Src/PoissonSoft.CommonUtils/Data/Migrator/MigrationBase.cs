using System;
using System.Data;

namespace PoissonSoft.Data.Migrator
{
    /// <summary>
    /// Base class for the migrations
    /// </summary>
    public abstract class MigrationBase
    {
        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version => throw new NotImplementedException();

        /// <summary>
        /// Applying the migration to the database
        /// </summary>
        /// <param name="connection"></param>
        public virtual void ApplyMigration(IDbConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
