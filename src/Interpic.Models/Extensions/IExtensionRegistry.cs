namespace Interpic.Models.Extensions
{
    public interface IExtensionRegistry
    {
        void RegisterProjectType(IProjectTypeProvider provider);
        void RegisterProjectBuilder(IProjectBuilder builder);
    }
}
