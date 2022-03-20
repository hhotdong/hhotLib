//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

class QueryManagerImplementation
{

    private delegate object QueryProvider(QueryRequest request); // The internal delegate that we manage

    private Dictionary<Type, QueryProvider> providerMap = new Dictionary<Type, QueryProvider>();

    public QueryManagerImplementation() { }

    public void RegisterProvider<R, V>(QueryManager.QueryProvider<R, V> provider) where R : QueryRequest
    {
        Type type = typeof(R);
        Assert.IsTrue(!this.providerMap.ContainsKey(type)); // Should not contain the provider for a certain request yet

        // Make the internal delegate which invokes the generic delegate
        QueryProvider internalProvider = delegate (QueryRequest request) {
            return provider((R)request);
        };
        this.providerMap[type] = internalProvider;
    }

    public bool HasProvider<R>() where R : QueryRequest
    {
        return this.providerMap.ContainsKey(typeof(R));
    }

    public V Query<R, V>(R request) where R : QueryRequest
    {
        Type type = typeof(R);

        // Invoke the provider
        // This will throw an error if a provider does not exist
        return (V)this.providerMap[type](request);
    }

}