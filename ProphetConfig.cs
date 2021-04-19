using System;
using Microsoft.EntityFrameworkCore;

namespace Prophet.Logging
{
    public class ProphetConfig
    {
        public string Group { get; set; }
        public string BoostrapServers { get; set; }
        public string Topic { get; set; }
        public Action<DbContextOptionsBuilder> DbContextOptions { get; set; }
    }
}