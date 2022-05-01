import game, menu_utils, df_utils
from srabuilder import rules
import dragonfly as df

item_grab = {
    'inventoryMenu': menu_utils.InventoryMenuWrapper(),
    'itemsToGrabMenu': menu_utils.InventoryMenuWrapper(),
}

def get_wrapper(menu):
    submenu_name = 'itemsToGrabMenu' if menu['itemsToGrabMenu']['containsMouse'] else 'inventoryMenu'
    submenu = menu[submenu_name]
    return item_grab[submenu_name]

async def click_button(name):
    menu = await get_container_menu()
    await menu_utils.click_component(menu[name])

async def get_container_menu():
    menu = await menu_utils.get_active_menu(menu_type='itemsToGrabMenu')
    if menu['shippingBin']:
        raise menu_utils.InvalidMenuOption()
    return menu

async def set_item_grab_submenu(submenu_name: str):
    assert submenu_name in ('inventoryMenu', 'itemsToGrabMenu')
    menu = await menu_utils.get_active_menu('itemsToGrabMenu')
    submenu = menu[submenu_name]
    if submenu['containsMouse']:
        return
    menu_wrapper = item_grab[submenu_name]
    await menu_wrapper.focus_previous(submenu)

async def focus_item(new_row, new_col):
    menu = await get_container_menu()
    submenu_name = 'itemsToGrabMenu' if menu['itemsToGrabMenu']['containsMouse'] else 'inventoryMenu'
    submenu = menu[submenu_name]
    submenu_wrapper = item_grab[submenu_name]
    await submenu_wrapper.focus_box(submenu, new_row, new_col)

async def click_range(start, end):
    menu = await get_container_menu()
    submenu = menu['inventoryMenu']
    submenu_wrapper = item_grab['inventoryMenu']
    await submenu_wrapper.click_range(submenu, start, end)

mapping = {
    "deposit <positive_index>": df_utils.async_action(click_range, "positive_index", None),
    "deposit <positive_index> through <positive_index2>": df_utils.async_action(click_range, "positive_index", 'positive_index2'),
    "item <positive_index>": df_utils.async_action(focus_item, None, 'positive_index'),
    "row <positive_index>": df_utils.async_action(focus_item, 'positive_index', None),
    "backpack": df_utils.async_action(set_item_grab_submenu, 'inventoryMenu'),
    "container": df_utils.async_action(set_item_grab_submenu, 'itemsToGrabMenu'),
    "ok": df_utils.async_action(click_button, 'okButton'),
    "(trash | garbage) can": df_utils.async_action(click_button, 'trashCan'),
    "[add to] existing stacks": df_utils.async_action(click_button, 'fillStacksButton'),
    "[toggle] color picker": df_utils.async_action(click_button, 'colorPickerToggleButton'),
    "organize": df_utils.async_action(click_button, 'organizeButton'),
    "community center": df_utils.async_action(click_button, 'junimoNoteIcon'),
}

@menu_utils.valid_menu_test
def is_active():
    menu = game.get_context_menu('itemsToGrabMenu')
    return not menu['shippingBin']

def load_grammar():
    grammar = df.Grammar("items_to_grab_menu")
    main_rule = df.MappingRule(
        name="items_to_grab_menu_rule",
        mapping=mapping,
        extras=[df_utils.positive_index, df_utils.positive_index2],
        context=is_active
    )
    grammar.add_rule(main_rule)
    grammar.load()