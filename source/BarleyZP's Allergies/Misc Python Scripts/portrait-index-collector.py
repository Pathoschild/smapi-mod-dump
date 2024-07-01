import argparse, os, json, re

# need to check:
# Characters/Dialogue/{name}.json DONE
#    if leo, also check LeoMainland.json DONE
# Characters/Dialogue/MarriageDialogue{name}.json DONE
# Characters/Dialogue/MarriageDialogue.json DONE (the first party anyway; it's good enough)
#    keys containing their name or NOT someone else's name
# Characters/Dialogue/rainy.json DONE
#    keys containing their name
# Data/ExtraDialogue.json DONE
#    keys containing their name
# Data/NPCGiftTastes.json DONE
#    keys containing their name
# Data/Quests.json  DONE
#    index 3 (4th item) starts with their name
# Data/Events/*.json DONE
#    find the "speak {name}" commands and get the stuff in quotes in before the next "/" delimiter
# Data/Festivals/*.json DONE
#    keys that contain their name
# Strings/1_6_Strings.json DONE
#    keys containing their name
# Strings/animationDescriptions.json DONE
#    keys containing their name
# Strings/schedules/{name}.json DONE

# Can't really check Strings/StringsFromCSFiles.json
#    it's a lawless land: there are some events in there but no way to see who they belong to

############

# need to return: character name, associated emotion, fromfile, original key in the file
# {
#   "character_name": {
#     "$h": {    // $neutral is unused
#       "filename": {
#         "key": "value"
#       },
#       "otherfile": {
#         "otherkey": "othervalue"
#       }
#     }
#   }
# }

all_characters: list[str] = ["abigail", "alex", "caroline", "clint", "demetrius", "dwarf",
                             "elliott", "emily", "evelyn", "george", "gil", "gus", "haley",
                             "harvey", "jas", "jodi", "kent", "krobus", "leah", "leo",
                             "lewis", "linus", "marnie", "maru", "mister qi", "pam", "penny",
                             "pierre", "robin", "sam", "sandy", "sebastian", "shane", "vincent",
                             "willy", "wizard"]

def quests_filter(character, v) -> bool:
  # Data/Quests.json
  # index 3 (4th item) starts with their name
  return v.split("/")[4].lower().startswith(character)

def events_filter(character, v) -> bool:
  # Data/Events/*.json
  # find the "speak {name}" commands and get the stuff in quotes in before the next "/" delimiter
  speak_commands = [s for s in v.split("/") if s.lower().startswith("speak")]
  for command in speak_commands:
    if command.split(" ")[1].lower() == character:
      return True
  
  return False


def get_portraits_in_str(s) -> list[str]:
  result = set()
  digits = re.findall(r"\$\d+", s)
  letters = re.findall(r"\$[hsula]", s)
  result.update(digits)
  result.update(letters)
  return result

# assumes fp is open
def get_file_data(fp, data, filter_fn = None):
  j = json.load(fp)
  for k, v in j.items():
    if filter_fn is not None and not filter_fn(k, v):
      continue

    for i, portrait in enumerate(get_portraits_in_str(v)):
      if portrait == "$1" and i == 0:  # $1 can't be used as the first dialogue command
        break
      if portrait not in data:
        data[portrait] = {}

      if fp.name not in data[portrait]:
        data[portrait][fp.name] = {}

      data[portrait][fp.name][k] = v


