namespace Services
{
    public class EntityFrameworkDataOptions
    {
        public string? DatabaseType { get; set; }

        public bool SeedData { get; set; }

        public string? AssemblyName { get; set; }

        public bool UseLazyLoadingProxies { get; set; }

        public SqlServerOptions? SqlServerOptions { get; set; }

        public SqliteOptions? SqliteOptions { get; set; }
    }
}
