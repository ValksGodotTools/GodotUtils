namespace GodotUtils;

public static class Math 
{
    public static uint UIntPow(this uint x, uint pow)
    {
        uint ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}