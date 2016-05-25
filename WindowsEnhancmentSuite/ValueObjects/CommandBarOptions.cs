using System;
using System.Collections.Generic;

namespace WindowsEnhancementSuite.ValueObjects
{
    public class CommandBarOptions
    {
        public Func<IEnumerable<string>> ExplorerHistoryFunc { get; set; }
        public IEnumerable<string> CommandHistoryList { get; set; }
    }
}
