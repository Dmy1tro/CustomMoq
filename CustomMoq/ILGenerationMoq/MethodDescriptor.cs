using System.Reflection;

namespace CustomMoq.ILGenerationMoq;

internal class MethodDescriptor
{
    private readonly string _methodName;
    private readonly Type _methodReturnType;
    private readonly List<ParameterDescriptor> _parameterDescriptors;

    public MethodDescriptor(MethodInfo methodInfo)
    {
        _methodName = methodInfo.Name;
        _methodReturnType = methodInfo.ReturnType;
        _parameterDescriptors = methodInfo.GetParameters().Select(p => new ParameterDescriptor(p)).ToList();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MethodDescriptor);
    }

    private bool Equals(MethodDescriptor? methodDescriptor)
    {
        if (methodDescriptor is null)
        {
            return false;
        }

        return _methodName == methodDescriptor._methodName &&
               _methodReturnType == methodDescriptor._methodReturnType &&
               _parameterDescriptors.Count == methodDescriptor._parameterDescriptors.Count &&
               _parameterDescriptors.All(p1 => methodDescriptor._parameterDescriptors.Any(p2 => p2.Equals(p1)));

    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_methodName, _methodReturnType, _parameterDescriptors.Sum(p => p.GetHashCode()));
    }

    internal class ParameterDescriptor
    {
        private readonly int _parameterPosition;
        private readonly Type _parameterType;

        public ParameterDescriptor(ParameterInfo parameterInfo)
        {
            _parameterPosition = parameterInfo.Position;
            _parameterType = parameterInfo.ParameterType;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ParameterDescriptor);
        }

        private bool Equals(ParameterDescriptor? parameterDescriptor)
        {
            if (parameterDescriptor is null)
            {
                return false;
            }

            return _parameterPosition == parameterDescriptor._parameterPosition &&
                   _parameterType == parameterDescriptor._parameterType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_parameterPosition, _parameterType);
        }
    }
}