import time
import functools
import math
import os
import os.path
import json
import collections
import contextlib
import asyncio
import server
import logger
import constants

import async_timeout
import events

last_faced_east_west = constants.WEST
last_faced_north_south = constants.SOUTH

direction_keys = {
    "north": [constants.MOVE_UP_BUTTON],
    "main": [constants.MOVE_UP_BUTTON, constants.MOVE_RIGHT_BUTTON],
    "east": [constants.MOVE_RIGHT_BUTTON],
    "floor": [constants.MOVE_RIGHT_BUTTON, constants.MOVE_DOWN_BUTTON],
    "south": [constants.MOVE_DOWN_BUTTON],
    "air": [constants.MOVE_DOWN_BUTTON, constants.MOVE_LEFT_BUTTON],
    "west": [constants.MOVE_LEFT_BUTTON],
    "wash": [constants.MOVE_LEFT_BUTTON, constants.MOVE_UP_BUTTON],
}
direction_nums = {
    "north": constants.NORTH,
    "east": constants.EAST,
    "south": constants.SOUTH,
    "west": constants.WEST,
}
nums_to_directions = {v: k for k, v in direction_nums.items()}
cardinal_directions = (constants.NORTH, constants.EAST, constants.SOUTH, constants.WEST)
directions_to_buttons = {d: direction_keys[nums_to_directions[d]][0] for d in cardinal_directions}
buttons_to_directions = {v: k for k, v in directions_to_buttons.items()}
cardinal_buttons = (
    constants.MOVE_UP_BUTTON,
    constants.MOVE_RIGHT_BUTTON,
    constants.MOVE_DOWN_BUTTON,
    constants.MOVE_LEFT_BUTTON,
)

tool_for_object = {
    constants.STONE: {"name": constants.PICKAXE, "level": 0},
    constants.MINE_ROCK: {"name": constants.PICKAXE, "level": 0},
    constants.TWIG: {"name": constants.AXE, "level": 0},
    constants.WEEDS: {"name": constants.SCYTHE, "level": 0},
    constants.BOULDER: {"name": constants.PICKAXE, "level": 2},
    constants.HOLLOW_LOG: {"name": constants.AXE, "level": 2},
    constants.STUMP: {"name": constants.AXE, "level": 1},
    constants.METEORITE: {"name": constants.PICKAXE, "level": 3},
}

DEBRIS = (constants.WEEDS, constants.TWIG, constants.STONE)

context_variables = {
    "ACTIVE_MENU": None,
    "GAME_EVENT": None,
}


async def update_held_buttons(to_hold=(), to_release=()):
    await server.request("UPDATE_HELD_BUTTONS", {"toHold": to_hold, "toRelease": to_release})
    await events.wait_for_update_ticked()


def update_held_buttons_nowait(to_hold=(), to_release=()):
    server.send_message("UPDATE_HELD_BUTTONS", {"toHold": to_hold, "toRelease": to_release})


