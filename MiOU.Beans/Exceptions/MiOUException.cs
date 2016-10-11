using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiOU.Entities.Exceptions
{
    public enum MiOUExceptionType
    {
        INFO,
        ERROR,
        WARN
    }
    public class MiOUException:Exception
    {
        public MiOUExceptionType Type { get; private set; }
        public MiOUException(MiOUExceptionType type = MiOUExceptionType.WARN)
        {
            this.Type = type;
        }

        public MiOUException(string message, MiOUExceptionType type = MiOUExceptionType.INFO) : base(message)
        {
            this.Type = type;
        }

        public MiOUException(string message, Exception inner, MiOUExceptionType type = MiOUExceptionType.INFO) : base(message, inner)
        {
            this.Type = type;
        }
    }
}
