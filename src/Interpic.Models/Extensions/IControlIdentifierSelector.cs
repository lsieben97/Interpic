namespace Interpic.Models.Extensions
{
    public interface IControlIdentifierSelector
    {
        ControlIdentifier ControlIdentifier { get; set; }

        Section Section { get; set; }

        void ShowSelector();
    }
}
