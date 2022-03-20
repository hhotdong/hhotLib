//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

namespace hhotLib.Common
{
    public abstract class QueryRequest { }

    public class TestRequest : QueryRequest
    {
        private readonly int intParam;
        private readonly string stringParam;

        public TestRequest(int intParam, string stringParam)
        {
            this.intParam = intParam;
            this.stringParam = stringParam;
        }

        public int IntParam => intParam;
        public string StringParam => stringParam;
    }

    public class CheckLoadingWindowVisibleRequest : QueryRequest
    {
        private readonly bool checkVisible;

        public CheckLoadingWindowVisibleRequest(bool checkVisible)
        {
            this.checkVisible = checkVisible;
        }

        public bool CheckVisible => checkVisible;
    }
}