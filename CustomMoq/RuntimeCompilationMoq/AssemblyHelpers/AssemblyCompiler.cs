using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CustomMoq.RuntimeCompilationMoq.AssemblyHelpers
{
    internal class AssemblyCompiler
    {
        public MoqImplementationWrapper<TInterface> CompileMoqImplementationFor<TInterface>()
        {
            var typeDescriptor = new TypeDescriptor<TInterface>();
            var codeGenerator = new CodeGenerator();
            var assemblyCompilerType = GetType();
            var interfaceName = typeDescriptor.GetTypeName();
            var mockedTypeName = $"{interfaceName}_Moq";

            typeDescriptor.AddMandatoryNamespaces(new[] {
                assemblyCompilerType.Namespace,
                typeof(Dictionary<,>).Namespace,
                typeof(Func<,>).Namespace
            });

            codeGenerator
                .DeclareNamespaces(typeDescriptor.GetMandatoryNamespaces())
                .DeclareMoqClass(mockedTypeName, interfaceName);

            foreach (var methodToMock in typeDescriptor.GetMethodsToMock())
            {
                var returnType = methodToMock.ReturnType;
                var returnMethodType = returnType == typeof(void) ? "void" : returnType.Name;
                var shouldReturnValue = returnType != typeof(void);
                var methodName = methodToMock.Name;
                var parameters = methodToMock
                    .GetParameters()
                    .Select(p => new ParameterDeclaration
                    {
                        Name = p.Name,
                        Type = p.ParameterType.ToString()
                    })
                    .ToArray();

                codeGenerator.DeclareMoqMethod(returnMethodType, methodName, parameters, shouldReturnValue);
            }

            var code = codeGenerator.GenerateCode();

            var compiledAssembly = Compile(mockedTypeName, code, new[]
            {
                typeDescriptor.GetDescribedType(),
                assemblyCompilerType,
                typeof(object)
            });

            var mockedType = compiledAssembly.ExportedTypes.First()!;

            return new MoqImplementationWrapper<TInterface>(mockedType);
        }

        private Assembly Compile(string assemblyName, string sourceCode, IEnumerable<Type> typesToInclude)
        {
            var references = typesToInclude.Select(r => r.Assembly.Location).ToArray();
            var sourceText = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, options);
            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references.Distinct().Select(r => MetadataReference.CreateFromFile(r)).ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                throw new Exception(string.Join(", ", result.Diagnostics.Select(d => d.GetMessage())));
            }

            var assembly = Assembly.Load(ms.ToArray());

            return assembly;
        }
    }
}
