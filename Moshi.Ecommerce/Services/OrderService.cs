using Moshi.Ecommerce.Data;
using Moshi.Ecommerce.Models;

namespace Moshi.Ecommerce.Services;

public class OrderService
{
    private readonly OrderRepository _repository;

    public OrderService(OrderRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Order>> GetOrdersAsync()
    {
        return _repository.GetOrdersAsync();
    }

    public Task<List<Order>> GetOrdersWithItemsAsync()
    {
        return _repository.GetOrdersWithItemsAsync();
    }

    public Task AddOrderAsync(Order order)
    {
        return _repository.AddOrderAsync(order);
    }

    public Task AddOrderItemAsync(OrderItem orderItem)
    {
        return _repository.AddOrderItemAsync(orderItem);
    }
}