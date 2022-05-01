import dragonfly as df
from srabuilder import rules
import title_menu, menu_utils, server, df_utils, game, container_menu, objective, server, constants

CARPENTER_MENU = 'carpenterMenu'

async def get_carpenter_menu():
    return await menu_utils.get_active_menu(CARPENTER_MENU)

async def click_button(name):
    menu = await get_carpenter_menu()
    await menu_utils.click_component(menu[name])

async def move_cursor_tile(direction, amount):
    await game.move_mouse_in_direction(direction, amount * 64)

mapping = {
    "pan <direction_keys>": objective.objective_action(objective.HoldKeyObjective, "direction_keys"),
    "previous": df_utils.async_action(click_button, "backButton"),
    "cancel": df_utils.async_action(click_button, "cancelButton"),
    "demolish [buildings]": df_utils.async_action(click_button, "demolishButton"),
    "next": df_utils.async_action(click_button, "forwardButton"),
    "move [buildings]": df_utils.async_action(click_button, "moveButton"),
    "build": df_utils.async_action(click_button, "okButton"),
    "paint": df_utils.async_action(click_button, "paintButton"),
    "upgrade": df_utils.async_action(click_button, "upgradeIcon"),
}

@menu_utils.valid_menu_test
def is_active():
    game.get_context_menu(CARPENTER_MENU)

def load_grammar():
    grammar = df.Grammar("carpenter_menu")
    main_rule = df.MappingRule(
        name="carpenter_menu_rule",
        mapping=mapping,
        extras=[
            df.Choice("direction_keys", game.direction_keys),
            df.Choice("direction_nums", game.direction_nums),
            rules.num,
            df_utils.positive_index,
            df_utils.positive_num
        ],
        context=is_active,
        defaults={'positive_num': 1},
    )
    grammar.add_rule(main_rule)
    grammar.load()
    