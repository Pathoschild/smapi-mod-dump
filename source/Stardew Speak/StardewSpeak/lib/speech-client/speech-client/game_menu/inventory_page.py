import dragonfly as df
import title_menu, menu_utils, server, df_utils, game, letters, items, server
from game_menu import game_menu

inventory_wrapper = menu_utils.InventoryMenuWrapper()

def get_inventory_page(menu):
    page = game_menu.get_page_by_name(menu, 'inventoryPage')
    return page

async def focus_item(page, new_row, new_col):
    menu = page['inventory']
    await inventory_wrapper.focus_box(menu, new_row, new_col)

async def click_equipment_icon(page, item):
    cmp = menu_utils.find_component_by_field(page['equipmentIcons'], 'name', item["name"])
    await menu_utils.focus_component(cmp)
    with server.player_items_stream() as stream:
        player_items = await stream.next()
    if player_items['cursorSlotItem'] and not player_items['equippedItems'][item['field']]:
        await menu_utils.click_component(cmp)
    else:
        await menu_utils.focus_component(cmp)

mapping = {
    "item <positive_index>": df_utils.async_action(focus_item, None, 'positive_index'),
    "row <positive_index>": df_utils.async_action(focus_item, 'positive_index', None),
    "trash can": menu_utils.simple_click("trashCan"),
    "<equipment_icons>": df_utils.async_action(click_equipment_icon, 'equipment_icons'),
}

equipment_icons = {
    "boots": {"name": "Boots", "field": "boots"},
    "hat": {"name": "Hat", "field": "hat"},
    "pants": {"name": "Pants", "field": "pants"},
    "left ring | ring one": {"name": "Left Ring", "field": "leftRing"},
    "right ring | ring to": {"name": "Right Ring", "field": "rightRing"},
    "shirt": {"name": "Shirt", "field": "shirt"},
}

def load_grammar():
    extras = [
        df_utils.positive_index,
        items.craftable_items_choice,
        df.Choice('equipment_icons', equipment_icons)
    ]
    grammar = menu_utils.build_menu_grammar(mapping, get_inventory_page, extras=extras)
    grammar.load()
