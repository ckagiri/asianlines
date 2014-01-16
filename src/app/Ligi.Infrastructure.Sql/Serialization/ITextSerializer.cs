using System.IO;

namespace Ligi.Infrastructure.Sql.Serialization
{
    public interface ITextSerializer
    {
        void Serialize(TextWriter writer, object graph);
        object Deserialize(TextReader reader);
    }
}
