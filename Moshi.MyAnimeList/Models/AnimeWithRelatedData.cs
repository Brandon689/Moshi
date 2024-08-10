namespace Moshi.MyAnimeList.Models;
using System.Collections.Generic;

public record AnimeWithRelatedData : MoshiAnime
{
    public List<MoshiAnimeSeason> Seasons { get; init; }
}
