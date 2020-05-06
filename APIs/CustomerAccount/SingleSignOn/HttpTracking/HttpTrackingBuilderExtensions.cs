using Owin;


namespace SingleSignOn.HttpTracking
{
    public static class HttpTrackingBuilderExtensions
    {
        public static IAppBuilder UseHttpTracking(this IAppBuilder builder, HttpTrackingOptions options)
        {
            return builder.Use<HttpTrackingMiddleware>(options);
        }
    }
}