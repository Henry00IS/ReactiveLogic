using System.Collections.Generic;
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
    }
}