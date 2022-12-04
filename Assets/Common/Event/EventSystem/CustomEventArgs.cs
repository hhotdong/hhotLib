namespace hhotLib.Common
{
    public abstract class CustomEventArgs { }

    public sealed class TestEventArgs : CustomEventArgs
    {
        public int num;

        public TestEventArgs(int num)
        {
            this.num = num;
        }
    }
}