class Path:
    def __init__(
        self,
        mod_path,
        location: str,
        stop_check=None,
        stop_moving_when_done=True,
        turn_threshold=0.07,
        last_tile_done_threshold=0.07,
    ):
        self._tiles = ()
        tiles = []
        self.tile_indices = {}
        for i, mod_tile in enumerate(mod_path):
            tile = (mod_tile["X"], mod_tile["Y"])
            tiles.append(tile)
            self.tile_indices[tile] = i
        self.tiles = tuple(tiles)
        self.location = location
        self.stop_check = stop_check
        self.stop_moving_when_done = stop_moving_when_done
        self.turn_threshold = turn_threshold
        self.last_tile_done_threshold = last_tile_done_threshold

    @property
    def tiles(self):
        return self._tiles

    @tiles.setter
    def tiles(self, new_tiles):
        assert new_tiles
        self._tiles = new_tiles

    def retarget(self, p):
        self.tiles = p.tiles
        self.tile_indices = p.tile_indices
        self.location = p.location

    async def travel(self, status_stream: server.Stream, next_location=None):
        is_done = False
        try:
            while not is_done:
                player_status = await status_stream.next()
                current_location = player_status["location"]
                if current_location != self.location:
                    if next_location == current_location:
                        break
                    raise NavigationFailed(f"Unexpected location {current_location}, pathfinding for {self.location}")
                try:
                    is_done = self.move_update(player_status)
                except KeyError as e:
                    target_x, target_y = self.tiles[-1]  # target can change so check whenever we need a new path
                    current_tiles = self.tiles
                    new_path = await path_to_tile(target_x, target_y, self.location)
                    if current_tiles == self.tiles:
                        self.retarget(new_path)
        finally:
            if self.stop_moving_when_done:
                await ensure_not_moving()

    def move_update(self, player_status):
        """Return False to continue, True when done"""
        if self.stop_check is not None and self.stop_check(player_status):
            return True
        current_tile = player_status["tileX"], player_status["tileY"]
        current_tile_index = self.tile_indices[current_tile]
        try:
            target_tile = self.tiles[current_tile_index + 1]
        except IndexError:
            # Last tile, all done!
            if player_status["isMoving"] and self.facing_tile_center(player_status, self.last_tile_done_threshold):
                return False
            return True
        direction_to_move = direction_from_tiles(current_tile, target_tile)
        # Rule out not moving, moving in the same direction as next tile, and moving in the opposite direction
        current_direction = player_status["facingDirection"]
        turn_coming = player_status["isMoving"] and abs(current_direction - direction_to_move) % 2 == 1
        if turn_coming and self.facing_tile_center(player_status, self.turn_threshold):
            return False
        start_moving([direction_to_move])

    def facing_tile_center(self, player_status, offset_threshold):
        """Keep moving towards center of tile before a turn for smoother pathfinding"""
        tile_size = 64  # TODO: get this info from the mod
        position = player_status["position"]
        tile_x, tile_y = player_status["tileX"], player_status["tileY"]
        # x rounds to the nearest tile, y rounds down unless above (or at?) .75, e.g. (21.68, 17.68) becomes (22, 17) and (21.44, 17.77) becomes (21, 18).
        # Normalize so greater than 0 means right/below the center and less than 0 means left/above
        x, y, = (
            position[0] / tile_size - tile_x,
            position[1] / tile_size - tile_y - 0.25,
        )
        assert -0.5 <= x <= 0.5
        assert -0.5 <= y <= 0.5
        current_direction = player_status["facingDirection"]
        # start turning when at least 43% into the tile
        offset_threshold = offset_threshold
        if current_direction == constants.NORTH:
            return y + offset_threshold <= 0
        if current_direction == constants.EAST:
            return x + offset_threshold <= 0
        if current_direction == constants.SOUTH:
            return y - offset_threshold >= 0
        if current_direction == constants.WEST:
            return x - offset_threshold >= 0
        return False


def distance_between_points(t1, t2):
    # pathfinding doesn't move diagonally for simplicity so just sum differences between x and y
    return abs(t1[0] - t2[0]) + abs(t1[1] - t2[1])


def distance_between_points_diagonal(p1, p2):
    x_score = abs(p1[0] - p2[0]) ** 2
    y_score = abs(p1[1] - p2[1]) ** 2
    return math.sqrt(x_score + y_score)


def score_objects_by_distance(start_tile, current_tile, obj_tile, start_weight=0.25, current_weight=0.75):
    assert start_weight + current_weight == 1
    distance_from_start = distance_between_points(start_tile, obj_tile)
    distance_from_current = distance_between_points(current_tile, obj_tile)
    return start_weight * distance_from_start + current_weight * distance_from_current


async def get_trees(location: str):
    terrain_features = await server.request("GET_TERRAIN_FEATURES", {"location": location})
    return [tf for tf in terrain_features if tf["type"] == "tree"]


async def get_grass(location: str):
    terrain_features = await server.request("GET_TERRAIN_FEATURES", {"location": location})
    return [tf for tf in terrain_features if tf["type"] == "grass"]


async def get_fully_grown_trees_and_stumps(location: str):
    trees = await get_trees(location)
    return [t for t in trees if t["stump"] or (t["growthStage"] >= 5 and not t["tapped"])]


async def get_hoe_dirt(location: str):
    hoe_dirt = await server.request("GET_HOE_DIRT", {"location": location})
    return hoe_dirt or []


async def get_location_objects(location: str):
    objects = await server.request(constants.GET_LOCATION_OBJECTS, {"location": location})
    return objects or []


async def get_resource_clumps(location: str):
    clumps = await server.request(constants.GET_RESOURCE_CLUMPS, {"location": location})
    return clumps


async def get_resource_clump_pieces(location: str):
    clumps = await get_resource_clumps(location)
    return break_into_pieces(clumps)


def break_into_pieces(items):
    pieces = []
    # break up resource clump like a boulder into one object for each tile
    for item in items:
        start_x, start_y = item["tileX"], item["tileY"]
        for x in range(item["width"]):
            for y in range(item["height"]):
                piece = {**item, "tileX": start_x + x, "tileY": start_y + y}
                pieces.append(piece)
    return pieces


