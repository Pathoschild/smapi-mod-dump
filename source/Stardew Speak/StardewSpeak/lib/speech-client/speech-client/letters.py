from srabuilder.actions import pydirectinput
import dragonfly as df
import server
import time
import df_utils

letter_map = {
    "(alpha)": "a",
    "(bravo) ": "b",
    "(charlie) ": "c",
    "(danger) ": "d",
    "(eureka) ": "e",
    "(foxtrot) ": "f",
    "(gorilla) ": "g",
    "(hotel) ": "h",
    "(india) ": "i",
    "(juliet) ": "j",
    "(kilo) ": "k",
    "(lima) ": "l",
    "(michael) ": "m",
    "(november) ": "n",
    "(Oscar) ": "o",
    "(papa) ": "p",
    "(quiet) ": "q",
    "(romeo) ": "r",
    "(sierra) ": "s",
    "(tango) ": "t",
    "(uniform) ": "u",
    "(victor) ": "v",
    "(whiskey) ": "w",
    "(x-ray) ": "x",
    "(yankee) ": "y",
    "(zulu) ": "z",
}

numbers = {k: str(v) for k, v in df_utils.digitMap.items()}

capital_letter_map = {f"(capital | upper | uppercase) {k}": v.upper() for k, v in letter_map.items()}

keys = {
    "backspace": "backspace",
    "space": "space",
    "(dot | period)": ".",
    "dash": "-",
    "underscore": "_",
}

def multiply_keys(rep):
    n = 1 if rep[0] is None else rep[0]
    key = rep[1]
    return [key for i in range(n)]

def flatten_list(rep):
    flattened = []
    for l in rep:
        flattened.extend(l)
    return flattened

all_chars_choice = df.Choice(None, {**letter_map, **capital_letter_map, **keys, **numbers})
letters_and_keys = df.Repetition(all_chars_choice, name="letters_and_keys", min=1, max=16)

def type_characters(letters: str):
    shift_down = False
    for char in letters:
        shift_char = char.isupper()
        char = char.lower()
        if shift_char and not shift_down:
            pydirectinput.keyDown('shift')
            shift_down = True
        elif not shift_char and shift_down:
            pydirectinput.keyUp('shift')
            shift_down = False
        pydirectinput.press(char)
    if shift_down:
        pydirectinput.keyUp('shift')


def title_case(words):
    return " ".join([x.title() for x in words])

def dictation_wrap(fn):
    return df.Function(lambda dictation: do_dictation(fn(dictation.split())))

def do_dictation(dictation):
    text = str(dictation)
    type_characters(text)

def typing_commands():
    return {
        "<letters_and_keys>": df.Function(lambda **kw: type_characters(kw['letters_and_keys'])),
        "title <dictation>": dictation_wrap(title_case),
    }