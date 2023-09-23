using System.Linq.Expressions;
using CustomMoq.RuntimeCompilationMoq.AssemblyHelpers;

namespace CustomMoq.RuntimeCompilationMoq;

internal class MoqBuilder<TObject> where TObject : class
{
    private readonly Dictionary<string, Func<object>> _interceptors = new();
    private TObject _object;

    public TObject Object
    {
        get
        {
            if (_object is null)
            {
                _object = new AssemblyCompiler()
                    .CompileMoqImplementationFor<TObject>()
                    .CreateMoqClass(_interceptors);
            }

            return _object;
        }
    }

    public MoqSetup<TObject, TValue> Setup<TValue>(Expression<Func<TObject, TValue>> methodCall)
    {
        return new MoqSetup<TObject, TValue>(this, methodCall);
    }

    internal void SetInterceptor(string methodName, Func<object> interceptor)
    {
        _interceptors[methodName] = interceptor;
    }
}

internal class MoqSetup<TObject, TValue> where TObject : class
{
    private readonly MoqBuilder<TObject> _moqBuilder;
    private readonly Expression<Func<TObject, TValue>> _expression;

    public MoqSetup(MoqBuilder<TObject> moqBuilder, Expression<Func<TObject, TValue>> expression)
    {
        _moqBuilder = moqBuilder;
        _expression = expression;
    }

    public void Returns(TValue value)
    {
        var methodName = ((MethodCallExpression)_expression.Body).Method.Name;
        var returnValue = () => (object)value!;

        _moqBuilder.SetInterceptor(methodName, returnValue);
    }
}

