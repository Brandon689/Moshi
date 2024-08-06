using Moshi.Ecommerce.Data;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Services;

public class ProductService
{
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Product>> GetProductsAsync()
    {
        return _repository.GetProductsAsync();
    }

    public Task AddProductAsync(Product product)
    {
        return _repository.AddProductAsync(product);
    }

    public Task UpdateProductAsync(int id, Product product)
    {
        return _repository.UpdateProductAsync(id, product);
    }

    public Task DeleteProductAsync(int id)
    {
        return _repository.DeleteProductAsync(id);
    }
}