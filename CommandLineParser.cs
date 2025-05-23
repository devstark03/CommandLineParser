using System.Collections.Generic;
using System.Linq;

namespace StarKNET.CommandLine {
    public class CommandLineParser {
        private readonly List<string> _args;
        public List<string> Items = new List<string>();
        public CommandLineParser(string[] args) {
            _args = args.ToList();
            foreach (var arg in args.Where(z => z.StartsWith("[") && z.EndsWith("]"))) {
                Items.Add(arg.Replace("[", string.Empty).Replace("]", string.Empty));
            }
        }
        public string GetStringArgument(string key, char shortKey) {
            var index = _args.IndexOf("--" + key);
            if (index >= 0 && _args.Count > index) {
                return _args[index + 1];
            }

            index = _args.IndexOf("-" + shortKey);

            if (index >= 0 && _args.Count > index) {
                return _args[index + 1];
            }

            return null;
        }

        public bool GetSwitchArgument(string value) {
            return _args.Contains("--" + value);
        }
    }

}