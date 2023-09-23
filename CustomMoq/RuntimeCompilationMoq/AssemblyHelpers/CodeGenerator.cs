using System.Text;

namespace CustomMoq.RuntimeCompilationMoq.AssemblyHelpers
{
    internal class CodeGenerator
    {
        private readonly StringBuilder _namespacesDeclaration = new();
        private readonly StringBuilder _classDeclaration = new();
        private readonly StringBuilder _methodsDeclaration = new();

        public CodeGenerator DeclareNamespaces(IEnumerable<string> namespaces)
        {
            foreach (var @namespace in namespaces)
            {
                _namespacesDeclaration.AppendLine($"using {@namespace};");
            }

            return this;
        }

        public CodeGenerator DeclareMoqClass(string moqName, string interfaceName)
        {
            _classDeclaration.AppendLine(
            @$"public class {moqName} : {interfaceName} {{
            private readonly Dictionary<string, Func<object>> _methodInterceptors;
            public {moqName}(Dictionary<string, Func<object>> methodInterceptors) {{
                _methodInterceptors = methodInterceptors;
            }}
            public object InterceptMethod(string methodName) {{
                if (_methodInterceptors.ContainsKey(methodName)) {{
                    return _methodInterceptors[methodName].Invoke();
                }}

                return default;
            }}
            ");

            return this;
        }

        public CodeGenerator DeclareMoqMethod(
            string methodReturnType,
            string methodName,
            IEnumerable<ParameterDeclaration> parametersDeclaration,
            bool returnsValue)
        {
            var parameters = parametersDeclaration.Select(p => $"{p.Type} {p.Name}").ToArray();

            _methodsDeclaration.AppendLine(
                    $@"public {methodReturnType} {methodName}({string.Join(',', parameters)}) {{
                        var result = InterceptMethod(""{methodName}"");
                        return {(returnsValue ? $"({methodReturnType})result" : string.Empty)};
                    }}");

            return this;
        }

        public string GenerateCode()
        {
            return $@"
                {_namespacesDeclaration.ToString()}
                {_classDeclaration.ToString()}
                {_methodsDeclaration.ToString()}
                }}
            ";
        }
    }

    internal class ParameterDeclaration
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}
