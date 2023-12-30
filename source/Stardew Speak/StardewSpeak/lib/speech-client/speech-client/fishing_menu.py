import game, server, menu_utils, constants, df_utils
import dragonfly as df

FISHING_MENU = 'fishingMenu'

async def catch_fish(menu):
    await server.request('CATCH_FISH')

async def start_fishing():
    async with server.player_status_stream() as stream:
        await game.equip_item_by_name(constants.FISHING_ROD)
    async with server.tool_status_stream() as tss:
        await cast_fishing_rod(tss)
        await wait_for_nibble(tss)

async def cast_fishing_rod(tss: server.Stream):
    async with game.press_and_release(constants.USE_TOOL_BUTTON):
        await tss.wait(lambda t: t['isTimingCast'] and t['castingPower'] > 0.95, timeout=10)

async def wait_for_nibble(tss):
    tool_status = await tss.wait(lambda t: t['isNibbling'] or not t['inUse'])
    if tool_status['inUse']:
        await game.press_key(constants.USE_TOOL_BUTTON)
        await tss.wait(lambda t: t['isReeling'])

mapping = {
    "catch fish": df_utils.async_action(catch_fish)
}

def load_grammar():
    grammar = menu_utils.build_menu_grammar(mapping, FISHING_MENU)
    grammar.load()
    