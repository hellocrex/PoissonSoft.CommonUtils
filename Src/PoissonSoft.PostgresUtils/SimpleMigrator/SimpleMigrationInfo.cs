using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.PostgresUtils.Migrations
{
    [Alias("_migrations_info")]
    class SimpleMigrationInfo
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int Version { get; set; }
        public bool Complete { get; set; }
        public DateTimeOffset StartMigrationTimestamp { get; set; }
        public DateTimeOffset? FinishMigrationTimestamp { get; set; }
    }
}
