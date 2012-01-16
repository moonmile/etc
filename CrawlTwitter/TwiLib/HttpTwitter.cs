using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;


namespace TwiLib
{
    public class HttpTwitter
    {

        //OAuth関連
        ///<summary>
        ///OAuthのコンシューマー鍵
        ///</summary>

        private const string ConsumerKey = "iOQHfiCUsyOyamW8JJ8jg";
        ///<summary>
        ///OAuthの署名作成用秘密コンシューマーデータ
        ///</summary>

        private const string ConsumerSecret = "5PS2oa5f2VaKMPrlZa7DTbz0aFULKd3Ojxqgsm142Dw";
        ///<summary>
        ///OAuthのアクセストークン取得先URI
        ///</summary>

        private const string AccessTokenUrlXAuth = "https://api.twitter.com/oauth/access_token";
        private static string _protocol = "http://";

        private Dictionary<string, string> _remainCountApi = new Dictionary<string, string>();
        private const string PostMethod = "POST";

        private const string GetMethod = "GET";
        //HttpConnectionApi or HttpConnectionOAuth
        private IHttpConnection httpCon;

        private HttpVarious httpConVar = new HttpVarious();
        private enum AuthMethod
        {
            OAuth,
            Basic
        }

        private AuthMethod connectionType = AuthMethod.Basic;
        public HttpTwitter()
        {
            _remainCountApi.Add("X-RateLimit-Remaining", "-1");
            _remainCountApi.Add("X-RateLimit-Limit", "-1");
            _remainCountApi.Add("X-RateLimit-Reset", "-1");
        }

        public void Initialize(string accessToken, string accessTokenSecret, string username)
        {
            //for OAuth
            HttpConnectionOAuth con = new HttpConnectionOAuth();
            con.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username, "screen_name");
            httpCon = con;
            connectionType = AuthMethod.OAuth;
        }

        public void Initialize(string username, string password)
        {
            //for BASIC auth
            HttpConnectionBasic con = new HttpConnectionBasic();
            con.Initialize(username, password);
            httpCon = con;
            connectionType = AuthMethod.Basic;
        }

        public string AccessToken
        {
            get
            {
                if (httpCon != null)
                {
                    if (connectionType == AuthMethod.Basic)
                        return "";
                    return ((HttpConnectionOAuth)httpCon).AccessToken;
                }
                else
                {
                    return "";
                }
            }
        }

        public string AccessTokenSecret
        {
            get
            {
                if (httpCon != null)
                {
                    if (connectionType == AuthMethod.Basic)
                        return "";
                    return ((HttpConnectionOAuth)httpCon).AccessTokenSecret;
                }
                else
                {
                    return "";
                }
            }
        }

