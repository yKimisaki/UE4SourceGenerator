namespace UE4SourceGenerator.Model
{
    public readonly struct TemplateReplacement
    {
        public readonly string ProjectApi { get; init; }
        public readonly string TypeName { get; init; }
        public readonly string FileName { get; init; }
    }

    public enum GenerateTo
    {
        File = 0,
        Clipboard = 1,
    }

    public interface ITemplate
    {
        string Generate(TemplateReplacement replacement, GenerateTo generateTo);
    }
}