def get_data_for_character(character: str, content_folder: str):
  data = {}

  # Characters/Dialogue/{name}.json
  characters_dialogue_path = os.path.join(content_folder, "Characters", "Dialogue", f"{character.title()}.json")

  # Characters/Dialogue/MarriageDialogue{name}.json
  characters_dialogue_marriagedialoguename_path = os.path.join(content_folder, "Characters", "Dialogue", f"MarriageDialogue{character.title()}.json")

  # Strings/schedules/{name}.json
  strings_schedulesname_path = os.path.join(content_folder, "Strings", "schedules", f"{character.title()}.json")

  all_keys_paths = [characters_dialogue_path, characters_dialogue_marriagedialoguename_path, strings_schedulesname_path]

  # Characters/Dialogue/LeoMainland.json (special case)
  if character.lower() == "leo":
    leomainland = os.path.join(content_folder, "Characters", "Dialogue", f"LeoMainland.json")
    all_keys_paths.append(leomainland)

  all_keys_paths = [(s, None) for s in all_keys_paths]

  
  # Characters/Dialogue/rainy.json
  characters_dialogue_rainy_path = os.path.join(content_folder, "Characters", "Dialogue", "rainy.json")

  # Data/ExtraDialogue.json
  data_extradialogue_path = os.path.join(content_folder, "Data", "ExtraDialogue.json")

  # Data/NPCGiftTastes.json
  data_npcgifttastes_path = os.path.join(content_folder, "Data", "NPCGiftTastes.json")

  # Strings/1_6_Strings.json
  strings_16strings_path = os.path.join(content_folder, "Strings", "1_6_Strings.json")

  # Strings/animationDescriptions.json
  strings_animationdesc_path = os.path.join(content_folder, "Strings", "animationDescriptions.json")

  name_filter_key_paths = [characters_dialogue_rainy_path, data_extradialogue_path, data_npcgifttastes_path, strings_16strings_path, strings_animationdesc_path]

  # Characters/Dialogue/MarriageDialogue.json (only if they're marriageable)
  if os.path.exists(characters_dialogue_marriagedialoguename_path):
    characters_dialogue_marriagedialogue_path = os.path.join(content_folder, "Characters", "Dialogue", "MarriageDialogue.json")
    name_filter_key_paths.append(characters_dialogue_marriagedialogue_path)
  
  # Data/Festivals/*.json
  for d in os.scandir(os.path.join(content_folder, "Data", "Festivals")):
    if len(d.name.split(".")) == 2:  # just default translation
      name_filter_key_paths.append(d.path)

  name_filter_key_paths = [(s, lambda k, _: character in k.lower()) for s in name_filter_key_paths]

  # Data/Quests.json
  data_quests_path = os.path.join(content_folder, "Data", "Quests.json")

  special_filters = [(data_quests_path, lambda _, v: quests_filter(character, v))]

  # Data/Events/*.json
  for d in os.scandir(os.path.join(content_folder, "Data", "Events")):
    if len(d.name.split(".")) == 2:  # just default translation
      special_filters.append((d.path, lambda _, v: events_filter(character, v)))


  # open files
  opened_fps = []
  for p, fn in all_keys_paths + name_filter_key_paths + special_filters:
    if os.path.exists(p):
      opened_fps.append((open(p), fn))
      print(p)

  for fp, fn in opened_fps:
    get_file_data(fp, data, fn)
    fp.close()


  return data



def main(unpacked_content_path, outfile, characters: list[str]):
  content_folder = os.path.abspath(unpacked_content_path)

  result = {}

  skipped = []

  for character in characters:
    character = character.lower()
    if character not in all_characters:
      skipped.append(character)
      continue

    if character in result:  # don't double-process
      continue

    result[character] = get_data_for_character(character, content_folder)

  with open(outfile, "w") as out:
    json.dump(result, out, indent=2)

  if skipped:
    print("The following character names were skipped because they were not recognized:", ", ".join(skipped))


if __name__ == "__main__":
  parser = argparse.ArgumentParser()
  parser.add_argument("--characters", help="list of characters to get dialogue for", nargs="+", default=all_characters)
  parser.add_argument("--content_unpacked", help="path to the Content (unpacked) folder", default="")
  parser.add_argument("--outfile", help="path to the JSON file to write output to", default="portrait-dialogues.json")
  args = parser.parse_args()
  main(args.content_unpacked, args.outfile, characters=args.characters)