using Microsoft.EntityFrameworkCore;

namespace Prophet.Logging
{
    [Owned]
    public class Error
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}