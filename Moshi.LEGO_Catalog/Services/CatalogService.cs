using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using build_lego_catalog.Models;
using Moshi.LEGO_Catalog.Models.Item;

namespace Moshi.LEGO_Catalog.Services
{
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
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Category>(
                "SELECT * FROM Categories ORDER BY CategoryID LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Category>(
                "SELECT * FROM Categories WHERE CategoryID = @CategoryID",
                new { CategoryID = categoryId }
            );
        }

        // Colors
        public async Task<IEnumerable<Color>> GetAllColorsAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Color>(
                "SELECT * FROM Colors ORDER BY ColorID LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Color> GetColorByIdAsync(string colorId)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Color>(
                "SELECT * FROM Colors WHERE ColorID = @ColorID",
                new { ColorID = colorId }
            );
        }

        // ItemTypes
        public async Task<IEnumerable<ItemType>> GetAllItemTypesAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<ItemType>(
                "SELECT * FROM ItemTypes ORDER BY ItemTypeID LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        // Parts
        public async Task<IEnumerable<Part>> GetAllPartsAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Part>(
                "SELECT * FROM Parts ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Part> GetPartByNumberAsync(string partNumber)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Part>(
                "SELECT * FROM Parts WHERE Number = @Number",
                new { Number = partNumber }
            );
        }

        // Sets
        public async Task<IEnumerable<Set>> GetAllSetsAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Set>(
                "SELECT * FROM Sets ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Set> GetSetByNumberAsync(string setNumber)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Set>(
                "SELECT * FROM Sets WHERE Number = @Number",
                new { Number = setNumber }
            );
        }

        // Minifigures
        public async Task<IEnumerable<Minifigure>> GetAllMinifiguresAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Minifigure>(
                "SELECT * FROM Minifigures ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Minifigure> GetMinifigureByNumberAsync(string minifigureNumber)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Minifigure>(
                "SELECT * FROM Minifigures WHERE Number = @Number",
                new { Number = minifigureNumber }
            );
        }

        public async Task<IEnumerable<Gear>> GetAllGearAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Gear>(
                "SELECT * FROM Gear ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<IEnumerable<OriginalBox>> GetAllBoxesAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<OriginalBox>(
                "SELECT * FROM OriginalBoxes ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<IEnumerable<Instructions>> GetAllInstructionsAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Instructions>(
                "SELECT * FROM OriginalBoxes ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Book>(
                "SELECT * FROM Books ORDER BY Number LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        // Codes
        public async Task<IEnumerable<Code>> GetAllCodesAsync(int page, int pageSize)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Code>(
                "SELECT * FROM Codes ORDER BY ItemNo, Color LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (page - 1) * pageSize }
            );
        }

        public async Task<Code> GetCodeByItemNoAndColorAsync(string itemNo, string color)
        {
            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Code>(
                "SELECT * FROM Codes WHERE ItemNo = @ItemNo AND Color = @Color",
                new { ItemNo = itemNo, Color = color }
            );
        }
    }
}