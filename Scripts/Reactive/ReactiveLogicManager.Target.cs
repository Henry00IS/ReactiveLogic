using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AlpacaIT.ReactiveLogic
{
    // this partial class handles everything related to target finding.
    public partial class ReactiveLogicManager
    {
        public IEnumerable<IReactive> ForEachReactive(IReactive caller, string target)
        {
            // we ignore targets that are null or empty or just whitespace characters.
            if (string.IsNullOrWhiteSpace(target)) yield break;

            // at this point we know that the target has at least one character in it.

            // we check for the special ! prefix that programmatically finds a target.
            if (target[0] == '!')
            {
                target = target.Substring(1);
                foreach (var reactive in ForEachReactiveSpecialTarget(caller, target.TrimStart('!')))
                    yield return reactive;
            }
            // we search for the target name with the caller as search context.
            else
            {
                foreach (var reactive in ForEachReactiveInContext(caller, target))
                    yield return reactive;
            }
        }

        /// <summary>Finds a special target within the context of the caller.</summary>
        /// <param name="caller">
        /// The <see cref="IReactive"/> that wants to look up the special target name.
        /// </param>
        /// <param name="keyword">The special keyword (e.g. "self" or "group").</param>
        private IEnumerable<IReactive> ForEachReactiveSpecialTarget(IReactive caller, string keyword)
        {
            switch (keyword)
            {
                case "self":
                    yield return caller;
                    break;

                case "group":
                    if (caller.reactiveData.group)
                        yield return caller.reactiveData.group;
                    break;
            }
        }

        /// <summary>
        /// Iterates over all of the <see cref="IReactive"/> that the <paramref name="caller"/> is
        /// able to interact with that match the name.
        /// </summary>
        /// <param name="caller">The <see cref="IReactive"/> that wants to look up the target name.</param>
        /// <param name="name">The target name to find.</param>
        private IEnumerable<IReactive> ForEachReactiveInContext(IReactive caller, string name)
        {
            var targetNameMatcher = CreateTargetNameMatcher(name);

            // match the given target name in all interactable reactives:
            foreach (var reactive in ForEachReactiveInContext(caller))
                if (targetNameMatcher(reactive.gameObject.name))
                    yield return reactive;
        }

        /// <summary>
        /// Iterates over all of the <see cref="IReactive"/> that the <paramref name="caller"/> is
        /// able to interact with.
        /// </summary>
        /// <param name="caller">
        /// The caller <see cref="IReactive"/> that makes up the search context (e.g. it may be in a
        /// group and can not see global reactives).
        /// </param>
        private IEnumerable<IReactive> ForEachReactiveInContext(IReactive caller)
        {
            var group = caller.reactiveData.group;

            // reactives in a group can only interact with other reactives in the same group.
            if (group)
            {
                return ForEachReactiveInGroup(group);
            }
            // reactives outside of a group can only interact with other global reactives.
            else
            {
                return ForEachReactiveGroupless();
            }
        }

        /// <summary>
        /// Iterates over all of the <see cref="IReactive"/> that belong to the <paramref name="group"/>.
        /// </summary>
        /// <param name="group">
        /// The <see cref="LogicGroup"/> that the <see cref="IReactive"/> components in the scene
        /// must be a part of.
        /// </param>
        private IEnumerable<IReactive> ForEachReactiveInGroup(LogicGroup group)
        {
            Debug.Assert(group, "The group can not be null!");

            // iterate over all reactives in the scene.
            var node = reactives.First;
            while (node != null)
            {
                var next = node.Next;
                var reactive = node.Value;

                // check whether the reactive is part of the group:
                if (reactive.reactiveData.group == group)
                    yield return reactive;

                node = next;
            }
        }

        /// <summary>
        /// Iterates over all of the <see cref="IReactive"/> that are not part of a <see cref="LogicGroup"/>.
        /// </summary>
        private IEnumerable<IReactive> ForEachReactiveGroupless()
        {
            // iterate over all reactives in the scene.
            var node = reactives.First;
            while (node != null)
            {
                var next = node.Next;
                var reactive = node.Value;

                // yield only reactive that are not part of a group:
                if (!reactive.reactiveData.group)
                    yield return reactive;

                node = next;
            }
        }

        /// <summary>
        /// Creates a function that matches the given target name to any string.
        /// <para>The character ? is a wildcard for one character.</para>
        /// <para>The character * is a wildcard for zero or more characters.</para>
        /// </summary>
        /// <param name="name">The target name to be matched.</param>
        /// <returns>The function that matches the given string to the target name.</returns>
        public static System.Func<string, bool> CreateTargetNameMatcher(string name)
        {
            if (name.Contains("?") || name.Contains("*"))
            {
                // wildcard regex name comparison:
                var pattern = "^" + Regex.Escape(name).Replace("\\?", ".").Replace("\\*", ".*") + "$";
                var regex = new Regex(pattern);
                return input => regex.IsMatch(input);
            }
            else
            {
                // direct name comparison.
                return input => input == name;
            }
        }
    }
}