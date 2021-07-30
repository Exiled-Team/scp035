// -----------------------------------------------------------------------
// <copyright file="Methods.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using MEC;
    using Scp035.Components;
    using Scp035.Handlers;
    using UnityEngine;
    using Random = System.Random;

    /// <summary>
    /// Contains all methods and various tracking.
    /// </summary>
    public static class Methods
    {
        private static readonly Config Config = Plugin.Instance.Config;

        private static readonly Random Random = new Random();

        /// <summary>
        /// Gets all active coroutines.
        /// </summary>
        internal static List<CoroutineHandle> CoroutineHandles { get; } = new List<CoroutineHandle>();

        /// <summary>
        /// Gets all user ids that currently have friendly fire enabled.
        /// </summary>
        internal static List<string> FriendlyFireUsers { get; } = new List<string>();

        /// <summary>
        /// Gets all Scp035 item instances.
        /// </summary>
        internal static List<Pickup> ScpPickups { get; } = new List<Pickup>();

        /// <summary>
        /// Gets or sets a value indicating whether a Scp035 item instance can spawn.
        /// </summary>
        internal static bool IsRotating { get; set; }

        /// <summary>
        /// Kills all active coroutines.
        /// </summary>
        internal static void KillAllCoroutines()
        {
            NotificationHandler.Stop();
            for (int i = 0; i < CoroutineHandles.Count; i++)
                Timing.KillCoroutines(CoroutineHandles[i]);

            CoroutineHandles.Clear();
        }

        /// <summary>
        /// Grants tracked friendly fire to a user.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be given friendly fire.</param>
        internal static void GrantFf(Player player)
        {
            player.IsFriendlyFireEnabled = true;
            FriendlyFireUsers.Add(player.UserId);
        }

        /// <summary>
        /// Revokes tracked friendly fire from a user.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be removed from friendly fire.</param>
        internal static void RemoveFf(Player player)
        {
            player.IsFriendlyFireEnabled = false;
            FriendlyFireUsers.Remove(player.UserId);
        }

        /// <summary>
        /// Deletes all active Scp035 item instances.
        /// </summary>
        internal static void RemoveScpPickups()
        {
            foreach (var pickup in ScpPickups)
            {
                if (pickup == null)
                    continue;

                pickup.Locked = true;
                pickup.Delete();
            }

            ScpPickups.Clear();
        }

        /// <summary>
        /// Spawns Scp035 item instances.
        /// </summary>
        /// <param name="amount">The number of Scp035 items to spawn.</param>
        /// <returns>All spawned <see cref="Pickup"/> objects.</returns>
        internal static List<Pickup> SpawnPickups(int amount)
        {
            Log.Debug($"Running {nameof(SpawnPickups)}.", Config.Debug);
            RemoveScpPickups();

            List<Pickup> pickups = Pickup.Instances.Where(pickup => !API.IsScp035Item(pickup)).ToList();
            if (Warhead.IsDetonated)
            {
                pickups.RemoveAll(pickup => Map.FindParentRoom(pickup.gameObject).Type != RoomType.Surface);
            }

            List<Pickup> returnPickups = new List<Pickup>();
            for (int i = 0; i < amount; i++)
            {
                if (pickups.Count == 0)
                    return returnPickups;

                Pickup mimicAs = pickups[Random.Next(pickups.Count)];
                Transform transform = mimicAs.transform;
                Pickup scpPickup = Config.ItemSpawning
                    .PossibleItems[Random.Next(Config.ItemSpawning.PossibleItems.Length)]
                    .Spawn(0, transform.position, transform.rotation);

                Log.Debug($"Spawned Scp035 item with ItemType of {scpPickup.itemId} at {scpPickup.transform.position}", Config.Debug);
                ScpPickups.Add(scpPickup);
                returnPickups.Add(scpPickup);

                pickups.Remove(mimicAs);
            }

            return returnPickups;
        }

        /// <summary>
        /// A coroutine which loops the spawning of Scp035 item instances.
        /// </summary>
        /// <returns>A delay in seconds based on <see cref="Configs.ItemSpawning.RotateInterval"/>.</returns>
        internal static IEnumerator<float> RunSpawning()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(Config.ItemSpawning.RotateInterval);
                Log.Debug($"Running {nameof(RunSpawning)} loop.", Config.Debug);
                if (IsRotating)
                {
                    SpawnPickups(Config.ItemSpawning.InfectedItemCount);
                }
            }
        }

        /// <summary>
        /// A coroutine which deals damage to the host of Scp035 over time.
        /// </summary>
        /// <param name="player">The Scp035 instance to corrode.</param>
        /// <returns>A delay in seconds based on <see cref="Configs.CorrodeHost.Interval"/>.</returns>
        internal static IEnumerator<float> CorrodeHost(Player player)
        {
            while (player != null && Scp035Component.IsScp035(player))
            {
                yield return Timing.WaitForSeconds(Config.CorrodeHost.Interval);
                Log.Debug($"Running {nameof(CorrodeHost)} loop.", Config.Debug);
                player.Hurt(Config.CorrodeHost.Damage, DamageTypes.Poison, player.Nickname, player.Id);
            }
        }
    }
}