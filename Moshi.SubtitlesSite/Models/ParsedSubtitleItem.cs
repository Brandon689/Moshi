namespace Moshi.SubtitlesSite.Services
{
    public class ParsedSubtitleItem
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public List<string> Lines { get; set; }
    }
}
