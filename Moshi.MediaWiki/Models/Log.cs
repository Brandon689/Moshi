namespace Moshi.MediaWiki.Models;

public class Log
{
    public int LogId { get; set; }
    public int? UserId { get; set; }
    public string ActionType { get; set; }
    public int? TargetId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; }
}
