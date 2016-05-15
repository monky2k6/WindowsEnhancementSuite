using System.IO;
using WindowsEnhancementSuite.Enums;

namespace WindowsEnhancementSuite.ValueObjects
{
    public class CommandBoxEntry
    {
        public string Command { get; private set; }
        public string Name { get; private set; }
        public CommandEntryKind Kind { get; set; }

        public CommandBoxEntry(string command) : this(command, CommandEntryKind.Command)
        {
        }

        public CommandBoxEntry(string command, CommandEntryKind kind)
        {
            this.Command = command;
            this.Name = Path.GetFileName(command);
            this.Kind = kind;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
