using System.IO;
using System.Net;
using System.Text;
using System;

namespace EZ_iTech.ToolKit {

    /// <summary>
    /// HTTP Helper
    /// </summary>
    public class HTTPHelper {

        /// <summary>
        /// GET
        /// </summary>
        public static string Get(string url, int timeout = -1, string content = null, string contentType = null, Cookie cookie = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "GET", timeout, content, contentType, cookie, encoding, cfg);
        }

        /// <summary>
        /// POST
        /// </summary>
        public static string Post(string url, int timeout = -1, string content = "", string contentType = "", Cookie cookie = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "POST", timeout, content, contentType, cookie, encoding, cfg);
        }

        /// <summary>
        /// PUT
        /// </summary>
        public static string Put(string url, int timeout = -1, string content = "", string contentType = "", Cookie cookie = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "PUT", timeout, content, contentType, cookie, encoding, cfg);
        }

        /// <summary>
        /// DELETE
        /// </summary>
        public static string Delete(string url, int timeout = -1, string content = "", string contentType = "", Cookie cookie = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "DELETE", timeout, content, contentType, cookie, encoding, cfg);
        }

        /// <summary>
        /// HTTP Config
        /// </summary>
        public sealed class Config {
            /// <summary>
            /// Ip
            /// </summary>
            public string Ip { get; set; }
            /// <summary>
            /// Port
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// Determine whether or not use proxy
            /// </summary>
            public bool UseProxy { get; set; }
            /// <summary>
            /// Determine whether or not use custom endpoint 
            /// If there have many network interface,you can select one as the endpoint
            /// </summary>
            public bool ManualSetIp { get; set; }

            /// <summary>
            /// If ManualSetIp is true ,return a selected endpoint
            /// </summary>
            /// <param name="servicePoint"></param>
            /// <param name="remoteEndPoint"></param>
            /// <param name="retryCount"></param>
            /// <returns></returns>
            public IPEndPoint BindIp(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) {
                return new IPEndPoint(IPAddress.Parse(Ip), Port);
            }
        }

        /// <summary>
        /// Process the http request
        /// </summary>
        /// <returns></returns>
        internal static string Process(string url, string method = null, int timeout = -1, string content = null, string contentType = null, Cookie cookie = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);
            }

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (null != cookie) {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    sw.WriteLine(content);
                    sw.Flush();
                    sw.Close();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    StreamReader sr = new StreamReader(inStream, encoding);
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
