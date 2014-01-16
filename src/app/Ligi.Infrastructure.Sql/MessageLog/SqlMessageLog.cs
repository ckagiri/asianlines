using Ligi.Core.Commands;
using Ligi.Core.Events;
using Ligi.Core.MessageLog;
using Ligi.Core.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ligi.Infrastructure.Sql.Common;
using Ligi.Infrastructure.Sql.Serialization;

namespace Ligi.Infrastructure.Sql.MessageLog
{
    public class SqlMessageLog : IEventLogReader
    {
        private readonly string _nameOrConnectionString;
        private readonly IMetadataProvider _metadataProvider;
        private readonly ITextSerializer _serializer;

        public SqlMessageLog(string nameOrConnectionString, ITextSerializer serializer, IMetadataProvider metadataProvider)
        {
            _nameOrConnectionString = nameOrConnectionString;
            _serializer = serializer;
            _metadataProvider = metadataProvider;
        }

        public void Save(IEvent @event)
        {
            using (var context = new MessageLogDbContext(_nameOrConnectionString))
            {
                var metadata = _metadataProvider.GetMetadata(@event);

                context.Set<MessageLogEntity>().Add(new MessageLogEntity
                {
                    Id = Guid.NewGuid(),
                    SourceId = @event.SourceId.ToString(),
                    Kind = metadata.TryGetValue(StandardMetadata.Kind),
                    AssemblyName = metadata.TryGetValue(StandardMetadata.AssemblyName),
                    FullName = metadata.TryGetValue(StandardMetadata.FullName),
                    Namespace = metadata.TryGetValue(StandardMetadata.Namespace),
                    TypeName = metadata.TryGetValue(StandardMetadata.TypeName),
                    SourceType = metadata.TryGetValue(StandardMetadata.SourceType) as string,
                    CreationDate = DateTime.UtcNow.ToString("o"),
                    Payload = _serializer.Serialize(@event),
                });
                context.SaveChanges();
            }
        }

        public void Save(ICommand command)
        {
            using (var context = new MessageLogDbContext(_nameOrConnectionString))
            {
                var metadata = _metadataProvider.GetMetadata(command);

                context.Set<MessageLogEntity>().Add(new MessageLogEntity
                {
                    Id = Guid.NewGuid(),
                    SourceId = command.Id.ToString(),
                    Kind = metadata.TryGetValue(StandardMetadata.Kind),
                    AssemblyName = metadata.TryGetValue(StandardMetadata.AssemblyName),
                    FullName = metadata.TryGetValue(StandardMetadata.FullName),
                    Namespace = metadata.TryGetValue(StandardMetadata.Namespace),
                    TypeName = metadata.TryGetValue(StandardMetadata.TypeName),
                    SourceType = metadata.TryGetValue(StandardMetadata.SourceType) as string,
                    CreationDate = DateTime.UtcNow.ToString("o"),
                    Payload = _serializer.Serialize(command),
                });
                context.SaveChanges();
            }
        }

        public IEnumerable<IEvent> Query(QueryCriteria criteria)
        {
            return new SqlQuery(_nameOrConnectionString, _serializer, criteria);
        }

        private class SqlQuery : IEnumerable<IEvent>
        {
            private readonly string _nameOrConnectionString;
            private readonly ITextSerializer _serializer;
            private readonly QueryCriteria _criteria;

            public SqlQuery(string nameOrConnectionString, ITextSerializer serializer, QueryCriteria criteria)
            {
                _nameOrConnectionString = nameOrConnectionString;
                _serializer = serializer;
                _criteria = criteria;
            }

            public IEnumerator<IEvent> GetEnumerator()
            {
                return new DisposingEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class DisposingEnumerator : IEnumerator<IEvent>
            {
                private readonly SqlQuery _sqlQuery;
                private MessageLogDbContext _context;
                private IEnumerator<IEvent> _events;

                public DisposingEnumerator(SqlQuery sqlQuery)
                {
                    _sqlQuery = sqlQuery;
                }

                ~DisposingEnumerator()
                {
                    if (_context != null) _context.Dispose();
                }

                public void Dispose()
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                        GC.SuppressFinalize(this);
                    }
                    if (_events != null)
                    {
                        _events.Dispose();
                    }
                }

                public IEvent Current { get { return _events.Current; } }
                object IEnumerator.Current { get { return Current; } }

                public bool MoveNext()
                {
                    if (_context == null)
                    {
                        _context = new MessageLogDbContext(_sqlQuery._nameOrConnectionString);
                        var queryable = _context.Set<MessageLogEntity>().AsQueryable()
                            .Where(x => x.Kind == StandardMetadata.EventKind);

                        var where = _sqlQuery._criteria.ToExpression();
                        if (where != null)
                            queryable = queryable.Where(where);

                        _events = queryable
                            .AsEnumerable()
                            .Select(x => _sqlQuery._serializer.Deserialize<IEvent>(x.Payload))
                            .GetEnumerator();
                    }

                    return _events.MoveNext();
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
