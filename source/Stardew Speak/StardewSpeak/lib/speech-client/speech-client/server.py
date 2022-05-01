import time
import logging
import args
import struct
import async_timeout
import traceback
import weakref
import functools
import queue
import sys
import asyncio
import threading
import uuid
import json
from dragonfly import *
from srabuilder import rules

import constants

if args.args.named_pipe:
    named_pipe_file = open(rf"\\.\pipe\{args.args.named_pipe}Reader", "r+b", 0)
    named_pipe_file_read = open(rf"\\.\pipe\{args.args.named_pipe}Writer", "r+b", 0)
else:
    named_pipe_file = None
    named_pipe_file_read = None

loop = None
streams = {}
mod_requests = {}

ongoing_tasks = {}  # not connected to an objective, slide mouse, swing sword etc


async def start_ongoing_task(name, str, async_fn):
    await stop_ongoing_task(name)

    async def to_call(awaitable):
        try:
            await awaitable
        except (Exception, BaseException):
            raise
        finally:
            del ongoing_tasks[name]

    wrapped_coro = async_fn()
    coro = to_call(wrapped_coro)
    task_wrapper = TaskWrapper(coro)
    ongoing_tasks[task_wrapper]


async def stop_ongoing_task(name):
    task = ongoing_tasks.get(name)
    if task:
        await task.cancel()


async def stop_all_ongoing_tasks():
    cancel_awaitables = [t.cancel() for t in ongoing_tasks.values()]
    await asyncio.gather(*cancel_awaitables)


async def stop_everything():
    import game
    import objective

    await asyncio.gather(stop_all_ongoing_tasks(), objective.cancel_active_objective())
    await game.release_all_keys()


class Stream:
    def __init__(self, name, data=None):
        self.has_value = False
        self.latest_value = None
        self.future = loop.create_future()
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
        streams[self.id] = self
        send_message(
            "NEW_STREAM",
            {
                "name": self.name,
                "stream_id": self.id,
                "data": data,
            },
        )

    def close(self):
        if not self.closed:
            self.closed = True
            send_message("STOP_STREAM", self.id)
            del streams[self.id]
            self.set_value(None)

    async def current(self):
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

    async def next(self):
        if self.closed:
            raise StreamClosedError("Stream is already closed")
        if not self.future.done():
            await self.future
        if self.closed:
            raise StreamClosedError(f"Stream {self.name} closed while waiting for next value")
        self.future = loop.create_future()
        return self.latest_value

    async def wait(self, condition, timeout=None):
        async with async_timeout.timeout(timeout):
            item = await self.current()
            while not condition(item):
                item = await self.next()
            return item


class StreamClosedError(Exception):
    pass


def player_status_stream(ticks=1):
    return Stream("UPDATE_TICKED", data={"type": "PLAYER_STATUS", "ticks": ticks})


def tool_status_stream(ticks=1):
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


def create_stream_next_task(awaitable):
    async def to_call(awaitable):
        try:
            return await awaitable
        except ValueError as e:
            pass

    return loop.create_task(to_call(awaitable))


def call_soon(awaitable, *args, **kw):
    loop.call_soon_threadsafe(_do_create_task, awaitable, *args, **kw)


def _do_create_task(awaitable, *args, **kw):
    loop.create_task(awaitable(*args, **kw))


def setup_async_loop():
    global loop
    loop = asyncio.new_event_loop()

    def async_setup(l):
        l.set_exception_handler(exception_handler)
        l.create_task(menu_changed())
        l.create_task(async_readline())
        l.create_task(heartbeat(300))
        l.create_task(populate_initial_game_event())
        l.run_forever()

    def exception_handler(loop, context):
        # This only works when there are no references to the above tasks.
        # https://bugs.python.org/issue39256y
        # get_engine().disconnect()
        # sys.exit(context.get("exception", "bad"))
        # return
        raise context.get("exception", "task shutdown error")

    async_thread = threading.Thread(target=async_setup, daemon=True, args=(loop,))
    async_thread.start()


