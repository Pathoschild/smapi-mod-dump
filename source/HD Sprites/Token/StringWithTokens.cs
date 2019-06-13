using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HDSprites.Token
{
    public class StringWithTokens
    {
        private static Regex RX = new Regex(@"{{(?<Key>\w+)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string RawString { get; set; }

        public StringWithTokens(string str)
        {
            this.RawString = str;
        }

        public List<string> GetTokenNames()
        {
            List<string> tokens = new List<string>();
            foreach (Match match in RX.Matches(this.RawString))
            {
                string[] splitName = match.Groups["Key"].Value.Split(':');
                if (!tokens.Contains(splitName[0]))
                {
                    tokens.Add(splitName[0]);
                }
            }
            return tokens;
        }

        public StringWithTokens Parse(Dictionary<string, DynamicToken> dynamicTokens)
        {
            return Parse(dynamicTokens, "");
        }

        public StringWithTokens Parse(Dictionary<string, DynamicToken> dynamicTokens, string target)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            foreach (Match match in RX.Matches(this.RawString))
            {
                string tokenName = match.Groups["Key"].Value;
                string[] splitName = tokenName.Split(':');

                DynamicToken token = null;
                foreach (var entry in dynamicTokens) {
                    if (entry.Key.ToLower().Equals(splitName[0].ToLower())) {
                        token = entry.Value;
                        break;
                    }
                }

                if (!replacements.ContainsKey("{{" + tokenName + "}}"))
                {
                    if (token != null)
                    {
                        string output;
                        if (splitName.Length > 1)
                        {
                            bool contains = false;
                            foreach (string value in token.GetSimpleValues())
                            {
                                if (value.ToLower().Equals(splitName[1].ToLower()))
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            output = contains.ToString().ToLower();
                        }
                        else
                        {
                            output = token.GetValue();
                        }

                        replacements.Add("{{" + tokenName + "}}", output);
                    }
                    else if (!target.Equals(""))
                    {
                        if (tokenName.ToLower().Equals("target"))
                        {
                            replacements.Add("{{" + tokenName + "}}", target);
                        }
                        else if (tokenName.ToLower().Equals("targetwithoutpath"))
                        {
                            int index = target.LastIndexOf("\\") + 1;
                            replacements.Add("{{" + tokenName + "}}", target.Substring(index, target.Length - index));
                        }
                    }
                }                
            }

            string parsed = this.RawString;
            foreach (var entry in replacements)
            {
                parsed = parsed.Replace(entry.Key, entry.Value);
            }

            return new StringWithTokens(parsed);
        }

        public string ToCleanString()
        {
            string clean = this.RawString;
            List<string> replacements = new List<string>();
            foreach (Match match in RX.Matches(this.RawString))
            {
                clean = clean.Replace("{{" + match.Groups["Key"].Value + "}}", "");
            }
            return clean;
        }
    }
}
