using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace SmartBEAM
{
    public class SmartBEAMDataContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/SmartBEAM.sdf";

        public SmartBEAMDataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<SmartBEAMDB> smartbeam;
        public Table<SmartBEAMDB> smartbeammatched;
    }
}
