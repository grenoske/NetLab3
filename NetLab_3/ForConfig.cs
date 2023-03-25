using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PL
{
    public class ForConfig
    {
        public string ConnectionString { get; set; }
        public ForConfig()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var myConnectionString = _configuration.GetConnectionString("connectionString");
            Console.WriteLine(myConnectionString);
        }
    }
}
