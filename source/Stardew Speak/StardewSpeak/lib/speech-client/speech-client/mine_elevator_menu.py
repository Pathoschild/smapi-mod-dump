import dragonfly as df
from srabuilder import rules
import menu_utils, server, df_utils, game, objective, server, constants, letters

MINE_ELEVATOR_MENU = 'mineElevatorMenu'

floors_map = {
    "zero": 0,
    "five": 5,
    "(ten | one zero)": 10,
    "(one five | fifteen)": 15,
    "(twenty | too zero)": 20,
    "(twenty | too) five": 25,
    "(thirty | three zero)": 30,
    "(thirty | three) five": 35,
    "(forty | four zero)": 40,
    "(forty | four) five": 45,
    "(fifty | five zero)": 50,
    "(fifty | five) five": 55,
    "(sixty | six zero)": 60,
    "(sixty | six) five": 65,
    "(seventy | seven zero)": 70,
    "(seventy | seven) five": 75,
    "(eighty | eight zero)": 80,
    "(eighty | eight) five": 85,
    "(ninety | nine zero)": 90,
    "(ninety | nine) five": 95,
    "one (hundred | zero zero)": 100,
    "one (hundred [and] | (oh | zero)) five": 105,
    "one [hundred [and]] (ten | one zero)": 110,
    "one [hundred [and]] (fifteen | one five)": 115,
    "one [hundred [and]] (twenty | too zero)": 120,
}

async def select_floor(menu, floor: int):
    floor_index = floor // 5
    await menu_utils.click_component(menu['elevators'][floor_index])

mapping = {
    "[floor | level] <mine_elevator_floors>": df_utils.async_action(select_floor, 'mine_elevator_floors'),
}

def load_grammar():
    extras = [df.Choice("mine_elevator_floors", floors_map)]
    grammar = menu_utils.build_menu_grammar(mapping, MINE_ELEVATOR_MENU, extras=extras)
    grammar.load()
    