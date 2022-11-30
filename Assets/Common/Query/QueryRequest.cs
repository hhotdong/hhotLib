//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

namespace hhotLib.Common
{
    public abstract class QueryRequest { }

    public class QueryTestData : QueryRequest
    {
        public readonly int    intParam;
        public readonly string stringParam;

        public QueryTestData(int intParam, string stringParam)
        {
            this.intParam    = intParam;
            this.stringParam = stringParam;
        }
    }

    public class QueryLoadingWindowVisible : QueryRequest
    {
        public readonly bool isVisible;

        public QueryLoadingWindowVisible(bool isVisible)
        {
            this.isVisible = isVisible;
        }
    }
}