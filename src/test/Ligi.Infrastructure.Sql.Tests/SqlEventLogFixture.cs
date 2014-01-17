using Ligi.Core.Events;
using Ligi.Core.MessageLog;
using Ligi.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Infrastructure.Sql.MessageLog;
using Ligi.Infrastructure.Sql.Serialization;
using Moq;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class given_a_sql_log_with_three_events : IDisposable
    {
        private readonly string _dbName = "SqlEventLogFixture_" + Guid.NewGuid().ToString();
        private readonly SqlMessageLog _sut;
        private Mock<IMetadataProvider> _metadata;
        private readonly EventA _eventA;
        private readonly EventB _eventB;
        private readonly EventC _eventC;

        public given_a_sql_log_with_three_events()
        {
            using (var context = new MessageLogDbContext(_dbName))
            {
                if (context.Database.Exists())
                {
                    context.Database.Delete();
                }

                context.Database.Create();
            }

            _eventA = new EventA();
            _eventB = new EventB();
            _eventC = new EventC();

            var metadata = Mock.Of<IMetadataProvider>(x =>
                x.GetMetadata(_eventA) == new Dictionary<string, string>
                {
                    { StandardMetadata.SourceId, _eventA.SourceId.ToString() },
                    { StandardMetadata.SourceType, "SourceA" }, 
                    { StandardMetadata.Kind, StandardMetadata.EventKind },
                    { StandardMetadata.AssemblyName, "A" }, 
                    { StandardMetadata.Namespace, "Namespace" }, 
                    { StandardMetadata.FullName, "Namespace.EventA" }, 
                    { StandardMetadata.TypeName, "EventA" }, 
                } &&
                x.GetMetadata(_eventB) == new Dictionary<string, string>
                {
                    { StandardMetadata.SourceId, _eventB.SourceId.ToString() },
                    { StandardMetadata.SourceType, "SourceB" }, 
                    { StandardMetadata.Kind, StandardMetadata.EventKind },
                    { StandardMetadata.AssemblyName, "B" }, 
                    { StandardMetadata.Namespace, "Namespace" }, 
                    { StandardMetadata.FullName, "Namespace.EventB" }, 
                    { StandardMetadata.TypeName, "EventB" }, 
                } &&
                x.GetMetadata(_eventC) == new Dictionary<string, string>
                {
                    { StandardMetadata.SourceId, _eventC.SourceId.ToString() },
                    { StandardMetadata.SourceType, "SourceC" }, 
                    { StandardMetadata.Kind, StandardMetadata.EventKind },
                    { StandardMetadata.AssemblyName, "B" }, 
                    { StandardMetadata.Namespace, "AnotherNamespace" }, 
                    { StandardMetadata.FullName, "AnotherNamespace.EventC" }, 
                    { StandardMetadata.TypeName, "EventC" }, 
                });

            _metadata = Mock.Get(metadata);
            _sut = new SqlMessageLog(_dbName, new JsonTextSerializer(), metadata);
            _sut.Save(_eventA);
            _sut.Save(_eventB);
            _sut.Save(_eventC);
        }

        public void Dispose()
        {
            using (var context = new MessageLogDbContext(_dbName))
            {
                if (context.Database.Exists())
                {
                    context.Database.Delete();
                }
            }
        }

        [Fact]
        public void then_can_read_all()
        {
            var events = _sut.ReadAll().ToList();

            Assert.Equal(3, events.Count);
        }

        [Fact]
        public void then_can_filter_by_assembly()
        {
            var events = _sut.Query(new QueryCriteria { AssemblyNames = { "A" } }).ToList();

            Assert.Equal(1, events.Count);
        }

        [Fact]
        public void then_can_filter_by_multiple_assemblies()
        {
            var events = _sut.Query(new QueryCriteria { AssemblyNames = { "A", "B" } }).ToList();

            Assert.Equal(3, events.Count);
        }

        [Fact]
        public void then_can_filter_by_namespace()
        {
            var events = _sut.Query(new QueryCriteria { Namespaces = { "Namespace" } }).ToList();

            Assert.Equal(2, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventA.SourceId));
            Assert.True(events.Any(x => x.SourceId == _eventB.SourceId));
        }

        [Fact]
        public void then_can_filter_by_namespaces()
        {
            var events = _sut.Query(new QueryCriteria { Namespaces = { "Namespace", "AnotherNamespace" } }).ToList();

            Assert.Equal(3, events.Count);
        }

        [Fact]
        public void then_can_filter_by_namespace_and_assembly()
        {
            var events = _sut.Query(new QueryCriteria { AssemblyNames = { "B" }, Namespaces = { "AnotherNamespace" } }).ToList();

            Assert.Equal(1, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventC.SourceId));
        }

        [Fact]
        public void then_can_filter_by_namespace_and_assembly2()
        {
            var events = _sut.Query(new QueryCriteria { AssemblyNames = { "A" }, Namespaces = { "AnotherNamespace" } }).ToList();

            Assert.Equal(0, events.Count);
        }

        [Fact]
        public void then_can_filter_by_full_name()
        {
            var events = _sut.Query(new QueryCriteria { FullNames = { "Namespace.EventA" } }).ToList();

            Assert.Equal(1, events.Count);
            Assert.Equal(_eventA.SourceId, events[0].SourceId);
        }

        [Fact]
        public void then_can_filter_by_full_names()
        {
            var events = _sut.Query(new QueryCriteria { FullNames = { "Namespace.EventA", "AnotherNamespace.EventC" } }).ToList();

            Assert.Equal(2, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventA.SourceId));
            Assert.True(events.Any(x => x.SourceId == _eventC.SourceId));
        }

        [Fact]
        public void then_can_filter_by_type_name()
        {
            var events = _sut.Query(new QueryCriteria { TypeNames = { "EventA" } }).ToList();

            Assert.Equal(1, events.Count);
            Assert.Equal(_eventA.SourceId, events[0].SourceId);
        }

        [Fact]
        public void then_can_filter_by_type_names()
        {
            var events = _sut.Query(new QueryCriteria { TypeNames = { "EventA", "EventC" } }).ToList();

            Assert.Equal(2, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventA.SourceId));
            Assert.True(events.Any(x => x.SourceId == _eventC.SourceId));
        }

        [Fact]
        public void then_can_filter_by_type_names_and_assembly()
        {
            var events = _sut.Query(new QueryCriteria { AssemblyNames = { "B" }, TypeNames = { "EventB", "EventC" } }).ToList();

            Assert.Equal(2, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventB.SourceId));
            Assert.True(events.Any(x => x.SourceId == _eventC.SourceId));
        }

        [Fact]
        public void then_can_filter_by_source_id()
        {
            var events = _sut.Query(new QueryCriteria { SourceIds = { _eventA.SourceId.ToString() } }).ToList();

            Assert.Equal(1, events.Count);
            Assert.Equal(_eventA.SourceId, events[0].SourceId);
        }

        [Fact]
        public void then_can_filter_by_source_ids()
        {
            var events = _sut.Query(new QueryCriteria { SourceIds = { _eventA.SourceId.ToString(), _eventC.SourceId.ToString() } }).ToList();

            Assert.Equal(2, events.Count);
            Assert.True(events.Any(x => x.SourceId == _eventA.SourceId));
            Assert.True(events.Any(x => x.SourceId == _eventC.SourceId));
        }

        [Fact]
        public void then_can_filter_by_source_type()
        {
            var events = _sut.Query(new QueryCriteria { SourceTypes = { "SourceA" } }).ToList();

            Assert.Equal(1, events.Count);
        }

        [Fact]
        public void then_can_filter_by_source_types()
        {
            var events = _sut.Query(new QueryCriteria { SourceTypes = { "SourceA", "SourceB" } }).ToList();

            Assert.Equal(2, events.Count);
        }

        [Fact]
        public void then_can_filter_in_by_end_date()
        {
            var events = _sut.Query(new QueryCriteria { EndDate = DateTime.UtcNow }).ToList();

            Assert.Equal(3, events.Count);
        }

        [Fact]
        public void then_can_filter_out_by_end_date()
        {
            var events = _sut.Query(new QueryCriteria { EndDate = DateTime.UtcNow.AddMinutes(-1) }).ToList();

            Assert.Equal(0, events.Count);
        }

        [Fact]
        public void then_can_use_fluent_criteria_builder()
        {
            var events = _sut.Query()
                .FromAssembly("A")
                .FromAssembly("B")
                .FromNamespace("Namespace")
                .FromSource("SourceB")
                .WithTypeName("EventB")
                .WithFullName("Namespace.EventB")
                .Until(DateTime.UtcNow)
                .ToList();

            Assert.Equal(1, events.Count);
        }

        public class EventA : IEvent
        {
            public EventA()
            {
                SourceId = Guid.NewGuid();
            }
            public Guid SourceId { get; set; }
        }

        public class EventB : IEvent
        {
            public EventB()
            {
                SourceId = Guid.NewGuid();
            }
            public Guid SourceId { get; set; }
        }

        public class EventC : IEvent
        {
            public EventC()
            {
                SourceId = Guid.NewGuid();
            }
            public Guid SourceId { get; set; }
        }
    }
}
