namespace Calls;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
public class RoutingKeyAttribute : Attribute
{
    public RoutingKeyAttribute(string? name = null)
    {
        Name = name;
    }

    public string? Name { get; }
}