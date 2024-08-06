namespace Moshi.Forums.Models;

public class Forum
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentForumId { get; set; } // For sub-forums, 0 or null if it's a top-level forum
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ThreadCount { get; set; }
    public int PostCount { get; set; }
    public int LastPostId { get; set; }
    public DateTime LastPostAt { get; set; }
    public int DisplayOrder { get; set; } // For custom ordering of forums

    // Navigation properties
    // public virtual ICollection<Thread> Threads { get; set; }
    // public virtual Forum ParentForum { get; set; }
    // public virtual ICollection<Forum> SubForums { get; set; }
}
