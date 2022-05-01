import dragonfly as df
from srabuilder import rules
import menu_utils, server, df_utils, game, objective, server, constants

LEVEL_UP_MENU = 'shippingMenu'

categories_list = ('farming', 'foraging', 'fishing', 'mining', 'other')
categories = {category: i for i, category in enumerate(categories_list)}

async def click_category(menu, idx: int):
    cmp = menu['categories'][idx]
    if cmp['visible']:
        await menu_utils.click_component(cmp)

mapping = {
    "ok": menu_utils.simple_click("okButton"),
    '<category>': df_utils.async_action(click_category, 'category'),
    "(back | previous)": menu_utils.simple_click("backButton"),
    "(forward | next)": menu_utils.simple_click("forwardButton"),
}

def load_grammar():
    grammar = menu_utils.build_menu_grammar(mapping, LEVEL_UP_MENU, extras=[df.Choice('category', categories)])
    grammar.load()
    