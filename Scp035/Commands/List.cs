// -----------------------------------------------------------------------
// <copyright file="List.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Commands
{
    using System;
    using System.Text;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;

    /// <summary>
    /// A command which lists all active Scp035 instances.
    /// </summary>
    public class List : ICommand
    {
        private const string RequiredPermission = "035.list";

        /// <inheritdoc/>
        public string Command { get; } = "list";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public string Description { get; } = "Lists all active Scp035 instances.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(RequiredPermission))
            {
                response = $"Insufficient permission. Required: {RequiredPermission}";
                return false;
            }

            StringBuilder responseBuilder = StringBuilderPool.Shared.Rent();
            responseBuilder.AppendLine("Alive Scp035 Instances:");
            foreach (var player in API.AllScp035)
            {
                responseBuilder.AppendLine($"{player.Nickname} [{player.Id}]");
            }

            response = StringBuilderPool.Shared.ToStringReturn(responseBuilder).TrimEnd();
            return true;
        }
    }
}