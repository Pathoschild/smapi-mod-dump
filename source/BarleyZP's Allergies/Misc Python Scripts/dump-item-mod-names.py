import argparse, json, csv, re

def main(fromfile, outfile):
  with open(fromfile) as i, open(outfile, "w", newline='') as o:
    data = json.load(i)
    fields = ["id", "display_name", "from_mod_guess"]
    writer = csv.writer(o)
    writer.writerow(fields)
    
    for k, v in data.items():
      guess1 = k.split("_")
      guess2 = k.split(".")
      if len(guess1) > 1:
        g = guess1[0]
      elif len(guess2) > 1:
        g = guess2[0]
      else:
        g = "Base game or unknown"
        
      row = [k, v["DisplayName"], g]
      writer.writerow(row)
    
    

if __name__ == "__main__":
  parser = argparse.ArgumentParser()
  parser.add_argument("--fromfile", help="path to the patch export of Data/Objects", required=True)
  parser.add_argument("--outfile", help="path to the CSV file to write output to", default="item-sources.csv")
  args = parser.parse_args()
  main(args.fromfile, args.outfile)