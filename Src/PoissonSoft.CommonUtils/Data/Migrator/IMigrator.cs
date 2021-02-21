namespace PoissonSoft.Data.Migrator
{
    /// <summary>
    /// Interface of the db migrator
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// Apply the migration to db
        /// </summary>
        /// <returns>Version after applying the migrations</returns>
        int Migrate();
    }
}