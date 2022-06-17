using System.Runtime.Serialization;

namespace nvm.Exceptions
{
    [Serializable]
    internal class VersionsNotFoundException : Exception
    {
        public VersionsNotFoundException()
        {
        }

        public VersionsNotFoundException(string? message) : base(message)
        {
        }

        public VersionsNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected VersionsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}