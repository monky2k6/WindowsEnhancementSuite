using WindowsEnhancementSuite.Services;

namespace WindowsEnhancementSuite.ValueObjects
{
    public class CommandBarOptions
    {
        public ExplorerBrowserService ExplorerService { get; set; }
        public string[] SystemSearchPaths { get; set; }
    }
}