        public string AuthenticatedUsername
        {
            get
            {
                if (httpCon != null)
                {
                    return httpCon.AuthUsername;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Password
        {
            get
            {
                if (httpCon != null)
                {
                    //OAuthではパスワード取得させない
                    if (connectionType == AuthMethod.Basic)
                        return ((HttpConnectionBasic)httpCon).Password;
                    return "";
                }
                else
                {
                    return "";
                }
            }
        }

        public bool AuthUserAndPass(string username, string password)
        {
            if (connectionType == AuthMethod.Basic)
            {
                return httpCon.Authenticate(CreateTwitterUri("/1/account/verify_credentials.xml"), username, password);
            }
            else
            {
                return httpCon.Authenticate(new Uri(AccessTokenUrlXAuth), username, password);
            }
        }

        public void ClearAuthInfo()
        {
            if (connectionType == AuthMethod.Basic)
            {
                this.Initialize("", "");
            }
            else
            {
                this.Initialize("", "", "");
            }
        }

        public static bool UseSsl
        {
            set
            {
                if (value)
                {
                    _protocol = "https://";
                }
                else
                {
                    _protocol = "http://";
                }
            }
        }

        public int RemainCountApi
        {
            get
            {
                if (string.IsNullOrEmpty(_remainCountApi["X-RateLimit-Remaining"]))
                    return 0;
                return int.Parse(_remainCountApi["X-RateLimit-Remaining"]);
            }
        }

        public int UpperCountApi
        {
            get
            {
                if (string.IsNullOrEmpty(_remainCountApi["X-RateLimit-Limit"]))
                    return 0;
                return int.Parse(_remainCountApi["X-RateLimit-Limit"]);
            }
        }

        public DateTime ResetTimeApi
        {
            get
            {
                int i = 0;
                if (int.TryParse(_remainCountApi["X-RateLimit-Reset"], out i))
                {
                    if (i >= 0)
                    {
                        return System.TimeZone.CurrentTimeZone.ToLocalTime((new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i));
                    }
                    else
                    {
                        return new DateTime();
                    }
                }
                else
                {
                    return new DateTime();
                }
            }
        }

        public HttpStatusCode UpdateStatus(string status, long replyToId, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("status", status);
            if (connectionType == AuthMethod.Basic)
                param.Add("source", "Tween");
            if (replyToId > 0)
                param.Add("in_reply_to_status_id", replyToId.ToString());

            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/update.xml"), param, ref content, null);
        }

        public HttpStatusCode DestroyStatus(long id)
        {
            string cont = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/destroy/" + id.ToString() + ".xml"), null, ref cont, null);
        }

        public HttpStatusCode DestroyDirectMessage(long id)
        {
            string cont = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/direct_messages/destroy/" + id.ToString() + ".xml"), null, ref cont, null);
        }

        public HttpStatusCode RetweetStatus(long id, ref string content)
        {
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/statuses/retweet/" + id.ToString() + ".xml"), null, ref content, null);
        }

        public HttpStatusCode CreateFriendships(string screenName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            string content = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/friendships/create.xml"), param, ref content, null);
        }

        public HttpStatusCode DestroyFriendships(string screenName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("screen_name", screenName);
            string content = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/friendships/destroy.xml"), param, ref content, null);
        }

        public HttpStatusCode ShowFriendships(string souceScreenName, string targetScreenName, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("source_screen_name", souceScreenName);
            param.Add("target_screen_name", targetScreenName);

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/friendships/show.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode ShowStatuses(long id, ref string content)
        {
            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/show/" + id.ToString() + ".xml"), null, ref content, _remainCountApi);
        }

        public HttpStatusCode CreateFavorites(long id)
        {
            string content = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/favorites/create/" + id.ToString() + ".xml"), null, ref content , null);
        }

        public HttpStatusCode DestroyFavorites(long id)
        {
            string content = "";
            return httpCon.GetContent(PostMethod, CreateTwitterUri("/1/favorites/destroy/" + id.ToString() + ".xml"), null, ref content , null);
        }

        public HttpStatusCode HomeTimeline(int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/home_timeline.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode Mentions(int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/statuses/mentions.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode DirectMessages(int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/direct_messages.xml"), null, ref content, _remainCountApi);
        }

        public HttpStatusCode DirectMessagesSent(int count, long max_id, long since_id, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count > 0)
            {
                param.Add("count", count.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/direct_messages/sent.xml"), null, ref content, _remainCountApi);
        }

        public HttpStatusCode Favorites(int count, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (count != 20)
                param.Add("count", count.ToString());

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/favorites.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode Search(string words, string lang, int rpp, int page, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(words))
                param.Add("q", words);
            if (!string.IsNullOrEmpty(lang))
                param.Add("lang", lang);
            if (rpp > 0)
                param.Add("rpp", rpp.ToString());
            if (page > 0)
                param.Add("page", page.ToString());

            if (param.Count == 0)
                return HttpStatusCode.BadRequest;

            return httpConVar.GetContent(GetMethod, CreateTwitterSearchUri("/search.atom"), param, ref content, null, "Tween");
        }

        public HttpStatusCode FollowerIds(long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cursor", cursor.ToString());

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/followers/ids.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode RateLimitStatus(ref string content)
        {
            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/account/rate_limit_status.xml"), null, ref content, null);
        }

        public HttpStatusCode GetLists(string user, long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cursor", cursor.ToString());
            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/" + user + "/lists.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode GetListsSubscriptions(string user, long cursor, ref string content)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cursor", cursor.ToString());
            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/" + user + "/lists/subscriptions.xml"), param, ref content, _remainCountApi);
        }

        public HttpStatusCode GetListsStatuses(string user, string list_id, int per_page, long max_id, long since_id, ref string content)
        {
            //認証なくても取得できるが、protectedユーザー分が抜ける
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (per_page > 0)
            {
                param.Add("per_page", per_page.ToString());
            }
            if (max_id > 0)
            {
                param.Add("max_id", max_id.ToString());
            }
            if (since_id > 0)
            {
                param.Add("since_id", since_id.ToString());
            }

            return httpCon.GetContent(GetMethod, CreateTwitterUri("/1/" + user + "/lists/" + list_id + "/statuses.xml"), param, ref content, _remainCountApi);
        }


        #region "Proxy API"
        private static string _twitterUrl = "api.twitter.com";
        //Private TwitterUrl As String = "sorayukigtap.appspot.com/api"
        private static string _TwitterSearchUrl = "search.twitter.com";
        //Private TwitterSearchUrl As String = "sorayukigtap.appspot.com/search"

        private Uri CreateTwitterUri(string path)
        {
            return new Uri(string.Format("{0}{1}{2}", _protocol, _twitterUrl, path));
        }

        private Uri CreateTwitterSearchUri(string path)
        {
            return new Uri(string.Format("{0}{1}{2}", _protocol, _TwitterSearchUrl, path));
        }

        public static string TwitterUrl
        {
            set { _twitterUrl = value; }
        }

        public static string TwitterSearchUrl
        {
            set { _TwitterSearchUrl = value; }
        }
        #endregion

    }
}
