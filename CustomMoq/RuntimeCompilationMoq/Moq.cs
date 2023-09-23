namespace CustomMoq.RuntimeCompilationMoq;

internal class Moq
{
    public static MoqBuilder<TObject> For<TObject>() where TObject : class
    {
        return new MoqBuilder<TObject>();
    }
}
