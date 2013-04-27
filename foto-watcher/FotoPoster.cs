using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace foto_watcher {
    class FotoPoster {
        private static FotoPoster _poster;
        private bool _postFile;
        private String _serverUrl;

        private FotoPoster() {
            _postFile = false;
            _serverUrl = null;
        }

        public static FotoPoster Instance() {
            if (_poster == null) {
                _poster = new FotoPoster();
            }

            return _poster;
        }

        public void Initialize(String serverUrl) {
            _serverUrl = serverUrl;
            _postFile = true;
        }

        public void postFile(String filePath, int groupId) {
            if (!_postFile) {
                return;
            }

            Console.WriteLine("Posting File: {0}", filePath);

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("utf8", "?");
            nvc.Add("photo[group_id]", String.Format("{0}", groupId));

            HttpUploadFile(_serverUrl, new FileFotoPosterField() {
                    paramName = @"photo[image]",
                    value = filePath
                }, "image/jpeg", nvc);
        }

        public static void HttpUploadFile(string url, FileFotoPosterField file, string contentType, NameValueCollection nvc) {
            string boundary = "----BoofBoundry" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys) {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, file.paramName, file.value, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            using (FileStream fileStream = File.OpenRead(file.value)) {
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
                    rs.Write(buffer, 0, bytesRead);
                }
            }
            
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
            } catch (Exception ex) {
                Console.WriteLine("Error: {0}", ex.Message);
                if (wresp != null) {
                    wresp.Close();
                    wresp = null;
                }
            } finally {
                wr = null;
            }
        }
    }
}
