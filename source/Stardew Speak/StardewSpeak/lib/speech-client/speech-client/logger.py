import sys
import logging
import os.path
import pathlib

import args

logging_dir = os.path.join(os.path.expanduser("~"), ".StardewSpeak", "logs")
pathlib.Path(logging_dir).mkdir(parents=True, exist_ok=True)
sys.stderr = open(os.path.join(logging_dir, "err.txt"), "w")
