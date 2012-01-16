using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Security;

namespace TwiLib
{

    ///<summary>
    ///OAuth認証を使用するHTTP通信。HMAC-SHA1固定
    ///</summary>
    ///<remarks>
    ///使用前に認証情報を設定する。認証確認を伴う場合はAuthenticate系のメソッドを、認証不要な場合はInitializeを呼ぶこと。
    ///</remarks>
    public class HttpConnectionOAuth : HttpConnection, IHttpConnection
    {

        ///<summary>
        ///OAuth署名のoauth_timestamp算出用基準日付（1970/1/1 00:00:00）
        ///</summary>

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        ///<summary>
        ///OAuth署名のoauth_nonce算出用乱数クラス
        ///</summary>

        private static readonly Random NonceRandom = new Random();
        ///<summary>
        ///OAuthのアクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
        ///</summary>

        private string token = "";
        ///<summary>
        ///OAuthの署名作成用秘密アクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
        ///</summary>

        private string tokenSecret = "";
        ///<summary>
        ///OAuthのコンシューマー鍵
        ///</summary>

        private string consumerKey;
        ///<summary>
        ///OAuthの署名作成用秘密コンシューマーデータ
        ///</summary>

        private string consumerSecret;
        ///<summary>
        ///認証成功時の応答でユーザー情報を取得する場合のキー。設定しない場合は、AuthUsernameもブランクのままとなる
        ///</summary>

        private string userIdentKey = "";
        ///<summary>
        ///認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
        ///</summary>

        private string authorizedUsername = "";
        ///<summary>
        ///OAuth認証で指定のURLとHTTP通信を行い、結果を返す
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
            if (string.IsNullOrEmpty(token))
                return HttpStatusCode.Unauthorized;

            HttpWebRequest webReq = CreateRequest(method, requestUri, param, false);
            //OAuth認証ヘッダを付加
            AppendOAuthInfo(webReq, param, token, tokenSecret);

            if (content == null)
            {
                return GetResponse(webReq, headerInfo, false);
            }
            else
            {
                return GetResponse(webReq, ref content, headerInfo, false);
            }
        }

        #region "認証処理"
        ///<summary>
        ///OAuth認証の開始要求（リクエストトークン取得）。PIN入力用の前段
        ///</summary>
        ///<remarks>
        ///呼び出し元では戻されたurlをブラウザで開き、認証完了後PIN入力を受け付けて、リクエストトークンと共にAuthenticatePinFlowを呼び出す
        ///</remarks>
        ///<param name="requestTokenUrl">リクエストトークンの取得先URL</param>
        ///<param name="requestToken">[OUT]認証要求で戻されるリクエストトークン。使い捨て</param>
        ///<param name="authUri">[OUT]requestUriを元に生成された認証用URL。通常はリクエストトークンをクエリとして付加したUri</param>
        ///<returns>取得結果真偽値</returns>
        public bool AuthenticatePinFlowRequest(string requestTokenUrl, string authorizeUrl, ref string requestToken, ref Uri authUri)
        {
            //PIN-based flow
            authUri = GetAuthenticatePageUri(requestTokenUrl, authorizeUrl, ref requestToken);
            if (authUri == null)
                return false;
            return true;
        }

