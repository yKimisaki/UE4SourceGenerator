namespace UE4SourceGenerator.Model
{
    public partial class TemplateCollector
    {
        class SourceTemplate : ITemplate
        {
            string content;

            public SourceTemplate(string content)
            {
                this.content = content;
            }

            public string Generate(TemplateReplacement replacement, GenerateTo generateTo)
            {
                if (replacement.TypeName.HasObjectTypePrefix() || replacement.TypeName.HasActorTypePrefix())
                {
                    return content.Replace(replacement);
                }

                throw new SourceGenerateException("A prefix is required U or A when generate .cpp file.");
            }
        }
    }
}
