/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons;

#region using directives

using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;

#endregion using directives

/// <summary>Represents one of the five heroic virtues.</summary>
public sealed class Virtue : SmartEnum<Virtue>
{
    #region enum values

    /// <summary>
    ///     Honor cannot be purchased. Honor also cannot be sold, for its value is greater than all the treasure in the world.
    ///     Yet one can lose it, and whoever does so shall have sullied his name for all eternity. A truly honorable man always
    ///     stands behind his actions, faces every challenge and refuses to lie.
    /// </summary>
    public static readonly Virtue Honor = new("Honor", 0);

    /// <summary>
    ///     There are many traits that bear witness to a man's true nature. Compassion is what separates men from beasts.
    ///     Whoever feels sympathy for his fellow man will never turn a blind eye to misfortune. He will always stand in
    ///     defense of the wronged.
    /// </summary>
    public static readonly Virtue Compassion = new("Compassion", 1);

    /// <summary>
    ///     Wisdom is a virtue which one should strive to cultivate throughout one's life, for it is impossible to be so wise
    ///     one cannot become even wiser. The wise know this... As we journey through life, we should seek to make wise choices
    ///     Remember, wise choices are not those which make our lives easier or simpler. Often, they make them more complicated.
    ///     But always, they make us better.
    /// </summary>
    public static readonly Virtue Wisdom = new("Wisdom", 2);

    /// <summary>
    ///     No man can be called good who does not share his prosperity with others. Generosity is required for dignity
    ///     in life and peace in death.
    /// </summary>
    public static readonly Virtue Generosity = new("Generosity", 3);

    /// <summary>
    ///     Valor does not make one good, yet how many good men have you met in your life's journey who were cowards?
    ///     Those who posses valor do not hesitate to stand against the majority, no matter what the consequences.
    /// </summary>
    public static readonly Virtue Valor = new("Valor", 4);

    #endregion enum values

    /// <summary>Initializes a new instance of the <see cref="Virtue"/> class.</summary>
    /// <param name="name">The name of the virtue.</param>
    /// <param name="value">The ID of the associated quest.</param>
    private Virtue(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>Gets the localized display name for this <see cref="Virtue"/>.</summary>
    internal string DisplayName
    {
        get
        {
            var text = string.Empty;
            this
                .When(Honor).Then(() => text = I18n.Get("virtues.honor.name"))
                .When(Compassion).Then(() => text = I18n.Get("virtues.compassion.name"))
                .When(Wisdom).Then(() => text = I18n.Get("virtues.wisdom.name"))
                .When(Generosity).Then(() => text = I18n.Get("virtues.generosity.name"))
                .When(Valor).Then(() => text = I18n.Get("virtues.valor.name"));
            return text;
        }
    }

    /// <summary>Gets the localized flavor inscription text for this <see cref="Virtue"/>.</summary>
    internal string FlavorText
    {
        get
        {
            var text = string.Empty;
            this
                .When(Honor).Then(() => text = I18n.Get("virtues.honor.flavor"))
                .When(Compassion).Then(() => text = I18n.Get("virtues.compassion.flavor"))
                .When(Wisdom).Then(() => text = I18n.Get("virtues.wisdom.flavor"))
                .When(Generosity).Then(() => text = I18n.Get("virtues.generosity.flavor"))
                .When(Valor).Then(() => text = I18n.Get("virtues.valor.flavor"));
            return text;
        }
    }

    /// <summary>Gets the threshold required to consider this <see cref="Virtue"/> as proven.</summary>
    internal int ProvenCondition
    {
        get
        {
            var target = int.MaxValue;
            switch (WeaponsModule.Config.VirtueTrialTrialDifficulty)
            {
                case Config.TrialDifficulty.Easy:
                    this
                        .When(Honor).Then(() => target = 1)
                        .When(Compassion).Then(() => target = 1)
                        .When(Wisdom).Then(() => target = 1)
                        .When(Generosity).Then(() => target = (int)1e4)
                        .When(Valor).Then(() => target = 2);
                    break;
                case Config.TrialDifficulty.Medium:
                    this
                        .When(Honor).Then(() => target = 3)
                        .When(Compassion).Then(() => target = 3)
                        .When(Wisdom).Then(() => target = 3)
                        .When(Generosity).Then(() => target = (int)1e5)
                        .When(Valor).Then(() => target = 5);
                    break;
                case Config.TrialDifficulty.Hard:
                    this
                        .When(Honor).Then(() => target = 5)
                        .When(Compassion).Then(() => target = 5)
                        .When(Wisdom).Then(() => target = 5)
                        .When(Generosity).Then(() => target = (int)5e5)
                        .When(Valor).Then(() => target = 10);
                    break;
            }

            return target;
        }
    }

    /// <summary>Checks if the <paramref name="farmer"/> has met the conditions for all virtues.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if all five virtue's conditions have been met, otherwise <see langword="false"/>.</returns>
    internal static bool AllProven(Farmer? farmer)
    {
        farmer ??= Game1.player;
        return List.All(virtue => virtue.Proven(farmer));
    }

    /// <summary>Checks if the <paramref name="farmer"/> has met the condition for this <see cref="Virtue"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the virtue's condition has been met, otherwise <see langword="false"/>.</returns>
    internal bool Proven(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        return this.GetProgress(farmer) >= this.ProvenCondition;
    }

    /// <summary>Gets the <paramref name="farmer"/>'s progress towards proving this <see cref="Virtue"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The current integer progress towards proving this <see cref="Virtue"/>.</returns>
    internal int GetProgress(Farmer? farmer = null)
    {
        farmer ??= Game1.player;
        var progress = 0;
        this
            .When(Honor).Then(() => progress = farmer.Read<int>(DataKeys.ProvenHonor))
            .When(Compassion).Then(() => progress = farmer.Read<int>(DataKeys.ProvenCompassion))
            .When(Wisdom).Then(() => progress = farmer.Read<int>(DataKeys.ProvenWisdom))
            .When(Generosity).Then(() => progress = farmer.Read<int>(DataKeys.ProvenGenerosity))
            .When(Valor).Then(() => progress = farmer.NumMonsterSlayerQuestsCompleted());
        return progress;
    }
}
