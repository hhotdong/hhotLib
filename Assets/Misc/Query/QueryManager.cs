//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

namespace hhotLib.Common
{
    public static class QueryManager
    {
        public delegate V QueryProvider<R, V>(R request) where R : QueryRequest;

        private static readonly QueryManagerImplementation INTERNAL_MANAGER = new QueryManagerImplementation();

        public static void RegisterProvider<R, V>(QueryProvider<R, V> provider) where R : QueryRequest
        {
            INTERNAL_MANAGER.RegisterProvider(provider);
        }

        public static bool HasProvider<R>() where R : QueryRequest
        {
            return INTERNAL_MANAGER.HasProvider<R>();
        }

        public static V Query<R, V>(R request) where R : QueryRequest
        {
            return INTERNAL_MANAGER.Query<R, V>(request);
        }

        public static void Clear()
        {
            INTERNAL_MANAGER.ResetProviders();
        }
    }
}