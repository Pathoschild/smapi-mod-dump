import game, server, menu_utils, df_utils, items
from srabuilder import rules
import functools
import dragonfly as df

QUEST_LOG_MENU = "questLogMenu"


async def focus_quest(menu, n):
    quest = menu["questLogButtons"][n]
    await menu_utils.click_component(quest)


mapping = {
    "previous": menu_utils.simple_click("backButton"),
    "next": menu_utils.simple_click("forwardButton"),
    "cancel [quest]": menu_utils.simple_click("cancelQuestButton"),
    "scroll up": menu_utils.simple_click("upArrow"),
    "scroll down": menu_utils.simple_click("downArrow"),
    "[collect] (reward | rewards)": menu_utils.simple_click("rewardBox"),
    "(item | quest) <positive_index>": df_utils.async_action(
        focus_quest, "positive_index"
    ),
}


def load_grammar():
    extras = [df_utils.positive_index]
    grammar = menu_utils.build_menu_grammar(mapping, QUEST_LOG_MENU, extras=extras)
    grammar.load()
