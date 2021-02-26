using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace LMS.Shared
{
    public enum DatabaseType
    {
        SQLServer = 1433,

        MySQL = 3306,

        PostgreSQL = 5432
    }

    public class Database
    {
        public DatabaseType Type { get; init; }

        public string Version { get; init; }

        public string Server { get; init; }

        public string Port { get; init; }

        public string UserName { get; init; }

        public string Password { get; init; }

        public string Name { get; init; }

        public string ConnectionString => this.Type switch
        {
            DatabaseType.SQLServer => $"Server = {Server}, {Port}; User Id = {UserName}; Password = {Password}; Database = {Name}",

            DatabaseType.MySQL => $"Server = {Server}; Port = {Port}; User = {UserName}; Password = {Password}; Database = {Name}",

            DatabaseType.PostgreSQL => $"Server = {Server}; Port = {Port}; User Id = {UserName}; Password = {Password}; Database = {Name}",

            _ => throw new NotImplementedException()
        };
    }

    public static class DbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder options, Database database) => database.Type switch
        {
            DatabaseType.SQLServer =>
                options.UseSqlServer(database.ConnectionString, sqlServerOptions =>
                    sqlServerOptions
                        .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), new int[] { 2 })),

            DatabaseType.MySQL =>
                options.UseMySql(database.ConnectionString, ServerVersion.FromString(database.Version), mysqlOptions =>
                    mysqlOptions
                        .CharSet(CharSet.Utf8Mb4)
                        .CharSetBehavior(CharSetBehavior.AppendToAllColumns)
                        .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), new int[] { 2 })),

            DatabaseType.PostgreSQL =>
                options.UseNpgsql(database.ConnectionString, npgsqlOptions =>
                    npgsqlOptions
                        .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), new string[] { "2" })
                        .SetPostgresVersion(new Version(database.Version))),

            _ => throw new NotImplementedException()
        };
    }
}
