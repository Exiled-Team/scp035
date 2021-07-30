// -----------------------------------------------------------------------
// <copyright file="CorrosionHandler.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Handlers
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using MEC;
    using Scp035.Components;
    using UnityEngine;

    /// <summary>
    /// Handles the corrosion functionality for a <see cref="Scp035Component"/>.
    /// </summary>
    public class CorrosionHandler
    {
        private readonly Config config;
        private readonly Scp035Component scp035;
        private CoroutineHandle corrosionHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrosionHandler"/> class.
        /// </summary>
        /// <param name="scp035Component">The <see cref="Scp035Component"/> instance this is to be attached to.</param>
        /// <param name="config">An instance of the <see cref="Config"/> class.</param>
        public CorrosionHandler(Scp035Component scp035Component, Config config)
        {
            this.config = config;
            scp035 = scp035Component;
            RefreshCorrosion();
            Exiled.Events.Handlers.Server.ReloadedConfigs += RefreshCorrosion;
        }

        /// <summary>
        /// Kills the <see cref="CorrodePlayers"/> coroutine.
        /// </summary>
        public void Destroy()
        {
            TryStopCorrosion();
            Exiled.Events.Handlers.Server.ReloadedConfigs -= RefreshCorrosion;
        }

        private void RefreshCorrosion()
        {
            if (config.CorrodePlayers.IsEnabled)
            {
                TryStartCorrosion();
                return;
            }

            TryStopCorrosion();
        }

        private void TryStartCorrosion()
        {
            if (corrosionHandle.IsRunning)
                return;

            corrosionHandle = Timing.RunCoroutine(CorrodePlayers());
        }

        private void TryStopCorrosion()
        {
            if (corrosionHandle.IsRunning)
                Timing.KillCoroutines(corrosionHandle);
        }

        private IEnumerator<float> CorrodePlayers()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(config.CorrodePlayers.Interval);

                foreach (var player in GetValidPlayers())
                {
                    if (Vector3.Distance(scp035.Player.Position, player.Position) <= config.CorrodePlayers.Distance)
                    {
                        CorrodePlayer(player);
                    }
                }
            }
        }

        private IEnumerable<Player> GetValidPlayers()
        {
            foreach (var player in Player.List)
            {
                if ((!config.ScpFriendlyFire && player.IsScp)
                    || (!config.TutorialFriendlyFire && player.Role == RoleType.Tutorial)
                    || Scp035Component.IsScp035(player)
                    || player.IsDead)
                    continue;

                yield return player;
            }
        }

        private void CorrodePlayer(Player player)
        {
            float damage = config.CorrodePlayers.Damage;
            Player scp035Player = scp035.Player;
            player.Hurt(damage, DamageTypes.Poison, scp035Player.Nickname, scp035Player.Id);
            if (config.CorrodePlayers.LifeSteal)
                scp035Player.ReferenceHub.playerStats.HealHPAmount(damage);
        }
    }
}