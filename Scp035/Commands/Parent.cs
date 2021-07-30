// -----------------------------------------------------------------------
// <copyright file="Parent.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Commands
{
#pragma warning disable SA1101
    using System;
    using System.Text;
    using CommandSystem;
    using NorthwoodLib.Pools;

    /// <summary>
    /// The command which all Scp035 commands are run off of.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Parent : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parent"/> class.
        /// </summary>
        public Parent() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command { get; } = "035";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public override string Description { get; } = "Parent command for Scp035";

        /// <inheritdoc/>
        public sealed override void LoadGeneratedCommands()
        {
            RegisterCommand(new Kill());
            RegisterCommand(new List());
            RegisterCommand(new Spawn());
            RegisterCommand(new SpawnItems());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder responseBuilder = StringBuilderPool.Shared.Rent();
            responseBuilder.AppendLine("Please enter a valid subcommand! Available:");
            foreach (ICommand command in AllCommands)
            {
                responseBuilder.AppendLine(command.Aliases.Length > 0
                    ? $"{command.Command} | Aliases: {string.Join(", ", command.Aliases)}"
                    : command.Command);
            }

            response = StringBuilderPool.Shared.ToStringReturn(responseBuilder).TrimEnd();
            return false;
        }
    }
}