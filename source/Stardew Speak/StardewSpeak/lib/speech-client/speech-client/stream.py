from __future__ import annotations
import async_timeout
import uuid
import asyncio
from typing import Callable
from sdv_types import PlayerStatus, ToolStatus

streams: dict[str, Stream] = {}

class Stream[T]:
    
    def __init__(self, name: str, data=None):
        import server
        self.has_value = False
        self.latest_value = None
        self.future: asyncio.Future[T | None] = server.loop.create_future()
        self.name = name
        self.id = f"{name}_{str(uuid.uuid4())}"
        self.closed = False
        self.open(data)

    def set_value(self, value):
        self.latest_value = value
        self.has_value = True
        try:
            self.future.set_result(None)
        except asyncio.InvalidStateError:
            pass

    def open(self, data):
        import server
        streams[self.id] = self
        server.send_message(
            "NEW_STREAM",
            {
                "name": self.name,
                "stream_id": self.id,
                "data": data,
            },
        )

    def close(self):
        import server
        if not self.closed:
            self.closed = True
            server.send_message("STOP_STREAM", self.id)
            del streams[self.id]
            self.set_value(None)

    async def current(self) -> T:
        if self.has_value:
            return self.latest_value
        return await self.next()

    async def __aenter__(self):
        return self

    async def __aexit__(self, exc_type, exc, tb):
        self.close()

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc, tb):
        self.close()

    async def next(self) -> T:
        import server
        if self.closed:
            raise StreamClosedError("Stream is already closed")
        if not self.future.done():
            await self.future
        if self.closed:
            raise StreamClosedError(f"Stream {self.name} closed while waiting for next value")
        self.future = server.loop.create_future()
        return self.latest_value

    async def wait(self, condition: Callable[[T], bool], timeout: float | None = None) -> T:
        async with async_timeout.timeout(timeout):
            item = await self.current()
            while not condition(item):
                item = await self.next()
            return item


class StreamClosedError(Exception):
    pass


def player_status_stream(ticks=1) -> Stream[PlayerStatus]:
    return Stream("UPDATE_TICKED", data={"type": "PLAYER_STATUS", "ticks": ticks})


def tool_status_stream(ticks=1) ->Stream[ToolStatus]:
    return Stream("UPDATE_TICKED", data={"type": "TOOL_STATUS", "ticks": ticks})


def characters_at_location_stream(ticks=1):
    return Stream("UPDATE_TICKED", data={"type": "CHARACTERS_AT_LOCATION", "ticks": ticks})


def animals_at_location_stream(ticks=1):
    return Stream("UPDATE_TICKED", data={"type": "ANIMALS_AT_LOCATION", "ticks": ticks})


def player_items_stream(ticks=1):
    return Stream("UPDATE_TICKED", data={"type": "PLAYER_ITEMS", "ticks": ticks})


def on_warped_stream(ticks=1):
    return Stream("ON_WARPED", data={"type": "PLAYER_STATUS", "ticks": ticks})


def on_terrain_feature_list_changed_stream():
    return Stream("ON_TERRAIN_FEATURE_LIST_CHANGED", data={})


def on_menu_changed_stream():
    return Stream("ON_MENU_CHANGED", data={})

