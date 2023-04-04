namespace Calls;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class IncludeTypeAttribute : Attribute
{
    public IncludeTypeAttribute(Type type)
    {
        Type = type ?? throw new ArgumentNullException("type");
    }

    public Type Type { get; }
}
