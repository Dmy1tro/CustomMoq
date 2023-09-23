using Castle.DynamicProxy;

namespace CustomMoq.ILGenerationMoq;

public class Moq
{
    public static T For<T>() where T : class
    {
        var proxyGenerator = new ProxyGenerator();
        var proxy = proxyGenerator.CreateClassProxy(
            typeof(object),
            new[] { typeof(T) },
            new Interceptor());

        return (T)proxy;
    }

    private class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            MoqContext.Context.SetMethodToConfigure(invocation.Method);

            var returnValue = MoqContext.Context.GetReturnValueForMethod(invocation.Method);

            invocation.ReturnValue = returnValue != null
                ? returnValue
                : DefaultValueProvider.GetDefaultValueForType(invocation.Method.ReturnType);
        }

        private static class DefaultValueProvider
        {
            public static object? GetDefaultValueForType(Type type)
            {
                if (type == typeof(void) || !type.IsValueType)
                {
                    return null;
                }

                return Activator.CreateInstance(type);
            }
        }
    }
}