async def get_diggable_tiles(test_tiles_set, location: str):
    test_tiles = [{"tileX": x, "tileY": y} for x, y in test_tiles_set]
    filtered = await server.request("GET_DIGGABLE_TILES", {"tiles": test_tiles})
    return filtered


async def gather_items_on_ground(radius):
    """
    Wood, coal, sap, stone etc.
    """
    async with server.player_status_stream() as stream:
        player_status = await stream.next()
        location = player_status["location"]
        start_tile = player_status["tileX"], player_status["tileY"]
        tile_blacklist = set([start_tile])
        while True:
            items_to_gather = collections.defaultdict(int)
            debris = await server.request("GET_DEBRIS", {"location": "location"})
            test_tiles_set = set()
            for item in debris:
                within_radius = distance_between_points(start_tile, (item["tileX"], item["tileY"])) < radius
                if within_radius:
                    debris_tile = item["tileX"], item["tileY"]
                    for tile in get_adjacent_tiles(debris_tile) + [debris_tile]:
                        items_to_gather[tile] += 1
                        if tile not in tile_blacklist:
                            test_tiles_set.add(tile)
            if not test_tiles_set:
                return
            player_status = await stream.next()
            current_tile = player_status["tileX"], player_status["tileY"]
            test_tiles = sort_test_tiles(test_tiles_set, start_tile, current_tile, items_to_gather)
            path, invalid = await pathfind_to_resource(test_tiles, location, stream, cutoff=250)
            if path is None:
                logger.trace(f"Unable to gather {len(test_tiles)} in radius {radius}")
                return
            for tile in path.tiles:
                tile_blacklist.add(tile)
            for tile in invalid:
                tile_blacklist.add(tile)


def sort_test_tiles(tiles, start_tile, current_tile, items_to_gather):
    sorted_tiles = []
    # get maxes for weighted average
    max_start_distance = -1
    max_current_distance = -1
    max_resources_on_tile = -1
    for tile in tiles:
        max_start_distance = max(max_start_distance, distance_between_points(start_tile, tile))
        max_current_distance = max(max_current_distance, distance_between_points(current_tile, tile))
        max_resources_on_tile = max(max_resources_on_tile, items_to_gather[tile])

    def score_tile(t):
        start_score = distance_between_points(start_tile, t) / max_start_distance
        current_score = distance_between_points(current_tile, t) / max_current_distance
        resources_score = items_to_gather[t] / max_resources_on_tile
        return 0.05 * start_score + 0.25 * current_score + 0.7 * resources_score

    return sorted(tiles, key=score_tile)


async def pathfind_to_resource(tiles, location, stream, cutoff=-1):
    path = None
    invalid = []
    for tile in tiles:
        try:
            path_to_take = await path_to_tile(tile[0], tile[1], location, cutoff=cutoff)
            await path_to_take.travel(stream)
        except NavigationFailed as e:
            invalid.append(tile)
        else:
            path = path_to_take
            break
    return path, invalid


async def move_to_location(location: str, stream: server.Stream):
    logger.debug(f"moving to {location}")
    await ensure_not_moving()
    route = await request_route(location)
    for i, location in enumerate(route[:-1]):
        next_location = route[i + 1]
        logger.debug(f"Getting path to next location {next_location}")
        await pathfind_to_next_location(next_location, stream)


async def request_route(location: str):
    route = await server.request("ROUTE", {"toLocation": location})
    if route is None:
        raise NavigationFailed(f"Cannot route to location {location}")
    return route


async def path_to_next_location(next_location: str, status_stream):
    player_status = await status_stream.next()
    location = player_status["location"]
    connections = await get_location_connections()
    connection_to_next_loc = [c for c in connections if c["TargetName"] == next_location]
    current_tile = await get_current_tile(status_stream)
    connection_to_next_loc.sort(key=lambda cn: distance_between_points(current_tile, (cn["X"], cn["Y"])))
    for lc in connection_to_next_loc:
        x, y, is_door = lc["X"], lc["Y"], lc["IsDoor"]
        try:
            if is_door:
                path = await path_to_adjacent(x, y)
                door_direction = direction_from_tiles(path.tiles[-1], (x, y))
            else:
                path = await path_to_tile(x, y, location)
                door_direction = None
        except NavigationFailed:
            continue
        return path, door_direction
    raise NavigationFailed(f"Cannot pathfind from {location} to {next_location}")


async def path_to_tile(x, y, location, cutoff=-1):
    path = await server.request("path_to_tile", {"x": x, "y": y, "location": location, "cutoff": cutoff})
    if path is None:
        raise NavigationFailed(f"Cannot pathfind to {x}, {y} at location {location}")
    return Path(path, location)


