import game, server, menu_utils, df_utils, items
from srabuilder import rules
import functools
import dragonfly as df

GEODE_MENU = "geodeMenu"

mapping = {
    **menu_utils.inventory_commands(),
    "(break | crack | process) geode": menu_utils.simple_click("geodeSpot"),
}


def load_grammar():
    extras = [df_utils.positive_index]
    grammar = menu_utils.build_menu_grammar(mapping, GEODE_MENU, extras=extras)
    grammar.load()
