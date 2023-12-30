import asyncio
import functools
import dragonfly as df
from srabuilder import rules
import title_menu, menu_utils, server, df_utils, game, container_menu, objective, constants, carpenter_menu


def validate_any_menu(menu):
    if menu is None:
        return False


async def move_cursor_to_next_component(menu, direction, n=1):
    current_position = None
    target_components = []
    clickable = list(menu_utils.yield_clickable_components(menu))
    for cmp in menu_utils.yield_clickable_components(menu):
        center = cmp["center"]
        if cmp["containsMouse"]:
            current_position = cmp
        else:
            target_components.append(cmp)
    if not target_components:
        return
    if current_position is None:
        cx, cy = await server.get_mouse_position()
        current_position = {"center": (cx, cy), 'visible': True}
    if direction == constants.NORTH:
        direction_index, multiplier = 1, -1
    elif direction == constants.EAST:
        direction_index, multiplier = 0, 1
    elif direction == constants.SOUTH:
        direction_index, multiplier = 1, 1
    elif direction == constants.WEST:
        direction_index, multiplier = 0, -1
    for i in range(n):
        sort_key = functools.partial(sort_fn, current_position, direction_index, multiplier)
        res = min(target_components, key=sort_key)
        right_direction = sort_fn(current_position, direction_index, multiplier, res)[0] == 0
        if not right_direction:
            break
        current_position = res
    await menu_utils.focus_component(current_position)


def sort_fn(current_cmp, direction_index, multiplier, cmp):
    center = cmp["center"]
    current_center = current_cmp["center"]
    val, target_val = current_center[direction_index], center[direction_index]
    direction_diff = (target_val - val) * multiplier
    side_index = 0 if direction_index == 1 else 1
    side_diff = abs(current_center[side_index] - center[side_index])
    right_direction = 0 if direction_diff > 0 else 1
    return (right_direction, 0.1 * direction_diff + 0.9 * side_diff)


mapping = {
    "<direction_nums> [<positive_num>]": df_utils.async_action(
        move_cursor_to_next_component, "direction_nums", "positive_num"
    ),
}


def load_grammar():
    extras = [
        rules.num,
        df_utils.positive_index,
        df_utils.positive_num,
        df.Choice("direction_nums", game.direction_nums),
    ]
    grammar = menu_utils.build_menu_grammar(mapping, validate_any_menu, extras=extras)
    grammar.load()
