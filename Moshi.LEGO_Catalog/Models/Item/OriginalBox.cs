using CsvHelper.Configuration.Attributes;

namespace Moshi.LEGO_Catalog.Models.Item;

public class OriginalBox : ItemBase
{
    [Name("Year Released")]
    public string YearReleased { get; set; }

    [Name("Dimensions")]
    public string Dimensions { get; set; }
}