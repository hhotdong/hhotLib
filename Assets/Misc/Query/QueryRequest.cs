//https://coffeebraingames.wordpress.com/2017/10/31/simple-query-system/

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