using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;
using SubtitlesParser.Classes.Writers;
using System.Text;
using System.Text.Json;

namespace Moshi.SubtitlesSite.Services
{
    public class SubtitleParserService
    {
        private readonly SubParser _universalParser;

        public SubtitleParserService()
        {
            _universalParser = new SubParser();
        }

        public string ParseSubtitleToJson(string filePath)
        {
            var subtitles = ParseSubtitle(filePath);
            var parsedSubtitles = subtitles.Select(item => new ParsedSubtitleItem
            {
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Lines = item.Lines
            }).ToList();

            return JsonSerializer.Serialize(parsedSubtitles, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public List<SubtitleItem> ParseSubtitle(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return _universalParser.ParseStream(fileStream, Encoding.UTF8);
            }
        }

        public string GetPlainTextSubtitles(string filePath)
        {
            var subtitles = ParseSubtitle(filePath);
            return string.Join("\n\n", subtitles.Select(item => string.Join("\n", item.Lines)));
        }

        public void WriteSubtitles(string filePath, List<SubtitleItem> subtitles, string format = "srt")
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                WriteSubtitlesToStream(fileStream, subtitles, format);
            }
        }

        public void WriteSubtitlesToStream(Stream stream, List<SubtitleItem> subtitles, string format = "srt")
        {
            ISubtitlesWriter writer;

            switch (format.ToLower())
            {
                case "srt":
                    writer = new SrtWriter();
                    break;
                default:
                    throw new ArgumentException("Unsupported subtitle format", nameof(format));
            }

            writer.WriteStream(stream, subtitles);
        }
    }
}