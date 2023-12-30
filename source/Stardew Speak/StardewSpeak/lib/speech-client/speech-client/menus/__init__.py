from menus import (
    gift_log,
    social_page,
    collections_page,
)
import logger


def list_imported_modules():
    import sys

    module_names = set(sys.modules) & set([f"menus.{x}" for x in globals()])
    for name in module_names:
        yield sys.modules[name]


def load_all_grammars():
    import server

    menu_modules = list_imported_modules()
    for module in menu_modules:
        grammar = module.get_grammar()
        logger.trace(f"Loading grammar from module {module.__name__}")
        grammar.load()
