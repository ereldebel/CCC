using System.Collections;
using System.Collections.Generic;
using CCC.Runtime.Utils;

namespace CCC.Runtime.SceneLoading
{
    /// <summary>
    /// Manages the collection of active scenes organized by scene type.
    /// </summary>
    internal class ActiveScenes
    {
        #region Private Fields

        private readonly HashSet<SceneEntry>[] _scenes;

        #endregion

        #region Properties

        /// <summary>
        /// Get a HashSet of all active scenes by type.
        /// </summary>
        /// <param name="type">The scene type to retrieve.</param>
        /// <returns>A HashSet containing all active scenes of the specified type.</returns>
        internal HashSet<SceneEntry> this[SceneType type] => _scenes[(int)type];

        /// <summary>
        /// Get an enumerable of all actives scenes of the given types. 
        /// </summary>
        /// <param name="types">The scene mask containing multiple scene types.</param>
        /// <returns>An enumerable of all active scenes matching the specified types.</returns>
        internal IEnumerable<SceneEntry> this[SceneMask types] => new EnumerableScenes(types, this);

        #endregion

        #region Conostructors

        /// <summary>
        /// Initializes a new ActiveScenes instance with arrays for each scene type.
        /// </summary>
        internal ActiveScenes()
        {
            int typeCount = EnumUtils.Count<SceneType>();
            _scenes = new HashSet<SceneEntry>[typeCount];

            for (int i = 0; i < typeCount; ++i)
            {
                _scenes[i] = new();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Adds a scene to the appropriate type collection.
        /// </summary>
        /// <param name="scene">The scene entry to add.</param>
        internal void Add(SceneEntry scene) => this[scene.type].Add(scene);

        /// <summary>
        /// Adds multiple scenes to their appropriate type collections.
        /// </summary>
        /// <param name="scenes">The collection of scene entries to add.</param>
        internal void Append(IEnumerable<SceneEntry> scenes)
        {
            foreach (SceneEntry scene in scenes)
            {
                this[scene.type].Add(scene);
            }
        }

        /// <summary>
        /// Removes a scene from its type collection.
        /// </summary>
        /// <param name="scene">The scene entry to remove.</param>
        internal void Remove(SceneEntry scene) => this[scene.type].Remove(scene);

        #endregion
            
        #region Types

        /// <summary>
        /// Enumerable wrapper that provides iteration over multiple scene types.
        /// </summary>
        private class EnumerableScenes : IEnumerable<SceneEntry>
        {
            private readonly SceneMask _types;
            private readonly ActiveScenes _activeScenes;

            /// <summary>
            /// Initializes a new EnumerableScenes instance.
            /// </summary>
            /// <param name="types">The scene mask containing the types to enumerate.</param>
            /// <param name="activeScenes">The ActiveScenes instance to enumerate from.</param>
            internal EnumerableScenes(SceneMask types, ActiveScenes activeScenes)
            {
                _types = types;
                _activeScenes = activeScenes;
            }

            /// <summary>
            /// Returns an enumerator that iterates through all scenes of the specified types.
            /// </summary>
            /// <returns>An enumerator for SceneEntry objects.</returns>
            public IEnumerator<SceneEntry> GetEnumerator()
            {
                foreach (SceneType type in _types)
                {
                    foreach (SceneEntry entry in _activeScenes[type])
                    {
                        yield return entry;
                    }
                }
            }

            /// <summary>
            /// Returns a non-generic enumerator.
            /// </summary>
            /// <returns>A non-generic enumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        #endregion
    }
}