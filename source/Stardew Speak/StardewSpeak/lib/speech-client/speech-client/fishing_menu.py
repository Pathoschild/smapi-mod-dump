import game, server, menu_utils, constants, df_utils, stream
import dragonfly as df
from typing import Literal, TypedDict

FISHING_MENU = 'fishingMenu'

class FishingMenu(menu_utils.BaseMenu):
    title: Literal["fishingMenu"]

class FishingTool(TypedDict):
    isNibbling: bool
    inUse: bool

async def catch_fish(menu: FishingMenu):
    await server.request('CATCH_FISH')

async def start_fishing():
    async with stream.player_status_stream() as pss:
        await game.equip_item_by_name(constants.FISHING_ROD)
    async with stream.tool_status_stream() as tss:
        await cast_fishing_rod(tss)
        await wait_for_nibble(tss)

async def cast_fishing_rod(tss: stream.Stream[FishingTool]):
    async with game.press_and_release(constants.USE_TOOL_BUTTON):
        await tss.wait(lambda t: t['isTimingCast'] and t['castingPower'] > 0.95, timeout=10)

async def wait_for_nibble(tss: stream.Stream[FishingTool]):
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
    