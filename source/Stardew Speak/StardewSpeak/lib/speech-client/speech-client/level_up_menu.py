import dragonfly as df
from srabuilder import rules
import menu_utils, server, df_utils, game, objective, server, constants

LEVEL_UP_MENU = "levelUpMenu"

mapping = {
    "ok": menu_utils.simple_click("okButton"),
}


def load_grammar():
    grammar = menu_utils.build_menu_grammar(mapping, LEVEL_UP_MENU)
    grammar.load()
