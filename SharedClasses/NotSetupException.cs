using System;
using System.Runtime.Serialization;

namespace SharedClasses
{
    [Serializable]
    public class NotSetupException : Exception
    {
        public NotSetupException()
        {
        }

        public NotSetupException(string message) : base(message)
        {
        }

        public NotSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotSetupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}