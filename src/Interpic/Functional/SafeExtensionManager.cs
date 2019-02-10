using Interpic.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Studio.Functional
{
    internal static class SafeExtensionManager
    {
        private static AppDomain extensionDomain;
        private static AppDomain tempExtensionDomain;

        public static AppDomain GetExtensionDomain()
        {
            if (extensionDomain == null)
            {
                extensionDomain = AppDomain.CreateDomain("interpicExtensions");
            }
            return extensionDomain;
        }

        public static AppDomain GetTempExtensionDomain()
        {
            if (tempExtensionDomain == null)
            {
                tempExtensionDomain = AppDomain.CreateDomain("interpicTempExtensions");
            }
            return tempExtensionDomain;
        }

        public static void UnloadTempExtensionDomain()
        {
            GC.Collect(); // collects all unused memory
            GC.WaitForPendingFinalizers(); // wait until GC has finished its work
            GC.Collect();
            try
            {
                AppDomain.Unload(tempExtensionDomain);
            }
            catch(Exception ex)
            {
                ErrorAlert.Show("Could not unload extensions, exiting...");
                Environment.Exit(2);
            }
        }

        public static void UnloadExtensionDomain()
        {
            GC.Collect(); // collects all unused memory
            GC.WaitForPendingFinalizers(); // wait until GC has finished its work
            GC.Collect();
            try
            {
                AppDomain.Unload(extensionDomain);
            }
            catch (Exception ex)
            {
                ErrorAlert.Show("Could not unload extensions, exiting...");
                Environment.Exit(2);
            }
        }
    }
}
