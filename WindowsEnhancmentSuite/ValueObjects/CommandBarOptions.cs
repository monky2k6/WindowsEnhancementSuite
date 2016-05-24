using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WindowsEnhancementSuite.ValueObjects
{
    public class CommandBarOptions
    {
        public ReadOnlyCollection<CommandBarEntry> ExplorerHistory { get; set; }
        public IEnumerable<string> CommandHistory { get; set; }
    }
}
