namespace WikiArea.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDb";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string UsersCollectionName { get; set; } = "users";
    public string WikiPagesCollectionName { get; set; } = "wiki_pages";
    public string WikiFoldersCollectionName { get; set; } = "wiki_folders";
    public string CommentsCollectionName { get; set; } = "comments";
} 