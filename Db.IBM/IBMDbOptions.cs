namespace Services
{
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.Annotations;

    /// <summary>Configuration settings for the <see cref="IBMDbService"/> class.</summary>
    [PublicAPI]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "IBM is an abbreviation")]
    public class IBMDbOptions
    {
        /// <summary>Gets or sets the database.</summary>
        /// <value>The database.</value>
        public string Database { get; set; }

        /// <summary>Gets or sets the name of the database.</summary>
        /// <value>The name of the database.</value>
        public string DBName { get; set; }

        /// <summary>Gets or sets the user identifier.</summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }
}
