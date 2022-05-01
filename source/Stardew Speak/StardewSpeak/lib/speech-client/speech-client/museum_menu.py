import game, server, menu_utils, df_utils, items, objective
from srabuilder import rules
import functools
import dragonfly as df

MUSEUM_MENU = "museumMenu"

mapping = {
    **menu_utils.inventory_commands(),
    "pan <direction_keys>": objective.objective_action(
        objective.HoldKeyObjective, "direction_keys"
    ),
}


def load_grammar():
    extras = [
        df_utils.positive_index,
        df_utils.positive_num,
        df.Choice("direction_keys", game.direction_keys),
        df.Choice("direction_nums", game.direction_nums),
    ]
    grammar = menu_utils.build_menu_grammar(mapping, MUSEUM_MENU, extras=extras)
    grammar.load()
