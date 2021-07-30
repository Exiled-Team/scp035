// -----------------------------------------------------------------------
// <copyright file="Scp035Item.cs" company="Build and Cyanox">
// Copyright (c) Build and Cyanox. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp035.Items
{
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Events.EventArgs;

    /// <inheritdoc />
    public class Scp035Item : CustomItem
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = 35;

        /// <inheritdoc />
        public override string Name { get; set; } = "Scp035";

        /// <inheritdoc />
        public override string Description { get; set; } = "The item that, when picked up, converts a player to Scp035.";

        /// <inheritdoc/>
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        /// <inheritdoc />
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            base.SubscribeEvents();
        }

        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            base.UnsubscribeEvents();
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
        }
    }
}