/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.EasyAccess.Features;

using System;
using System.Collections.Generic;
using Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.EasyAccess.Enums;
using StardewMods.EasyAccess.Interfaces.Config;
using StardewMods.EasyAccess.Interfaces.ManagedObjects;
using StardewMods.FuryCore.Enums;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Interfaces.ClickableComponents;
using StardewMods.FuryCore.Models.ClickableComponents;
using StardewMods.FuryCore.Models.CustomEvents;
using StardewValley;

/// <inheritdoc />
internal class CollectOutputs : Feature
{
    private readonly PerScreen<IClickableComponent> _collectButton = new();
    private readonly Lazy<IHudComponents> _hudComponents;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CollectOutputs" /> class.
    /// </summary>
    /// <param name="config">The <see cref="IConfigData" /> for options set by the player.</param>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="services">Provides access to internal and external services.</param>
    public CollectOutputs(IConfigModel config, IModHelper helper, IModServices services)
        : base(config, helper, services)
    {
        this._hudComponents = services.Lazy<IHudComponents>();
    }

    /// <summary>
    ///     Gets a value indicating which producers are eligible for collecting from.
    /// </summary>
    public IEnumerable<IManagedProducer> EligibleProducers
    {
        get
        {
            IList<IManagedProducer> eligibleProducers = new List<IManagedProducer>();
            foreach (var ((location, (x, y)), producer) in this.ManagedObjects.Producers)
            {
                // Disabled in config or by location name
                if (producer.CollectOutputs == FeatureOptionRange.Disabled)
                {
                    continue;
                }

                switch (producer.CollectOutputs)
                {
                    // Disabled if not current location for location chest
                    case FeatureOptionRange.Location when !location.Equals(Game1.currentLocation):
                        continue;
                    case FeatureOptionRange.World:
                    case FeatureOptionRange.Location when producer.CollectOutputDistance == -1:
                    case FeatureOptionRange.Location when Utility.withinRadiusOfPlayer((int)x * 64, (int)y * 64, producer.CollectOutputDistance, Game1.player):
                        eligibleProducers.Add(producer);
                        continue;
                    case FeatureOptionRange.Default:
                    case FeatureOptionRange.Disabled:
                    default:
                        continue;
                }
            }

            return eligibleProducers;
        }
    }

    private IClickableComponent CollectButton
    {
        get => this._collectButton.Value ??= new CustomClickableComponent(
            new(
                new(0, 0, 32, 32),
                this.Helper.Content.Load<Texture2D>($"{EasyAccess.ModUniqueId}/Icons", ContentSource.GameContent),
                new(0, 0, 16, 16),
                2f)
            {
                name = "Collect Outputs",
                hoverText = I18n.Button_CollectOutputs_Name(),
            },
            ComponentArea.Right);
    }

    private IHudComponents HudComponents
    {
        get => this._hudComponents.Value;
    }

    /// <inheritdoc />
    protected override void Activate()
    {
        this.HudComponents.AddToolbarIcon(this.CollectButton);
        this.HudComponents.HudComponentPressed += this.OnHudComponentPressed;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        this.HudComponents.RemoveToolbarIcon(this.CollectButton);
        this.HudComponents.HudComponentPressed -= this.OnHudComponentPressed;
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;
    }

    private bool CollectItems()
    {
        var collectedAny = false;
        foreach (var producer in this.EligibleProducers)
        {
            if (producer.TryGetOutput(out var item))
            {
                Log.Info($"Collected {item.Name} from producer {producer.QualifiedItemId}.");
                collectedAny = true;
            }
        }

        if (collectedAny)
        {
            return true;
        }

        Game1.showRedMessage(I18n.Alert_CollectOutputs_NoEligible());
        return false;
    }

    private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (!this.Config.ControlScheme.CollectItems.JustPressed())
        {
            return;
        }

        if (Context.IsPlayerFree && this.CollectItems())
        {
            this.Helper.Input.SuppressActiveKeybinds(this.Config.ControlScheme.CollectItems);
        }
    }

    private void OnHudComponentPressed(object sender, ClickableComponentPressedEventArgs e)
    {
        if (ReferenceEquals(this.CollectButton, e.Component))
        {
            this.CollectItems();
            e.SuppressInput();
        }
    }
}