using System.IO;

namespace Ligi.Infrastructure.Sql.Serialization
{
    public static class TextSerializerExtensions
    {
        public static string Serialize<T>(this ITextSerializer serializer, T data)
        {
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(this ITextSerializer serializer, string serialized)
        {
            using (var reader = new StringReader(serialized))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
