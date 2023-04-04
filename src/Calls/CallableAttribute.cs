namespace Calls;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class CallableAttribute : Attribute
{
    public CallableAttribute(string? name = null)
    {
        Name = name;
    }

    public CallableAttribute(bool useMethodName)
    {
        UseMethodName = useMethodName;
    }

    public string? Name { get; }

    public bool UseMethodName { get; }
}
