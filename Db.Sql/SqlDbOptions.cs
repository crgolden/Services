namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="SqlDbService"/> class.</summary>
    [PublicAPI]
    public class SqlDbOptions
    {
        /// <summary>Gets or sets the data source.</summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; }

        /// <summary>Gets or sets the user identifier.</summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }
}
