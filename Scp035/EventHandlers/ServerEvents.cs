// -----------------------------------------------------------------------
// <copyright file="ServerEvents.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.EventHandlers
{
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using Scp035.Components;
    using Scp035.Handlers;

    /// <summary>
    /// All event handlers which use <see cref="Exiled.Events.Handlers.Server"/>.
    /// </summary>
    public static class ServerEvents
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        internal static void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!API.AllScp035.Any())
                return;

            var teams = from player in Player.List where !Scp035Component.IsScp035(player) select player.Team;
            if (teams.All(team => (team == Team.TUT && Plugin.Instance.Config.WinWithTutorial) || team == Team.SCP || team == Team.RIP))
            {
                ev.LeadingTeam = LeadingTeam.Anomalies;
                ev.IsRoundEnded = true;
                return;
            }

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        internal static void OnRoundStarted()
        {
            Methods.IsRotating = true;
            Methods.ScpPickups.Clear();

            Methods.CoroutineHandles.Add(Timing.RunCoroutine(Methods.RunSpawning()));

            if (Plugin.Instance.Config.RangedNotification.IsEnabled)
            {
                NotificationHandler.Start();
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        internal static void OnWaitingForPlayers()
        {
            Methods.KillAllCoroutines();
        }
    }
}