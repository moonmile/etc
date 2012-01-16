using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Net;

namespace TwiLib
{
    ///<summary>
    ///BASIC認証を使用するHTTP通信
    ///</summary>
    ///<remarks>
    ///使用前にユーザー／パスワードを設定する。認証確認を伴う場合はAuthenticateを、認証不要な場合はInitializeを呼ぶこと。
    ///</remarks>
    public class HttpConnectionBasic : HttpConnection, IHttpConnection
    {

        ///<summary>
        ///認証用ユーザー名
        ///</summary>
        private string _userName = "";
        ///<summary>
        ///認証用パスワード
        ///</summary>
        private string _password = "";
        ///<summary>
        ///Authorizationヘッダに設定するエンコード済み文字列
        ///</summary>

        private string credential = "";
        ///<summary>
        ///BASIC認証で指定のURLとHTTP通信を行い、結果を返す
        ///</summary>
        ///<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
        ///<param name="requestUri">通信先URI</param>
        ///<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
        ///<param name="content">[OUT]HTTP応答のボディデータ</param>
        ///<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
        ///<returns>HTTP応答のステータスコード</returns>

        public HttpStatusCode GetContent(string method, Uri requestUri, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo)
        {
            //認証済かチェック
            if (string.IsNullOrEmpty(this.credential))
                return HttpStatusCode.Unauthorized;

            HttpWebRequest webReq = CreateRequest(method, requestUri, param, false);
            //BASIC認証用ヘッダを付加
            AppendApiInfo(webReq);

            if (content == null)
            {
                return GetResponse(webReq, headerInfo, false);
            }
            else
            {
                return GetResponse(webReq, ref content, headerInfo, false);
            }
        }

        ///<summary>
        ///BASIC認証とREST APIで必要なヘッダを付加
        ///</summary>
        ///<param name="webRequest">付加対象となるHTTPリクエストオブジェクト</param>
        private void AppendApiInfo(HttpWebRequest webRequest)
        {
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "text/html, */*";
            webRequest.Headers.Add(HttpRequestHeader.Authorization, credential);
        }

        ///<summary>
        ///BASIC認証で使用するユーザー名とパスワードを設定。
        ///</summary>
        ///<param name="userName">認証で使用するユーザー名</param>
        ///<param name="password">認証で使用するパスワード</param>
        public void Initialize(string userName, string password)
        {
            this._userName = userName;
            this._password = password;
            this.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
        }

        ///<summary>
        ///設定されているユーザー名
        ///</summary>
        public string AuthUsername
        {
            get { return _userName; }
        }

        ///<summary>
        ///パスワード
        ///</summary>
        public string Password
        {
            get { return this._password; }
        }

        ///<summary>
        ///BASIC認証で使用するユーザー名とパスワードを設定。指定のURLにGETリクエストを投げて、OK応答なら認証OKとみなして認証情報を保存する
        ///</summary>
        ///<param name="url">認証先のURL</param>
        ///<param name="userName">認証で使用するユーザー名</param>
        ///<param name="password">認証で使用するパスワード</param>
        public bool Authenticate(Uri url, string username, string password)
        {
            //urlは認証必要なGETメソッドとする
            string orgCre = this.credential;
            this.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            try
            {
                string content = "";
                if (this.GetContent("GET", url, null, ref content, null) == HttpStatusCode.OK)
                {
                    this._userName = username;
                    this._password = password;
                    return true;
                }
                this.credential = orgCre;
                return false;
            }
            catch (Exception ex)
            {
                this.credential = orgCre;
                return false;
            }
        }

    }
}
