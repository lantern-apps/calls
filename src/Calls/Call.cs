using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calls;

public abstract class Call : ICall
{
    protected readonly List<MethodDescription> Methods = new();

    protected void AddMethod(MethodDescription method) => Methods.Add(method);

    public virtual void Invoke(string? route, object?[] arguments)
    {
        var method = FindRequiredMethod(false, route, null, arguments, out object?[] normalizedArguments);
        if (method.Awaitable)
        {
            InvokeCoreAsync(method, normalizedArguments).GetAwaiter().GetResult();
        }
        else
        {
            InvokeCore(method, normalizedArguments);
        }
    }

    public virtual Task InvokeAsync(string? route, object?[] arguments)
    {
        var method = FindRequiredMethod(true, route, null, arguments, out object?[] normalizedArguments);
        if (method.Awaitable)
        {
            return InvokeCoreAsync(method, normalizedArguments);
        }
        else
        {
            InvokeCore(method, normalizedArguments);
            return Task.CompletedTask;
        }
    }

    public virtual T? Invoke<T>(string? route, object?[] arguments)
    {
        var method = FindRequiredMethod(false, route, typeof(T), arguments, out object?[] normalizedArguments);
        if (method.ReturnType == null)
        {
            if (method.Awaitable)
            {
                InvokeCoreAsync(method, normalizedArguments).GetAwaiter().GetResult();
            }
            else
            {
                InvokeCore(method, normalizedArguments);
            }
            return default;
        }
        else
        {
            if (method.Awaitable)
            {
                return (T?)InvokeCoreAsync(method, normalizedArguments).GetAwaiter().GetResult();
            }
            else
            {
                return (T?)InvokeCore(method, normalizedArguments);
            }
        }
    }

    public virtual async Task<T?> InvokeAsync<T>(string? route, object?[] arguments)
    {
        var method = FindRequiredMethod(true, route, typeof(T), arguments, out object?[] normalizedArguments);
        if (method.ReturnType == null)
        {
            if (method.Awaitable)
            {
                await InvokeCoreAsync(method, normalizedArguments);
            }
            else
            {
                InvokeCore(method, normalizedArguments);
            }

            return default;
        }
        else
        {
            if (method.Awaitable)
            {
                return (T?)await InvokeCoreAsync(method, normalizedArguments);
            }
            else
            {
                return (T?)InvokeCore(method, normalizedArguments);
            }
        }
    }

    protected virtual object? InvokeCore(MethodDescription method, object?[] arguments)
    {
        throw new InvalidOperationException($"Not found handler of route key '{method.Route}'.");
    }

    protected virtual Task<object?> InvokeCoreAsync(MethodDescription method, object?[] arguments)
    {
        throw new InvalidOperationException($"Not found handler of route key '{method.Route}'.");
    }

    private MethodDescription? FindMethod(
        bool awaitable,
        string? route,
        Type? returnType,
        object?[] arguments)
    {
        MethodDescription? candidate = null;
        foreach (MethodDescription method in Methods)
        {
            if (method.Awaitable != awaitable || method.Route != route)
                continue;

            if (!VerifyReturnType(method.ReturnType, returnType))
                continue;

            var argumentMatch = VerifyParameterType(method, arguments);

            if (argumentMatch == TypeMatching.Matched)
            {
                return method;
            }
            else if (argumentMatch == TypeMatching.NoMatched)
            {
                continue;
            }
            candidate = method;
        }

        return candidate;
    }

    private MethodDescription FindRequiredMethod(
        bool awaitable,
        string? route,
        Type? returnType,
        object?[] arguments,
        out object?[] normalizedArguments)
    {
        arguments ??= Array.Empty<object?>();

        var method = FindMethod(awaitable, route, returnType, arguments) ?? FindMethod(!awaitable, route, returnType, arguments);

        if (method == null)
        {
            throw new InvalidOperationException($"Not found method of route key '{route}'.");
        }

        var parameters = method.Parameters;

        if (arguments.Length < parameters.Length)
        {
            normalizedArguments = new object?[parameters.Length];

            Array.Copy(arguments, normalizedArguments, arguments.Length);

            for (int i = arguments.Length; i < parameters.Length; i++)
            {
                if (i == parameters.Length - 1 && method.HasCancellationTokenParameterAtLast)
                {
                    normalizedArguments[i] = CancellationToken.None;
                }
                else
                {
                    normalizedArguments[i] = parameters[i].DefaultValue;
                }
            }
        }
        else if(arguments.Length > parameters.Length)
        {
            normalizedArguments = new object?[parameters.Length];
            Array.Copy(arguments, normalizedArguments, parameters.Length);
        }
        else
        {
            normalizedArguments = arguments;
        }

        return method;
    }

