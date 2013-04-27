using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace foto_watcher {
    class Program {
        static void Main(string[] args) {

            
            String serverUrl = ConfigurationManager.AppSettings["server_url"];
            String path = ConfigurationManager.AppSettings["watch_dir"];

            String has_serverValue = ConfigurationManager.AppSettings["has_server"];
            bool has_server = false; ;
            bool.TryParse(has_serverValue, out has_server);

            if (has_server) {
                FotoPoster.Instance().Initialize(serverUrl);
            }

            Console.WriteLine("Server Url: {0}", serverUrl);
            Console.WriteLine("Watch Dir: {0}", path);

            Console.WriteLine("Posting To Server: {0}", has_server);

            FotoWatcher watcher = new FotoWatcher(path);

            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();
        }
    }
}
