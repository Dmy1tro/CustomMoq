using System.Reflection;

namespace CustomMoq.RuntimeCompilationMoq.AssemblyHelpers
{
    internal class TypeDescriptor<T>
    {
        private readonly Type _typeToDescribe = typeof(T);
        private readonly List<string?> _additionalNamespaces = new();
        private MethodInfo[]? _methodsToMock;

        public string GetTypeName() => _typeToDescribe.Name;

        public Type GetDescribedType() => _typeToDescribe;

        public void AddMandatoryNamespaces(string?[] additionalNamespaces)
        {
            _additionalNamespaces.AddRange(additionalNamespaces);
        }

        public ICollection<string> GetMandatoryNamespaces()
        {
            var methodsToMock = GetMethodsToMock();

            var references = methodsToMock
                .Select(m => m.ReturnType)
                .Concat(
                    methodsToMock.SelectMany(m => m.GetParameters()).Select(p => p.ParameterType)
                )
                .ToArray();

            var requiredNamespaces = references
                .Select(r => r.Namespace)
                .Concat(new[] { _typeToDescribe.Namespace })
                .Concat(_additionalNamespaces)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToArray();

            return requiredNamespaces;
        }

        public MethodInfo[] GetMethodsToMock()
        {
            _methodsToMock ??= _typeToDescribe.GetMethods().Where(m => m.IsAbstract || m.IsVirtual).ToArray();

            return _methodsToMock;
        }
    }
}
