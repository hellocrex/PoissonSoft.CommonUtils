using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PoissonSoft.PostgresUtils
{
    /// <summary>
    /// Helper for working with postgres
    /// </summary>
    public class PostgresHelper
    {
        /// <summary>
        /// Create the connection from connection string
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionString)
        {
            var conn = new NpgsqlConnection(connectionString);
            return conn;
        }

        /// <summary>
        /// Waiting for a start of database
        /// </summary>
        /// <param name="settings">Database settings</param>
        /// <param name="waitMs">Time in milliseconds</param>
        public void WaitTillDbStarting(PostgresConnectionSettings settings, int waitMs = 5000)
        {
            var cs = settings.GetConnectionStringToServer().ConnectionString;
            const int sleepTimeMs = 50;
            for (var i = 1; ; i++)
            {
                using (var conn = GetConnection(cs))
                {
                    try
                    {
                        conn.Open();
                        return;
                    }
                    catch (Exception ee)
                    {
                        Trace.WriteLine(ee.Message);
                    }
                }
                if (i * sleepTimeMs > waitMs)
                {
                    throw new InvalidOperationException("Не удалось дождаться запуска базы данных.");
                }
                Thread.Sleep(sleepTimeMs);
            }
        }

        /// <summary>
        /// Create database
        /// </summary>
        /// <param name="settings">Settings with database name</param>
        public void CreateDatabase(PostgresConnectionSettings settings)
        {
            using var conn = GetConnection(settings.GetConnectionStringToServer().ConnectionString);
            try
            {
                conn.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = (NpgsqlConnection)conn;

                cmd.CommandText = $"CREATE DATABASE {settings.Database} ENCODING 'UTF8'";
                cmd.ExecuteNonQuery();
            }
            catch (PostgresException e)
            {
                // "42P04: database already exists"
                // Error codes: http://www.postgresql.org/docs/current/static/errcodes-appendix.html
                if (e.SqlState != "42P04")
                {
                    throw new InvalidOperationException(e.Message);
                }

                throw new InvalidOperationException($"Database with name {settings.Database} already exists", e);
            }
        }

        /// <summary>
        /// Check if database already exists
        /// </summary>
        /// <param name="settings">Settings with database name</param>
        /// <returns></returns>
        public bool IsDatabaseExists(PostgresConnectionSettings settings)
        {
            using var conn = GetConnection(settings.GetConnectionStringToServer().ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = (NpgsqlConnection)conn;

            cmd.CommandText = $"SELECT datname FROM pg_catalog.pg_database WHERE lower(datname) = lower('{settings.Database}');";
            var res = cmd.ExecuteScalar()?.ToString();
            return settings.Database.ToLowerInvariant() == res?.ToLowerInvariant();
        }

        /// <summary>
        /// Create database with given name
        /// </summary>
        /// <param name="settings"></param>
        public void CreateDatabaseIfNotExists(PostgresConnectionSettings settings)
        {
            if (IsDatabaseExists(settings) == false)
            {
                Trace.WriteLine($"Creating the database with name {settings.Database}");
                CreateDatabase(settings);
            }
        }
    }
}
