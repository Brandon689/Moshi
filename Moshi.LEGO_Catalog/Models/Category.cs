using CsvHelper.Configuration.Attributes;

namespace build_lego_catalog.Models;

public class Category
{
    [Name("Category ID")]
    public string CategoryID { get; set; }

    [Name("Category Name")]
    public string CategoryName { get; set; }
}