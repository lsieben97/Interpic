using Interpic.Models.Extensions;

namespace Interpic.Models
{
    public abstract class Extension
    {
        /// <summary>
        /// Get the name of the extension.
        /// </summary>
        /// <returns>The name of the extension.</returns>
        public abstract string GetName();

        /// <summary>
        /// Get a description of the extension.
        /// </summary>
        /// <returns>A description of the extension.</returns>
        public abstract string GetDescription();

        /// <summary>
        /// Get the type of extension.
        /// </summary>
        /// <returns>The type of extension.</returns>
        public abstract ExtensionType GetExtensionType();

        /// <summary>
        /// Extension registry to register this extension.
        /// </summary>
        public IExtensionRegistry ExtensionRegistry { get; set; }
    }
}
