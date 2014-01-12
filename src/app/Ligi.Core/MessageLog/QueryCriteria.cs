using System;
using System.Collections.Generic;

namespace Ligi.Core.MessageLog
{
    /// <summary>
    /// The query criteria for filtering events from the message log when reading.
    /// </summary>
    public class QueryCriteria
    {
        public QueryCriteria()
        {
            SourceTypes = new List<string>();
            SourceIds = new List<string>();
            AssemblyNames = new List<string>();
            Namespaces = new List<string>();
            FullNames = new List<string>();
            TypeNames = new List<string>();
        }

        public ICollection<string> SourceTypes { get; private set; }
        public ICollection<string> SourceIds { get; private set; }
        public ICollection<string> AssemblyNames { get; private set; }
        public ICollection<string> Namespaces { get; private set; }
        public ICollection<string> FullNames { get; private set; }
        public ICollection<string> TypeNames { get; private set; }
        public DateTime? EndDate { get; set; }
    }
}
