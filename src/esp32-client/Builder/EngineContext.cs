
public static class EngineContext
{
#nullable disable
    private static IServiceProvider serviceProvider;

    public static void SetServiceProvider(IServiceProvider provider)
    {
        serviceProvider = provider;
    }

    public static T Resolve<T>() where T : class
    {
        if (serviceProvider == null)
        {
            throw new InvalidOperationException("ServiceProvider has not been set. Call SetServiceProvider() first.");
        }

        return serviceProvider.GetRequiredService<T>();
    }
}
