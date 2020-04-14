#!/bin/bash
set -$-ue"${DEBUG+xv}"

cat > days.json <<EOT
{
    "SnapshotRules": [
EOT
for j in Spring Summer Fall Winter; do
    for i in {1..28}; do
        cat >> days.json <<EOT
        {
            "Name": "Days No Overlap Season $j Day $i",
            "ZoomLevel": 0.1,
            "Directory": "Default",
            "FileName": "GameID, Date",
            "Trigger": {
                "Days": "$j, Day_$i",
                "Weather": "Any",
                "Location": "Any",
                "Key": "None",
                "StartTime": 600,
                "EndTime": 2600
            }
        },
EOT
    done
    for k in Sundays Mondays Tuesdays Wednesdays Thursdays Fridays Saturdays FirstDayOfTheMonth LastDayOfTheMonth; do
        cat >> days.json <<EOT
        {
            "Name": "Days Overlap Season $j $k",
            "ZoomLevel": 0.1,
            "Directory": "Default",
            "FileName": "GameID, Date",
            "Trigger": {
                "Days": "$j, $k",
                "Weather": "Any",
                "Location": "Any",
                "Key": "None",
                "StartTime": 600,
                "EndTime": 2600
            }
        },
EOT
    done
done

for i in AnyDay AnySeason; do
    cat >> days.json <<EOT
        {
            "Name": "Days Inactive $i",
            "ZoomLevel": 0.1,
            "Directory": "Default",
            "FileName": "GameID, Date",
            "Trigger": {
                "Days": "$i",
                "Weather": "Any",
                "Location": "Any",
                "Key": "None",
                "StartTime": 600,
                "EndTime": 2600
            }
        },
EOT
done

cat >> days.json <<EOT
        {
            "Name": "Days Overlap All",
            "ZoomLevel": 0.1,
            "Directory": "Default",
            "FileName": "GameID, Date",
            "Trigger": {
                "Days": "Daily",
                "Weather": "Any",
                "Location": "Any",
                "Key": "None",
                "StartTime": 600,
                "EndTime": 2600
            }
        }
    ]
}
EOT