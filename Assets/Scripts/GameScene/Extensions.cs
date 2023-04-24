using System;

static class Extensions
{
    public static void For<T>(this T[] array,Action<T,int> action)
    {

        for (int i = 0; i < array.Length; i++)
        {
            action(array[i], i);
        }

    }
}
