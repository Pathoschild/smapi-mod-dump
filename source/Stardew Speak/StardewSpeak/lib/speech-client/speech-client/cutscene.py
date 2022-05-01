import dragonfly as df
import functools
from srabuilder import rules
import characters, locations, fishing_menu, title_menu, menu_utils, server, df_utils, game, container_menu, objective, constants, items

async def click_skip_btn():
    evt = game.get_context_value('GAME_EVENT')
    if 'skipBounds' in evt:
        await menu_utils.click_component(evt['skipBounds'])

mapping = {
    "skip [cutscene | event]": df_utils.async_action(click_skip_btn),
}

def is_active():
    evt = game.get_context_value('GAME_EVENT')
    return evt is not None and evt['skippable']

def load_grammar():
    grammar = df.Grammar("cutscene")
    main_rule = df.MappingRule(
        name="cutscene_rule",
        mapping=mapping,
        context=df.FuncContext(is_active),
    )
    grammar.add_rule(main_rule)
    grammar.load()
    