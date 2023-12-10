using System;
namespace TestMAUI.Helpers;

public static class CollectionExtension
{
    public static bool TryGetValue<T>(this IDictionary<string, object> queryParams,string key, out T value)
    {
        if(queryParams.ContainsKey(key) &&
            queryParams.TryGetValue(key, out object objValue) &&
            objValue is T tValue)
        {
            value = tValue;
            return true;
        }
        value = default;
        return false;
    }

    public static bool TryGetRefValue<T>(this IDictionary<string, object> queryParams, string key, ref T value)
    {
        if (queryParams.ContainsKey(key) &&
            queryParams.TryGetValue(key, out object objValue) &&
            objValue is T tValue)
        {
            value = tValue;
            return true;
        }
        return false;
    }
}

public class RandomizerHelper
{
    public static int GetInt(int min = 0, int max = 100)
    {
        return Random.Shared.Next(min, max);
    }

    public static bool GetBoolean()
    {
        return GetInt() % 2 == 0;
    }

    public static DateTime GetDate(int minDateInDays = -30, int maxDateInDays = 0)
    {
        return DateTime.Now.Add(TimeSpan.FromDays(Random.Shared.Next(minDateInDays, maxDateInDays)));
    }

    public static string GetImage()
    {
        return $"https://picsum.photos/300/300?random={Random.Shared.Next(1, 100)}";
    }

    public static string GetName(int maxWords = 1)
    {
        if (maxWords <= 1)
        {
            return RandomName.GenerateName();
        }
        string randomNames = "";
        for (int index = 0; index < maxWords; index++)
        {
            randomNames += RandomName.GenerateName() + " ";
        }
        return randomNames;
    }

    public static char GetSmallLetter()
    {
        return RandomName.RandomLetter();
    }

    public static char GetCapitalLetter()
    {
        return RandomName.RandomLetter(true);
    }

    #region Random Names
    private class RandomName
    {
        static bool enableLogging = false;
        static int a = (int)'a'; static int z = (int)'z';
        static int A = (int)'A'; static int Z = (int)'Z';
        static int[] Vowel = new int[] { (int)'a', (int)'e', (int)'i', (int)'o', (int)'u' };
        static int[] CapitalVowel = new int[] { (int)'A', (int)'E', (int)'I', (int)'O', (int)'U' };

        public static string[] GenerateNames(int maxNameLength = 10, int numberOfName = 1)
        {
            string[] names = new string[numberOfName];
            for (int count = 0; count < numberOfName; count++)
            {

                names[count] = GenerateName(maxNameLength);
            }
            return names;
        }

        public static string GenerateName(int maxNameLength = 10)
        {
            string name = "";
            int length = Random.Shared.Next(5, 10);
            char firstChar = RandomLetter(true);
            name += firstChar;
            bool isVowel = IsVowel(firstChar);
            for (int index = 1; index < length; index++)
            {
                name += isVowel ? RandomLetter(false, true) : RandomVowel();
                isVowel = !isVowel;
            }
            if (enableLogging)
            {
                Console.WriteLine(name);
            }
            return name;
        }

        public static bool IsVowel(char character)
        {
            return CapitalVowel.Contains((int)character) || Vowel.Contains((int)character);
        }

        public static char RandomVowel(bool isCapital = false)
        {
            return (char)(isCapital ? CapitalVowel : Vowel)[Random.Shared.Next(0, 4)];
        }

        public static char RandomLetter(bool isCapital = false, bool excludeVowel = false)
        {
            int character = 0;
        GENERATE_CHAR:
            character = isCapital ? Random.Shared.Next(A, Z) : Random.Shared.Next(a, z);
            if (excludeVowel && IsVowel((char)character))
            {
                goto GENERATE_CHAR;
            }
            return (char)character;
        }
    }
    #endregion
}

