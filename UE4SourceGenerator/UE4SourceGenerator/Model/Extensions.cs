using System;
using System.Linq;

namespace UE4SourceGenerator.Model
{
    public static class Extensions
    {
        public static bool IsValueTypeKey(this string typeKey)
        {
            return typeKey == Constants.StructTypeKey || typeKey == Constants.EnumTypeKey;
        }

        public static bool IsValidTypeName(this string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return false;

            if (typeName.Length == 1)
                return Constants.SupportPrefixes.Contains(typeName[0]);

            return char.IsUpper(typeName[1]) && Constants.SupportPrefixes.Contains(typeName[0]);
        }

        public static bool HasObjectTypePrefix(this string typeName)
        {
            return IsValidTypeName(typeName) && (typeName[0] == 'U');
        }

        public static bool HasActorTypePrefix(this string typeName)
        {
            return IsValidTypeName(typeName) && (typeName[0] == 'A');
        }

        public static bool HasStructTypePrefix(this string typeName)
        {
            return IsValidTypeName(typeName) && (typeName[0] == 'F');
        }

        public static bool HasEnumTypePrefix(this string typeName)
        {
            return IsValidTypeName(typeName) && (typeName[0] == 'E');
        }

        public static string Replace(this string template, TemplateReplacement replacement)
        {
            return template
                .Replace("{PROJECT_API}", replacement.ProjectApi)
                .Replace("{TypeName}", replacement.TypeName)
                .Replace("{FileName}", replacement.FileName);
        }
    }
}
