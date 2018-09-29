namespace Igorious.StardewValley.DynamicApi2.Utils
{
    public interface IConstructor<out TClass>
    {
        TClass Invoke();
    }

    public interface IConstructor<in TArg, out TClass>
    {
        TClass Invoke(TArg arg);
    }

    public interface IConstructor<in TArg1, in TArg2, out TClass>
    {
        TClass Invoke(TArg1 arg1, TArg2 arg2);
    }
}