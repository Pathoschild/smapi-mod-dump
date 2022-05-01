import dragonfly as df
from srabuilder import rules
import menu_utils, server, df_utils, game, objective, server, constants

LETTER_VIEWER_MENU = "letterViewerMenu"

mapping = {
    "accept [quest]": menu_utils.simple_click("acceptQuestButton"),
    "previous": menu_utils.simple_click("backButton"),
    "next": menu_utils.simple_click("forwardButton"),
}


def load_grammar():
    grammar = menu_utils.build_menu_grammar(mapping, LETTER_VIEWER_MENU)
    grammar.load()
