﻿namespace Interpic.Studio.Functional
{
    public static class Extensions
    {
        //public static List<Interpic.Extensions.Extension> LoadedExtensions = new List<Interpic.Extensions.Extension>();

        //public static bool GlobalExtensionsAvailable()
        //{
        //    if (!File.Exists(App.EXECUTABLE_DIRECTORY + "\\" + App.GLOBAL_EXTENSION_FILE))
        //    {
        //        return false;
        //    }

        //    if (GetGlobalExtensions().Count == 0)
        //    {
        //        return false;
        //    }
        //    return true;

        //}

        //public static List<ExtensionDeclaration> GetGlobalExtensions()
        //{
        //    try
        //    {
        //        return JsonConvert.DeserializeObject<List<ExtensionDeclaration>>(File.ReadAllText(App.EXECUTABLE_DIRECTORY + "\\" + App.GLOBAL_EXTENSION_FILE));
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorAlert.Show("Could not load global extensions:\n" + ex.Message);
        //    }
        //    return new List<ExtensionDeclaration>();
        //}

        //public static List<Interpic.Extensions.Extension> GetExtensionsFromDll(string path)
        //{
        //    List<Interpic.Extensions.Extension> extensions = new List<Interpic.Extensions.Extension>();
        //    Assembly assembly = null;
        //    try
        //    {
        //        assembly = SafeExtensionManager.GetTempExtensionDomain().Load(path);
        //        foreach( TypeInfo type in assembly.DefinedTypes)
        //        {
        //            // get all types who inherit from Extension
        //            if (type.BaseType == typeof(Interpic.Extensions.Extension))
        //            {
        //                Interpic.Extensions.Extension extension = assembly.CreateInstance(type.FullName) as Interpic.Extensions.Extension;
        //                extensions.Add(extension);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (assembly != null)
        //        {
        //            SafeExtensionManager.UnloadTempExtensionDomain();
        //        }
        //        ErrorAlert.Show(String.Format("Could not load Extension {0}:\n{1}", path, ex.Message));

        //    }
        //    return extensions;
        //}

        //internal static bool LoadExtension(string path, Interpic.Extensions.IExtensionRegistry registry)
        //{
        //    Assembly assembly = null;
        //    try
        //    {
        //        assembly = SafeExtensionManager.GetTempExtensionDomain().Load(path);
        //        foreach (TypeInfo type in assembly.DefinedTypes)
        //        {
        //            // get all types who inherit from Extension
        //            if (type.BaseType == typeof(Interpic.Extensions.Extension))
        //            {
        //                Interpic.Extensions.Extension extension = assembly.CreateInstance(type.FullName) as Interpic.Extensions.Extension;
        //                extension.ExtensionRegistry = registry;
        //                LoadedExtensions.Add(extension);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (assembly != null)
        //        {
        //            SafeExtensionManager.UnloadTempExtensionDomain();
        //        }
        //        ErrorAlert.Show(String.Format("Could not load Extension {0}:\n{1}\nUnloading extension.", path, ex.Message));
        //        return false;
        //    }
        //    return true;
        //}

    }
}