async def path_to_player(x, y, location, cutoff=-1):
    path = await server.request("PATH_TO_PLAYER", {"x": x, "y": y, "location": location, "cutoff": cutoff})
    if path is None:
        raise NavigationFailed(f"Cannot pathfind to player from {x}, {y} at location {location}")
    return Path(reversed(path), location)


async def pathfind_to_next_location(next_location: str, status_stream: server.Stream):
    path, door_direction = await path_to_next_location(next_location, status_stream)
    await path.travel(status_stream, next_location)
    if door_direction is not None:
        await face_direction(door_direction, status_stream, move_cursor=True)
        await do_action()


def get_adjacent_tiles(tile):
    x, y = tile
    return [(x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y)]


async def path_to_adjacent(x, y, tiles_from_target=1, cutoff=-1):
    resp = await server.request("PATH_TO_PLAYER", {"x": x, "y": y, "cutoff": cutoff})
    tiles, location = resp["tiles"], resp["location"]
    if tiles is None:
        raise NavigationFailed(f"Cannot pathfind to player from {x}, {y} at location {location}")
    return tiles_to_adjacent_path(tiles, location, tiles_from_target=tiles_from_target)


def tiles_to_adjacent_path(tiles, location, tiles_from_target=1):
    if tiles is None:
        raise NavigationFailed()
    assert tiles
    adj_tiles = tiles[:1] if len(tiles) <= tiles_from_target else reversed(tiles[tiles_from_target:])
    return Path(adj_tiles, location)


async def pathfind_to_adjacent(x, y, status_stream: server.Stream, tiles_from_target=1, cutoff=-1):
    path = await path_to_adjacent(x, y, tiles_from_target=tiles_from_target, cutoff=cutoff)
    await path.travel(status_stream)
    if path.tiles[-1] != (x, y):
        direction_to_face = direction_from_tiles(path.tiles[-1], (x, y))
        await face_direction(direction_to_face, status_stream)
    return path


def direction_from_tiles(tile, target_tile):
    x, y = tile
    target_x, target_y = target_tile
    if x == target_x and y > target_y:
        return constants.NORTH
    elif x < target_x and y == target_y:
        return constants.EAST
    elif x == target_x and y < target_y:
        return constants.SOUTH
    elif x > target_x and y == target_y:
        return constants.WEST
    raise ValueError(f"Could not extract direction from {tile} to {target_tile}")


def direction_from_positions(xy, target_xy):
    if xy == target_xy:
        return
    x, y = xy
    target_x, target_y = target_xy
    xdiff = target_x - x
    ydiff = target_y - y
    if abs(xdiff) > abs(ydiff):
        return constants.EAST if xdiff > 0 else constants.WEST
    return constants.SOUTH if ydiff > 0 else constants.NORTH


async def press_key(key: str):
    await server.request("PRESS_KEY", {"key": key})
    await events.wait_for_update_ticked()


@contextlib.asynccontextmanager
async def press_and_release(keys):
    keys = keys if isinstance(keys, (list, tuple)) else [keys]
    try:
        await update_held_buttons(to_hold=keys)
        yield
    except (BaseException, Exception) as e:
        raise e
    finally:
        await update_held_buttons(to_release=keys)


async def release_all_keys():
    return await server.request("RELEASE_ALL_KEYS")


def start_moving(directions):
    buttons_to_hold = []
    for d in directions:
        buttons_to_hold.append(direction_keys[nums_to_directions[d]][0])
    to_release = []
    for direction in cardinal_directions:
        name = nums_to_directions[direction]
        btn = direction_keys[name][0]
        if btn not in buttons_to_hold:
            to_release.append(btn)
    update_held_buttons_nowait(to_hold=buttons_to_hold, to_release=to_release)


def set_last_faced_direction(direction: int):
    global last_faced_east_west
    global last_faced_north_south
    if direction in (constants.NORTH, constants.SOUTH):
        last_faced_north_south = direction
    else:
        last_faced_east_west = direction


def stop_moving():
    to_release = cardinal_buttons
    update_held_buttons_nowait(to_release=to_release)


async def ensure_not_moving():
    stop_moving()
    await events.wait_for_update_ticked()

