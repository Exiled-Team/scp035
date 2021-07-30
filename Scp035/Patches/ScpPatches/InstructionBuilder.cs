// -----------------------------------------------------------------------
// <copyright file="InstructionBuilder.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Patches.ScpPatches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using Scp035.Components;
    using UnityEngine;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Builds required instructions for similar transpilers.
    /// </summary>
    internal static class InstructionBuilder
    {
        /// <summary>
        /// Builds instructions to make SCP subjects respect the <see cref="Config.ScpFriendlyFire"/> config.
        /// </summary>
        /// <param name="instructions">The original method's instructions.</param>
        /// <param name="generator">An instance of the <see cref="ILGenerator"/> class.</param>
        /// <returns>The modified instructions.</returns>
        internal static IEnumerable<CodeInstruction> CheckAttackInstructions(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Get the index of the first instruction after the initial checks.
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + 1;

            // The label to attach to the first instruction after the initial checks.
            var continueLabel = generator.DefineLabel();
            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                // if (!Scp035Component.IsScp035(Player.Get(target)) goto cooldownLabel;
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(API), nameof(Scp035Component.IsScp035), new[] { typeof(Player) })),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),

                // if (Plugin.Instance.Config.ScpFriendlyFire) goto continueLabel;
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.ScpFriendlyFire))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),

                // return;
                new CodeInstruction(OpCodes.Ret),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}