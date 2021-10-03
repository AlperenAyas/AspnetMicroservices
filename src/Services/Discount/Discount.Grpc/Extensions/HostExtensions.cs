using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host,int? retry = 0)
        {
            int retyForAvailability = retry.Value;

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database.");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon;";
                    command.ExecuteNonQuery();

                    command.CommandText = @"Create table Coupon(Id SERIAL PRIMARY KEY,ProductName VARCHAR(24) NOT NULL,Description TEXT,Amount INT);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName,Description,Amount) Values('IPhone X','IPhone X Discount',150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName,Description,Amount) Values('Samsung 10','Samsung 10 Discount',90);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postgresql database");

                    connection.Close();
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postgresql database");

                    if (retyForAvailability < 50)
                    {
                        retyForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retyForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
