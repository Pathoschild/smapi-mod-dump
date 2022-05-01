import dragonfly as df
import server, constants
import asyncio
import functools
import inspect

MENU_GRAMMAR_COUNT = 0

async def focus_component(cmp):
    if not cmp['visible']:
        raise InvalidMenuOption('Cannot focus non-visible component')   
    x, y = cmp.get('focusTarget', cmp['center'])
    await server.set_mouse_position(x, y)

def find_component_containing_mouse(list_of_cmp_rows):
    for row_num, row in enumerate(list_of_cmp_rows):
        for col_num, cmp in enumerate(row):
            if cmp['containsMouse']:
                return row_num, col_num

def list_of_rows(cmps, y_threshold=25):
    '''
    Use center y property to create a list of rows from top to bottom. Assumes all items in a row
    have the same y value
    '''
    if not cmps:
        return []
    y_sorted = sorted(cmps, key=lambda c: c['center'][1])
    rows = [[y_sorted[0]]]
    for cmp in y_sorted[1:]:
        cmp_y = cmp['center'][1]
        first_in_row_y = rows[-1][0]['center'][1]
        new_row = cmp_y >= first_in_row_y + y_threshold
        if new_row:
            rows.append([])
        rows[-1].append(cmp)
    return rows

async def get_active_menu(menu_type=None):
    menu = await server.request('GET_ACTIVE_MENU')
    if menu_type is not None:
        if menu is None:
            raise InvalidMenuOption(f'Expecting {menu_type}, got None')
        if menu['menuType'] != menu_type:
            raise InvalidMenuOption(f"Expecting {menu_type}, got {menu['menuType']}")
    return menu


async def click_menu_button(button_property, menu_getter=get_active_menu):
    menu = await menu_getter()
    if menu is None:
        raise InvalidMenuOption()
    btn = menu.get(button_property)
    if btn is None:
        raise InvalidMenuOption()
    await click_component(btn)

def find_component_by_field(list_of_components, field_name, field_value):
    return next((x for x in list_of_components if x.get(field_name) == field_value), None)

async def click_component(cmp):
    await focus_component(cmp)
    await server.mouse_click()

async def scroll_up(menu, count=1):
    cmp = menu[constants.UP_ARROW]
    for i in range(count):
        await click_component(cmp)
        await asyncio.sleep(0.05)

async def scroll_down(menu, count=1):
    cmp = menu[constants.DOWN_ARROW]
    for i in range(count):
        await click_component(cmp)
        await asyncio.sleep(0.05)

async def try_menus(try_fns, *a):
    for fn in try_fns:
        try:
            await fn(*a)
        except InvalidMenuOption:
            pass
        else:
            return

def valid_menu_test(fn):
    def test_fn():
        try:
            res = fn()
        except InvalidMenuOption:
            return False
        else:
            return res is not False
    return df.FuncContext(test_fn)

class InventoryMenuWrapper:

    def __init__(self):
        self.row = 0
        self.col = 0

    async def focus_previous(self, inventory_menu):
        await self.focus_box(inventory_menu, self.row, self.col)

    async def focus_box(self, inventory_menu, new_row, new_col):
        rows = list_of_rows(inventory_menu['inventory'])
        indices = find_component_containing_mouse(rows) or (self.row, self.col)
        row = indices[0] if new_row is None else new_row
        col = indices[1] if new_col is None else new_col
        cmp = rows[row][col]
        await focus_component(cmp)
        self.row = row
        self.col = col

    async def click_range(self, inventory_menu, start, end):
        end = start if end is None else end
        if not (start <= end < 12):
            raise ValueError
        rows = list_of_rows(inventory_menu['inventory'])
        row, _ = find_component_containing_mouse(rows) or (self.row, self.col)
        for col in range(start, end + 1):
            cmp = rows[row][col]
            await click_component(cmp)
        self.row = row
        self.col = end

    async def focus_box_by_item_name(self, name: str):
        pass

def scroll_commands(page_size=4):
    import df_utils

    return {
        "scroll up [<positive_num>]": df_utils.async_action(scroll_up, 'positive_num'),
        "scroll down [<positive_num>]": df_utils.async_action(scroll_down, 'positive_num'),
        "page up [<positive_num>]": df_utils.AsyncFunction(scroll_up, format_args=lambda **kw: [kw['positive_num'] * page_size]),
        "page down [<positive_num>]": df_utils.AsyncFunction(scroll_down, format_args=lambda **kw: [kw['positive_num'] * page_size]),
    }

