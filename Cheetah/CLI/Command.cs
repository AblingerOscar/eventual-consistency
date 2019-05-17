using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheetah.CLI
{
    internal class Command
    {
        public string Name { get; }
        public Action<Arguments> Action { get; }
        public string Description { get; }
        public string[] Aliases { get; }

        public Command(string name, Action<Arguments> action, string description, string[] aliases)
        {
            Name = name;
            Action = action;
            Description = description;
            Aliases = aliases;
        }

        public override string ToString()
        {
            return $"{Name}: {Description.Replace("\n", "\n\t")}\n" +
                (Aliases.Length > 0
                    ? $"\tAliases: {Aliases.Aggregate((t, n) => t + ", " + n)}"
                    : "");
        }
    }
}
