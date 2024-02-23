using System;
namespace TestMAUI.Helpers.Extensions
{
	public static class CollectionExtension
    {
        public static void ParallelSelect<T>(this IEnumerable<T> data, Action<T> action)
        {
            Parallel.ForEach(data, action);
        }

        public static void ForEach<T>(this IEnumerable<T> data, Action<int, T> action)
        {
            int index = 0;
            foreach(T item in data)
            {                
                action?.Invoke(index++, item);
            }
        }

        public static void For<T>(this int maxNumber, Action<int> action, int startIndex = 0)
        {
            for (int index = startIndex; index < maxNumber; index++)
            {
                action?.Invoke(index);
            }
        }


        public static IEnumerable<T> Select<T>(this int maxNumber, Func<int, T> action, int startIndex = 0)
        {
            List<T> data = new List<T>();
            for (int index = startIndex; index < maxNumber; index++)
            {
                T item = action.Invoke(index);
                if (item != null)
                {
                    data.Add(item);
                    //yield return item;
                }
            }

            return data;
        }

    }
}

