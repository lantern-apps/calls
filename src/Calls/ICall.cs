namespace Calls;

public interface ICall
{
    void Invoke(string? routingKey, params object?[] arguments);
    Task InvokeAsync(string? routingKey, params object?[] arguments);
    T? Invoke<T>(string? routingKey, params object?[] arguments);
    Task<T?> InvokeAsync<T>(string? routingKey, params object?[] arguments);
}
