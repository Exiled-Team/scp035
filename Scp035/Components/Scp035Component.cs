// -----------------------------------------------------------------------
// <copyright file="Scp035Component.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Components
{
    using Exiled.API.Features;
    using Scp035.Handlers;
    using UnityEngine;

    /// <summary>
    /// Handles the core logic for an Scp035 instance.
    /// </summary>
    public class Scp035Component : MonoBehaviour
    {
        private const string SessionVariable = "IsScp035";
        private Config config;
        private CorrosionHandler corrosionHandler;

        /// <summary>
        /// Gets the attached <see cref="Exiled.API.Features.Player"/> instance.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Determines if a given <see cref="Player"/> is a Scp035.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check for being a Scp035 instance.</param>
        /// <returns>A value indicating whether the <see cref="Player"/> is a Scp035 instance.</returns>
        public static bool IsScp035(Player player) => player.SessionVariables.ContainsKey(SessionVariable);

        /// <summary>
        /// Checks a <see cref="Exiled.API.Features.Player"/>'s <see cref="Exiled.API.Features.Player.SessionVariables"/>
        /// for a key value pair containing a <see cref="Scp035Component"/> instance.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="scp035Component">The found <see cref="Scp035Component"/> instance or null.</param>
        /// <returns>A value indicating whether an <see cref="Scp035Component"/> instance is found.</returns>
        public static bool TryGet(Player player, out Scp035Component scp035Component)
        {
            return player.GameObject.TryGetComponent(out scp035Component);
        }

        /// <summary>
        /// Creates a new <see cref="Scp035Component"/> around a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="player">The player to wrap.</param>
        /// <param name="toReplace">The player to use the properties of.</param>
        public static void Create(Player player, Player toReplace = null)
        {
            if (player == null)
                return;

            var scp035 = player.GameObject.AddComponent<Scp035Component>();
            scp035.Spawn(toReplace);
            player.SessionVariables.Add(SessionVariable, scp035);
        }

        private void Awake()
        {
            if (!(Player.Get(gameObject) is Player player))
            {
                Destroy(this);
                return;
            }

            Player = player;
            config = Plugin.Instance.Config;
            corrosionHandler = new CorrosionHandler(this, config);
        }

        private void OnDestroy()
        {
            corrosionHandler.Destroy();
            Player.SessionVariables.Remove(SessionVariable);
        }

        private void Spawn(Player toReplace)
        {
            if (toReplace != null && Player != toReplace)
            {
            }
        }
    }
}