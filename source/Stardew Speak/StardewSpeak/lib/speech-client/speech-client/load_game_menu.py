import asyncio
import dragonfly as df
import title_menu, menu_utils, server, df_utils, game, letters

LOAD_GAME_MENU = "loadGameMenu"


def validate_load_game_menu(menu):
    return title_menu.get_submenu(menu, LOAD_GAME_MENU)


async def load_game(menu, game_idx: int):
    button_index = game_idx
    try:
        btn = menu["slotButtons"][button_index]
    except IndexError:
        return
    await menu_utils.click_component(btn)


async def delete_game(menu, game_idx: int):
    button_index = game_idx
    try:
        btn = menu["deleteButtons"][button_index]
    except IndexError:
        return
    await menu_utils.click_component(btn)


mapping = {
    "[go] back": menu_utils.simple_click("backButton"),
    "(yes | ok)": menu_utils.simple_click("okDeleteButton"),
    "(no | cancel)": menu_utils.simple_click("cancelDeleteButton"),
    "(load [game] | [load] game) <positive_index>": df_utils.async_action(
        load_game, "positive_index"
    ),
    "delete [game] <positive_index>": df_utils.async_action(
        delete_game, "positive_index"
    ),
    **menu_utils.scroll_commands(),
}


def load_grammar():
    grammar = menu_utils.build_menu_grammar(
        mapping,
        validate_load_game_menu,
        extras=[df_utils.positive_index, df_utils.positive_num],
    )
    grammar.load()
