import json
import csv
from file_info import files_to_convert

for entry in files_to_convert:
    with open('originals/' + entry[0] + '.json') as f:
        data = json.load(f)

        with open('csv_empty/' + entry[0] + '.csv', mode='w', newline='') as csv_file:
            fieldnames = ['key', 'name', 'unsettling_description', 'original_description']
            writer = csv.DictWriter(csv_file, fieldnames=fieldnames)

            writer.writeheader()

            for key in data:
                item_key = key
                details = data[key].split(entry[1])

                if len(details) > entry[2] and len(details) > entry[3]:
                    item_name = details[entry[2]]
                    item_desc = details[entry[3]]
                    writer.writerow({'key': item_key,
                                     'name': item_name,
                                     'unsettling_description': "TODO",
                                     'original_description': item_desc})

