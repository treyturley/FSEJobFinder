using FSEDataFeed;
using FSEDataFeedAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;



namespace FSEDataFeedAPI.Services
{
#pragma warning disable CS1591
    public class MongoDBService
    {
        private readonly IMongoCollection<Assignments> _AssignmentsCollection;
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("FSEJOBFINDER_CONNECTIONSTRING"));
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _AssignmentsCollection = database.GetCollection<Assignments>(mongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Assignments assignments)
        {
            await _AssignmentsCollection.InsertOneAsync(assignments);
            return;
        }

        public async Task<List<Assignments>> GetAsync()
        {
            return await _AssignmentsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Assignments> GetAssignmentByMakeModelAsync(string aircraft)
        {
            FilterDefinition<Assignments> filter = Builders<Assignments>.Filter.Eq("aircraft", aircraft);
            return await _AssignmentsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Assignments> UpdateAssignmentByMakeModelAsync(string aircraft, Assignments assignments)
        {
            FilterDefinition<Assignments> filter = Builders<Assignments>.Filter.Eq("aircraft", aircraft);
            UpdateDefinition<Assignments> update = Builders<Assignments>.Update
                .Set("jobs", assignments.jobs)
                .Set("updatedAt", DateTime.Now)
                .SetOnInsert("createdAt", DateTime.Now);
            FindOneAndUpdateOptions<Assignments> options = new FindOneAndUpdateOptions<Assignments>();
            options.IsUpsert = true;
            options.ReturnDocument = ReturnDocument.After;
            return await _AssignmentsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Assignments> filter = Builders<Assignments>.Filter.Eq("Id", id);
            await _AssignmentsCollection.DeleteOneAsync(filter);
            return;
        }
    }
#pragma warning restore CS1591
}

