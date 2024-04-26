import os
import shutil
import zipfile
from datetime import datetime

# Create the "old_zips" directory if it doesn't exist
if not os.path.exists("old_zips"):
    os.makedirs("old_zips")

# Get the current date and time
current_datetime = datetime.now().strftime("%Y-%m-%d-%H-%M-%S")

# Iterate over each folder in the current directory
for folder_name in os.listdir():
    if os.path.isdir(folder_name) and folder_name != "old_zips":  # Skip the "old_zips" directory
        # Create the zip file name
        zip_file_name = folder_name + ".zip"

        # Check if the zip file already exists
        if os.path.exists(zip_file_name):
            # Rename the old zip file with the current date and time
            new_zip_file_name = folder_name + "-" + current_datetime + ".zip"
            os.rename(zip_file_name, new_zip_file_name)

            # Move the old zip file to the "old_zips" directory
            shutil.move(new_zip_file_name, os.path.join("old_zips", new_zip_file_name))

        # Check if the folder or file contains "-ignore-"
        if "-ignore-" not in folder_name:
            # Create a new zip file for the folder
            with zipfile.ZipFile(zip_file_name, "w") as zipf:
                # Add all files and subdirectories to the zip file
                for root, dirs, files in os.walk(folder_name):
                    for file in files:
                        file_path = os.path.join(root, file)
                        zipf.write(file_path, os.path.join(folder_name, os.path.relpath(file_path, folder_name)))

                # Add empty directories to the zip file
                for root, dirs, files in os.walk(folder_name):
                    for dir in dirs:
                        dir_path = os.path.join(root, dir)
                        zipf.write(dir_path, os.path.join(folder_name, os.path.relpath(dir_path, folder_name)))
