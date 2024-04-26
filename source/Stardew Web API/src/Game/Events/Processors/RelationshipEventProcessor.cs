/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewWebApi.Game.NPCs;
using StardewWebApi.Game.Players;
using StardewWebApi.Server;

namespace StardewWebApi.Game.Events.Processors;

internal record TrackedRelationship(
    int Points,
    bool IsDating,
    bool IsEngaged
);

internal record TrackedSpouse(
    string Name,
    bool IsRoommate
);

public record RelationshipChangedEventData(
    NPCStub NPC,
    int PreviousPoints,
    int NewPoints
)
{
    public int PreviousHearts => Relationship.GetHeartsFromPoints(PreviousPoints);
    public int NewHearts => Relationship.GetHeartsFromPoints(NewPoints);
}

public record PlayerStartedDatingEventData(
    NPCStub NPC
);

public record PlayerStoppedDatingEventData(
    NPCStub NPC
);

public record PlayerEngagedEventData(
    NPCStub NPC
);

public record PlayerNoLongerEngagedEventData(
    NPCStub NPC
);

public record PlayerMarriedEventData(
    NPCStub NPC,
    bool IsRoommate
);

public record PlayerDivorcedEventData(
    NPCStub NPC,
    bool WasRoommate
);

internal class RelationshipEventProcessor : IEventProcessor
{
    private readonly Dictionary<string, TrackedRelationship> _relationships = new();
    private TrackedSpouse? _spouse = null;

    public void Initialize() { }

    public void InitializeGameData()
    {
        RefreshFriendshipList();
    }

    public void ProcessEvents()
    {
        if (!Game1.hasLoadedGame)
        {
            return;
        }

        var increasedFriendships = new List<string>();
        var decreasedFriendships = new List<string>();

        foreach (var name in _relationships.Keys)
        {
            if (Game1.player.friendshipData[name].Points > _relationships[name].Points)
            {
                increasedFriendships.Add(name);
            }
            else if (Game1.player.friendshipData[name].Points < _relationships[name].Points)
            {
                decreasedFriendships.Add(name);
            }

            if (Game1.player.friendshipData[name].IsDating() != _relationships[name].IsDating)
            {
                TriggerDatingChanged(name);
            }

            if (Game1.player.friendshipData[name].IsEngaged() != _relationships[name].IsEngaged)
            {
                TriggerEngagementChanged(name);
            }
        }

        if (increasedFriendships.Count == 1)
        {
            TriggerFriendshipIncreased(increasedFriendships[0]);
        }
        else if (increasedFriendships.Count > 1)
        {
            TriggerMultipleFriendshipIncreased(increasedFriendships);
        }

        if (decreasedFriendships.Count == 1)
        {
            TriggerFriendshipDecreased(decreasedFriendships[0]);
        }
        else if (decreasedFriendships.Count > 1)
        {
            TriggerMultipleFriendshipDecreased(decreasedFriendships);
        }

        if (Game1.player.spouse != _spouse?.Name)
        {
            TriggerSpouseChanged();

            _spouse = Game1.player.spouse is not null
                ? new TrackedSpouse(Game1.player.spouse, Game1.player.hasRoommate())
                : null;
        }

        RefreshFriendshipList();
    }

    private void RefreshFriendshipList()
    {
        foreach (var name in Game1.player.friendshipData.Keys)
        {
            _relationships[name] = new(
                Game1.player.friendshipData[name].Points,
                Game1.player.friendshipData[name].IsDating(),
                Game1.player.friendshipData[name].IsEngaged()
            );
        }
    }

    private RelationshipChangedEventData BuildRelationshipChangeData(string name)
    {
        return new(
            NPCUtilities.GetNPCByName(name)!.CreateStub(),
            _relationships[name].Points,
            Game1.player.friendshipData[name].Points
        );
    }

    private IEnumerable<RelationshipChangedEventData> BuildRelationshipChangeData(IEnumerable<string> names) =>
        names.Select(n => BuildRelationshipChangeData(n));

    private void TriggerFriendshipIncreased(string name)
    {
        WebServer.Instance.SendGameEvent("FriendshipIncreased", BuildRelationshipChangeData(name));
    }

    private void TriggerMultipleFriendshipIncreased(IEnumerable<string> names)
    {
        WebServer.Instance.SendGameEvent("MultipleFriendshipsIncreased", BuildRelationshipChangeData(names));
    }

    private void TriggerFriendshipDecreased(string name)
    {
        WebServer.Instance.SendGameEvent("FriendshipDecreased", BuildRelationshipChangeData(name));
    }

    private void TriggerMultipleFriendshipDecreased(IEnumerable<string> names)
    {
        WebServer.Instance.SendGameEvent("MultipleFriendshipsDecreased", BuildRelationshipChangeData(names));
    }

    private void TriggerDatingChanged(string name)
    {
        if (Game1.player.friendshipData[name].IsDating())
        {
            WebServer.Instance.SendGameEvent("PlayerStartedDating", new PlayerStartedDatingEventData(
                NPCUtilities.GetNPCByName(name)!.CreateStub()
            ));
        }
        else
        {
            WebServer.Instance.SendGameEvent("PlayerStoppedDating", new PlayerStoppedDatingEventData(
                NPCUtilities.GetNPCByName(name)!.CreateStub()
            ));
        }
    }

    private void TriggerEngagementChanged(string name)
    {
        if (Game1.player.friendshipData[name].IsEngaged())
        {
            WebServer.Instance.SendGameEvent("PlayerEngaged", new PlayerEngagedEventData(
                NPCUtilities.GetNPCByName(name)!.CreateStub()
            ));
        }
        else
        {
            WebServer.Instance.SendGameEvent("PlayerNoLongerEngaged", new PlayerNoLongerEngagedEventData(
                NPCUtilities.GetNPCByName(name)!.CreateStub()
            ));
        }
    }

    private void TriggerSpouseChanged()
    {
        if (_spouse is null)
        {
            WebServer.Instance.SendGameEvent("PlayerMarried", new PlayerMarriedEventData(
                NPCUtilities.GetNPCByName(Game1.player.spouse)!.CreateStub(),
                Game1.player.hasRoommate()
            ));
        }
        else
        {
            WebServer.Instance.SendGameEvent("PlayerDivorced", new PlayerDivorcedEventData(
                NPCUtilities.GetNPCByName(_spouse.Name)!.CreateStub(),
                _spouse.IsRoommate
            ));
        }
    }
}