async def face_direction(direction: int, stream: server.Stream, move_cursor=False):
    await ensure_not_moving()
    status = await server.request("PLAYER_STATUS")
    if status["facingDirection"] != direction:
        btn = directions_to_buttons[direction]
        await press_key(btn)
        try:
            await stream.wait(lambda s: s["facingDirection"] == direction, timeout=0.1)
        except asyncio.TimeoutError:
            async with press_and_release(btn):
                await stream.wait(lambda s: s["facingDirection"] == direction, timeout=5)
    if move_cursor:
        player_status = await stream.next()
        current_tile = player_status["tileX"], player_status["tileY"]
        target_tile = next_tile(current_tile, direction)
        await set_mouse_position_on_tile(target_tile)


async def equip_item(predicate):
    matched_index = None
    row_size = 12
    with server.player_items_stream() as stream, server.async_timeout.timeout(5):
        while True:
            items_info = await stream.next()
            items = items_info["items"]
            for idx, inventory_item in enumerate(items):
                if inventory_item and predicate(inventory_item):
                    matched_index = idx
                    break
            if matched_index is None:
                return False
            else:
                if items_info["currentToolIndex"] == matched_index:
                    return True
                if matched_index >= row_size:
                    await press_key(constants.TOOLBAR_SWAP)
                    await asyncio.sleep(0.1)
                else:
                    await equip_item_by_index(matched_index)
                    return True


async def equip_melee_weapon():
    predicate = lambda x: x["type"] == constants.MELEE_WEAPON
    success = await equip_item(predicate)
    if not success:
        raise HUDMessageException(f"No melee weapons are currently available")


async def equip_item_by_name(item):
    import items

    name = item.name if isinstance(item, items.Item) else item
    predicate = lambda x: x["netName"] == name
    success = await equip_item(predicate)
    if not success:
        raise HUDMessageException(f"{name} is not in inventory")


async def equip_item_by_index(idx: int):
    assert 0 <= idx <= 11
    btn = f"inventorySlot{idx + 1}"
    await press_key(btn)


def show_hud_message(msg: str, msg_type: int):
    server.send_message("SHOW_HUD_MESSAGE", {"message": msg, "msgType": msg_type})


def closest_item_key(start_tile, current_tile, item, player_status):
    target_tile = item["tileX"], item["tileY"]
    return distance_between_points(current_tile, target_tile)


def generic_next_item_key(start_tile, current_tile, item, player_status):
    target_tile = item["tileX"], item["tileY"]
    score = score_objects_by_distance(start_tile, current_tile, target_tile)
    return score


def next_crop_key(start_tile, current_tile, target_tile, player_status):
    score = score_objects_by_distance(start_tile, current_tile, target_tile)
    if direction_from_tiles(current_tile, target_tile) == player_status["facingDirection"]:
        score -= 0.1
    return score


def next_debris_key(start_tile, current_tile, debris_obj, player_status):
    target_tile = debris_obj["tileX"], debris_obj["tileY"]
    score = score_objects_by_distance(start_tile, current_tile, target_tile)
    return score


def next_hoe_key(start_tile, current_tile, target_tile, player_status):
    score = score_objects_by_distance(start_tile, current_tile, target_tile)
    return score


async def get_tools():
    tools = {}
    async with server.player_items_stream(ticks=10) as stream:
        items_info = await stream.next()
        items = items_info["items"]
        for item in items:
            if item and item["isTool"]:
                tools[item["netName"]] = item
    return tools


async def swing_tool():
    with server.tool_status_stream(ticks=1) as tss:
        async with press_and_release(constants.USE_TOOL_BUTTON):
            await tss.wait(lambda t: t["inUse"], timeout=1)
        await tss.wait(lambda t: not t["inUse"], timeout=10)


async def do_action():
    await press_key(constants.ACTION_BUTTON)


async def pathfind_to_adjacent_tile_from_current(stream):
    player_status = await stream.next()
    location = player_status["location"]
    for x, y in get_adjacent_tiles((player_status["tileX"], player_status["tileY"])):
        try:
            return await pathfind_to_tile(x, y, stream)
        except NavigationFailed:
            continue
    raise NavigationFailed


async def navigate_tiles(
    get_items,
    sort_items=generic_next_item_key,
    pathfind_fn=pathfind_to_adjacent,
    items_ok=lambda prev, curr: True,
    allow_action_on_same_tile=True,
    index=None,
):
    import events

    async with server.player_status_stream() as stream:
        player_status = await stream.next()
        start_tile = player_status["tileX"], player_status["tileY"]
        previous_items = []
        while True:
            current_tile = player_status["tileX"], player_status["tileY"]
            items = await get_items(player_status["location"])
            if not items:
                return
            sorted_items = sorted(items, key=lambda t: sort_items(start_tile, current_tile, t, player_status))
            if index is not None:
                sorted_items = [sorted_items[index]]
            if not items_ok(previous_items, items):
                raise RuntimeError("Unable to modify current tile")
            previous_items = sorted_items
            item_path = None
            for item in sorted_items:
                item_tile = (item["tileX"], item["tileY"])
                if current_tile == item_tile and not allow_action_on_same_tile:
                    await pathfind_to_adjacent_tile_from_current(stream)
                    await face_tile(stream, item_tile)
                try:
                    item_path = await pathfind_fn(item["tileX"], item["tileY"], stream)
                except NavigationFailed:
                    pass
                else:
                    await set_mouse_position_on_tile(item_tile)
                    yield item
                    break
            if not item_path:
                return
            player_status = await stream.next()


