using CsvHelper.Configuration.Attributes;

namespace Moshi.LEGO_Catalog.Models.Item;

public class ItemBase
{
    [Name("Category ID")]
    public string CategoryID { get; set; }

    [Name("Category Name")]
    public string CategoryName { get; set; }

    [Name("Number")]
    public string Number { get; set; }

    [Name("Name")]
    public string Name { get; set; }

    [Name("Weight (in Grams)")]
    public string Weight { get; set; }
}