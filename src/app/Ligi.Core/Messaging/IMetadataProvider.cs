using System.Collections.Generic;

namespace Ligi.Core.Messaging
{
    /// <summary>
    /// Extracts metadata about a payload so that it's placed in the 
    /// message envelope.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Gets metadata associated with the payload, which can be 
        /// used by processors to filter and selectively subscribe to 
        /// messages.
        /// </summary>
        IDictionary<string, string> GetMetadata(object payload);
    }
}