async def navigate_nearest_tile(get_items, pathfind_fn=pathfind_to_adjacent, index=None):
    async for item in navigate_tiles(get_items, sort_items=closest_item_key, pathfind_fn=pathfind_fn, index=index):
        return item
    raise NavigationFailed


async def set_mouse_position_on_tile(tile):
    x, y = tile
    await server.request("SET_MOUSE_POSITION_ON_TILE", {"x": x, "y": y})
    await events.wait_for_update_ticked()


def is_debris(obj):
    return obj.get("name") in DEBRIS


def next_tile(current_tile, direction: int):
    x, y = current_tile
    if direction == constants.NORTH:
        return x, y - 1
    if direction == constants.EAST:
        return x + 1, y
    if direction == constants.SOUTH:
        return x, y + 1
    if direction == constants.WEST:
        return x - 1, y


async def chop_tree_and_gather_resources(tree):
    evt = events.wait_for_event("TERRAIN_FEATURE_LIST_CHANGED")
    tree_tile = tree["tileX"], tree["tileY"]
    async with press_and_release(constants.USE_TOOL_BUTTON), server.tool_status_stream(ticks=1) as tss:
        while not evt.done():
            tool_status = await tss.next()
            if (
                tool_status is None
                or tool_status["baseName"] != "Axe"
                or tree_tile != (tool_status["tileX"], tool_status["tileY"])
            ):
                return
    await gather_items_on_ground(10)


async def clear_object(obj, obj_getter, tool_name):
    obj_tile = obj["tileX"], obj["tileY"]
    async with server.tool_status_stream(ticks=1) as tss, press_and_release(constants.USE_TOOL_BUTTON):
        while True:
            clumps, tool_status = await asyncio.gather(obj_getter(""), tss.next())
            if obj_tile != (tool_status["tileX"], tool_status["tileY"]) or tool_name != tool_status["baseName"]:
                return
            target = None
            for c in clumps:
                if obj_tile == (c["tileX"], c["tileY"]):
                    target = c
                    break
            if not target:
                return
            await asyncio.sleep(0.1)


def find_character_by_name(name: str, characters):
    for char in characters:
        if char["name"] == name:
            return char
    raise NavigationFailed(f"{name} is not in the current location")


async def get_current_tile(stream: server.Stream):
    ps = await stream.next()
    current_tile = ps["tileX"], ps["tileY"]
    return current_tile


async def refill_watering_can():
    await equip_item_by_name(constants.WATERING_CAN)
    await navigate_nearest_tile(get_water_tiles)
    await equip_item_by_name(constants.WATERING_CAN)
    await swing_tool()


async def write_game_state():
    import menu_utils

    objs = await get_location_objects("")
    log(objs, "location_objects.json")
    hdt = await get_hoe_dirt("")
    log(hdt, "hoe_dirt.json")
    menu = await menu_utils.get_active_menu()
    log(menu, "menu.json")


async def get_ready_crafted(loc):
    objs = await get_location_objects(loc)
    ready_crafted = [x for x in objs if x["readyForHarvest"] and x["type"] == "Crafting"]
    return ready_crafted


def visible_wrapper(fn):
    seen_tiles = set()

    async def get_visible(loc):
        items = await fn(loc)
        for item in items:
            if item["isOnScreen"]:
                seen_tiles.add((item["tileX"], item["tileY"]))
        seen_or_visible_items = [x for x in items if (x["tileX"], x["tileY"]) in seen_tiles]
        return seen_or_visible_items

    return get_visible


async def get_forage_visible_items(loc):
    items = await get_location_objects(loc)
    return [x for x in items if x["canBeGrabbed"] and x["type"] == "Basic" and x["isForage"]]


async def get_visible_artifact_spots(loc):
    objs = await get_location_objects(loc)
    return [x for x in objs if x["name"] == "Artifact Spot"]


