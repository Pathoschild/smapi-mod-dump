import args
import logger
import winsound
import traceback
import csv
import tempfile
import shutil
import subprocess
import os.path
import os
import logging
import sys
from io import BytesIO
from zipfile import ZipFile
import urllib.request
import menus
import asyncio.queues
import threading

from dragonfly import RecognitionObserver, get_engine, AppContext
from dragonfly.log import setup_log
from srabuilder import sleep

import approximate_matching
import any_context, purchase_animals_menu, new_game_menu, shop_menu, container_menu, title_menu, load_game_menu, dialogue_menu, no_menu, any_menu, shipping_bin_menu, carpenter_menu, billboard_menu, geode_menu, museum_menu
from game_menu import game_menu, crafting_page, inventory_page, exit_page, skills_page
import letter_viewer_menu, quest_log_menu, animal_query_menu, coop_menu, title_text_input_menu, cutscene, level_up_menu, shipped_items_menu, fishing_menu, mine_elevator_menu
import locations

IS_FROZEN = getattr(sys, "frozen", False)


MODELS_DIR = os.path.abspath(os.path.join(args.args.python_root, "models"))

async def asleep():
    return 6

class Observer(RecognitionObserver):
    def on_begin(self):
        import server
        future = asyncio.run_coroutine_threadsafe(server.request_and_update_active_menu(), server.loop)
        # Wait for the result with an optional timeout argument
        future.result(3)

    def on_recognition(self, words):
        logger.info("Recognized:", " ".join(words))

    def on_failure(self):
        pass


def add_base_user_lexicon(model_dir: str):
    import df_utils

    dst = os.path.join(model_dir, "user_lexicon.txt")
    shutil.copyfile(df_utils.lexicon_source_path(), dst)


def download_model(write_dir):
    import game

    model_url = (
        "https://github.com/daanzu/kaldi-active-grammar/releases/download/v3.0.0/kaldi_model_daanzu_20211030-biglm.zip"
    )

    game.show_hud_message(f"Downloading speech recognition model. This may take a few minutes...", 2)
    url_open = urllib.request.urlopen(model_url)
    with ZipFile(BytesIO(url_open.read())) as my_zip_file:
        my_zip_file.extractall(write_dir)
    shutil.rmtree(os.path.join(write_dir, "kaldi_model.tmp"), ignore_errors=True)


def setup_engine(silence_timeout, model_dir):
    if not os.path.isdir(model_dir):
        if IS_FROZEN:
            raise RuntimeError(
                f"Cannot find kaldi model at {os.path.abspath(model_dir)} using executable path {__file__}"
            )
        download_model(MODELS_DIR)
    if not IS_FROZEN:
        add_base_user_lexicon(model_dir)
    # Set any configuration options here as keyword arguments.
    engine = get_engine(
        "kaldi",
        model_dir=model_dir,
        expected_error_rate_threshold=0.05,
        # tmp_dir='kaldi_tmp',  # default for temporary directory
        # vad_aggressiveness=3,  # default aggressiveness of VAD
        vad_padding_start_ms=0,  # default ms of required silence surrounding VAD
        vad_padding_end_ms=silence_timeout,  # default ms of required silence surrounding VAD
    )
    # Call connect() now that the engine configuration is set.
    engine.connect()
    return engine


def ensure_exclusive_mode_disabled_for_default_mic():
    fd, path = tempfile.mkstemp()
    with open(fd, "w") as f:
        pass
    svv_path = os.path.join(args.args.python_root, "bin", "SoundVolumeView", "SoundVolumeView.exe")
    subprocess.run((svv_path, "/scomma", path))
    with open(path) as f:
        reader = csv.DictReader(f, delimiter=",")
        for row in reader:
            if row["Default"] == "Capture":  # found our default audio input device
                device_id = row["Command-Line Friendly ID"]
                subprocess.run((svv_path, "/SetAllowExclusive", device_id, "0"))
                break
    os.remove(path)


def run_engine():

    import game

    engine = get_engine()
    engine.prepare_for_recognition()
    approximate_matching.initialize()
    game.show_hud_message("Speech recognition is ready", 4)
    logger.info("Speech recognition is ready")
    try:
        engine.do_recognition()
    except KeyboardInterrupt:
        pass


def main():
    import server

    logging.basicConfig(level=logging.INFO)
    try:
        ensure_exclusive_mode_disabled_for_default_mic()
    except Exception as e:
        logger.warning(
            f"Unable to disable exclusive mode for default audio device: {traceback.format_exc()}",
            level=2,
        )
    model_dir = os.path.join(MODELS_DIR, "kaldi_model")
    engine = setup_engine(300, model_dir)

    # Register a recognition observer
    observer = Observer()
    observer.register()

    sleep.load_sleep_wake_grammar(True)
    stardew_context = AppContext(title="stardew")
    server.setup_async_loop()
    menus.load_all_grammars()
    any_context.load_grammar()
    new_game_menu.load_grammar()
    shop_menu.load_grammar()
    container_menu.load_grammar()
    game_menu.load_grammar()
    crafting_page.load_grammar()
    inventory_page.load_grammar()
    exit_page.load_grammar()
    skills_page.load_grammar()
    title_menu.load_grammar()
    load_game_menu.load_grammar()
    dialogue_menu.load_grammar()
    no_menu.load_grammar()
    any_menu.load_grammar()
    shipping_bin_menu.load_grammar()
    carpenter_menu.load_grammar()
    billboard_menu.load_grammar()
    geode_menu.load_grammar()
    museum_menu.load_grammar()
    letter_viewer_menu.load_grammar()
    quest_log_menu.load_grammar()
    animal_query_menu.load_grammar()
    coop_menu.load_grammar()
    title_text_input_menu.load_grammar()
    locations.load_grammar()
    cutscene.load_grammar()
    level_up_menu.load_grammar()
    shipped_items_menu.load_grammar()
    fishing_menu.load_grammar()
    mine_elevator_menu.load_grammar()
    purchase_animals_menu.load_grammar()
    # copy back user lexicon to speech-client root. May want to rethink this approach.
    if not IS_FROZEN:
        src = os.path.join(model_dir, "user_lexicon.txt")
        dst = os.path.abspath(os.path.join(os.path.abspath(__file__), "..", "..", "user_lexicon.txt"))
        shutil.copyfile(src, dst)
    run_engine()


if __name__ == "__main__":
    main()
