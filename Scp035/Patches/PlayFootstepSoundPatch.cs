// -----------------------------------------------------------------------
// <copyright file="PlayFootstepSoundPatch.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Patches
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using Exiled.API.Features;
    using HarmonyLib;
    using Scp035.Components;

    /// <summary>
    /// Spawns the corrosion trail under a Scp035 instance.
    /// </summary>
    [HarmonyPatch(typeof(FootstepSync), nameof(FootstepSync.PlayFootstepSound))]
    internal static class PlayFootstepSoundPatch
    {
        private static readonly Dictionary<Player, int> StepCounter = new Dictionary<Player, int>();

        private static void Postfix(FootstepSync __instance)
        {
            if (!Plugin.Instance.Config.CorrodeTrail)
                return;

            Player player = Player.Get(__instance.gameObject);
            if (!Scp035Component.IsScp035(player))
                return;

            if (++StepCounter[player] >= Plugin.Instance.Config.CorrodeTrailInterval)
            {
                player.ReferenceHub.characterClassManager.RpcPlaceBlood(player.Position, 1, 2f);
                StepCounter[player] = 0;
            }
        }
    }
}