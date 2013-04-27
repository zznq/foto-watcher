using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace foto_watcher {
    class FotoWatcher {
        FileSystemWatcher watcher;
        String serverUrl;

        public FotoWatcher(String path) {
            watcher = new FileSystemWatcher(path, ".jpg");

            watcher.IncludeSubdirectories = true;
            watcher.Filter = "";
            watcher.Created += new FileSystemEventHandler(changed);

            watcher.EnableRaisingEvents = true;
        }

        private static void changed(object sender, FileSystemEventArgs e) {

            DirectoryInfo di = new DirectoryInfo(e.FullPath);

            String groupIdStr = di.Parent.Name;
            int groupId = -1;

            if(int.TryParse(groupIdStr, out groupId)) {
                String path = e.FullPath;

                const int max_retry = 5;
                int count = 0;
                bool success = false;

                // We are picking up the file right when it is saved, sometimes there is overlap in accessing the file
                while(count < max_retry && !success) {
                    try {
                        using (FileStream stream = File.OpenRead(path)) {
                            success = true;
                        }
                    } catch (Exception) {
                        success = false;
                    } finally {
                        count++;
                    }
                }

                if (success) {
                    if (path.Contains("strip")) {
                        //FotoPrinter.Instance().Print(e.FullPath);
                    }

                    FotoPoster.Instance().postFile(e.FullPath, groupId);
                }
            }
        }
    }
}