        ///<summary>
        ///OAuth認証のアクセストークン取得。PIN入力用の後段
        ///</summary>
        ///<remarks>
        ///事前にAuthenticatePinFlowRequestを呼んで、ブラウザで認証後に表示されるPINを入力してもらい、その値とともに呼び出すこと
        ///</remarks>
        ///<param name="accessTokenUrl">アクセストークンの取得先URL</param>
        ///<param name="pinCode">Webで認証後に表示されるPINコード</param>
        ///<returns>取得結果真偽値</returns>
        public bool AuthenticatePinFlow(string accessTokenUrl, string requestToken, string pinCode)
        {
            //PIN-based flow
            if (string.IsNullOrEmpty(requestToken))
                throw new Exception("Sequence error.(requestToken is blank)");

            //アクセストークン取得
            NameValueCollection accessTokenData = GetOAuthToken(new Uri(accessTokenUrl), pinCode, requestToken, null);

            if (accessTokenData != null)
            {
                token = accessTokenData["oauth_token"];
                tokenSecret = accessTokenData["oauth_token_secret"];
                //サービスごとの独自拡張対応
                if (!string.IsNullOrEmpty(this.userIdentKey))
                {
                    authorizedUsername = accessTokenData[this.userIdentKey];
                }
                else
                {
                    authorizedUsername = "";
                }
                if (string.IsNullOrEmpty(token))
                    return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        ///<summary>
        ///OAuth認証のアクセストークン取得。xAuth方式
        ///</summary>
        ///<param name="accessTokenUrl">アクセストークンの取得先URL</param>
        ///<param name="username">認証用ユーザー名</param>
        ///<param name="password">認証用パスワード</param>
        ///<returns>取得結果真偽値</returns>
        public bool AuthenticateXAuth(Uri accessTokenUrl, string username, string password)
        {
            //ユーザー・パスワードチェック
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Sequence error.(username or password is blank)");
            }
            //xAuthの拡張パラメータ設定
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("x_auth_mode", "client_auth");
            parameter.Add("x_auth_username", username);
            parameter.Add("x_auth_password", password);

            //アクセストークン取得
            NameValueCollection accessTokenData = GetOAuthToken(accessTokenUrl, "", "", parameter);

            if (accessTokenData != null)
            {
                token = accessTokenData["oauth_token"];
                tokenSecret = accessTokenData["oauth_token_secret"];
                //サービスごとの独自拡張対応
                if (!string.IsNullOrEmpty(this.userIdentKey))
                {
                    authorizedUsername = accessTokenData[this.userIdentKey];
                }
                else
                {
                    authorizedUsername = "";
                }
                if (string.IsNullOrEmpty(token))
                    return false;
                return true;
            }
            else
            {
                return false;
            }
        }
        bool IHttpConnection.Authenticate(Uri accessTokenUrl, string username, string password)
        {
            return AuthenticateXAuth(accessTokenUrl, username, password);
        }

        ///<summary>
        ///OAuth認証のリクエストトークン取得。リクエストトークンと組み合わせた認証用のUriも生成する
        ///</summary>
        ///<param name="authorizeUrl">ブラウザで開く認証用URLのベース</param>
        ///<param name="requestToken">[OUT]取得したリクエストトークン</param>
        ///<returns>取得結果真偽値</returns>
        private Uri GetAuthenticatePageUri(string requestTokenUrl, string authorizeUrl, ref string requestToken)
        {
            const string tokenKey = "oauth_token";

            //リクエストトークン取得
            NameValueCollection reqTokenData = GetOAuthToken(new Uri(requestTokenUrl), "", "", null);
            if (reqTokenData != null)
            {
                requestToken = reqTokenData[tokenKey];
                //Uri生成
                UriBuilder ub = new UriBuilder(authorizeUrl);
                ub.Query = string.Format("{0}={1}", tokenKey, requestToken);
                return ub.Uri;
            }
            else
            {
                return null;
            }
        }

        ///<summary>
        ///OAuth認証のトークン取得共通処理
        ///</summary>
        ///<param name="requestUri">各種トークンの取得先URL</param>
        ///<param name="pinCode">PINフロー時のアクセストークン取得時に設定。それ以外は空文字列</param>
        ///<param name="requestToken">PINフロー時のリクエストトークン取得時に設定。それ以外は空文字列</param>
        ///<param name="parameter">追加パラメータ。xAuthで使用</param>
        ///<returns>取得結果のデータ。正しく取得出来なかった場合はNothing</returns>
        private NameValueCollection GetOAuthToken(Uri requestUri, string pinCode, string requestToken, Dictionary<string, string> parameter)
        {
            HttpWebRequest webReq = null;
            //HTTPリクエスト生成。PINコードもパラメータも未指定の場合はGETメソッドで通信。それ以外はPOST
            if (string.IsNullOrEmpty(pinCode) && parameter == null)
            {
                webReq = CreateRequest("GET", requestUri, null, false);
            }
            else
            {
                webReq = CreateRequest("POST", requestUri, parameter, false);
                //ボディに追加パラメータ書き込み
            }
            //OAuth関連パラメータ準備。追加パラメータがあれば追加
            Dictionary<string, string> query = new Dictionary<string, string>();
            if (parameter != null)
            {
                foreach (KeyValuePair<string, string> kvp in parameter)
                {
                    query.Add(kvp.Key, kvp.Value);
                }
            }
            //PINコードが指定されていればパラメータに追加
            if (!string.IsNullOrEmpty(pinCode))
                query.Add("oauth_verifier", pinCode);
            //OAuth関連情報をHTTPリクエストに追加
            AppendOAuthInfo(webReq, query, requestToken, "");
            //HTTP応答取得
            try
            {
                string contentText = "";
                HttpStatusCode status = GetResponse(webReq, ref contentText, null, false);
                if (status == HttpStatusCode.OK)
                {
                    return ParseQueryString(contentText);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region "OAuth認証用ヘッダ作成・付加処理"
        ///<summary>
        ///HTTPリクエストにOAuth関連ヘッダを追加
        ///</summary>
        ///<param name="webRequest">追加対象のHTTPリクエスト</param>
        ///<param name="query">OAuth追加情報＋クエリ or POSTデータ</param>
        ///<param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        ///<param name="tokenSecret">アクセストークンシークレット。認証処理では空文字列</param>
        private void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query, string token, string tokenSecret)
        {
            //OAuth共通情報取得
            Dictionary<string, string> parameter = GetOAuthParameter(token);
            //OAuth共通情報にquery情報を追加
            if (query != null)
            {
                foreach (KeyValuePair<string, string> item in query)
                {
                    parameter.Add(item.Key, item.Value);
                }
            }
            //署名の作成・追加
            parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter));
            //HTTPリクエストのヘッダに追加
            StringBuilder sb = new StringBuilder("OAuth ");
            foreach (KeyValuePair<string, string> item in parameter)
            {
                //各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
                if (item.Key.StartsWith("oauth_"))
                {
                    sb.AppendFormat("{0}=\"{1}\",", item.Key, UrlEncode(item.Value));
                }
            }
            webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString());
        }

