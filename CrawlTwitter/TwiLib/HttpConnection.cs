using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;


namespace TwiLib
{

    ///<summary>
    ///HttpWebRequest,HttpWebResponseを使用した基本的な通信機能を提供する
    ///</summary>
    ///<remarks>
    ///プロキシ情報などを設定するため、使用前に静的メソッドInitializeConnectionを呼び出すこと。
    ///通信方式によって必要になるHTTPヘッダの付加などは、派生クラスで行う。
    ///</remarks>
    public class HttpConnection
    {
        ///<summary>
        ///プロキシ
        ///</summary>

        private static WebProxy proxy = null;
        ///<summary>
        ///ユーザーが選択したプロキシの方式
        ///</summary>

        private static ProxyType proxyKind = ProxyType.IE;
        ///<summary>
        ///クッキー保存用コンテナ
        ///</summary>

        private static CookieContainer cookieContainer = new CookieContainer();
        ///<summary>
        ///初期化済みフラグ
        ///</summary>

        private static bool isInitialize = false;
        public enum ProxyType
        {
            None,
            IE,
            Specified
        }

        ///<summary>
        ///HttpWebRequestオブジェクトを取得する。パラメータはGET/HEAD/DELETEではクエリに、POST/PUTではエンティティボディに変換される。
        ///</summary>
        ///<remarks>
        ///追加で必要となるHTTPヘッダや通信オプションは呼び出し元で付加すること
        ///（Timeout,AutomaticDecompression,AllowAutoRedirect,UserAgent,ContentType,Accept,HttpRequestHeader.Authorization,カスタムヘッダ）
        ///POST/PUTでクエリが必要な場合は、requestUriに含めること。
        ///</remarks>
        ///<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        ///<param name="requestUri">通信先URI</param>
        ///<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        ///<param name="withCookie">通信にcookieを使用するか</param>
        ///<returns>引数で指定された内容を反映したHttpWebRequestオブジェクト</returns>
        protected HttpWebRequest CreateRequest(string method, Uri requestUri, Dictionary<string, string> param, bool withCookie)
        {
            if (!isInitialize)
                throw new Exception("Sequence error.(not initialized)");

            //GETメソッドの場合はクエリとurlを結合
            UriBuilder ub = new UriBuilder(requestUri.AbsoluteUri);
            if (param != null && (method == "GET" || method == "DELETE" || method == "HEAD"))
            {
                ub.Query = CreateQueryString(param);
            }

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);

            //プロキシ設定
            if (proxyKind != ProxyType.IE)
                webReq.Proxy = proxy;

            webReq.Method = method;
            if (method == "POST" || method == "PUT")
            {
                webReq.ContentType = "application/x-www-form-urlencoded";
                //POST/PUTメソッドの場合は、ボディデータとしてクエリ構成して書き込み
                using (StreamWriter writer = new StreamWriter(webReq.GetRequestStream()))
                {
                    writer.Write(CreateQueryString(param));
                }
            }
            //cookie設定
            if (withCookie)
                webReq.CookieContainer = cookieContainer;
            //タイムアウト設定
            webReq.Timeout = DefaultTimeout;

            return webReq;
        }

