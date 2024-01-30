import dragonfly as df
import functools
from srabuilder import rules
import characters, locations, fishing_menu, title_menu, menu_utils, server, df_utils, game, container_menu, objective, constants, items
import stream

mouse_directions = {
    "up": "up",
    "right": "right",
    "down": "down",
    "left": "left",
}


async def get_objects_by_name(name: str, loc: str):
    objs = await game.get_location_objects()
    return [x for x in objs if x["name"] == name]


async def go_to_object(item: items.Item, index):
    obj_getter = functools.partial(get_objects_by_name, item.name)
    await game.navigate_nearest_tile(obj_getter, index=index)


async def move_and_face_previous_direction(direction: int, n: int):
    async with stream.player_status_stream() as pss:
        ps = await pss.next()
        await game.move_n_tiles(direction, n, stream)
        await game.face_direction(ps["facingDirection"], pss, move_cursor=True)


async def get_shipping_bin_tiles(item):
    tile = await server.request("SHIPPING_BIN_TILE")
    return game.break_into_pieces([tile])


async def go_to_shipping_bin():
    await game.navigate_nearest_tile(get_shipping_bin_tiles)
    await game.do_action()


async def get_bed_tile(item):
    tile = await server.request("BED_TILE")
    return [tile]


async def go_to_bed():
    await game.navigate_nearest_tile(get_bed_tile, pathfind_fn=game.pathfind_to_tile)


async def get_ladders_down(item):
    return await server.request("GET_LADDERS_DOWN")


async def ladder_down():
    await game.navigate_nearest_tile(get_ladders_down)
    await game.do_action()


async def navigate_direction(direction: int):
    async with stream.player_status_stream() as pss:
        player_status = await pss.next()
        location = player_status["location"]
        path_tiles = await server.request("PATH_TO_EDGE", {"direction": direction})
        if path_tiles:
            path = game.Path(path_tiles, location)
            await path.travel(pss)


async def pet_farm_pet():
    pass


numrep2 = df.Sequence(
    [df.Choice(None, rules.nonZeroDigitMap), df.Repetition(df.Choice(None, rules.digitMap), min=0, max=10)],
    name="n2",
)
num2 = df.Modifier(numrep2, rules.parse_numrep)

debris = {
    "(stones | rocks)": constants.STONE,
    "(wood | twigs)": constants.TWIG,
    "weeds": constants.WEEDS,
    "debris": "debris",
}

mapping = {
    "<direction_keys>": objective.objective_action(objective.HoldKeyObjective, "direction_keys"),
    "<direction_nums> <n>": objective.objective_action(objective.MoveNTilesObjective, "direction_nums", "n"),
    "start swinging [tool]": objective.objective_action(objective.HoldKeyObjective, constants.USE_TOOL_BUTTON),
    "[equip] item <positive_index>": df_utils.async_action(game.equip_item_by_index, "positive_index"),
    "equip [melee] weapon": df_utils.async_action(game.equip_melee_weapon),
    "equip <items>": df_utils.async_action(game.equip_item_by_name, "items"),
    "nearest <items> [<positive_index>]": objective.function_objective(go_to_object, "items", "positive_index"),
    "jump <direction_nums> [<positive_num>]": df_utils.async_action(
        move_and_face_previous_direction, "direction_nums", "positive_num"
    ),
    "go to bed": objective.function_objective(go_to_bed),
    "go to shipping bin": objective.function_objective(go_to_shipping_bin),
    "start shopping": objective.function_objective(objective.start_shopping),
    "water crops": objective.objective_action(objective.WaterCropsObjective),
    "harvest crops": objective.objective_action(objective.HarvestCropsObjective),
    "[open | read] (quests | journal | quest log)": df_utils.async_action(game.press_key, constants.JOURNAL_BUTTON),
    "face <direction_nums>": objective.objective_action(objective.FaceDirectionObjective, "direction_nums"),
    "halt": df_utils.async_action(server.stop_everything),
    "swing": df_utils.async_action(game.press_key, constants.USE_TOOL_BUTTON),
    "next toolbar": df_utils.async_action(game.press_key, constants.TOOLBAR_SWAP),
    "<points>": objective.function_objective(objective.move_to_point, "points"),
    "chop trees": objective.objective_action(objective.ChopTreesObjective),
    "start planting": objective.objective_action(objective.PlantSeedsOrFertilizerObjective),
    "clear <debris>": objective.objective_action(objective.ClearDebrisObjective, "debris"),
    "clear grass": objective.objective_action(objective.ClearGrassObjective),
    "(clear | mine) ore": objective.objective_action(objective.ClearOreObjective),
    "attack": objective.objective_action(objective.AttackObjective),
    "defend": objective.objective_action(objective.DefendObjective),
    "ladder down": objective.function_objective(ladder_down),
    "(hoe | dig) <n> by <n2>": objective.objective_action(objective.HoePlotObjective, "n", "n2"),
    "talk to <npcs>": objective.objective_action(objective.TalkToNPCObjective, "npcs"),
    "refill watering can": objective.function_objective(game.refill_watering_can),
    "gather crafting": objective.function_objective(game.gather_crafted_items),
    "forage": objective.function_objective(game.gather_forage_items),
    "gather (objects | items)": objective.function_objective(game.gather_objects),
    "dig (artifact | artifacts)": objective.function_objective(game.dig_artifacts),
    "go inside": objective.function_objective(game.go_inside),
    "go outside": objective.function_objective(game.go_outside),
    "pet animals": objective.function_objective(objective.pet_animals),
    "milk animals": objective.function_objective(objective.use_tool_on_animals, constants.MILK_PAIL),
    "start fishing": objective.function_objective(fishing_menu.start_fishing),
    "navigate <direction_nums>": objective.function_objective(navigate_direction, "direction_nums"),
}


@menu_utils.valid_menu_test
def is_active():
    return game.get_context_menu() is None


def load_grammar():
    grammar = df.Grammar("no_menu")
    main_rule = df.MappingRule(
        name="no_menu_rule",
        mapping=mapping,
        extras=[
            rules.num,
            df_utils.positive_index,
            df_utils.positive_num,
            df.Choice("npcs", characters.npcs),
            num2,
            df.Choice("direction_keys", game.direction_keys),
            df.Choice("direction_nums", game.direction_nums),
            df.Choice("debris", debris),
            items.items_choice,
            df.Choice("points", locations.commands(locations.points)),
        ],
        context=is_active,
        defaults={"n": 1, "positive_num": 1, "positive_index": 0},
    )
    grammar.add_rule(main_rule)
    grammar.load()
