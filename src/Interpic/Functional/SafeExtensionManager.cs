using Interpic.Alerts;
using Interpic.Models;
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
        private static Dictionary<string, AppDomain> appDomains = new Dictionary<string, AppDomain>();

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

        public static AppDomain CreateAppDomainForAssembly(string path)
        {
            if (appDomains.ContainsKey(path))
            {
                return null;
            }

            AppDomain domain = AppDomain.CreateDomain(path);
            appDomains.Add(path, domain);
            return domain;
        }

        public static bool UnloadAppdomain(string path)
        {
            if (! appDomains.ContainsKey(path))
            {
                return false;
            }
            GC.Collect(); // collects all unused memory
            GC.WaitForPendingFinalizers(); // wait until GC has finished its work
            GC.Collect();
            try
            {
                AppDomain.Unload(appDomains[path]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