def graceful_exit(msg):
    get_engine().disconnect()
    sys.exit(msg)


async def request_active_menu_with_delay():
    import menu_utils

    await asyncio.sleep(1)
    menu = await menu_utils.get_active_menu()
    return menu


async def menu_changed():
    import game

    async with on_menu_changed_stream() as mcs:
        while True:
            changed_event_coro, active_menu_coro = mcs.next(), request_active_menu_with_delay()
            done, pending = await asyncio.wait(
                [changed_event_coro, active_menu_coro], return_when=asyncio.FIRST_COMPLETED
            )
            done_task = list(done)[0]
            done_coro = done_task.get_coro()
            if done_coro == changed_event_coro:
                new_menu = done_task.result()["newMenu"]
            else:
                new_menu = done_task.result()
            current_menu = game.context_variables["ACTIVE_MENU"]
            is_new_menu = not is_same_menu(current_menu, new_menu)
            game.set_context_menu(new_menu)
            if is_new_menu:
                await stop_everything()


def is_same_menu(menu1, menu2):
    if (menu1, menu2) == (None, None):
        return True
    if (menu1, menu2).count(None) == 1:
        return False
    if menu1["menuType"] != menu2["menuType"]:
        return False
    if menu1["menuType"] == "titleMenu":
        return is_same_menu(menu1["subMenu"], menu2["subMenu"])
    if menu1.get("onFarm") != menu2.get("onFarm"):  # carpenter menu, likely others
        return False
    return True


async def populate_initial_game_event():
    import game

    game_event = await request("GET_LATEST_GAME_EVENT")
    game.set_context_value("GAME_EVENT", game_event)


async def heartbeat(timeout):
    # await asyncio.sleep(20)
    # get_engine().disconnect()
    # sys.exit(1)
    # raise RuntimeError('aasdasdad')
    while True:
        fut = request("HEARTBEAT")
        try:
            resp = await asyncio.wait_for(fut, timeout=timeout)
        except asyncio.TimeoutError as e:
            raise e
        await asyncio.sleep(timeout)


async def async_readline():
    # Is there a better way to read async stdin on Windows?
    q = queue.Queue()

    def _run(future_queue):
        while True:
            fut = future_queue.get()
            try:
                n = struct.unpack("I", named_pipe_file_read.read(4))[0]  # Read str length
                line = named_pipe_file_read.read(n).decode("utf8")  # Read str
                named_pipe_file_read.seek(0)
                loop.call_soon_threadsafe(fut.set_result, line)
            except:
                graceful_exit("pipe disconnected")

    threading.Thread(target=_run, daemon=True, args=(q,)).start()
    while True:
        fut = loop.create_future()
        q.put(fut)
        line = await fut
        on_message(line)


class RequestBuilder:
    def __init__(self, request_type: str, data=None):
        self.request_type = request_type
        self.data = {} if data is None else data

    def request(self, data=None):
        data = self.data if data is None else data
        self._fut = loop.create_future()
        sent_msg = send_message(self.request_type, data)
        mod_requests[sent_msg["id"]] = self._fut
        return self._fut

    def stream(self, ticks=1):
        return Stream("UPDATE_TICKED", data={"type": self.request_type, "ticks": ticks})

    @classmethod
    def batch(cls, *reqs):
        batched = []
        for r in reqs:
            if isinstance(r, RequestBuilder):
                msg = {"type": r.request_type, "data": r.data}
            else:
                msg = {"type": msg[0], data: msg[1]}
            batched.append(msg)
        return cls("REQUEST_BATCH", batched)


def request_batch(messages):
    msg_type = "REQUEST_BATCH"
    return request(msg_type, messages)


def request(msg_type, msg=None):
    return RequestBuilder(msg_type, msg).request()


