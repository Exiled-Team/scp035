// -----------------------------------------------------------------------
// <copyright file="NotificationHandler.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Handlers
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using Mirror;
    using Scp035.Components;
    using UnityEngine;

    /// <summary>
    /// Handles the display of ranged notifications for all Scp035 players.
    /// </summary>
    public static class NotificationHandler
    {
        private static readonly Config Config;
        private static CoroutineHandle notificationHandle;

        static NotificationHandler() => Config = Plugin.Instance.Config;

        /// <summary>
        /// Starts the <see cref="RangedNotification"/> coroutine.
        /// </summary>
        public static void Start() => notificationHandle = Timing.RunCoroutine(RangedNotification());

        /// <summary>
        /// Kills the <see cref="RangedNotification"/> coroutine.
        /// </summary>
        public static void Stop() => Timing.KillCoroutines(notificationHandle);

        private static IEnumerator<float> RangedNotification()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(Config.RangedNotification.Interval);
                var broadcast = Config.RangedNotification.Notification;
                if (!broadcast.Show)
                    continue;

                foreach (var player in Player.List)
                    TryShowNotification(player, broadcast);
            }
        }

        private static void TryShowNotification(Player player, Broadcast broadcast)
        {
            Vector3 forward = player.CameraTransform.forward;
            if (!Physics.Raycast(player.CameraTransform.position + forward, forward, out var hit, Config.RangedNotification.MaximumRange))
                return;

            if (hit.distance < Config.RangedNotification.MinimumRange)
                return;

            GameObject parent = hit.collider.GetComponentInParent<NetworkIdentity>()?.gameObject;
            if (parent == null)
                return;

            if (!(Player.Get(parent) is Player target) || !Scp035Component.IsScp035(target))
                return;

            if (Config.RangedNotification.UseHints)
            {
                player.ShowHint(broadcast.Content, broadcast.Duration);
                return;
            }

            player.Broadcast(broadcast);
        }
    }
}