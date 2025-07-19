namespace WikiArea.Core.ValueObjects;

public class PageMetadata
{
    public string MetaTitle { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public string Author { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public bool IsIndexable { get; set; } = true;
    public Dictionary<string, string> CustomProperties { get; set; } = new();

    public void SetMetaData(string title, string description, IEnumerable<string>? keywords = null)
    {
        MetaTitle = title;
        MetaDescription = description;
        
        if (keywords != null)
        {
            Keywords.Clear();
            Keywords.AddRange(keywords);
        }
    }

    public void AddCustomProperty(string key, string value)
    {
        CustomProperties[key] = value;
    }

    public void RemoveCustomProperty(string key)
    {
        CustomProperties.Remove(key);
    }

    public string GetCustomProperty(string key) => CustomProperties.GetValueOrDefault(key, string.Empty);
} 