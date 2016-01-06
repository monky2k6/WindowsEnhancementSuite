using System;
using System.Threading;

namespace WindowsEnhancementSuite.Helper
{
    public static class ThreadHelper
    {
        public static void RunAsStaThread(Action synchronAction)
        {
            var thread = new Thread(synchronAction.Invoke) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
