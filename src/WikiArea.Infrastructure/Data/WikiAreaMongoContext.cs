using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Infrastructure.Configuration;

namespace WikiArea.Infrastructure.Data;

public class WikiAreaMongoContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;
    private static bool _isConfigured = false;
    private static readonly object _lock = new object();

    public WikiAreaMongoContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        
        // Configure SmartEnum serialization
        ConfigureSmartEnumSerialization();
        
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    private static void ConfigureSmartEnumSerialization()
    {
        if (_isConfigured) return;
        
        lock (_lock)
        {
            if (_isConfigured) return;

            // Register SmartEnum serializers
            BsonSerializer.RegisterSerializer(typeof(UserStatus), new SmartEnumSerializer<UserStatus>());
            BsonSerializer.RegisterSerializer(typeof(UserRole), new SmartEnumSerializer<UserRole>());
            BsonSerializer.RegisterSerializer(typeof(PageStatus), new SmartEnumSerializer<PageStatus>());
            BsonSerializer.RegisterSerializer(typeof(ContentType), new SmartEnumSerializer<ContentType>());
            
            _isConfigured = true;
        }
    }

    public IMongoCollection<User> Users => 
        _database.GetCollection<User>(_settings.UsersCollectionName);

    public IMongoCollection<WikiPage> WikiPages => 
        _database.GetCollection<WikiPage>(_settings.WikiPagesCollectionName);

    public IMongoCollection<WikiFolder> WikiFolders => 
        _database.GetCollection<WikiFolder>(_settings.WikiFoldersCollectionName);

    public IMongoCollection<Comment> Comments => 
        _database.GetCollection<Comment>(_settings.CommentsCollectionName);

    public async Task CreateIndexesAsync()
    {
        try
        {
            await CreateUserIndexesAsync();
        }
        catch (MongoCommandException ex) when (ex.ErrorMessage.Contains("already exists"))
        {
            // Index already exists, ignore
        }

        try
        {
            await CreateWikiPageIndexesAsync();
        }
        catch (MongoCommandException ex) when (ex.ErrorMessage.Contains("already exists"))
        {
            // Index already exists, ignore
        }

        try
        {
            await CreateWikiFolderIndexesAsync();
        }
        catch (MongoCommandException ex) when (ex.ErrorMessage.Contains("already exists"))
        {
            // Index already exists, ignore
        }

        try
        {
            await CreateCommentIndexesAsync();
        }
        catch (MongoCommandException ex) when (ex.ErrorMessage.Contains("already exists"))
        {
            // Index already exists, ignore
        }
    }

    private async Task CreateUserIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Username)),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Email)),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.AdfsId)),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Role)),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(x => x.Department))
        };

        await Users.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateWikiPageIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Ascending(x => x.Slug)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Ascending(x => x.FolderId)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Ascending(x => x.Status)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Ascending(x => x.Tags)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Ascending(x => x.CreatedBy)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Descending(x => x.UpdatedAt)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Descending(x => x.ViewCount)),
            new CreateIndexModel<WikiPage>(Builders<WikiPage>.IndexKeys.Text(x => x.Title).Text(x => x.Content))
        };

        await WikiPages.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateWikiFolderIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<WikiFolder>(Builders<WikiFolder>.IndexKeys.Ascending(x => x.Path)),
            new CreateIndexModel<WikiFolder>(Builders<WikiFolder>.IndexKeys.Ascending(x => x.ParentFolderId)),
            new CreateIndexModel<WikiFolder>(Builders<WikiFolder>.IndexKeys.Ascending(x => x.SortOrder))
        };

        await WikiFolders.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateCommentIndexesAsync()
    {
        var indexes = new[]
        {
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(x => x.WikiPageId)),
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(x => x.AuthorId)),
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(x => x.ParentCommentId)),
            new CreateIndexModel<Comment>(Builders<Comment>.IndexKeys.Ascending(x => x.Mentions))
        };

        await Comments.Indexes.CreateManyAsync(indexes);
    }
} 