    private static TypeMatching VerifyParameterType(MethodDescription method, object?[] arguments)
    {
        bool lastCancellationToken = arguments.Length != 0 && arguments[arguments.Length - 1] is CancellationToken;
        int argumentsCount = lastCancellationToken ? arguments.Length - 1 : arguments.Length;

        if (arguments.Length < method.RequiredParameterCount)
        {
            return TypeMatching.NoMatched;
        }

        var parameters = method.EffectiveParameters;

        if (argumentsCount > parameters.Length)
        {
            return TypeMatching.NoMatched;
        }

        if (argumentsCount < parameters.Length)
        {
            if (!parameters[argumentsCount].HasDefaultValue)
                return TypeMatching.NoMatched;
        }

        bool compatible = false;

        for (int i = 0; i < argumentsCount; i++)
        {
            var type = arguments[i]?.GetType();
            var parameterType = parameters[i].Type;
            if (type == null)
            {
                if (!parameters[i].Nullable)
                    return TypeMatching.NoMatched;

                continue;
            }

            if (type == parameterType)
                continue;

            if (parameterType.IsAssignableFrom(type))
            {
                compatible = true;
                continue;
            }

            return TypeMatching.NoMatched;
        }

        return compatible ? TypeMatching.Compatible : TypeMatching.Matched;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool VerifyReturnType(Type? definitionType, Type? type, bool ignoredReturn = true)
    {
        if (definitionType == null)
        {
            return type == null || ignoredReturn;
        }
        else if (type == null)
        {
            return definitionType == null && ignoredReturn;
        }
        else
        {
            return type.IsAssignableFrom(definitionType);
        }
    }

    private enum TypeMatching : byte
    {
        NoMatched,
        Matched,
        Compatible
    }

    [DebuggerDisplay("{ToDebugString()}")]
    protected sealed class MethodDescription
    {
        public MethodDescription(
            int id,
            string? route,
            bool awaitable,
            Type? returnType,
            ParameterDescription[] parameters)
        {
            Id = id;
            Route = route;
            Awaitable = awaitable;
            ReturnType = returnType;
            Parameters = parameters;

            if (parameters.Length == 0)
            {
                RequiredParameterCount = 0;
                EffectiveParameters = parameters;
            }
            else
            {
                var last = parameters[parameters.Length - 1];
                if(last.Type == typeof(CancellationToken))
                {
                    HasCancellationTokenParameterAtLast = true;
                    EffectiveParameters = new ParameterDescription[parameters.Length - 1];
                    Array.Copy(parameters, EffectiveParameters, parameters.Length - 1);
                }
                else
                {
                    EffectiveParameters = parameters;
                }

                RequiredParameterCount = EffectiveParameters.Count(x => !x.HasDefaultValue);
            }
        }

        public readonly int RequiredParameterCount;
        public readonly bool HasCancellationTokenParameterAtLast;
        public readonly int Id;
        public readonly string? Route;
        public readonly bool Awaitable;
        public readonly ParameterDescription[] EffectiveParameters;
        public readonly ParameterDescription[] Parameters;
        public readonly Type? ReturnType;

        private string ToDebugString()
        {
            StringBuilder sb = new();
            sb.Append(ReturnType?.Name ?? "void");
            sb.Append($" {Route}(");

            for (int i = 0; i < EffectiveParameters.Length; i++)
            {
                ParameterDescription? parameter = EffectiveParameters[i];
                sb.Append(parameter.Type.Name);
                if (i < EffectiveParameters.Length - 1)
                    sb.Append(", ");
            }

            sb.Append(')');

            return sb.ToString();
        }
    }

    protected sealed class ParameterDescription
    {
        public ParameterDescription(Type type, bool nullable, bool hasDefaultValue, object? defaultValue)
        {
            Type = type;
            Nullable = nullable;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
        }

        public readonly Type Type;
        public readonly bool Nullable;
        public readonly bool HasDefaultValue;
        public readonly object? DefaultValue;
    }

}