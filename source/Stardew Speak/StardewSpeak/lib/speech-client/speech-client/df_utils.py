import functools
import sys
import os
import os.path
import inspect
import asyncio
import traceback
import dragonfly as df
import server

def dictation_rule(default=None):
    return df.Dictation('dictation', default=default)

class AsyncFunction(df.ActionBase):
    def __init__(self, async_fn, format_args=None):
        super().__init__()
        self.async_fn = async_fn
        self.format_args = format_args

    async def to_call(self, kwargs):
        import server
        if self.format_args:
            args = await self.get_formatted_args(kwargs)
            kwargs = {}
        else:
            args = []
        try:
            await self.async_fn(*args, **kwargs)
        except (Exception, asyncio.CancelledError, asyncio.TimeoutError) as e:
            server.log(traceback.format_exc(), level=1)

    def execute(self, data=None):
        assert isinstance(data, dict)
        kwargs = {k: v for k, v in data.items() if not k.startswith("_")}
        return server.call_soon(self.to_call, kwargs)

    async def get_formatted_args(self, kwargs):
        args = self.format_args(**kwargs)
        if inspect.isawaitable(args):
            args = await args
        return args

class SyncFunction(df.ActionBase):
    def __init__(self, fn, format_args=None):
        super().__init__()
        self.fn = fn
        self.format_args = format_args

    def execute(self, data=None):
        assert isinstance(data, dict)
        kwargs = {k: v for k, v in data.items() if not k.startswith("_")}
        if self.format_args:
            args = self.format_args(**kwargs)
            return self.fn(*args)
        return self.fn(**kwargs)

def format_args(args, **kw):
    formatted_args = []
    for a in args:
        try:
            formatted_arg = kw.get(a, a)
        except TypeError:
            formatted_arg = a
        formatted_args.append(formatted_arg)
    return formatted_args


def sync_action(fn, *args):
    format_args_fn = functools.partial(format_args, args)
    return SyncFunction(fn, format_args=format_args_fn)

def async_action(async_fn, *args):
    format_args_fn = functools.partial(format_args, args)
    return AsyncFunction(async_fn, format_args=format_args_fn)

def lexicon_source_path():
    import __main__
    dirname = os.path.dirname(__main__.__file__)
    return os.path.abspath(os.path.join(dirname, '..', 'user_lexicon.txt'))

ten_through_twelve = {
    "ten": 10,
    "eleven": 11,
    "twelve": 12,
}

digitMap = {
    "zero": 0,
    "one": 1,
    "two": 2,
    "three": 3,
    "four": 4,
    "five": 5,
    "six": 6,
    "seven": 7,
    "eight": 8,
    "nine": 9,
}

nonZeroDigitMap = {
    "one": 1,
    "two": 2,
    "three": 3,
    "four": 4,
    "five": 5,
    "six": 6,
    "seven": 7,
    "eight": 8,
    "nine": 9,
}


def parse_numrep(rep):
    first, rest = rep
    numstr = str(first) + "".join(str(d) for d in rest)
    return int(numstr)

def index_choice_from_list(name: str, l):
    return df.Choice(name, {k: i for (i, k) in enumerate(l)})


positive_digits = df.Sequence(
    [df.Choice(None, nonZeroDigitMap), df.Repetition(df.Choice(None, digitMap), min=0, max=2)],
    name="positive_digits",
)
positive_num = df.Alternative([df.Modifier(positive_digits, parse_numrep), df.Choice(None, ten_through_twelve)], name="positive_num")
positive_index = df.RuleWrap("positive_index", df.Modifier(positive_num, lambda x: x - 1))
positive_index2 = df.RuleWrap("positive_index2", df.Modifier(positive_num, lambda x: x - 1))