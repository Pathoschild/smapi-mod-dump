import asyncio
import dragonfly as df
import title_menu, menu_utils, server, df_utils, game, letters, items, server
from game_menu import game_menu

async def get_exit_page():
    menu = await menu_utils.get_active_menu('gameMenu')
    page = game_menu.get_page_by_name(menu, 'exitPage')
    return page

async def click_button(btn):
    page = await get_exit_page()
    await menu_utils.click_component(page[btn])

mapping = {
    "exit to desktop": df_utils.async_action(click_button, 'exitToDesktop'),
    "exit to title": df_utils.async_action(click_button, 'exitToTitle'),
}

@menu_utils.valid_menu_test
def is_active():
    menu = game.get_context_menu('gameMenu')
    game_menu.get_page_by_name(menu, 'exitPage')

def load_grammar():
    grammar = df.Grammar("exit_page")
    main_rule = df.MappingRule(
        name="exit_page_rule",
        mapping=mapping,
        extras=[
            df_utils.positive_num,
        ],
        defaults={'positive_num': 1},
        context=is_active,
    )
    grammar.add_rule(main_rule)
    grammar.load()
