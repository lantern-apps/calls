using System.Reflection;

namespace Calls;

public static class ICallExtensions
{
    public static void Send(this ICall call, object argument, CancellationToken cancellationToken = default) => call.Invoke(argument.GetRoutingKey(), argument, cancellationToken);

    public static Task SendAsync(this ICall call, object argument, CancellationToken cancellationToken = default) => call.InvokeAsync(argument.GetRoutingKey(), argument, cancellationToken);

    public static TResult? Send<TResult>(this ICall call, object argument, CancellationToken cancellationToken = default) => call.Invoke<TResult>(argument.GetRoutingKey(), argument, cancellationToken);

    public static Task<TResult?> SendAsync<TResult>(this ICall call, object argument, CancellationToken cancellationToken = default) => call.InvokeAsync<TResult>(argument.GetRoutingKey(), argument, cancellationToken);

    private static string? GetRoutingKey(this object argument)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(nameof(argument), "Argument cannot be null.");
        }

        return argument.GetType().GetCustomAttribute<RoutingKeyAttribute>()?.Name;
    }
}
