from __future__ import annotations
import time
import logging
import args
import struct
import traceback
import weakref
import functools
import queue
import sys
import asyncio
import asyncio.queues
import threading
import uuid
import json
from dragonfly import *
from srabuilder import rules
from typing import Any, Coroutine
from asyncio.futures import Future
import logger
from typing import Callable

class NamedPipeHandler:

    def __init__(self, named_pipe: str) -> None:
        self.named_pipe_file = open(rf"\\.\pipe\{named_pipe}Reader", "r+b", 0)
        self.named_pipe_file_read = open(rf"\\.\pipe\{named_pipe}Writer", "r+b", 0)

    def read(self):
        pass

    def write(self, msg: str):
        pass

if args.args.named_pipe:
    named_pipe_file = open(rf"\\.\pipe\{args.args.named_pipe}Reader", "r+b", 0)
    named_pipe_file_read = open(rf"\\.\pipe\{args.args.named_pipe}Writer", "r+b", 0)
else:
    named_pipe_file = None
    named_pipe_file_read = None

loop = asyncio.new_event_loop()

mod_requests: dict[str, Future] = {}

ongoing_tasks = {}  # not connected to an objective, slide mouse, swing sword etc

async def stop_all_ongoing_tasks():
    cancel_awaitables = [t.cancel() for t in ongoing_tasks.values()]
    await asyncio.gather(*cancel_awaitables)


async def stop_everything():
    import game
    import objective

    await asyncio.gather(stop_all_ongoing_tasks(), objective.cancel_active_objective())
    await game.release_all_keys()


def call_soon(awaitable, *args, **kw):
    loop.call_soon_threadsafe(_do_create_task, awaitable, *args, **kw)


def _do_create_task(awaitable, *args, **kw):
    loop.create_task(awaitable(*args, **kw))


def setup_async_loop():

    def async_setup():
        loop.set_exception_handler(exception_handler)
        if args.args.named_pipe:
            loop.create_task(async_readline())
        loop.create_task(heartbeat(2))
        loop.create_task(populate_initial_game_event())
        loop.run_forever()

    def exception_handler(loop, context):
        # This only works when there are no references to the above tasks.
        # https://bugs.python.org/issue39256y
        # get_engine().disconnect()
        # sys.exit(context.get("exception", "bad"))
        # return
        raise context.get("exception", RuntimeError(f"task shutdown error {context}"))

    async_thread = threading.Thread(target=async_setup, daemon=True)
    async_thread.start()


def graceful_exit(msg):
    get_engine().disconnect()
    sys.exit(msg)


async def request_and_update_active_menu():
    import menu_utils
    new_menu = await menu_utils.get_active_menu()
    await handle_new_menu(new_menu)


async def handle_new_menu(new_menu):
    import game
    current_menu = game.context_variables["ACTIVE_MENU"]
    is_new_menu = not is_same_menu(current_menu, new_menu)
    game.set_context_menu(new_menu)
    if is_new_menu:
        logger.debug(f"Got new menu {new_menu['menuType']}")
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


async def heartbeat(timeout: int):
    while True:
        fut = request("HEARTBEAT")
        try:
            resp = await asyncio.wait_for(fut, timeout=timeout)
        except asyncio.TimeoutError as e:
            raise e
        await asyncio.sleep(timeout)


async def async_readline():
    # Is there a better way to read async stdin on Windows?
    q: queue.Queue[Future[str]] = queue.Queue()
    def _run():
        while True:
            fut = q.get()
            try:
                n = struct.unpack("I", named_pipe_file_read.read(4))[0]  # Read str length
                line = named_pipe_file_read.read(n).decode("utf8")  # Read str
                named_pipe_file_read.seek(0)
                loop.call_soon_threadsafe(fut.set_result, line)
            except Exception as e:
                graceful_exit("pipe disconnected")

    threading.Thread(target=_run, daemon=True).start()
    while True:
        fut: Future[str] = loop.create_future()
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
        import stream
        return stream.Stream("UPDATE_TICKED", data={"type": self.request_type, "ticks": ticks})

    @classmethod
    def batch(cls, *reqs):
        batched = []
        for r in reqs:
            if isinstance(r, RequestBuilder):
                msg = {"type": r.request_type, "data": r.data}
            else:
                msg = {"type": msg[0], "data": msg[1]}
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
    if named_pipe_file:
        try:
            named_pipe_file.write(struct.pack("I", len(msg_str)) + msg_str.encode("utf8"))  # Write str length and str
            named_pipe_file.seek(0)
        except:
            graceful_exit("Named pipe broken")
    else:
        print(msg_str)
    return full_msg


def on_message(msg_str: str):
    import events, stream

    try:
        msg = json.loads(msg_str)
    except json.JSONDecodeError:
        logger.trace(f"Got invalid message from mod {msg_str}")
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

        stream_obj = stream.streams.get(stream_id)
        if stream_obj is None:
            send_message("STOP_STREAM", stream_id)
            return
        stream_value = msg_data["value"]
        stream_error = msg_data.get("error")
        if stream_error is not None:
            logger.debug(f"Stream {stream_id} error: {stream_value}")
            stream_obj.close()
            return
        stream_obj.set_value(stream_value)
        stream_obj.latest_value = stream_value
        try:
            stream_obj.future.set_result(None)
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

    done: bool

    def __init__(self, coro: Coroutine):
        self.result = None
        self.exception: BaseException | None = None
        self.exception_trace: str | None = None
        self.done = False
        self.task = loop.create_task(self._wrap_coro(coro))

    # I don't understand asyncio task exception handling. So let's just catch any coroutine exceptions here and expose
    # the result/exception through self.result and self.exception
    async def _wrap_coro(self, coro: Coroutine):
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

def read_queue(q): 
    items = [q.get()]
    while not q.empty():
        try:
            next_item = q.get(block=False)
            items.append(next_item)
        except TypeError:
            continue
    return items