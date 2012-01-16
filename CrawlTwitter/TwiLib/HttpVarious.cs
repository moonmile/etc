using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Drawing;
using System.IO;

namespace TwiLib
{
    public class HttpVarious : HttpConnection
    {

        private const string PostMethod = "POST";

        private const string GetMethod = "GET";
        public string GetRedirectTo(string url)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
            req.Timeout = 5000;
            req.AllowAutoRedirect = false;
            try
            {
                string data = "";
                Dictionary<string, string> head = new Dictionary<string, string>();
                HttpStatusCode ret = GetResponse(req, ref data, head, false);
                if (head.ContainsKey("Location"))
                {
                    return head["Location"];
                }
                else
                {
                    return url;
                }
            }
            catch (Exception ex)
            {
                return url;
            }
        }

        public Image GetImage(string url)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
            req.Timeout = 5000;
            try
            {
                Bitmap img = null;
                HttpStatusCode ret = GetResponse(req, ref img, null, false);
                if (ret == HttpStatusCode.OK)
                    return img;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Image GetImage(string url, string referer)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(url), null, false);
            req.Referer = referer;
            req.Timeout = 5000;
            try
            {
                Bitmap img = null;
                HttpStatusCode ret = GetResponse(req, ref img, null, false);
                if (ret == HttpStatusCode.OK)
                    return img;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param)
        {
            HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
            try
            {
                HttpStatusCode res = this.GetResponse(req, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PostData(string Url, Dictionary<string, string> param, ref string content)
        {
            HttpWebRequest req = CreateRequest(PostMethod, new Uri(Url), param, false);
            try
            {
                HttpStatusCode res = this.GetResponse(req, ref content, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), param, false);
            try
            {
                HttpStatusCode res = this.GetResponse(req, ref content, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetData(string Url, Dictionary<string, string> param, ref string content, int timeout)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), param, false);
            req.Timeout = timeout;
            try
            {
                HttpStatusCode res = this.GetResponse(req, ref content, null, false);
                if (res == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public HttpStatusCode GetContent(string method, Uri Url, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo, string userAgent)
        {
            HttpWebRequest req = CreateRequest(method, Url, param, false);
            req.UserAgent = userAgent;
            return this.GetResponse(req, ref content, headerInfo, false);
        }

        public bool GetDataToFile(string Url, string savePath)
        {
            HttpWebRequest req = CreateRequest(GetMethod, new Uri(Url), null, false);
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            using (System.IO.FileStream strm = new System.IO.FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    HttpStatusCode res = this.GetResponse(req, strm, null, false);
                    strm.Close();
                    if (res == HttpStatusCode.OK)
                        return true;
                    return false;
                }
                catch (Exception ex)
                {
                    strm.Close();
                    return false;
                }
            }
        }
    }
}
