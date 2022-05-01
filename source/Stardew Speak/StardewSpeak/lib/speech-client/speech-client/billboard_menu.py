import dragonfly as df
from srabuilder import rules
import title_menu, menu_utils, server, df_utils, game, container_menu, objective, server, constants

BILLBOARD = 'billboard'

async def get_billboard_menu():
    return await menu_utils.get_active_menu(BILLBOARD)

async def click_button(name):
    menu = await get_billboard_menu()
    await menu_utils.click_component(menu[name])

mapping = {
    "accept quest": df_utils.async_action(click_button, "acceptQuestButton"),
}

@menu_utils.valid_menu_test
def is_active():
    game.get_context_menu(BILLBOARD)

def load_grammar():
    grammar = df.Grammar("billboard_menu")
    main_rule = df.MappingRule(
        name="billboard_menu_rule",
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
    