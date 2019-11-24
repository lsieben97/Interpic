namespace Interpic.Models.Extensions
{
    public interface ISectionIdentifierSelector
    {
        SectionIdentifier SectionIdentifier { get; set; }
        Page Page { get; set; }

        void ShowSelector();
    }
}
