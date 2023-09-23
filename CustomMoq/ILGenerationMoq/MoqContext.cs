using System.Reflection;

namespace CustomMoq.ILGenerationMoq;

internal class MoqContext
{
    public static MoqContext Context = new();

    private readonly Dictionary<MethodDescriptor, object> _valuesToReturn = new();
    private MethodDescriptor _methodToConfigure;

    public void SetMethodToConfigure(MethodInfo methodInfo)
    {
        _methodToConfigure = new MethodDescriptor(methodInfo);
    }

    public void LastMethodCallShouldReturn(object value)
    {
        if (_methodToConfigure is null)
        {
            throw new Exception("Cannot configure return value.");
        }

        _valuesToReturn[_methodToConfigure] = value;
    }

    public object? GetReturnValueForMethod(MethodInfo methodInfo)
    {
        var methodDescriptor = new MethodDescriptor(methodInfo);

        if (!_valuesToReturn.ContainsKey(methodDescriptor))
        {
            return default;
        }

        return _valuesToReturn[methodDescriptor];
    }
}