async def get_grabble_visible_objects(loc):
    objs = await get_location_objects(loc)
    filtered_objs = []
    for o in objs:
        if o["canBeGrabbed"] and o["type"] == "Basic" and o["category"] != 0:
            filtered_objs.append(o)
    return filtered_objs


def disallow_previous_item(previous, current):
    return not previous or previous == current


async def dig_artifacts():
    await equip_item_by_name(constants.HOE)
    getter = visible_wrapper(get_visible_artifact_spots)
    async for item in navigate_tiles(getter, generic_next_item_key, items_ok=disallow_previous_item):
        await equip_item_by_name(constants.HOE)
        await swing_tool()


async def gather_crafted_items():
    async for item in navigate_tiles(get_ready_crafted, generic_next_item_key, items_ok=disallow_previous_item):
        await do_action()


async def gather_forage_items():
    getter = visible_wrapper(get_forage_visible_items)
    async for item in navigate_tiles(getter, generic_next_item_key, items_ok=disallow_previous_item):
        await do_action()


async def gather_objects():
    getter = visible_wrapper(get_grabble_visible_objects)
    async for item in navigate_tiles(getter, generic_next_item_key, items_ok=disallow_previous_item):
        await do_action()


async def get_water_tiles(loc):
    tiles = await server.request("GET_WATER_TILES")
    return [{"tileX": x, "tileY": y} for (x, y) in tiles]


async def move_mouse_in_direction(direction: int, amount: int):
    dx, dy = 0, 0
    if direction == constants.NORTH:
        dy = -amount
    elif direction == constants.EAST:
        dx = amount
    elif direction == constants.SOUTH:
        dy = amount
    if direction == constants.WEST:
        dx = -amount
    await server.set_mouse_position_relative(dx, dy)


async def on_menu_changed(new_menu):
    await server.stop_everything()


def get_context_value(key):
    return context_variables[key]


def set_context_value(key, val):
    assert key in context_variables
    context_variables[key] = val


def set_context_menu(menu):
    context_variables["ACTIVE_MENU"] = menu


def get_context_menu(menu_type: str | None = None):
    import menu_utils

    menu = context_variables["ACTIVE_MENU"]
    if menu_type is not None:
        if menu is None:
            raise menu_utils.InvalidMenuOption(f"Expecting {menu_type}, got None")
        if menu["menuType"] != menu_type:
            raise menu_utils.InvalidMenuOption(f"Expecting {menu_type}, got {menu['menuType']}")
    return menu


async def get_location_connections():
    return await server.request("GET_LOCATION_CONNECTIONS")


async def get_location_buildings():
    return await server.request("GET_LOCATION_BUILDINGS")


async def go_inside():
    indoors_connections = [x for x in (await get_location_connections()) if not x["TargetIsOutdoors"]]
    if indoors_connections:
        with server.player_status_stream() as pss:
            current_tile = await get_current_tile(pss)
            indoors_connections.sort(key=lambda t: distance_between_points(current_tile, (t["X"], t["Y"])))
            await pathfind_to_next_location(indoors_connections[0]["TargetName"], pss)

async def go_outside():
    outdoor_connections = [x for x in (await get_location_connections()) if x["TargetIsOutdoors"]]
    if outdoor_connections:
        with server.player_status_stream() as pss:
            player_status = await pss.next()
            server.log(player_status)
            current_location = player_status["location"]
            if not current_location['TargetIsOutdoors']:
                current_tile = await get_current_tile(pss)
                outdoor_connections.sort(key=lambda t: distance_between_points(current_tile, (t["X"], t["Y"])))
                await pathfind_to_next_location(outdoor_connections[0]["TargetName"], pss)


async def get_animals(animals_stream, player_stream):
    animals, player_status = await asyncio.gather(animals_stream.next(), player_stream.next())
    player_tile = player_status["tileX"], player_status["tileY"]
    animals.sort(key=lambda x: distance_between_points(player_tile, (x["tileX"], x["tileY"])))
    return animals


async def use_tool_on_animal_by_name(name: str):
    did_use = await server.request("USE_TOOL_ON_ANIMAL_BY_NAME", {"name": name})
    async with server.tool_status_stream() as tss:
        await tss.wait(lambda t: not t["inUse"])
    return did_use


def log(obj, name):
    import __main__

    path = os.path.join(os.path.dirname(__main__.__file__), "..", "debug", name)
    with open(path, "w") as f:
        if isinstance(obj, str):
            f.write(obj)
        else:
            json.dump(obj, f, indent=4)


async def pet_animal_by_name(name: str):
    resp = await server.request("PET_ANIMAL_BY_NAME", {"name": name})


