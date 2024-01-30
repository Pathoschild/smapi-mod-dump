import dragonfly as df
from srabuilder import rules
import title_menu, menu_utils, server, df_utils, game, container_menu

prev_for_sale_index = 0
inventory_wrapper = menu_utils.InventoryMenuWrapper()


async def focus_menu_section(menu, submenu_name: str):
    assert submenu_name in ("inventory", "forSale")
    for_sale_focused = any(x["containsMouse"] for x in menu["forSaleButtons"])
    if submenu_name == "forSale" and not for_sale_focused:
        await focus_for_sale_index(menu, prev_for_sale_index)
    elif submenu_name == "inventory":
        await inventory_wrapper.focus_previous(menu["inventory"])


async def focus_item(menu, idx, key):
    inventory = menu["inventory"]
    if not inventory["containsMouse"] and key == "item":
        await focus_for_sale_index(menu, idx)
        return
    row, col = (idx, None) if key == "row" else (None, idx)
    await inventory_wrapper.focus_box(inventory, row, col)


async def focus_for_sale_index(menu, idx: int):
    global prev_for_sale_index
    buttons = menu["forSaleButtons"]
    await menu_utils.focus_component(buttons[idx])
    prev_for_sale_index = 0


async def buy_item_index(menu, idx: int):
    buttons = menu["forSaleButtons"]
    await menu_utils.focus_component(buttons[idx])


async def buy_item(menu, n: int):
    await server.mouse_click(count=n)


async def click_range(menu, start, end):
    await inventory_wrapper.click_range(menu["inventory"], start, end)


mapping = {
    "sell <positive_index> [through <positive_index2>]": df_utils.async_action(
        click_range, "positive_index", "positive_index2"
    ),
    "item <positive_index>": df_utils.async_action(
        focus_item, "positive_index", "item"
    ),
    "row <positive_index>": df_utils.async_action(focus_item, "positive_index", "row"),
    "(shop | for sale)": df_utils.async_action(focus_menu_section, "forSale"),
    "backpack | (player items)": df_utils.async_action(focus_menu_section, "inventory"),
    "buy [<positive_num>]": df_utils.async_action(buy_item, "positive_num"),
    **menu_utils.scroll_commands(),
}


def load_grammar():
    extras = [
        rules.num,
        df_utils.positive_index,
        df_utils.positive_num,
        df_utils.positive_index2,
    ]
    defaults = {"positive_num": 1, "positive_index2": None}
    grammar = menu_utils.build_menu_grammar(
        mapping, "shopMenu", extras=extras, defaults=defaults
    )
    grammar.load()
