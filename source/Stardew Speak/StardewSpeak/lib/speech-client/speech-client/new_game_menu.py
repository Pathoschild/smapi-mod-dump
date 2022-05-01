import asyncio
import dragonfly as df
import title_menu, menu_utils, server, df_utils, game, letters

CHARACTER_CUSTOMIZATION_MENU = "characterCustomizationMenu"


def validate_new_game_menu(menu):
    return title_menu.get_submenu(menu, CHARACTER_CUSTOMIZATION_MENU)


async def click_farm(menu, farm):
    cmp = menu_utils.find_component_by_field(menu["farmTypeButtons"], "name", farm)
    await menu_utils.click_component(cmp)


async def click_cabin_layout(menu, index):
    cmp = menu["cabinLayoutButtons"][index]
    await menu_utils.click_component(cmp)


async def click_arrow_field(menu, field, cmp_list_name, count):
    cmp = menu_utils.find_component_by_field(menu[cmp_list_name], "name", field)
    for i in range(count):
        await menu_utils.click_component(cmp)
        if i < count - 1:
            await asyncio.sleep(0.1)


farm_types = {
    "standard": "Standard",
    "riverland": "Riverland",
    "forest": "Forest",
    "hill [top]": "Hills",
    "wilderness": "Wilderness",
    "four corners": "Four Corners",
    "beach": "Beach",
}

arrows = {
    "previous": "leftSelectionButtons",
    "next": "rightSelectionButtons",
}
arrow_fields = {
    "(accessory | accessories)": "Acc",
    "direction": "Direction",
    "hair": "Hair",
    "pants": "Pants Style",
    "(pet | animal) [preference]": "Pet",
    "shirt": "Shirt",
    "skin": "Skin",
    "(wallets | money style)": "Wallets",
    "(difficulty | profit margin)": "Difficulty",
    "[starting] cabins": "Cabins",
}

mapping = {
    "name": menu_utils.simple_click("nameBoxCC"),
    "((nearby | close) cabin layout | cabin layout (nearby | close))": df_utils.async_action(
        click_cabin_layout, 0
    ),
    "(separate cabin layout | cabin layout separate)": df_utils.async_action(
        click_cabin_layout, 1
    ),
    "farm name": menu_utils.simple_click("farmnameBoxCC"),
    "favorite thing": menu_utils.simple_click("favThingBoxCC"),
    "(random | [roll] dice)": menu_utils.simple_click("randomButton"),
    "(ok [button] | start game)": menu_utils.simple_click("okButton"),
    "skip (intro | introduction)": menu_utils.simple_click("skipIntroButton"),
    "help": menu_utils.simple_click("coopHelpButton"),
    "next": menu_utils.simple_click("coopHelpLeftButton"),
    "previous": menu_utils.simple_click("coopHelpRightButton"),
    "advanced options": menu_utils.simple_click("advancedOptionsButton"),
    "<farm_types> farm": df_utils.async_action(click_farm, "farm_types"),
    "<arrows> <arrow_fields> [<positive_num>]": df_utils.async_action(
        click_arrow_field, "arrow_fields", "arrows", "positive_num"
    ),
    "[go] back": menu_utils.simple_click("backButton"),
    **letters.typing_commands(),
}


def load_grammar():
    extras = [
        df.Choice("farm_types", farm_types),
        df.Choice("arrow_fields", arrow_fields),
        df.Choice("arrows", arrows),
        df_utils.positive_num,
        letters.letters_and_keys,
        df.Dictation("dictation"),
    ]
    grammar = menu_utils.build_menu_grammar(
        mapping, validate_new_game_menu, extras=extras
    )
    grammar.load()
