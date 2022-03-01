/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.EasyAccess.Models.ManagedObjects;

using System.Collections.Generic;
using System.Linq;
using StardewMods.EasyAccess.Enums;
using StardewMods.EasyAccess.Interfaces.Config;

/// <inheritdoc />
internal class ProducerModel : IProducerData
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProducerModel" /> class.
    /// </summary>
    /// <param name="producerData"><see cref="IProducerData" /> representing this producer type.</param>
    /// <param name="defaultProducer"><see cref="IProducerData" /> representing the default producer.</param>
    public ProducerModel(IProducerData producerData, IProducerData defaultProducer)
    {
        this.Data = producerData;
        this.DefaultProducer = defaultProducer;
    }

    /// <inheritdoc />
    public int CollectOutputDistance
    {
        get
        {
            if (this.Data.CollectOutputDistance != 0)
            {
                return this.Data.CollectOutputDistance;
            }

            return this.DefaultProducer.CollectOutputDistance == 0
                ? -1
                : this.DefaultProducer.CollectOutputDistance;
        }
        set => this.Data.CollectOutputDistance = value;
    }

    /// <inheritdoc />
    public HashSet<string> CollectOutputItems
    {
        get => this.Data.CollectOutputItems.Any()
            ? this.Data.CollectOutputItems
            : this.DefaultProducer.CollectOutputItems;
        set => this.Data.CollectOutputItems = value;
    }

    /// <inheritdoc />
    public FeatureOptionRange CollectOutputs
    {
        get => this.DefaultProducer.CollectOutputs switch
        {
            FeatureOptionRange.Disabled => FeatureOptionRange.Disabled,
            _ when this.Data.CollectOutputs is not FeatureOptionRange.Default => this.Data.CollectOutputs,
            FeatureOptionRange.Default => FeatureOptionRange.Disabled,
            _ => this.DefaultProducer.CollectOutputs,
        };
        set => this.Data.CollectOutputs = value;
    }

    /// <inheritdoc />
    public int DispenseInputDistance
    {
        get
        {
            if (this.Data.DispenseInputDistance != 0)
            {
                return this.Data.DispenseInputDistance;
            }

            return this.DefaultProducer.DispenseInputDistance == 0
                ? -1
                : this.DefaultProducer.DispenseInputDistance;
        }
        set => this.Data.DispenseInputDistance = value;
    }

    /// <inheritdoc />
    public HashSet<string> DispenseInputItems
    {
        get => this.Data.DispenseInputItems.Any()
            ? this.Data.DispenseInputItems
            : this.DefaultProducer.DispenseInputItems;
        set => this.Data.DispenseInputItems = value;
    }

    /// <inheritdoc />
    public int DispenseInputPriority
    {
        get => this.Data.DispenseInputPriority != 0 ? this.Data.DispenseInputPriority : this.DefaultProducer.DispenseInputPriority;
        set => this.Data.DispenseInputPriority = value;
    }

    /// <inheritdoc />
    public FeatureOptionRange DispenseInputs
    {
        get => this.DefaultProducer.DispenseInputs switch
        {
            FeatureOptionRange.Disabled => FeatureOptionRange.Disabled,
            _ when this.Data.DispenseInputs is not FeatureOptionRange.Default => this.Data.DispenseInputs,
            FeatureOptionRange.Default => FeatureOptionRange.Disabled,
            _ => this.DefaultProducer.DispenseInputs,
        };
        set => this.Data.DispenseInputs = value;
    }

    private IProducerData Data { get; }

    private IProducerData DefaultProducer { get; }
}