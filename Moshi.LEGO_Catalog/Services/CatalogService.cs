namespace Moshi.LEGO_Catalog.Services;

using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using build_lego_catalog.Models;
using Moshi.LEGO_Catalog.Models.Item;

public class CatalogService
{
    private readonly string _connectionString;

    public CatalogService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return new SQLiteConnection(_connectionString);
    }

    // Categories
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Category>("SELECT * FROM Categories");
    }

    public async Task<Category> GetCategoryByIdAsync(string categoryId)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Category>("SELECT * FROM Categories WHERE CategoryID = @CategoryID", new { CategoryID = categoryId });
    }

    // Colors
    public async Task<IEnumerable<Color>> GetAllColorsAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Color>("SELECT * FROM Colors");
    }

    public async Task<Color> GetColorByIdAsync(string colorId)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Color>("SELECT * FROM Colors WHERE ColorID = @ColorID", new { ColorID = colorId });
    }

    // ItemTypes
    public async Task<IEnumerable<ItemType>> GetAllItemTypesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<ItemType>("SELECT * FROM ItemTypes");
    }

    // Parts
    public async Task<IEnumerable<Part>> GetAllPartsAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Part>("SELECT * FROM Parts");
    }

    public async Task<Part> GetPartByNumberAsync(string partNumber)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Part>("SELECT * FROM Parts WHERE Number = @Number", new { Number = partNumber });
    }

    // Sets
    public async Task<IEnumerable<Set>> GetAllSetsAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Set>("SELECT * FROM Sets");
    }

    public async Task<Set> GetSetByNumberAsync(string setNumber)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Set>("SELECT * FROM Sets WHERE Number = @Number", new { Number = setNumber });
    }

    // Minifigures
    public async Task<IEnumerable<Minifigure>> GetAllMinifiguresAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Minifigure>("SELECT * FROM Minifigures LIMIT 100");
    }

    public async Task<Minifigure> GetMinifigureByNumberAsync(string minifigureNumber)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Minifigure>("SELECT * FROM Minifigures WHERE Number = @Number", new { Number = minifigureNumber });
    }

    // Codes
    public async Task<IEnumerable<Code>> GetAllCodesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Code>("SELECT * FROM Codes");
    }

    public async Task<Code> GetCodeByItemNoAndColorAsync(string itemNo, string color)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Code>("SELECT * FROM Codes WHERE ItemNo = @ItemNo AND Color = @Color", new { ItemNo = itemNo, Color = color });
    }
}