        ///<summary>
        ///HTTPの応答を処理し、引数で指定されたストリームに書き込み
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///gzipファイルのダウンロードを想定しているため、他形式の場合は伸張時に問題が発生する可能性があります。
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="contentStream">[OUT]HTTP応答のボディストリームのコピー先</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<param name="withCookie">通信にcookieを使用する</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, Stream contentStream, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (withCookie)
                        SaveCookie(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをコピーして戻す
                    if (webRes.ContentLength > 0)
                    {
                        //gzipなら応答ストリームの内容は伸張済み。それ以外なら伸張する。
                        if (webRes.ContentEncoding == "gzip" || webRes.ContentEncoding == "deflate")
                        {
                            using (Stream stream = webRes.GetResponseStream())
                            {
                                if (stream != null)
                                    CopyStream(stream, contentStream);
                            }
                        }
                        else
                        {
                            using (Stream stream = new System.IO.Compression.GZipStream(webRes.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress))
                            {
                                if (stream != null)
                                    CopyStream(stream, contentStream);
                            }
                        }
                    }
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    return res.StatusCode;
                }
                throw ex;
            }
        }

        ///<summary>
        ///HTTPの応答を処理し、応答ボディデータをテキストとして返却する
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///テキストの文字コードはUTF-8を前提として、エンコードはしていません
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="contentText">[OUT]HTTP応答のボディデータ</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<param name="withCookie">通信にcookieを使用する</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, ref string contentText, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (withCookie)
                        SaveCookie(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをテキストに書き出し
                    if (contentText == null)
                        throw new ArgumentNullException("contentText");
                    using (StreamReader sr = new StreamReader(webRes.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        contentText = sr.ReadToEnd();
                    }
                    return res.StatusCode;
                }
                throw ex;
            }
        }

        ///<summary>
        ///HTTPの応答を処理します。応答ボディデータが不要な用途向け。
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<param name="withCookie">通信にcookieを使用する</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (withCookie)
                        SaveCookie(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    return res.StatusCode;
                }
                throw ex;
            }
        }

        ///<summary>
        ///HTTPの応答を処理し、応答ボディデータをBitmapとして返却します
        ///</summary>
        ///<remarks>
        ///リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///</remarks>
        ///<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///<param name="withCookie">通信にcookieを使用する</param>
        ///<returns>HTTP応答のステータスコード</returns>
        protected HttpStatusCode GetResponse(HttpWebRequest webRequest, ref Bitmap contentBitmap, Dictionary<string, string> headerInfo, bool withCookie)
        {
            try
            {
                using (HttpWebResponse webRes = (HttpWebResponse)webRequest.GetResponse())
                {
                    HttpStatusCode statusCode = webRes.StatusCode;
                    //cookie保持
                    if (withCookie)
                        SaveCookie(webRes.Cookies);
                    //リダイレクト応答の場合は、リダイレクト先を設定
                    GetHeaderInfo(webRes, headerInfo);
                    //応答のストリームをBitmapにして戻す
                    //If webRes.ContentLength > 0 Then contentBitmap = New Bitmap(webRes.GetResponseStream)
                    contentBitmap = new Bitmap(webRes.GetResponseStream());
                    return statusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = (HttpWebResponse)ex.Response;
                    return res.StatusCode;
                }
                throw ex;
            }
        }

        ///<summary>
        ///クッキーを保存。ホスト名なしのドメインの場合、ドメイン名から先頭のドットを除去して追加しないと再利用されないため
        ///</summary>
        private void SaveCookie(CookieCollection cookieCollection)
        {
            foreach (Cookie ck in cookieCollection)
            {
                if (ck.Domain.StartsWith("."))
                {
                    ck.Domain = ck.Domain.Substring(1, ck.Domain.Length - 1);
                    cookieContainer.Add(ck);
                }
            }
        }

        ///<summary>
        ///in/outのストリームインスタンスを受け取り、コピーして返却
        ///</summary>
        ///<param name="inStream">コピー元ストリームインスタンス。読み取り可であること</param>
        ///<param name="outStream">コピー先ストリームインスタンス。書き込み可であること</param>
        private void CopyStream(Stream inStream, Stream outStream)
        {
            if (inStream == null)
                throw new ArgumentNullException("inStream");
            if (outStream == null)
                throw new ArgumentNullException("outStream");
            if (!inStream.CanRead)
                throw new ArgumentException("Input stream can not read.");
            if (!outStream.CanWrite)
                throw new ArgumentException("Output stream can not write.");
            if (inStream.CanSeek && inStream.Length == 0)
                throw new ArgumentException("Input stream do not have data.");

            do
            {
                byte[] buffer = new byte[1025];
                int i = buffer.Length;
                i = inStream.Read(buffer, 0, i);
                if (i == 0)
                    break; // TODO: might not be correct. Was : Exit Do
                outStream.Write(buffer, 0, i);
            } while (true);
        }

        ///<summary>
        ///headerInfoのキー情報で指定されたHTTPヘッダ情報を取得・格納する。redirect応答時はLocationヘッダの内容を追記する
        ///</summary>
        ///<param name="webResponse">HTTP応答</param>
        ///<param name="headerInfo">[IN/OUT]キーにヘッダ名を指定したデータ空のコレクション。取得した値をデータにセットして戻す</param>

        private void GetHeaderInfo(HttpWebResponse webResponse, Dictionary<string, string> headerInfo)
        {
            if (headerInfo == null)
                return;

            if (headerInfo.Count > 0)
            {
                string[] keys = new string[headerInfo.Count];
                headerInfo.Keys.CopyTo(keys, 0);
                foreach (string key in keys)
                {
                    if (Array.IndexOf(webResponse.Headers.AllKeys, key) > -1)
                    {
                        headerInfo[key] = webResponse.Headers[key];
                    }
                    else
                    {
                        headerInfo[key] = "";
                    }
                }
            }

            HttpStatusCode statusCode = webResponse.StatusCode;
            if (statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther || statusCode == HttpStatusCode.TemporaryRedirect)
            {
                if (headerInfo.ContainsKey("Location"))
                {
                    headerInfo["Location"] = webResponse.Headers["Location"];
                }
                else
                {
                    headerInfo.Add("Location", webResponse.Headers["Location"]);
                }
            }
        }

        ///<summary>
        ///クエリコレクションをkey=value形式の文字列に構成して戻す
        ///</summary>
        ///<param name="param">クエリ、またはポストデータとなるkey-valueコレクション</param>
        protected string CreateQueryString(IDictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
                return string.Empty;

            StringBuilder query = new StringBuilder();
            foreach (string key in param.Keys)
            {
                query.AppendFormat("{0}={1}&", UrlEncode(key), UrlEncode(param[key]));
            }
            return query.ToString(0, query.Length - 1);
        }

        ///<summary>
        ///クエリ形式（key1=value1%&amp;key2=value2&amp;...）の文字列をkey-valueコレクションに詰め直し
        ///</summary>
        ///<param name="queryString">クエリ文字列</param>
        ///<returns>key-valueのコレクション</returns>
        protected NameValueCollection ParseQueryString(string queryString)
        {
            NameValueCollection query = new NameValueCollection();
            string[] parts = queryString.Split('&');
            foreach (string part in parts)
            {
                int index = part.IndexOf('=');
                if (index == -1)
                {
                    query.Add(Uri.UnescapeDataString(part), "");
                }
                else
                {
                    query.Add(Uri.UnescapeDataString(part.Substring(0, index)), Uri.UnescapeDataString(part.Substring(index + 1)));
                }
            }
            return query;
        }

        ///<summary>
        ///2バイト文字も考慮したUrlエンコード
        ///</summary>
        ///<returns>エンコード結果文字列</returns>
        protected string UrlEncode(string stringToEncode)
        {
            const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(stringToEncode);

            foreach (byte b in bytes)
            {
                if (UnreservedChars.IndexOf((char)b) != -1)
                {
                    sb.Append((char)b);
                }
                else
                {
                    sb.AppendFormat("%{0:X2}", b);
                }
            }
            return sb.ToString();
        }

        #region "DefaultTimeout"
        ///<summary>
        ///通信タイムアウト時間（ms）
        ///</summary>

        private static int timeout = 20000;
        ///<summary>
        ///通信タイムアウト時間（ms）。10～120秒の範囲で指定。範囲外は20秒とする
        ///</summary>
        protected static int DefaultTimeout
        {
            get { return timeout; }
            set
            {
                const int TimeoutMinValue = 10000;
                const int TimeoutMaxValue = 120000;
                const int TimeoutDefaultValue = 20000;
                if (value < TimeoutMinValue || value > TimeoutMaxValue)
                {
                    // 範囲外ならデフォルト値設定
                    timeout = TimeoutDefaultValue;
                }
                else
                {
                    timeout = value;
                }
            }
        }
        #endregion

        ///<summary>
        ///通信クラスの初期化処理。タイムアウト値とプロキシを設定する
        ///</summary>
        ///<remarks>
        ///通信開始前に最低一度呼び出すこと
        ///</remarks>
        ///<param name="timeout">タイムアウト値（秒）</param>
        ///<param name="proxyType">なし・指定・IEデフォルト</param>
        ///<param name="proxyAddress">プロキシのホスト名orIPアドレス</param>
        ///<param name="proxyPort">プロキシのポート番号</param>
        ///<param name="proxyUser">プロキシ認証が必要な場合のユーザ名。不要なら空文字</param>
        ///<param name="proxyPassword">プロキシ認証が必要な場合のパスワード。不要なら空文字</param>
        public static void InitializeConnection(int timeout, ProxyType proxyType, string proxyAddress, int proxyPort, string proxyUser, string proxyPassword)
        {
            isInitialize = true;
            ServicePointManager.Expect100Continue = false;
            DefaultTimeout = timeout * 1000;
            //s -> ms
            switch (proxyType)
            {
                case ProxyType.None:
                    proxy = null;
                    break;
                case ProxyType.Specified:
                    proxy = new WebProxy("http://" + proxyAddress + ":" + proxyPort.ToString());
                    if (!string.IsNullOrEmpty(proxyUser) || !string.IsNullOrEmpty(proxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword);
                    }
                    break;
                case ProxyType.IE:
                    break;
                //IE設定（システム設定）はデフォルト値なので処理しない
            }
            proxyKind = proxyType;
        }
    }
}
