using Interpic.Models;
using Interpic.Models.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models.Extensions
{
    public interface IDLLManager
    {
        /// <summary>
        /// Checks if the given assembly is present in the trusted assembly list, if not it will ask the user for permission to load the assembly and load it.
        /// </summary>
        /// <param name="path">The path to the assembly to load.</param>
        /// <param name="requestingExtension">The extension that is requesting to load the given assembly.</param>
        /// <returns>A <see cref="LoadedAssembly"/> object when loading succeeded or <code>null</code> if loading failed. (or user prevented the load)</returns>
        LoadedAssembly LoadAssembly(string path, Extension requestingExtension);

        /// <summary>
        /// Checks if the given assembly is present in the trusted assembly list, if not it will ask the user for permission to load the assembly and load it.
        /// </summary>
        /// <param name="path">The path to the assembly to load.</param>
        /// <param name="requestingExtension">The extension that is requesting to load the given assembly.</param>
        /// <returns>A <see cref="LoadedAssembly"/> object when loading succeeded or <code>null</code> if loading failed. (or user prevented the load)</returns>
        LoadedAssembly LoadAssembly(string path, Extension requestingExtension, PackageDefinition packagingDefinition);

        /// <summary>
        /// Checks if the given assemblies are present in the trusted assembly list, if not it will ask the user for permission to load the assemblies and load it.
        /// </summary>
        /// <param name="path">The paths to the assemblies to load.</param>
        /// <param name="requestingExtension">The extension that is requesting to load the given assemblies.</param>
        /// <returns>A list of <see cref="LoadedAssembly"/> objects when loading succeeded. Only successfully loaded assemblies will be returned.</returns>
        List<LoadedAssembly> LoadAssemblies(List<string> paths, Extension requestingExtension);
        List<LoadedAssembly> LoadAssemblies(List<string> paths, Extension requestingExtension, PackageDefinition packagingDefinition);

        /// <summary>
        /// Unloads the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to unload.</param>
        /// <returns>Whether the assembly was successfully unloaded.</returns>
        bool UnLoadAssembly(LoadedAssembly assembly);

        /// <summary>
        /// Unloads the given assemblies.
        /// </summary>
        /// <param name="assembly">The assemblies to unload.</param>
        /// <returns>A list to show whether the assemblies where successfully unloaded.</returns>
        List<bool> UnloadAssemblies(List<LoadedAssembly> assemblies);

        List<LoadedAssembly> GetLoadedAssemblies(Extension extension);

        List<string> GetLoadedPackages();
    }
}
