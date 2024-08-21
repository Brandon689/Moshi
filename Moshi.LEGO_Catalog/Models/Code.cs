using CsvHelper.Configuration.Attributes;

namespace build_lego_catalog.Models;

public class Code
{
    [Name("Item No")]
    public string ItemNo { get; set; }

    [Name("Color")]
    public string Color { get; set; }

    [Name("Code")]
    public string CodeValue { get; set; }
}