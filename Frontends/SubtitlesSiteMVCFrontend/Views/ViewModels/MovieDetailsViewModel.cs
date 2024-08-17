using Moshi.SubtitlesSite.Models;

namespace SubtitlesSiteMVCFrontend.Views.ViewModels;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; }
    public IEnumerable<Subtitle> Subtitles { get; set; }
    public IEnumerable<AlternativeTitle> AlternativeTitles { get; set; }
    public IEnumerable<MovieLink> MovieLinks { get; set; }
    public IEnumerable<SubtitleCommentView> Comments { get; set; }
}
