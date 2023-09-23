namespace CustomMoq.ILGenerationMoq;

public static class MoqExtensions
{
    public static void Returns<T>(this T substitute, T returnThis)
    {
        MoqContext.Context.LastMethodCallShouldReturn(returnThis);
    }
}
