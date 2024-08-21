using CsvHelper.Configuration.Attributes;

namespace build_lego_catalog.Models;

public class Color
{
    [Name("Color ID")]
    public string ColorID { get; set; }

    [Name("Color Name")]
    public string ColorName { get; set; }

    [Name("RGB")]
    public string RGB { get; set; }

    [Name("Type")]
    public string Type { get; set; }

    [Name("Parts")]
    public int? Parts { get; set; }

    [Name("In Sets")]
    public int? InSets { get; set; }

    [Name("Wanted")]
    public int? Wanted { get; set; }

    [Name("For Sale")]
    public int? ForSale { get; set; }

    [Name("Year From")]
    public int? YearFrom { get; set; }

    [Name("Year To")]
    public int? YearTo { get; set; }
}