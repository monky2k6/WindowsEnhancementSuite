using WindowsEnhancementSuite.Enums;
using WindowsEnhancementSuite.Extensions;
using WindowsEnhancementSuite.Helper;

namespace WindowsEnhancementSuite.ValueObjects
{
    public class CommandBarEntry
    {
        public string Command { get; private set; }
        public string Name { get; private set; }
        public CommandEntryKind Kind { get; private set; }
        public string Hash { get; private set; }
        public uint Rank
        {
            get
            {
                return RankingHelper.GetRank(this);
            }
        }

        public CommandBarEntry(string command, CommandEntryKind kind, string name = "")
        {
            this.Command = command;            
            this.Kind = kind;
            this.Hash = command.GetMd5Hash();
            this.Name = StringHelper.GetFirstNonEmpty(name, command);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