def send_message(msg_type: str, msg=None):
    msg_id = str(uuid.uuid4())
    full_msg = {"type": msg_type, "id": msg_id, "data": msg}
    msg_str = json.dumps(full_msg)
    # print(msg_str, flush=True)
    if named_pipe_file:
        try:
            named_pipe_file.write(struct.pack("I", len(msg_str)) + msg_str.encode("utf8"))  # Write str length and str
            named_pipe_file.seek(0)
        except:
            graceful_exit("Named pipe broken")
    else:
        print(msg_str)
    return full_msg


def on_message(msg_str):
    import events

    try:
        msg = json.loads(msg_str)
    except json.JSONDecodeError:
        log(f"Got invalid message from mod {msg_str}", level=1)
        return
    msg_type = msg["type"]
    msg_data = msg["data"]
    if msg_type == "RESPONSE":
        fut = mod_requests.pop(msg_data["id"], None)
        if fut:
            resp_value = msg_data["value"]
            resp_error = msg_data["error"]
            try:
                if resp_error is None:
                    fut.set_result(resp_value)
                else:
                    exception = Exception(resp_value)
                    fut.set_exception(exception)
            except asyncio.InvalidStateError:
                pass
    elif msg_type == "STREAM_MESSAGE":
        stream_id = msg_data["stream_id"]

        stream = streams.get(stream_id)
        if stream is None:
            send_message("STOP_STREAM", stream_id)
            return
        stream_value = msg_data["value"]
        stream_error = msg_data.get("error")
        if stream_error is not None:
            log(f"Stream {stream_id} error: {stream_value}")
            stream.close()
            return
        stream.set_value(stream_value)
        stream.latest_value = stream_value
        try:
            stream.future.set_result(None)
        except asyncio.InvalidStateError:
            pass
    elif msg_type == "EVENT":
        events.handle_event(msg_data)
    else:
        raise RuntimeError(f"Unhandled message type from mod: {msg_type}")


async def set_mouse_position(x: int, y: int, from_viewport=False):
    await request("SET_MOUSE_POSITION", {"x": x, "y": y, "from_viewport": from_viewport})


async def get_mouse_position():
    return await request("GET_MOUSE_POSITION")


async def set_mouse_position_relative(x: int, y: int):
    await request("SET_MOUSE_POSITION_RELATIVE", {"x": x, "y": y})


async def mouse_click(btn="left", count=1):
    for i in range(count):
        await request("MOUSE_CLICK", {"btn": btn})
        if i + 1 < count:
            await asyncio.sleep(0.1)

async def mouse_hold(btn="left"):
    import game
    assert btn in ("left", "right")
    sbutton = "MOUSE_LEFT" if btn == "left" else "MOUSE_RIGHT"
    game.update_held_buttons_nowait(to_hold=(sbutton,))

async def mouse_release(btn="left"):
    import game
    assert btn in ("left", "right")
    sbutton = "MOUSE_LEFT" if btn == "left" else "MOUSE_RIGHT"
    game.update_held_buttons_nowait(to_release=(sbutton,))


def log(*a, sep=" ", level=1):
    to_send = [x if isinstance(x, str) else json.dumps(x) for x in a]
    return send_message("LOG", {"value": sep.join(to_send), "level": level})


async def sleep_forever():
    while True:
        await asyncio.sleep(3600)


async def cancel_task(task):
    task.cancel()
    try:
        await task
    except asyncio.CancelledError:
        pass


class TaskWrapper:
    def __init__(self, coro):
        self.result = None
        self.exception = None
        self.exception_trace = None
        self.done = False
        self.task = loop.create_task(self.wrap_coro(coro))

    # I don't understand asyncio task exception handling. So let's just catch any coroutine exceptions here and expose
    # the result/exception through self.result and self.exception
    async def wrap_coro(self, coro):
        try:
            self.result = await coro
        except (asyncio.CancelledError, Exception) as e:
            self.exception = e
            self.exception_trace = traceback.format_exc()
        self.done = True

    async def cancel(self):
        self.task.cancel()
        try:
            await self.task
        except asyncio.CancelledError:
            pass
