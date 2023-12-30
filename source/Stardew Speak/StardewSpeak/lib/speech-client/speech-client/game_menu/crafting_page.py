import asyncio
import dragonfly as df
import title_menu, menu_utils, server, df_utils, game, letters, items, server, approximate_matching
from game_menu import game_menu


def get_crafting_page(menu):
    menu_utils.validate_menu_type("gameMenu", menu)
    page = game_menu.get_page_by_name(menu, "craftingPage")
    return page


async def focus_item(page, item):
    for cmp, serialized_item in page["currentRecipePage"]:
        if item.name == serialized_item["name"]:
            await menu_utils.focus_component(cmp)
            return True
    return False


async def focus_item_dictation(page, text):
    items_on_page = [x[1]["name"] for x in page["currentRecipePage"]]
    best_idx = approximate_matching.do_match(str(text), items_on_page)
    if best_idx is not None:
        cmp = page["currentRecipePage"][best_idx][0]
        await menu_utils.focus_component(cmp)


mapping = {
    "<craftable_items>": df_utils.async_action(focus_item, "craftable_items"),
    "scroll up [<positive_num>]": df_utils.async_action(
        menu_utils.scroll_up, "positive_num"
    ),
    "scroll down [<positive_num>]": df_utils.async_action(
        menu_utils.scroll_down, "positive_num"
    ),
    **menu_utils.inventory_commands(),
    "<dictation>": df_utils.async_action(focus_item_dictation, "dictation"),
}


def load_grammar():
    grammar = df.Grammar("crafting_page")
    extras = [
        df_utils.positive_num,
        df_utils.positive_index,
        items.craftable_items_choice,
        df_utils.dictation_rule(),
    ]
    grammar = menu_utils.build_menu_grammar(mapping, get_crafting_page, extras=extras)
    grammar.load()
