import game, server, menu_utils, df_utils
import dragonfly as df

main_button_choice = df.Choice("main_buttons", {"new": "New", "load": "Load", "co op": "Co-op", "exit": "Exit"})

TITLE_MENU = 'titleMenu'

def get_title_menu(menu):
    menu_utils.validate_menu_type(TITLE_MENU, menu)
    if menu['subMenu']:
        raise menu_utils.InvalidMenuOption()
    return menu

async def click_main_button(menu, btn_name: str):
    button = menu_utils.find_component_by_field(menu['buttons'], 'name', btn_name)
    await menu_utils.click_component(button)

def get_submenu(tm, menu_type):
    menu_utils.validate_menu_type(TITLE_MENU, tm)
    submenu = tm.get('subMenu')
    menu_utils.validate_menu_type(menu_type, submenu)
    return submenu

mapping = {
    "<main_buttons> [game]": df_utils.async_action(click_main_button, 'main_buttons'),
    "[change | select] (language | languages)": menu_utils.simple_click('languageButton'),
    "about": menu_utils.simple_click('aboutButton'),
}

def load_grammar():
    grammar = menu_utils.build_menu_grammar(
        mapping,
        get_title_menu,
        extras=[main_button_choice],
        defaults={'positive_num': 1},
    )
    grammar.load()
