/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace HelpForHire;

using Chores;
using Common.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

public class ModEntry : Mod
{
    internal ServiceLocator ServiceLocator { get; private set; }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // Init
        Log.Init(this.Monitor);
        this.ServiceLocator = new(this.Helper, this.ModManifest);
        this.ServiceLocator.Create(
            new[]
            {
                typeof(FeedAnimals),
                typeof(FeedPet),
                typeof(PetAnimals),
                typeof(RepairFences),
                typeof(WaterCrops),
                typeof(WaterSlimes),
            });

        // Events
        this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;
    }

    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        foreach (var chore in this.ServiceLocator.GetAll<GenericChore>().Where(chore => chore.IsActive && chore.IsPossible))
        {
            chore.PerformChore();
        }
    }
}