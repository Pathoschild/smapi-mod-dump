import dragonfly as df
import constants, server, game, df_utils

mouse_directions = {
    "up": constants.NORTH,
    "right": constants.EAST,
    "down": constants.SOUTH,
    "left": constants.WEST,
}


async def move_mouse_by_tile(direction, n):
    await game.move_mouse_in_direction(direction, n * 64)


non_repeat_mapping = {
    "click [<positive_num>]": df_utils.async_action(server.mouse_click, "left", "positive_num"),
    "right click [<positive_num>]": df_utils.async_action(server.mouse_click, "right", "positive_num"),
    "mouse <mouse_directions> [<positive_num>]": df_utils.async_action(
        move_mouse_by_tile, "mouse_directions", "positive_num"
    ),
    "small mouse <mouse_directions> [<positive_num>]": df_utils.async_action(
        game.move_mouse_in_direction, "mouse_directions", "positive_num"
    ),
    "write game state": df_utils.async_action(game.write_game_state),
    "(action | check)": df_utils.async_action(game.press_key, constants.ACTION_BUTTON),
    "(escape | [open | close] menu)": df_utils.async_action(game.press_key, constants.MENU_BUTTON),
    "hold mouse": df_utils.async_action(server.mouse_hold),
    "release mouse": df_utils.async_action(server.mouse_release),
}


def is_active():
    return True


def load_grammar():
    grammar = df.Grammar("any_context")
    main_rule = df.MappingRule(
        name="any_context_rule",
        mapping=non_repeat_mapping,
        extras=[
            df_utils.positive_num,
            df.Choice("mouse_directions", mouse_directions),
        ],
        context=df.FuncContext(is_active),
        defaults={"positive_num": 1},
    )
    grammar.add_rule(main_rule)
    grammar.load()
