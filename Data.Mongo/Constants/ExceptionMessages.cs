namespace Services.Constants
{
    internal static class ExceptionMessages
    {
        internal static string ClientAlreadyRegistered(string name) => $"Client already registered for '{name}'";

        internal static string ClientNotRegistered(string name) => $"Client not registered for '{name}'";

        internal static string CollectionNameNotFound(string name) => $"Collection name not found for type '{name}'";

        internal static string MissingDatabaseInfo(string name) => $"{nameof(MongoDataOptions.DatabaseName)} cannot be null for '{name}'";
    }
}
