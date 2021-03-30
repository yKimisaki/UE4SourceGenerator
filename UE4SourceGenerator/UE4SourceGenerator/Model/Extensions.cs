using System;
using System.IO;
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

        public static string GetPrefix(this string typeNameOrTypeKey)
        {
            if (typeNameOrTypeKey.IsValueTypeKey())
            {
                if (typeNameOrTypeKey == Constants.StructTypeKey)
                    return "F";
                if (typeNameOrTypeKey == Constants.EnumTypeKey)
                    return "E";
            }

            if (typeNameOrTypeKey.IsValidTypeName())
                return typeNameOrTypeKey[0].ToString();

            return "U";
        }

        public static string CorrectTypeName(this string originalTypeName, string baseType)
        {
            var baseTypePrefix = baseType.GetPrefix();

            if (!originalTypeName.IsValidTypeName())
            {
                if (!string.IsNullOrWhiteSpace(originalTypeName) && originalTypeName.Length > 1)
                {
                    if (!char.IsUpper(originalTypeName[1]))
                        return originalTypeName.GetPrefix() + originalTypeName;
                    else
                        return originalTypeName.GetPrefix() + originalTypeName.Substring(1);
                }
                return baseTypePrefix;
            }

            if (originalTypeName.GetPrefix() == baseTypePrefix)
            {
                return originalTypeName;
            }

            return baseTypePrefix + originalTypeName.Substring(1);
        }

        public static string ToPrivateDirectory(this string publicDirectory)
        {
            return publicDirectory.Replace("/Public", "/Private").Replace("\\Public", "\\Private");
        }
    }
}
