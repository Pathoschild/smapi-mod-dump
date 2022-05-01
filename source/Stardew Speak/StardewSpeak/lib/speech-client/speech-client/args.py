import os.path
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--python_root", default=None, help="Root python directory")
parser.add_argument("--named_pipe", default=None, help="Named pipe file name for communicating with C#")
args = parser.parse_args()
if args.python_root is None:
    args.python_root = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