class NavigationFailed(Exception):
    pass


class MoveToCharacter:
    def __init__(self, get_character_builder: server.RequestBuilder, tiles_from_target=1, distance=75):
        self.get_character_builder = get_character_builder
        self.tiles_from_target = tiles_from_target
        self.distance = distance

    async def move(self):
        import objective

        tiles_from_target = self.tiles_from_target
        while True:
            npc = await self.get_character(None)
            if "pathTiles" in npc:
                async with server.player_status_stream() as travel_path_stream:
                    path = tiles_to_adjacent_path(
                        npc["pathTiles"], npc["location"], tiles_from_target=tiles_from_target
                    )
                    # don't share streams between tasks, otherwise they steal .next() from each other and cause resource starvation issues
                    pathfind_coro = path.travel(travel_path_stream)
                    pathfind_task_wrapper = objective.active_objective.add_task(pathfind_coro)
                    while not pathfind_task_wrapper.done:
                        npc = await self.get_character(npc)
                        if "pathTiles" in npc:
                            new_path = tiles_to_adjacent_path(
                                npc["pathTiles"], npc["location"], tiles_from_target=tiles_from_target
                            )
                            path.retarget(new_path)
                    if pathfind_task_wrapper.exception:
                        raise pathfind_task_wrapper.exception
            try:
                await self.move_directly_to_character(npc, threshold=self.distance)
            except asyncio.TimeoutError:
                tiles_from_target = 1
            else:
                break
        npc = await self.get_character(npc, get_path=False)
        npx_x, npc_y = npc["center"]
        await server.set_mouse_position(npx_x, npc_y, from_viewport=True)
        return npc

    async def get_character(self, current_target, get_path=True):
        """
        Doing pathfinding can be expensive so only do it the first time and when
        target moves tiles
        """
        req_data = {**self.get_character_builder.data, "target": current_target, "getPath": get_path}
        resp = await self.get_character_builder.request(req_data)
        if resp is None:
            raise NavigationFailed()
        return resp

    async def move_directly_to_character(self, target, threshold=75, timeout=4):

        req_data = {**self.get_character_builder.data, "target": target, "getPath": False}
        char_req_builder = server.RequestBuilder(self.get_character_builder.request_type, req_data)
        batched_builder = server.RequestBuilder.batch(server.RequestBuilder("PLAYER_STATUS"), char_req_builder)
        is_moving = False
        async with async_timeout.timeout(timeout):
            try:
                while True:
                    directions = []
                    player_status, character = await batched_builder.request()
                    assert character["name"] == target["name"]
                    req_data["target"] = character
                    player_pos, character_pos = player_status["center"], character["center"]
                    if distance_between_points_diagonal(player_pos, character_pos) < threshold:
                        return
                    px, py = player_pos
                    cx, cy = character_pos
                    xdiff = px - cx
                    if abs(xdiff) > 10:
                        direction = constants.WEST if xdiff > 0 else constants.EAST
                        directions.append(direction)
                    ydiff = py - cy
                    if abs(ydiff) > 10:
                        direction = constants.NORTH if ydiff > 0 else constants.SOUTH
                        directions.append(direction)
                    assert directions
                    start_moving(directions)
                    is_moving = True
            finally:
                if is_moving:
                    await ensure_not_moving()


async def face_tile(stream, tile):
    player_status = await stream.next()
    player_tile = player_status["tileX"], player_status["tileY"]
    direction_to_face = direction_from_tiles(player_tile, tile)
    await face_direction(direction_to_face, stream)


async def pathfind_to_tile(x, y, stream, cutoff=-1):
    status = await stream.next()
    loc = status["location"]
    path = await path_to_tile(x, y, loc, cutoff=cutoff)
    await path.travel(stream)
    return path


async def move_n_tiles(direction: int, n: int, stream):
    status = await get_player_status()
    await ensure_not_moving()
    from_x, from_y = status["tileX"], status["tileY"]
    to_x, to_y = from_x, from_y
    if direction == constants.NORTH:
        to_y -= n
    elif direction == constants.EAST:
        to_x += n
    elif direction == constants.SOUTH:
        to_y += n
    elif direction == constants.WEST:
        to_x -= n
    else:
        raise ValueError(f"Unexpected direction {direction}")
    path = await path_to_tile(to_x, to_y, status["location"])
    await path.travel(stream)


async def get_player_status():
    req_builder = server.RequestBuilder("PLAYER_STATUS")
    status = await req_builder.request()
    return status


class HUDMessageException(Exception):
    def __init__(self, message, level=2):
        super().__init__(message)
        show_hud_message(message, level)
