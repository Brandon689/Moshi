using CsvHelper.Configuration.Attributes;

namespace Moshi.LEGO_Catalog.Models.Item;

public class Part : ItemBase
{
    [Name("Alternate Item Number")]
    public string AlternateItemNumber { get; set; }

    [Name("Dimensions")]
    public string Dimensions { get; set; }
}