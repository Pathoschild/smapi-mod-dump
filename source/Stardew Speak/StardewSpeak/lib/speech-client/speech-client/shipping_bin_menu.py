import game, server, menu_utils, df_utils, items
from srabuilder import rules
import functools
import dragonfly as df

wrapper = menu_utils.InventoryMenuWrapper()

async def get_shipping_menu():
    menu = await menu_utils.get_active_menu(menu_type='itemsToGrabMenu')
    if not menu['shippingBin']:
        raise menu_utils.InvalidMenuOption()
    return menu

async def focus_item(new_row, new_col):
    menu = await get_shipping_menu()
    submenu = menu['inventoryMenu']
    await wrapper.focus_box(submenu, new_row, new_col)

mapping = {
    "item <positive_index>": df_utils.async_action(focus_item, None, 'positive_index'),
    "row <positive_index>": df_utils.async_action(focus_item, 'positive_index', None),
    "ok": df_utils.async_action(menu_utils.click_menu_button, 'okButton', get_shipping_menu),
    "undo": df_utils.async_action(menu_utils.click_menu_button, 'lastShippedHolder', get_shipping_menu),
}

@menu_utils.valid_menu_test
def is_active():
    menu = game.get_context_menu('itemsToGrabMenu')
    return menu['shippingBin']

def load_grammar():
    grammar = df.Grammar("shipping_bin_menu")
    main_rule = df.MappingRule(
        name="shipping_bin_menu_rule",
        mapping=mapping,
        extras=[rules.num, df_utils.positive_index],
        context=is_active
    )
    grammar.add_rule(main_rule)
    grammar.load()