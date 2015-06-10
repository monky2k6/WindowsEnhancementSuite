﻿using System;
using System.Threading;

namespace WindowsEnhancementSuite.Extensions
{
    public static class VoidExtensions
    {
        public static void RunAsStaThread(this Action synchronAction)
        {
            var thread = new Thread(() => synchronAction());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}