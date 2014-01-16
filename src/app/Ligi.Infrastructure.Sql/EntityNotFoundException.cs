using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Ligi.Infrastructure.Sql
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        private readonly Guid _entityId;
        private readonly string _entityType;

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(Guid entityId) : base(entityId.ToString())
        {
            _entityId = entityId;
        }

        public EntityNotFoundException(Guid entityId, string entityType)
            : base(entityType + ": " + entityId.ToString())
        {
            _entityId = entityId;
            _entityType = entityType;
        }

        public EntityNotFoundException(Guid entityId, string entityType, string message, Exception inner) 
            : base(message, inner)
        {
            _entityId = entityId;
            _entityType = entityType;
        }

        protected EntityNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            _entityId = Guid.Parse(info.GetString("entityId"));
            _entityType = info.GetString("entityType");
        }

        public Guid EntityId
        {
            get { return _entityId; }
        }

        public string EntityType
        {
            get { return _entityType; }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("entityId", _entityId.ToString());
            info.AddValue("entityType", _entityType);
        }
    }
}