class InvalidMenuOption(Exception):
    pass

def yield_clickable_components(item):
    if isinstance(item, dict):
        if item.get('type') == 'clickableComponent':
            if item['visible']:
                yield item
        else:
            menu_type = item.get('menuType')
            for child in item.values():
                yield from yield_clickable_components(child)
    if isinstance(item, (list, tuple)):
        for child in item:
            yield from yield_clickable_components(child)

def inventory_commands():
    import df_utils
    inventory_wrapper = InventoryMenuWrapper()
    async def inventory_focus(menu, new_row, new_col):
        inventory = menu['inventory']
        await inventory_wrapper.focus_box(inventory, new_row, new_col)

    async def click_button(menu, name):
        await click_component(menu[name])

    commands = {
        "item <positive_index>": df_utils.async_action(inventory_focus, None, 'positive_index'),
        "row <positive_index>": df_utils.async_action(inventory_focus, 'positive_index', None),
        "ok": df_utils.async_action(click_button, "okButton"),
        "trash can": df_utils.async_action(click_button, "trashCan"),
    }
    return commands

def simple_focus(*fields):
    import df_utils

    async def to_call(menu, field_list):
        for field in field_list:
            cmp = menu.get(field)
            if cmp:
                await focus_component(cmp)
                return

    return df_utils.async_action(to_call, fields)

def simple_click(*fields):
    import df_utils

    async def to_call(menu, field_list):
        for field in field_list:
            cmp = menu.get(field)
            if cmp:
                await click_component(cmp)
                return
    return df_utils.async_action(to_call, fields)

async def ensure_awaited(obj):
    if inspect.isawaitable(obj):
        return await obj
    return obj

def build_menu_grammar(mapping, menu_validator, extras=(), defaults=None):
    global MENU_GRAMMAR_COUNT
    import df_utils, game, server
    mgb = MenuGrammarBuilder(mapping, menu_validator)
    defaults = {'positive_num': 1} if defaults is None else defaults
    n = MENU_GRAMMAR_COUNT + 1
    MENU_GRAMMAR_COUNT = n
    grammar = df.Grammar(f"menu_grammar_{n}")
    new_mapping = {}
    for cmd, v in mapping.items():
        if isinstance(v, df_utils.AsyncFunction) and v.format_args: # insert active menu as first arg
            v.format_args = mgb.format_args_menu_provider(v.format_args)
        new_mapping[cmd] = v
    main_rule = df.MappingRule(
        name=f"menu_grammar_rule_{n}",
        mapping=new_mapping,
        extras=extras,
        defaults=defaults,
        context=df.FuncContext(mgb.is_active)
    )
    grammar.add_rule(main_rule)
    return grammar

class MenuGrammarBuilder:

    def __init__(self, mapping, menu_validator):
        self.mapping = mapping
        self.menu_validator = menu_validator

    def format_args_menu_provider(self, old_format_args):
        async def format_args_with_menu(**kw):
            menu = await self.get_menu()
            old_args = await ensure_awaited(old_format_args(**kw))
            return [menu] + old_args
        return format_args_with_menu

    def is_active(self):
        import game
        menu = game.get_context_menu()
        return self.run_menu_validator(menu)
    
    async def get_menu(self):
        menu = await get_active_menu()
        res = self.test_validator(menu)
        if res is False:
            raise InvalidMenuOption()
        return res or menu

    @property
    def test_validator(self):
        if isinstance(self.menu_validator, str):
            menu_type = self.menu_validator
            return functools.partial(validate_menu_type, menu_type)
        return self.menu_validator

    def run_menu_validator(self, menu):
        try:
            res = self.test_validator(menu)
        except (InvalidMenuOption, KeyError, TypeError):
            return False
        else:
            return res is not False

def validate_menu_type(menu_type, menu):
    if menu_type is not None:
        if menu is None:
            raise InvalidMenuOption(f'Expecting {menu_type}, got None')
        if menu['menuType'] != menu_type:
            raise InvalidMenuOption(f"Expecting {menu_type}, got {menu['menuType']}")