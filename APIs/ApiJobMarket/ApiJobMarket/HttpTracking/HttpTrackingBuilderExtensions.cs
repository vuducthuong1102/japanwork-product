using Owin;


namespace ApiJobMarket.HttpTracking
{
    public static class HttpTrackingBuilderExtensions
    {
        public static IAppBuilder UseHttpTracking(this IAppBuilder builder, HttpTrackingOptions options)
        {
            return builder.Use<HttpTrackingMiddleware>(options);
        }
    }
}