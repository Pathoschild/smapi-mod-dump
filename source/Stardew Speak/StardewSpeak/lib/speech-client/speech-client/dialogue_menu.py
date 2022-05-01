import os
import dragonfly as df
from srabuilder import rules
import title_menu, menu_utils, server, df_utils, game, container_menu, approximate_matching

DIALOGUE_BOX = 'dialogueBox'

async def do_dictation(text):
    menu = await get_dialogue_menu()
    responses = [x['responseText'] for x in menu['responses']]
    top_index = approximate_matching.do_match(text, responses)
    if top_index is not None:
        await menu_utils.click_component(menu['responseCC'][top_index])

async def get_dialogue_menu():
    return await menu_utils.get_active_menu(DIALOGUE_BOX)

async def focus_item(idx):
    menu = await get_dialogue_menu()
    await menu_utils.click_component(menu['responseCC'][idx])

mapping = {
    "(item | response) <positive_index>": df_utils.async_action(focus_item, 'positive_index'),
    "<dictation>": df_utils.async_action(do_dictation, 'dictation')
}

@menu_utils.valid_menu_test
def is_active():
    game.get_context_menu(DIALOGUE_BOX)

def load_grammar():
    grammar = df.Grammar("dialogue_menu")
    main_rule = df.MappingRule(
        name="dialogue_menu_rule",
        mapping=mapping,
        extras=[rules.num, df_utils.positive_index, df_utils.positive_num, df_utils.dictation_rule()],
        context=is_active,
        defaults={'positive_num': 1},
    )
    grammar.add_rule(main_rule)
    grammar.load()
    