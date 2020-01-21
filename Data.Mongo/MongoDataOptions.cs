namespace Services
{
    using System.Collections.Concurrent;
    using JetBrains.Annotations;
    using MongoDB.Driver;

    /// <summary>Configuration settings for the <see cref="MongoDataService"/> class.</summary>
    [PublicAPI]
    public class MongoDataOptions
    {
        /// <summary>
        /// <para>Gets or sets the connection string of the <see cref="IMongoClient"/> used by the <see cref="MongoDataService"/>.</para>
        /// <para>Will not be used if the <see cref="MongoClientSettings"/> are set.</para>
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>Gets or sets the name of the <see cref="IMongoDatabase"/> accessed by the <see cref="MongoDataService"/>.</summary>
        /// <value>The database name.</value>
        public string DatabaseName { get; set; }

        /// <summary>Gets or sets a value indicating whether the <see cref="MongoDataService"/> should use an <see cref="IClientSessionHandle"/>.</summary>
        /// <value>The client session flag.</value>
        public bool UseClientSession { get; set; }

        /// <summary>Gets the (type name, collection name) pairs used by the <see cref="MongoDataService"/> to access the appropriate <see cref="IMongoCollection{TDocument}"/> for a given document type.</summary>
        /// <value>The (type name, collection name) pairs.</value>
        public ConcurrentDictionary<string, string> CollectionNames { get; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// <para>Gets or sets the settings of the <see cref="IMongoClient"/> used by the <see cref="MongoDataService"/>.</para>
        /// <para>Will be used instead of the <see cref="ConnectionString"/> if both are set.</para>
        /// </summary>
        /// <value>The client settings.</value>
        public MongoClientSettings MongoClientSettings { get; set; }

        /// <summary>
        /// <para>Gets or sets the URL of the <see cref="IMongoClient"/> used by the <see cref="MongoDataService"/>.</para>
        /// <para>Will only be used if both <see cref="ConnectionString"/> and <see cref="MongoClientSettings"/> are <see langword="null" />.</para>
        /// </summary>
        /// <value>The url.</value>
        public MongoUrl MongoUrl { get; set; }

        /// <summary>Gets or sets the collection settings used by the <see cref="MongoDataService"/>.</summary>
        /// <value>The collection settings.</value>
        public MongoCollectionSettings MongoCollectionSettings { get; set; } = new MongoCollectionSettings();

        /// <summary>Gets or sets the database settings used by the <see cref="MongoDataService"/>.</summary>
        /// <value>The database settings.</value>
        public MongoDatabaseSettings DatabaseSettings { get; set; } = new MongoDatabaseSettings();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to list the collection names.</summary>
        /// <value>The list collection names options.</value>
        public ListCollectionNamesOptions ListCollectionNamesOptions { get; set; } = new ListCollectionNamesOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to create the collections.</summary>
        /// <value>The create collection options.</value>
        public CreateCollectionOptions CreateCollectionOptions { get; set; } = new CreateCollectionOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to handle the client session.</summary>
        /// <value>The client session options.</value>
        public ClientSessionOptions ClientSessionOptions { get; set; } = new ClientSessionOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to handle the client session transactions.</summary>
        /// <value>The client session transaction options.</value>
        public TransactionOptions TransactionOptions { get; set; } = new TransactionOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to aggregate the collections.</summary>
        /// <value>The aggregate options.</value>
        public AggregateOptions AggregateOptions { get; set; } = new AggregateOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to aggregate the collections.</summary>
        /// <value>The aggregate options.</value>
        public CountOptions CountOptions { get; set; } = new CountOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to insert one document.</summary>
        /// <value>The insert one options.</value>
        public InsertOneOptions InsertOneOptions { get; set; } = new InsertOneOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to insert many documents.</summary>
        /// <value>The insert many options.</value>
        public InsertManyOptions InsertManyOptions { get; set; } = new InsertManyOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to replace document(s).</summary>
        /// <value>The replace options.</value>
        public ReplaceOptions ReplaceOptions { get; set; } = new ReplaceOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to insert delete document(s).</summary>
        /// <value>The delete options.</value>
        public DeleteOptions DeleteOptions { get; set; } = new DeleteOptions();

        /// <summary>Gets or sets the options used by the <see cref="MongoDataService"/> to bulk write documents.</summary>
        /// <value>The bulk write options.</value>
        public BulkWriteOptions BulkWriteOptions { get; set; } = new BulkWriteOptions();
    }
}
