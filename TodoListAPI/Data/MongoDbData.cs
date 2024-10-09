using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoListAPI.Data.Settings;

namespace TodoListAPI.Data
{
    public class MongoDbData {

    private readonly IMongoDatabase _database;

    public MongoDbData(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }
}
}
