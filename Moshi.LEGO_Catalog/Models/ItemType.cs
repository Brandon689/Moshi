using CsvHelper.Configuration.Attributes;

namespace build_lego_catalog.Models;

public class ItemType
{
    [Name("Item Type ID")]
    public string ItemTypeID { get; set; }

    [Name("Item Type Name")]
    public string ItemTypeName { get; set; }
}