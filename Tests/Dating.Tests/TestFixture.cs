using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dating.Tests
{
    public class TestFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly TestServer Server;

        public TestFixture()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("Development")
                .UseConfiguration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()).UseStartup<TStartup>();

            Server = new TestServer(builder);
        }

        public DatingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DatingDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

            var dbContext = new DatingDbContext(options);
            return dbContext;
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
