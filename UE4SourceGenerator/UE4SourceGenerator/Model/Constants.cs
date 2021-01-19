namespace UE4SourceGenerator.Model
{
    public static class Constants
    {
        public const string StructTypeKey = "USTRUCT";
        public const string EnumTypeKey = "UENUM";

        public const string TemplateDirectoryName = "SourceGeneratorTamplates";

        public static readonly char[] SupportPrefixes = new[] { 'U', 'A', 'F', 'E' };
    }
}
