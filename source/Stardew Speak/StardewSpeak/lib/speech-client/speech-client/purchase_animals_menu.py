import dragonfly as df
from srabuilder import rules
import server
import title_menu, menu_utils, server, df_utils, game, container_menu, objective
from typing import Literal, Final

TITLE: Final = "purchaseAnimalsMenu"

class PurchaseAnimalsMenu(menu_utils.BaseMenu):
    title: Literal["purchaseAnimalsMenu"]
    animalsToPurchase: list[menu_utils.ClickableComponent]

animals = {
    "[a] chicken": 0,
    "[a] [dairy] cow": 1,
    "[a] goat": 2,
}


async def buy_animal(menu: PurchaseAnimalsMenu, animal_index: str):
    await menu_utils.click_component(menu["animalsToPurchase"][0])


mapping = {
    "ok": menu_utils.simple_click("doneNamingButton"),
    "cancel": menu_utils.simple_click("okButton"), # strange but using existing field names
    "random": menu_utils.simple_click("randomButton"),
    "(buy | purchase) <animals>": df_utils.async_action(buy_animal, "animals"),
    "pan <direction_keys>": objective.objective_action(objective.HoldKeyObjective, "direction_keys"),
}


def load_grammar():
    extras = [
        df.Choice("direction_keys", game.direction_keys),
        rules.num,
        df_utils.positive_index,
        df_utils.positive_num,
        df.Choice("animals", animals),
    ]
    defaults = {"positive_num": 1}
    grammar = menu_utils.build_menu_grammar(mapping, TITLE, extras=extras, defaults=defaults)
    grammar.load()
