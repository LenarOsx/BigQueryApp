using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Configuration
{
    public class DatabaseConfiguration
    {
        public string? ConnectionString { get; internal set; }
        public int QuerySplittingBehavior { get; internal set; }
        public int Timeout { get; internal set; }
        public bool SensitiveDataLogging { get; internal set; }

        public DatabaseConfiguration()
        {
            ConnectionString = "";
            QuerySplittingBehavior = 0;
            Timeout = 600;
            SensitiveDataLogging = false;
        }
    }
}
