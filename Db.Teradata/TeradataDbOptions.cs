namespace Services
{
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="TeradataDbService"/> class.</summary>
    [PublicAPI]
    public class TeradataDbOptions
    {
        /// <summary>Gets or sets the data source.</summary>
        /// <value>The data source.</value>
        public string DataSource { get; set; }

        /// <summary>Gets or sets the database.</summary>
        /// <value>The database.</value>
        public string Database { get; set; }

        /// <summary>Gets or sets the user identifier.</summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>Gets or sets the authentication mechanism.</summary>
        /// <value>The authentication mechanism.</value>
        public string AuthenticationMechanism { get; set; }
    }
}
