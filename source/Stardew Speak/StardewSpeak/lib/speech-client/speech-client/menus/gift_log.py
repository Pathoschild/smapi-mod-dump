import game, server, menu_utils, constants, df_utils
import dragonfly as df

TITLE = 'profileMenu'

mapping = {
    "previous (character | npc)": menu_utils.simple_click('previousCharacterButton'),
    "next (character | npc)": menu_utils.simple_click('nextCharacterButton'),
    "previous [gift] type": menu_utils.simple_click('backButton'),
    "next [gift] type": menu_utils.simple_click('forwardButton')
}

def get_grammar():
    grammar = menu_utils.build_menu_grammar(mapping, TITLE)
    return grammar