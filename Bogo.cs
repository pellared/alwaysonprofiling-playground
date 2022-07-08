public static class Bogo
{
    private static readonly Random Rnd = new Random();

    public static void Sort<T>(IList<T> list)
        where T : IComparable<T>
    {
        do
        {
            Shuffle(list);
        } while (!IsSorted(list));
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var k = Rnd.Next(i, list.Count);
            var tmp = list[k];
            list[k] = list[i];
            list[i] = tmp;
        }
    }

    private static bool IsSorted<T>(IList<T> list)
        where T : IComparable<T>
    {
        if (list is null || list.Count <= 1)
        {
            return true;
        }

        var prevChar = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            var currChar = list[i];
            if (prevChar.CompareTo(currChar) > 0)
            {
                return false;
            }

            prevChar = currChar;
        }

        return true;
    }
}