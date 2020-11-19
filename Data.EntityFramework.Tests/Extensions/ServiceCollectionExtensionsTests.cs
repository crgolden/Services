namespace Services.Data.EntityFramework.Tests.Extensions
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;
    using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

    public class ServiceCollectionExtensionsTests
    {
        private readonly Random _random;

        public ServiceCollectionExtensionsTests()
        {
            _random = new Random();
        }

        [Theory]
        [InlineData(true, Scoped)]
        [InlineData(true, Transient)]
        [InlineData(true, Singleton)]
        [InlineData(false, Scoped)]
        [InlineData(false, Transient)]
        [InlineData(false, Singleton)]
        public void AddEntityFrameworkDataService(bool flag, ServiceLifetime serviceLifetime)
        {
            // Arrange
            var poolSize = flag ? _random.Next() : default(int?);
            DbContextOptions<DbContext> dbContextOptions;
            var services = new ServiceCollection();

            // Act
            using (var provider = services.AddEntityFrameworkDataService<DbContext>(
                options =>
                {
                    options.PoolSize = poolSize;
                    options.UsePooling = flag;
                },
                options =>
                {
                    options.EnableDetailedErrors(flag);
                    options.EnableSensitiveDataLogging(flag);
                    options.EnableServiceProviderCaching(flag);
                },
                serviceLifetime,
                serviceLifetime).BuildServiceProvider(true))
            {
                if (serviceLifetime == Scoped)
                {
                    using (var scope = provider.CreateScope())
                    {
                        dbContextOptions = scope.ServiceProvider.GetRequiredService<DbContextOptions<DbContext>>();
                        if (flag)
                        {
                            scope.ServiceProvider.GetRequiredService<DbContextPool<DbContext>>();
                        }
                        else
                        {
                            scope.ServiceProvider.GetRequiredService<DbContext>();
                        }
                    }
                }
                else
                {
                    dbContextOptions = provider.GetRequiredService<DbContextOptions<DbContext>>();
                    if (flag)
                    {
                        provider.GetRequiredService<DbContextPool<DbContext>>();
                    }
                    else
                    {
                        provider.GetRequiredService<DbContext>();
                    }
                }
            }

            // Assert
            var extension = Assert.Single(dbContextOptions.Extensions);
            var coreOptionsExtension = Assert.IsType<CoreOptionsExtension>(extension);
            if (flag)
            {
                Assert.Equal(poolSize, coreOptionsExtension.MaxPoolSize);
            }

            Assert.Equal(flag, coreOptionsExtension.DetailedErrorsEnabled);
            Assert.Equal(flag, coreOptionsExtension.IsSensitiveDataLoggingEnabled);
            Assert.Equal(flag, coreOptionsExtension.ServiceProviderCachingEnabled);
        }
    }
}
