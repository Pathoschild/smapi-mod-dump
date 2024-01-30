import re
import game, constants, server, objective
import dragonfly as df
import menu_utils
from typing import Iterable

DEFAULT_LOCATIONS = (
    "FarmHouse",
    "Farm",
    "FarmCave",
    "Town",
    "JoshHouse",
    "HaleyHouse",
    "SamHouse",
    "Blacksmith",
    "ManorHouse",
    "SeedShop",
    "Saloon",
    "Trailer",
    "Hospital",
    "HarveyRoom",
    "Beach",
    "ElliottHouse",
    "Mountain",
    "ScienceHouse",
    "SebastianRoom",
    "Tent",
    "Forest",
    "WizardHouse",
    "AnimalShop",
    "LeahHouse",
    "BusStop",
    "Mine",
    "Sewer",
    "BugLand",
    "Desert",
    "Club",
    "SandyHouse",
    "ArchaeologyHouse",
    "WizardHouseBasement",
    "AdventureGuild",
    "Woods",
    "Railroad",
    "WitchSwamp",
    "WitchHut",
    "WitchWarpCave",
    "Summit",
    "FishShop",
    "BathHouse_Entry",
    "BathHouse_MensLocker",
    "BathHouse_WomensLocker",
    "BathHouse_Pool",
    "CommunityCenter",
    "JojaMart",
    "Greenhouse",
    "SkullCave",
    "Backwoods",
    "Tunnel",
    "Trailer_Big",
    "Cellar",
    "Cellar2",
    "Cellar3",
    "Cellar4",
    "BeachNightMarket",
    "MermaidHouse",
    "Submarine",
    "AbandonedJojaMart",
    "MovieTheater",
    "Sunroom",
    "BoatTunnel",
    "IslandSouth",
    "IslandSouthEast",
    "IslandSouthEastCave",
    "IslandEast",
    "IslandWest",
    "IslandNorth",
    "IslandHut",
    "IslandWestCave1",
    "IslandNorthCave1",
    "IslandFieldOffice",
    "IslandFarmHouse",
    "CaptainRoom",
    "IslandShrine",
    "IslandFarmCave",
    "Caldera",
    "LeoTreeHouse",
    "QiNutRoom",
)

LOCATION_COMMANDS = {
    "AnimalShop": ["marnie's house", "animal shop"],
    "BathHouse_Entry": ["bath house"],
    "BathHouse_MensLocker": ["men's locker"],
    "BathHouse_WomensLocker": ["women's locker"],
    "BathHouse_Pool": ["bath house pool"],
    "Trailer_Big": ["trailer big this is a placeholder"],
    "Cellar": ["cellar one"],
    "Cellar2": ["cellar two"],
    "Cellar3": ["cellar three"],
    "Cellar4": ["cellar four"],
    "ElliottHouse": ["elliott's house"],
    "HaleyHouse": ["haley's house", "emily's house", "to willow lane"],
    "JoshHouse": ["(alex's | george's | evelyn's) house", "one river road"],
    "LeahHouse": ["leah's house"],
    "ArchaeologyHouse": ["library museum", "library", "museum", "archaeology house"],
    "ManorHouse": ["manor house", "[mayor] lewis' house"],
    "Mine": ["[the] (mine | mines)"],
    "SamHouse": ["(sam's | jodi's | kent's | vincent's) house", "one willow lane"],
    "Saloon": ["[stardrop] saloon"],
    "ScienceHouse": ["[the] science house", "[the] carpenter's house"],
    "SeedShop": [
        "[the] seed (shop | store)",
        "pierre's [general] (shop | store)",
        "[pierre's] general (shop | store)",
        "[the] general (shop | store)",
    ],
}

grammar = None
loaded_location_names = []


def get_locations():
    names = DEFAULT_LOCATIONS
    return names


class Location:
    def __init__(self, name: str, commands=None):
        self.name = name
        if commands is None:
            self.commands = self.commands_from_name(name)
        else:
            self.commands = commands

    def commands_from_name(self, name: str):
        # 'FarmCave' -> ['farm cave']
        capitals_split = re.findall("[A-Z][a-z]*", name)
        command = " ".join(capitals_split).lower()
        return [f"[the] {command}"]


class Point:
    def __init__(
        self,
        commands,
        tiles,
        location,
        pathfind_fn=game.pathfind_to_tile,
        facing_direction=None,
        on_arrival=None,
    ):
        self.commands = commands
        if not callable(tiles) and not isinstance(tiles[0], (list, tuple)):
            tiles = [tiles]
        self.tiles = tiles
        self.location = location
        self.pathfind_fn = pathfind_fn
        self.facing_direction = facing_direction
        self.on_arrival = on_arrival

    def commands_from_name(self, name: str):
        # 'FarmCave' -> ['farm cave']
        capitals_split = re.findall("[A-Z][a-z]*", name)
        command = " ".join(capitals_split).lower()
        return [f"[the] {command}"]

    async def get_tiles(self):
        if callable(self.tiles):
            return await self.tiles()
        return [{"tileX": x[0], "tileY": x[1]} for x in self.tiles]


async def get_elevator_tiles(item):
    tile = await server.request("GET_ELEVATOR_TILE")
    return [tile]


async def get_ladder_up_tiles(item):
    tile = await server.request("GET_LADDER_UP_TILE")
    return [tile]


points = (
    Point(
        ["go to mail box", "(check | read) mail"],
        (68, 16),
        "Farm",
        pathfind_fn=game.pathfind_to_adjacent,
        on_arrival=game.do_action,
    ),
    Point(
        ["buy backpack"],
        (7, 19),
        "SeedShop",
        facing_direction=constants.NORTH,
        on_arrival=game.do_action,
    ),
    Point(
        ["go to calendar"],
        (41, 57),
        "Town",
        facing_direction=constants.NORTH,
        on_arrival=game.do_action,
    ),
    Point(
        ["go to (billboard | bulletin board)"],
        (42, 57),
        "Town",
        facing_direction=constants.NORTH,
        on_arrival=game.do_action,
    ),
    Point(
        ["go to elevator"],
        get_elevator_tiles,
        re.compile(r"UndergroundMine\d+"),
        facing_direction=constants.NORTH,
        on_arrival=game.do_action,
    ),
    Point(
        ["[go to] ladder up"],
        get_ladder_up_tiles,
        re.compile(r"UndergroundMine\d+"),
        facing_direction=constants.NORTH,
        on_arrival=game.do_action,
    ),
)


def commands(locs: Iterable[Location]):

    commands: dict[str, Location] = {}
    for loc in locs:
        for cmd in loc.commands:
            if cmd in commands:
                raise ValueError(f"Duplicate location {cmd}")
            commands[cmd] = loc
    return commands


mapping = {
    "go to <locations>": objective.objective_action(
        objective.MoveToLocationObjective, "locations"
    ),
}


@menu_utils.valid_menu_test
def is_active():
    return game.get_context_menu() is None


def load_grammar():
    import df_utils
    from srabuilder import rules

    names = get_locations()
    locs = []
    for name in names:
        loc_commands = LOCATION_COMMANDS.get(name)
        locs.append(Location(name, loc_commands))
    grammar = df.Grammar("locations")
    main_rule = df.MappingRule(
        name="locations_rule",
        mapping=mapping,
        extras=[
            rules.num,
            df_utils.positive_index,
            df_utils.positive_num,
            df.Choice("locations", commands(locs)),
        ],
        context=is_active,
        defaults={"n": 1, "positive_num": 1, "positive_index": 0},
    )
    grammar.add_rule(main_rule)
    grammar.load()
