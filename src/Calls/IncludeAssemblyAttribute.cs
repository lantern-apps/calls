using System.Reflection;

namespace Calls;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class IncludeAssemblyAttribute : Attribute
{
    public IncludeAssemblyAttribute(Type? type = null)
    {
        Assembly = type?.Assembly;
    }

    public Assembly? Assembly { get; }

}