        ///<summary>
        ///OAuthで使用する共通情報を取得する
        ///</summary>
        ///<param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        ///<returns>OAuth情報のディクショナリ</returns>
        private Dictionary<string, string> GetOAuthParameter(string token)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("oauth_consumer_key", consumerKey);
            parameter.Add("oauth_signature_method", "HMAC-SHA1");
            parameter.Add("oauth_timestamp", Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString());
            //epoch秒
            parameter.Add("oauth_nonce", NonceRandom.Next(123400, 9999999).ToString());
            parameter.Add("oauth_version", "1.0");
            if (!string.IsNullOrEmpty(token))
                parameter.Add("oauth_token", token);
            //トークンがあれば追加
            return parameter;
        }

        ///<summary>
        ///OAuth認証ヘッダの署名作成
        ///</summary>
        ///<param name="tokenSecret">アクセストークン秘密鍵</param>
        ///<param name="method">HTTPメソッド文字列</param>
        ///<param name="uri">アクセス先Uri</param>
        ///<param name="parameter">クエリ、もしくはPOSTデータ</param>
        ///<returns>署名文字列</returns>
        private string CreateSignature(string tokenSecret, string method, Uri uri, Dictionary<string, string> parameter)
        {
            //パラメタをソート済みディクショナリに詰替（OAuthの仕様）
            SortedDictionary<string, string> sorted = new SortedDictionary<string, string>(parameter);
            //URLエンコード済みのクエリ形式文字列に変換
            string paramString = CreateQueryString(sorted);
            //アクセス先URLの整形
            string url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);
            //署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
            string signatureBase = string.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString));
            //署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
            string key = UrlEncode(consumerSecret) + "&";
            if (!string.IsNullOrEmpty(tokenSecret))
                key += UrlEncode(tokenSecret);
            //鍵生成＆署名生成
            System.Security.Cryptography.HMACSHA1 hmac = new System.Security.Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key));
            byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
            return Convert.ToBase64String(hash);
        }

        #endregion

        ///<summary>
        ///初期化。各種トークンの設定とユーザー識別情報設定
        ///</summary>
        ///<param name="consumerKey">コンシューマー鍵</param>
        ///<param name="consumerSecret">コンシューマー秘密鍵</param>
        ///<param name="accessToken">アクセストークン</param>
        ///<param name="accessTokenSecret">アクセストークン秘密鍵</param>
        ///<param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
        public void Initialize(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string userIdentifier)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.token = accessToken;
            this.tokenSecret = accessTokenSecret;
            this.userIdentKey = userIdentifier;
        }

        ///<summary>
        ///初期化。各種トークンの設定とユーザー識別情報設定
        ///</summary>
        ///<param name="consumerKey">コンシューマー鍵</param>
        ///<param name="consumerSecret">コンシューマー秘密鍵</param>
        ///<param name="accessToken">アクセストークン</param>
        ///<param name="accessTokenSecret">アクセストークン秘密鍵</param>
        ///<param name="username">認証済みユーザー名</param>
        ///<param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
        public void Initialize(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string username, string userIdentifier)
        {
            Initialize(consumerKey, consumerSecret, accessToken, accessTokenSecret, userIdentifier);
            authorizedUsername = username;
        }

        ///<summary>
        ///アクセストークン
        ///</summary>
        public string AccessToken
        {
            get { return token; }
        }

        ///<summary>
        ///アクセストークン秘密鍵
        ///</summary>
        public string AccessTokenSecret
        {
            get { return tokenSecret; }
        }

        ///<summary>
        ///認証済みユーザー名
        ///</summary>
        public string AuthUsername
        {
            get { return authorizedUsername; }
        }

    }
}
