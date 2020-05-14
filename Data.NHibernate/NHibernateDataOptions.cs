namespace Services
{
    using JetBrains.Annotations;
    using NHibernate;

    /// <summary>Configuration settings for the <see cref="NHibernateDataOptions"/> class.</summary>
    [PublicAPI]
    public class NHibernateDataOptions
    {
        /// <summary>Gets or sets a value indicating whether the <see cref="ISession"/> should use an <see cref="ITransaction"/>.</summary>
        /// <value>The transaction flag.</value>
        public bool UseTransaction { get; set; }
    }
}
