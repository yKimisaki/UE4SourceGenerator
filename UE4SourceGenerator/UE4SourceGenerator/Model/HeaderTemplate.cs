namespace UE4SourceGenerator.Model
{
    public partial class TemplateCollector
    {
        class HeaderTemplate : ITemplate
        {
            HeaderType headerType;
            string content;

            public static ITemplate StructTemplate => new HeaderTemplate(HeaderType.Struct, "");
            public static ITemplate EnumTemplate => new HeaderTemplate(HeaderType.Enum, "");

            public HeaderTemplate(HeaderType headerType, string content)
            {
                this.headerType = headerType;
                this.content = content;
            }

            public string Generate(TemplateReplacement replacement, GenerateTo generateTo)
            {
                return headerType switch
                {
                    HeaderType.Object when replacement.TypeName.HasObjectTypePrefix() => content.Replace(replacement),
                    HeaderType.Actor when replacement.TypeName.HasActorTypePrefix() => content.Replace(replacement),
                    HeaderType.Struct when replacement.TypeName.HasStructTypePrefix() => generateTo switch
                    {
                        GenerateTo.File => StructToFile.Replace(replacement),
                        GenerateTo.Clipboard => StructToClipboard.Replace(replacement),
                        _ => throw new SourceGenerateException("Unsupported GenerateTo enum type."),
                    },
                    HeaderType.Enum when replacement.TypeName.HasEnumTypePrefix() => generateTo switch
                    {
                        GenerateTo.File => EnumToFIle.Replace(replacement),
                        GenerateTo.Clipboard => EnumToClipboard.Replace(replacement),
                        _ => throw new SourceGenerateException("Unsupported GenerateTo enum type."),
                    },
                    _ => throw new SourceGenerateException("Type name prefix and base type prefix is not same."),
                };
            }

            const string StructToClipboard = @"USTRUCT(BlueprintType)
struct {PROJECT_API} {TypeName}
{
    GENERATED_BODY()
};";

            const string StructToFile = @"#pragma once

#include ""CoreMinimal.h""
#include ""{FileName}.generated.h""

USTRUCT(BlueprintType)
struct {PROJECT_API} {TypeName}
{{
    GENERATED_BODY()
}};";

            const string EnumToClipboard = @"UENUM(BlueprintType)
enum class {TypeName} : uint8
{
    None = 0,
};";

            const string EnumToFIle = @"#pragma once

#include ""CoreMinimal.h""

UENUM(BlueprintType)
enum class {TypeName} : uint8
{
    None = 0,
};";
        }

        enum HeaderType
        {
            Object = 0,
            Actor = 1,
            Struct = 2,
            Enum = 3,
        }

        static HeaderType GetHeaderType(string fileName)
        {
            if (fileName.HasObjectTypePrefix()) return HeaderType.Object;
            if (fileName.HasActorTypePrefix()) return HeaderType.Actor;

            throw new TemplateCollectException("Templace file name prefix must be U or A.");
        }
    }
}
