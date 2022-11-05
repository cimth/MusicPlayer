namespace Common;

public static class MathUtil
{
    /// <summary>
    /// Computes <c>a mod b</c>.
    /// <para/>
    /// Be aware that <c>a % b</c> returns the remainder (which can be negative) while <c>a mod b</c> always returns
    /// a number between <c>0</c> and <c>b</c>.
    /// <para/>
    /// For more information see https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The result of <c>a mod b</c> which is a number between <c>0</c> and <c>b</c></returns>
    public static int MathMod(int a, int b)
    {
        int r = a % b;
        return r < 0 ? (r + b) : r;
    }
}