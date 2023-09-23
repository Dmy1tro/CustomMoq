namespace CustomMoq.RuntimeCompilationMoq.AssemblyHelpers
{
    internal class MoqImplementationWrapper<TInterface>
    {
        public MoqImplementationWrapper(Type mockedType)
        {
            MockedType = mockedType;
        }

        public Type MockedType { get; }

        public TInterface CreateMoqClass(Dictionary<string, Func<object>> interceptors)
        {
            return (TInterface)Activator.CreateInstance(MockedType, interceptors)!;
        }
    }
}
