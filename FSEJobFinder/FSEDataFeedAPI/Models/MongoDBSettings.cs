namespace FSEDataFeedAPI.Models
{
    /// <summary>
    /// Describes the Mongo DB settings that must be provided to setup a connection to a MongoDB cluster.
    /// </summary>
    public class MongoDBSettings
    {
        /// <summary>
        /// The connection string describing which mongodb cluster to connect to.
        /// </summary>
        public string ConnectionURI { get; set; } = null!;

        /// <summary>
        /// The name of the mongodb database to connect to.
        /// </summary>
        public string DatabaseName { get; set; } = null!;

        /// <summary>
        /// The name of the collection
        /// </summary>
        public string CollectionName { get; set; } = null!;

    }
}
