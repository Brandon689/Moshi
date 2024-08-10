namespace Moshi.MyAnimeList.Models;
using System.Collections.Generic;

public class AnimeWithRelatedData : MoshiAnime
{
    public List<MoshiAnimeSeason> Seasons { get; set; }
}
