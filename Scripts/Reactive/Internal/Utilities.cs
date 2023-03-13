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

        /// <summary>
        /// Mathematical construct for linear motion at constant speed that can reverse and complete
        /// the travel back to the beginning in the same amount of time as it took to get, for
        /// example, halfway there. The travel is between 0.0 and 1.0.
        /// </summary>
        public class LinearMotion
        {
            /// <summary>The current position of the linear motion between 0 and 1.</summary>
            public float position { get; private set; }

            /// <summary>
            /// Gets whether the linear motion is currently going forwards (true) from 0 to 1 or
            /// backwards (false) from 1 to 0.
            /// </summary>
            public bool forwards { get; private set; }

            /// <summary>
            /// Gets whether the linear motion is still moving towards the destination (when going
            /// forwards <see cref="position"/> &lt; 1 and when going backwards <see
            /// cref="position"/> &gt; 0).
            /// </summary>
            public bool moving => forwards ? position < 1f : position > 0f;

            /// <summary>
            /// An event that returns true after <see cref="Update"/> when the linear arrived at the
            /// destination (when going forwards <see cref="position"/> == 1 and when going
            /// backwards <see cref="position"/> == 0) else false. It won't be true again until the
            /// linear motion departs from the destination first.
            /// </summary>
            public bool arrived { get; private set; }

            /// <summary>
            /// An event that returns true after <see cref="Update"/> when the linear motion was at
            /// the destination (when going forwards <see cref="position"/> == 1 and when going
            /// backwards <see cref="position"/> == 0) and started moving away else false. It won't
            /// be true again until the linear motion arrives at the destination first.
            /// </summary>
            public bool departed { get; private set; }

            /// <summary>The traveling speed per second.</summary>
            private float speed;

            /// <summary>Reverses the linear motion so that it travels in the other direction.</summary>
            public void Reverse()
            {
                forwards = !forwards;
                speed *= -1f;
            }

            /// <summary>
            /// Reverses the linear motion so that it travels in the other direction. It picks one
            /// of the specified movement times depending on the reversed direction.
            /// </summary>
            /// <param name="openTime">
            /// The time in seconds the full linear motion from 0 to 1 would take.
            /// </param>
            /// <param name="closeTime">
            /// The time in seconds the full linear motion from 1 to 0 would take.
            /// </param>
            public void Reverse(float openTime, float closeTime)
            {
                forwards = !forwards;
                if (forwards)
                    Open(openTime);
                else
                    Close(closeTime);
            }

            /// <summary>Immediately reach the end of the linear motion with positive speed.</summary>
            public void OpenImmediately()
            {
                forwards = true;
                position = 1f;
                speed = Mathf.Abs(speed);
            }

            /// <summary>Immediately reach the beginning of the linear motion with negative speed.</summary>
            public void CloseImmediately()
            {
                forwards = false;
                position = 0f;
                speed = -Mathf.Abs(speed);
            }

            /// <summary>Move towards the end of the linear motion with positive speed.</summary>
            public void Open()
            {
                forwards = true;
                speed = Mathf.Abs(speed);
            }

            /// <summary>Move towards the beginning of the linear motion with negative speed.</summary>
            public void Close()
            {
                forwards = false;
                speed = -Mathf.Abs(speed);
            }

            /// <summary>Move towards the end of the linear motion with positive speed.</summary>
            /// <param name="time">The time in seconds the full linear motion from 0 to 1 would take.</param>
            public void Open(float time)
            {
                forwards = true;
                SetTime(time);
            }

            /// <summary>Move towards the beginning of the linear motion with negative speed.</summary>
            /// <param name="time">The time in seconds the full linear motion from 1 to 0 would take.</param>
            public void Close(float time)
            {
                forwards = false;
                SetTime(time);
            }

            /// <summary>
            /// Sets the time in seconds that a full linear motion takes. Only the current speed can
            /// be adjusted with this method (negative values are absolute).
            /// </summary>
            /// <param name="time">The time in seconds that a full linear motion takes.</param>
            public void SetTime(float time)
            {
                time = Mathf.Abs(time) * (forwards ? 1f : -1f);

                if (time == 0f)
                    speed = 0f; // handled in update.
                else
                    speed = (1f / time);
            }

            /// <summary>Sets the current position of the linear motion between 0 and 1.</summary>
            /// <param name="position">The position of the linear motion between 0 and 1.</param>
            public void SetPosition(float position)
            {
                this.position = Mathf.Clamp01(position);
            }

            /// <summary>
            /// Updates the linear motion, this should be called every update (unless the linear
            /// motion should be paused).
            /// </summary>
            /// <param name="deltaTime">
            /// The interval in seconds from the last frame to the current one.
            /// </param>
            /// <returns>The current position of the linear motion between 0 and 1.</returns>
            public float Update(float deltaTime)
            {
                var previousPosition = position;

                position += speed * deltaTime;
                position = Mathf.Clamp01(position);

                // edge case where the travel takes zero seconds:
                if (speed == 0f)
                {
                    if (forwards)
                        OpenImmediately();
                    else
                        CloseImmediately();
                }

                // check whether we arrived at the destination.
                if (forwards)
                    arrived = (previousPosition != 1f && position == 1f);
                else
                    arrived = (previousPosition != 0f && position == 0f);

                // check whether we departed from the beginning.
                if (forwards)
                    departed = (previousPosition == 0f && position != 0f);
                else
                    departed = (previousPosition == 1f && position != 1f);

                return position;
            }

            /// <summary>
            /// Updates the linear motion, this should be called every update. This overload uses
            /// <see cref="Time.deltaTime"/>.
            /// </summary>
            /// <returns>The current position of the linear motion between 0 and 1.</returns>
            public float Update() => Update(Time.deltaTime);
        }
    }
}