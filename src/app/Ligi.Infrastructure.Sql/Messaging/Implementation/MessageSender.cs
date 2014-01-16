using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Transactions;
using Ligi.Infrastructure.Sql.Messaging;

namespace Ligi.Infrastructure.Messaging.Implementation
{
    public class MessageSender : IMessageSender
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string _name;
        private readonly string _insertQuery;

        public MessageSender(IDbConnectionFactory connectionFactory, string name, string tableName)
        {
            _connectionFactory = connectionFactory;
            _name = name;
            _insertQuery = string.Format("INSERT INTO {0} (Body, DeliveryDate, CorrelationId) VALUES (@Body, @DeliveryDate, @CorrelationId)", tableName);
        }

        public void Send(Message message)
        {
            using (var connection = _connectionFactory.CreateConnection(_name))
            {
                connection.Open();

                InsertMessage(message, connection);
            }
        }

        public void Send(IEnumerable<Message> messages)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (var connection = _connectionFactory.CreateConnection(_name))
                {
                    connection.Open();

                    foreach (var message in messages)
                    {
                        InsertMessage(message, connection);
                    }
                }

                scope.Complete();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Does not contain user input.")]
        private void InsertMessage(Message message, DbConnection connection)
        {
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                command.CommandType = CommandType.Text;

                command.Parameters.Add("@Body", SqlDbType.NVarChar).Value = message.Body;
                command.Parameters.Add("@DeliveryDate", SqlDbType.DateTime).Value = message.DeliveryDate.HasValue ? (object)message.DeliveryDate.Value : DBNull.Value;
                command.Parameters.Add("@CorrelationId", SqlDbType.NVarChar).Value = (object)message.CorrelationId ?? DBNull.Value;

                command.ExecuteNonQuery();
            }
        }
    }
}
