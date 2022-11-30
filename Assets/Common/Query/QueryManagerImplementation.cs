// Credit: https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/
using System;
using System.Collections.Generic;

namespace hhotLib.Common
{
    public class QueryManagerImplementation
    {
        public QueryManagerImplementation() { }

        private delegate object QueryProvider(QueryRequest request);  // The internal delegate that we manage

        private Dictionary<Type, QueryProvider> providerMap = new Dictionary<Type, QueryProvider>();

        public void RegisterProvider<R, V>(QueryManager.QueryProvider<R, V> provider) where R : QueryRequest
        {
            if (HasProvider<R>())
            {
                Debug.LogError($"Provider for this request({typeof(R)}) already registered!");
                return;
            }

            // Make the internal delegate which invokes the generic delegate
            QueryProvider internalProvider = delegate (QueryRequest request)
            {
                return provider((R)request);
            };
            providerMap[typeof(R)] = internalProvider;
        }

        public void UnregisterProvider<R>() where R : QueryRequest
        {
            if (HasProvider<R>())
            {
                Debug.Log($"Provider for the request({typeof(R)}) unregistered successfully.");
                providerMap.Remove(typeof(R));
            }
        }

        public bool HasProvider<R>() where R : QueryRequest
        {
            return providerMap.ContainsKey(typeof(R));
        }

        public V Query<R, V>(R request) where R : QueryRequest
        {
            if (HasProvider<R>() == false)
            {
                Debug.LogError($"Provider for the request({typeof(R)}) not registered! Default value of ({typeof(V)}) is returned!");
                return default;
            }
            return (V)providerMap[typeof(R)](request);
        }

        public void ResetProviders()
        {
            providerMap.Clear();
        }
    }
}