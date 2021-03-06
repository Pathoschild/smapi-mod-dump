import os.path
import json
import csv
from collections import OrderedDict
from file_info import files_to_convert

non_starters = ["TODO", "UNOBTAINABLE", "NON-INVENTORY", "ALREADY UNSETTLING"]
generic_shirt = "Shirt/Shirt/A wearable shirt./0/-1/50/255 255 255/false/Shirt"

cp_json_object = OrderedDict({"Changes": []})

# Add Fields patches for each file
for entry in files_to_convert:
    target_name = entry[0]
    desc_index = entry[3]
    config_token = entry[4]

    # Create patch object
    new_patch = OrderedDict()

    if target_name == 'StringsFromCSFiles':
        new_patch["LogName"] = "Edit " + target_name
    else:
        new_patch["LogName"] = "Edit " + target_name + " descriptions"
    new_patch["Action"] = "EditData"
    if target_name == 'StringsFromCSFiles':
        new_patch["Target"] = "Strings/" + target_name
    else:
        new_patch["Target"] = "Data/" + target_name

    # Check if this filled csv file exists
    if not os.path.isfile('csv_filled/Unsettling Item Descriptions - ' + target_name + '.csv'):
        continue

    # Read and enter data from each csv file
    with open('csv_filled/Unsettling Item Descriptions - ' + target_name + '.csv') as csv_file:
        print(target_name)
        reader = csv.DictReader(csv_file)

        if target_name == 'StringsFromCSFiles':  # or target_name == "ClothingInformation":
            new_patch["Entries"] = OrderedDict()
        if target_name != 'StringsFromCSFiles':
            new_patch["Fields"] = OrderedDict()

        for row in reader:

            hasDesc = True
            if row["unsettling_description"] == "":
                hasDesc = False
            for text in non_starters:
                if row["unsettling_description"].startswith(text):
                    hasDesc = False

            if hasDesc:
                # Add to Entries for StringsFromCSFiles only
                if target_name == 'StringsFromCSFiles':
                    new_patch["Entries"][row["key"]] = row["unsettling_description"]
                    continue  # Skip remaining non-tool logic

                # # Manually add entry for generic shirts to allow custom description
                # if target_name == "ClothingInformation" and row["name"] == "Shirt":
                #     new_patch["Entries"][row["key"]] = generic_shirt

                # Skip all the shirts without entries for now
                if target_name == "ClothingInformation" and row["key"] != "-2" and row["name"] == "Shirt":
                    continue

                # Add the unsettling description (for Shirts and all other non-tool items)
                new_patch["Fields"][row["key"]] = {desc_index: row["unsettling_description"]}

    # Add When conditions for config
    new_patch["When"] = OrderedDict()
    new_patch["When"][config_token] = True

    # Add patch
    fields = new_patch.get("Fields")
    entries = new_patch.get("Entries")
    if fields is not None and len(fields) > 0 or entries is not None and len(entries) > 0:
        cp_json_object["Changes"].append(new_patch)

# # Special logic for StringsFromCSFiles Entries patch
# target_name = 'StringsFromCSFiles'
# if os.path.isfile('csv_filled/Unsettling Item Descriptions - ' + target_name + '.csv'):
#
#     # Create patch object
#     new_patch = OrderedDict()
#
#     new_patch["LogName"] = "Edit " + target_name
#     new_patch["Action"] = "EditData"
#     new_patch["Target"] = "Strings/" + target_name
#     new_patch["Entries"] = OrderedDict()
#
#     # Read and enter data from the csv file
#     with open('csv_filled/Unsettling Item Descriptions - ' + target_name + '.csv') as csv_file:
#         print(target_name)
#         reader = csv.DictReader(csv_file)
#
#         for row in reader:
#
#             hasDesc = True
#             if row["unsettling_description"] == "":
#                 hasDesc = False
#             for text in non_starters:
#                 if row["unsettling_description"].startswith(text):
#                     hasDesc = False
#
#             if hasDesc:
#                 new_patch["Entries"][row["key"]] = row["unsettling_description"]
#
#     # Add patch
#     if len(new_patch["Entries"]) > 0:
#         cp_json_object["Changes"].append(new_patch)

# Write to .json file
with open('default.json', 'w') as cp_file:
    json.dump(cp_json_object, cp_file, indent=2, ensure_ascii=False)


# with open('default.json', mode='w', newline='') as cp_file:
#     # Open Changes
#     cp_file.writelines([
#         '{',
#         '  "Changes": ['
#     ])
#
#     for entry in files_to_convert:
#         target_name = entry[0]
#         desc_index = entry[3]
#
#         # Check if this filled csv file exists
#         if not os.path.isfile('csv_filled/' + target_name + '.csv'):
#             continue
#
#         # Open patch
#         cp_file.writelines([
#             '    {',
#             f'      "LogName": "Edit {target_name} descriptions",',
#             f'      "Action": "EditData",',
#             f'      "Target": "Data/{target_name}",',
#             '      "Fields": {'
#         ])
#
#         # Read and enter data from each csv file
#         with open('csv_filled/' + target_name + '.csv') as csv_file:
#             reader = csv.DictReader(csv_file)
#
#             line_count = 0
#             for row in reader:
#
#                 hasDesc = True
#                 if row["unsettling_description"] == "":
#                     hasDesc = False
#                 for text in non_starters:
#                     if row["unsettling_description"].startswith(text):
#                         hasDesc = False
#
#                 if line_count != 0 and hasDesc:
#                     cp_file.write(
#                         f'        "{row["key"]}": {{ {desc_index}: "{row["unsettling_description"]}" }}, //{row["name"]}'
#                     )
#                 line_count += 1
#
#         # Close patch
#         cp_file.writelines([
#             '      }',
#             '    },'
#         ])
#
#     # Close Changes
#     cp_file.writelines(['  ]', '}'])

