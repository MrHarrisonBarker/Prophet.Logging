using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Serilog.Events;

namespace Prophet.Logging
{
    public class Logg
    {
        public Guid Id { get; set; }
        [AllowNull] public Guid TraceId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public LogEventLevel LogLevel { get; set; }
        public Error Error { get; set; }
        [NotMapped] public Exception Exception { get; set; }
        public String Message { get; set; }
        public String Service { get; set; }
        public Guid InstanceId { get; set; }
    }
}