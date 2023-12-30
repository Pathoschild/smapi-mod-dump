/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Features.Tracker;

using Utils;
using static Utils.MovementHelpers;
using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class TileTrackerBase
{

    public SortedList<string, Dictionary<string, SpecialObject>> Objects = new();

    public TileTrackerBase(object? arg = null)
    {
        FindObjects(arg);
    }

    public virtual void FindObjects(object? arg = null)
    {
            
    }

    public Boolean HasObjects()
    {
        return Objects.Any();
    }

    public SortedList<string, Dictionary<string, SpecialObject>> GetObjects()
    {
        return Objects;
    }

    public void AddFocusableObject(string category, string name, Vector2 tile, NPC? character = null)
    {

        if (!Objects.ContainsKey(category)) {
            Objects.Add(category, new());
        }

        SpecialObject sObject = new(name, tile);

        if(character != null) {
            sObject.character = character;
        }

        if(Objects[category].ContainsKey(name)) {
            sObject = GetClosest(sObject, Objects[category][name]);
        }

        Objects[category][name] = sObject;

    }

    public static SpecialObject GetClosest(SpecialObject item1, SpecialObject item2)
    {

        Vector2 player_tile = Game1.player.getTileLocation();

        double collide_distance = GetDistance(player_tile, item2.TileLocation);
        double new_distance = GetDistance(player_tile, item1.TileLocation);

        if (new_distance < collide_distance) {
            return item1;
        }
        return item2;
    }

}