using Npgsql;
using PoissonSoft.CommonUtils.Environments;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace PoissonSoft.PostgresUtils
{
    /// <summary>
    /// Wrapper for run postgres db in docker for testing purposes
    /// </summary>
    public class DockerPostgres : IDisposable
    {
        static Random random = new Random();
        static HashSet<int> portsSet = new HashSet<int>();

        /// <summary>
        /// Port of the container
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Setting for connect to database
        /// </summary>
        public PostgresConnectionSettings DbSettings { get; private set; }
        DockerContainerWrap DockerContainerWrap { get; set; }

        readonly PostgresHelper postgresHelper;

        /// <summary>
        /// Create and run a container 
        /// </summary>
        /// <param name="containerName">Container name</param>
        public DockerPostgres(string containerName = null)
        {
            postgresHelper = new PostgresHelper();
            Init(containerName);
        }

        void Init(string containerName = null)
        {
            lock (portsSet)
            {
                do
                {
                    Port = random.Next(10000, 20000);
                } while (portsSet.Contains(Port) == true);
                portsSet.Add(Port);
            }
            var password = "1111";
            DockerContainerWrap = new DockerContainerWrap(new DockerContainerSettings
            {
                ContainerName = containerName,
                Image = "postgres:12",
                InternalPort = 5432,
                PublicPort = Port,
                Variables = new[]
                {
                    ("POSTGRES_PASSWORD", password)
                },
                EntryPointParameters = new[]
                {
                    "-c wal_level=logical",
                }
            });
            DockerContainerWrap.StartContainer();
            DbSettings = new PostgresConnectionSettings
            {
                User = "postgres",
                Host = "localhost",
                Password = password,
                Port = Port,    
                Database = "obligator"
            };
            postgresHelper.WaitTillDbStarting(DbSettings);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (DockerContainerWrap != null)
                DockerContainerWrap.Dispose();
            lock (portsSet)
            {
                portsSet.Remove(Port);
            }
        }
    }
}
