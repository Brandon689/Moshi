using CsvHelper.Configuration.Attributes;

namespace Moshi.LEGO_Catalog.Models.Item;

public class Minifigure : ItemBase
{
    [Name("Year Released")]
    public string YearReleased { get; set; }
}