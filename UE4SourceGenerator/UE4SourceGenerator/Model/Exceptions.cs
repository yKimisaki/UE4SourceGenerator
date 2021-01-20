using System;

namespace UE4SourceGenerator.Model
{
    public class TemplateCollectException : Exception
    {
        public TemplateCollectException(string message) : base(message) { }
    }

    public class SourceGenerateException : Exception
    {
        public SourceGenerateException(string message) : base(message) { }
    }
}
