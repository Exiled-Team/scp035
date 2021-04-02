﻿// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using Scp035.EventHandlers;
    using MapHandlers = Exiled.Events.Handlers.Map;
    using PlayerHandlers = Exiled.Events.Handlers.Player;
    using Scp096Handlers = Exiled.Events.Handlers.Scp096;
    using Scp106Handlers = Exiled.Events.Handlers.Scp106;
    using ServerHandlers = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main class which inherits <see cref="Plugin{TConfig}"/>.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        private static readonly Plugin InstanceValue = new ();
        private static Harmony harmony;

        private Plugin()
        {
        }

        /// <summary>
        /// Gets an instance of <see cref="Plugin"/>.
        /// </summary>
        public static Plugin Instance { get; } = InstanceValue;

        /// <inheritdoc/>
        public override string Author { get; } = "Build, formerly by Cyanox";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new (2, 9, 4);

        /// <inheritdoc/>
        public override Version Version { get; } = new (2, 0, 1);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            SubscribeAll();
            harmony = new Harmony($"build.scp035.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnSubscribeAll();
            Methods.KillAllCoroutines();
            harmony.UnpatchAll(harmony.Id);
            base.OnDisabled();
        }

        private static void SubscribeAll()
        {
            MapHandlers.ExplodingGrenade += MapEvents.OnExplodingGrenade;

            PlayerHandlers.ChangingRole += PlayerEvents.OnChangingRole;
            PlayerHandlers.Destroying += PlayerEvents.OnDestroying;
            PlayerHandlers.Died += PlayerEvents.OnDied;
            PlayerHandlers.EnteringPocketDimension += PlayerEvents.OnEnteringPocketDimension;
            PlayerHandlers.Escaping += PlayerEvents.OnEscaping;
            PlayerHandlers.Hurting += PlayerEvents.OnHurting;
            PlayerHandlers.InsertingGeneratorTablet += PlayerEvents.OnInsertingGeneratorTablet;
            PlayerHandlers.MedicalItemUsed += PlayerEvents.OnMedicalItemUsed;
            PlayerHandlers.PickingUpItem += PlayerEvents.OnPickingUpItem;
            PlayerHandlers.Shooting += PlayerEvents.OnShooting;
            PlayerHandlers.UsingMedicalItem += PlayerEvents.OnUsingMedicalItem;

            Scp096Handlers.AddingTarget += Scp096Events.OnAddingTarget;

            Scp106Handlers.Containing += Scp106Events.OnContaining;

            if (Instance.Config.CheckWinConditions)
            {
                ServerHandlers.EndingRound += ServerEvents.OnEndingRound;
            }

            ServerHandlers.RoundStarted += ServerEvents.OnRoundStarted;
            ServerHandlers.WaitingForPlayers += ServerEvents.OnWaitingForPlayers;
        }

        private static void UnSubscribeAll()
        {
            MapHandlers.ExplodingGrenade -= MapEvents.OnExplodingGrenade;

            PlayerHandlers.ChangingRole -= PlayerEvents.OnChangingRole;
            PlayerHandlers.Destroying -= PlayerEvents.OnDestroying;
            PlayerHandlers.Died -= PlayerEvents.OnDied;
            PlayerHandlers.EnteringPocketDimension -= PlayerEvents.OnEnteringPocketDimension;
            PlayerHandlers.Escaping -= PlayerEvents.OnEscaping;
            PlayerHandlers.Hurting -= PlayerEvents.OnHurting;
            PlayerHandlers.InsertingGeneratorTablet -= PlayerEvents.OnInsertingGeneratorTablet;
            PlayerHandlers.MedicalItemUsed -= PlayerEvents.OnMedicalItemUsed;
            PlayerHandlers.PickingUpItem -= PlayerEvents.OnPickingUpItem;
            PlayerHandlers.Shooting -= PlayerEvents.OnShooting;
            PlayerHandlers.UsingMedicalItem -= PlayerEvents.OnUsingMedicalItem;

            Scp096Handlers.AddingTarget -= Scp096Events.OnAddingTarget;

            Scp106Handlers.Containing -= Scp106Events.OnContaining;

            ServerHandlers.EndingRound -= ServerEvents.OnEndingRound;
            ServerHandlers.RoundStarted -= ServerEvents.OnRoundStarted;
            ServerHandlers.WaitingForPlayers -= ServerEvents.OnWaitingForPlayers;
        }
    }
}