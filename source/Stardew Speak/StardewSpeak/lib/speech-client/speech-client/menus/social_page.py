import asyncio
import dragonfly as df
import menu_utils, server, df_utils, characters

PAGE_SIZE = 5


def get_social_page(menu):
    from game_menu import game_menu

    menu_utils.validate_menu_type("gameMenu", menu)
    page = game_menu.get_page_by_name(menu, "socialPage")
    return page


async def click_npc(menu, name: str):
    index = menu["names"].index(name) - menu["slotPosition"]
    if 0 <= index < PAGE_SIZE:
        await menu_utils.click_component(menu["characterSlots"][index])


mapping = {**menu_utils.scroll_commands(page_size=PAGE_SIZE), "<npcs>": df_utils.async_action(click_npc, "npcs")}


def get_grammar():
    extras = [
        df_utils.positive_num,
        df.Choice("npcs", characters.npcs),
        df_utils.positive_index,
        df_utils.dictation_rule(),
    ]
    grammar = menu_utils.build_menu_grammar(mapping, get_social_page, extras=extras)
    return grammar
