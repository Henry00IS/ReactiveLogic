using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlpacaIT.ReactiveLogic.Internal
{
    /// <summary>Provides common useful utility methods used to implement reactive logic.</summary>
    internal static class Utilities
    {
        /// <summary>Iterates over all components in the scene that implement <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The interface type that components should have implemented.</typeparam>
        public static IEnumerable<T> FindObjectsOfTypeImplementing<T>()
        {
            var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                var interfaces = gameObjects[i].GetComponentsInChildren<T>();
                for (int j = 0; j < interfaces.Length; j++)
                    yield return interfaces[j];
            }
        }

        /// <summary>Returns a random number between min and max (exclusive) with an exception.</summary>
        public static int RandomExcept(int min, int max, int except)
        {
            if (min == max) return min;
            if (min == max - 1) return min;
            int result = Random.Range(min, max - 1);
            if (result >= except) result += 1;
            return result;
        }

        /// <summary>Returns a random item from the given array with an exception.</summary>
        public static T RandomExcept<T>(T[] array, T except)
        {
            if (array.Length == 0) return default;
            int index = System.Array.IndexOf(array, except);
            if (index == -1) return array[Random.Range(0, array.Length)];
            return array[RandomExcept(0, array.Length, index)];
        }

        /// <summary>Returns a random item from the given list with an exception.</summary>
        public static T RandomExcept<T>(List<T> list, T except)
        {
            var count = list.Count;
            if (count == 0) return default;
            int index = list.IndexOf(except);
            if (index == -1) return list[Random.Range(0, count)];
            return list[RandomExcept(0, count, index)];
        }

        /// <summary>Returns a random item from the given array.</summary>
        public static T RandomItem<T>(T[] array)
        {
            if (array.Length == 0) return default;
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>Returns a random item from the given list.</summary>
        public static T RandomItem<T>(List<T> list)
        {
            var count = list.Count;
            if (count == 0) return default;
            return list[Random.Range(0, count)];
        }

        /// <summary>
        /// Returns an enumerable that iterates over the collection in random order.
        /// <para>Fisher-Yates-Durstenfeld shuffle.</para>
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var buffer = source.ToArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                int j = Random.Range(i, buffer.Length);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}