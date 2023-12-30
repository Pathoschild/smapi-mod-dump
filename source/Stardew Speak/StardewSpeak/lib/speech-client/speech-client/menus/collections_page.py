import menu_utils, server, df_utils
from typing_extensions import TypedDict, Any

class CollectionsPage(TypedDict):
    tabs: list[Any]

tabs = df_utils.index_choice_from_list(
    "tabs",
    [
        "[items] shipped [farm and forage]",
        "fish",
        "artifacts",
        "minerals",
        "cooking",
        "achievements",
        "letters",
        "[secret] notes",
    ],
)


def get_collections_page(menu):
    from game_menu import game_menu

    menu_utils.validate_menu_type("gameMenu", menu)
    page = game_menu.get_page_by_name(menu, "collectionsPage")
    return page


async def click_side_tab(menu: CollectionsPage, idx: int):
    cmp = menu["tabs"][idx]
    await menu_utils.click_component(cmp)


mapping = {
    "<tabs>": df_utils.async_action(click_side_tab, "tabs"),
    "previous": menu_utils.simple_click("backButton"),
    "next": menu_utils.simple_click("forwardButton"),
}


def get_grammar():
    extras = [
        df_utils.positive_num,
        df_utils.positive_index,
        tabs,
    ]
    grammar = menu_utils.build_menu_grammar(mapping, get_collections_page, extras=extras)
    return grammar
