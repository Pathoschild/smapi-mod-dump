using System.Collections.Generic;
using System.IO;

namespace HDSprites.Token
{
    public class TokenEntry
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public bool IsConditional { get; set; }
        public StringWithTokens Condition { get; set; }

        public TokenEntry(string name, string value)
        {
            this.Values = new List<string>();
            foreach (string str in value.Split(','))
            {
                this.Values.Add(str.StartsWith(" ") ? str.Substring(1) : str);
            }
            
            string[] splitName = name.Split(':');
            this.Name = splitName[0];
            if (splitName.Length > 1)
            {
                this.Condition = new StringWithTokens(splitName[1]);
                this.IsConditional = true;
            }
        }

        public bool IsEnabled(DynamicTokenManager manager)
        {
            switch(this.Name.ToLower())
            {                
                case "hasmod": return true; // TODO: Add support for checking UniqueID of installed mods
                case "hasfile": return File.Exists(Path.Combine(HDSpritesMod.ModHelper.DirectoryPath, this.Condition.Parse(manager.DynamicTokens).ToCleanString())) == this.Values[0].ToLower().Equals("true");
                case "hasvalue": return !this.Condition.Parse(manager.DynamicTokens).ToCleanString().Equals("") == this.Values[0].ToLower().Equals("true");
            } 

            DynamicToken token = null;
            foreach (var entry in manager.DynamicTokens)
            {
                if (entry.Key.ToLower().Equals(this.Name.ToLower()))
                {
                    token = entry.Value;
                    break;
                }
            }

            if (token != null)
            {
                if (!this.IsConditional)
                {
                    foreach (string value in this.Values)
                    {
                        if (token.GetValue().ToLower().Equals(value.ToLower()))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    string condition = this.Condition.Parse(manager.DynamicTokens).ToCleanString().ToLower();
                    foreach (var value in token.GetValues())
                    {
                        if (value.SubValues.Count > 0)
                        {
                            // "SkillLevel:Combat": "1, 2, 3"
                            //
                            // this.Values should be ~ 1, 2, 3
                            // value.SubValues should be 1, 2, 3, 4, 5

                            if (value.Value.ToLower().Equals(condition))
                            {
                                foreach (string condVal in this.Values)
                                {
                                    bool found = false;
                                    foreach (string subVal in value.SubValues)
                                    {
                                        if (condVal.ToLower().Equals(subVal.ToLower()))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // "PlayerGender:Male": "true"
                            // 
                            // this.Values should be "true" or "false"
                            // this.SubValues should be empty
                            if (value.Value.ToLower().Equals(condition) == this.Values[0].ToLower().Equals("true"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
