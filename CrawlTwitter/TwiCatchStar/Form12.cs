using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TwiLib;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net;
using moonmile.ExDoc;
using System.Collections.ObjectModel;

namespace TwiCatchStar
{
	public partial class Form1 : Form
	{
		string APPTITLE = "ツイートキャッチ★★★ ver.0.2";
		// twitter用
		string consumerKey = "Sfh6p3BUwYbC0UtGekQAHQ";
		string consumerSecret = "OmHrnEHsAypRs4DnnerTrA8YrDGL4s7X2SoCQ2Kl0L0";
		string accessToken = "";
		string accessSecret = "";
		string username = "";
		// 設定ファイル
		string initfile = "TwiCatchStar.xml";
		HttpConnectionOAuth httpCon = new HttpConnectionOAuth();
		HttpConnectionOAuth oauth = new HttpConnectionOAuth();
		string accessTokenUrl = "";
		string requestToken = "";

		int tweet_count = 0;
		string max_id = "";
		DateTime startTime;
		List<Tweet> _tweets = null;

		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 実行ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGo_Click(object sender, EventArgs e)
		{
			if (accessToken == "")
			{
				// 初期設定の場合
				oauth.Initialize(consumerKey, consumerSecret, "", "", username);
				Uri authUri = new Uri("http://twitter.com/");
				oauth.AuthenticatePinFlowRequest(
					"https://twitter.com/oauth/request_token",
					"https://twitter.com/oauth/authorize", ref requestToken, ref authUri);
				Process pro = new Process();
				accessTokenUrl = authUri.ToString();
				pro.StartInfo.FileName = authUri.ToString();
				pro.Start();
				return;
			}
			startTime = DateTime.Now;

			// 通常の取得
			username = textBox1.Text.Trim();
			if (username == "") return;
			labelTweets.Text = "0";
			pictureBox1.Image = null;


			// ツイート数を取得
			string xml = GetUserInfo(username);
			if (xml == "")
			{
				MessageBox.Show("ユーザーが見つかりませんでした", APPTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			EXDocument doc = new EXDocument();
			doc.LoadXML(xml);
			string profile_image_url = doc * "profile_image_url";
			string statuses_count = doc * "statuses_count";
			max_id = doc * "status" / "id";
			tweet_count = int.Parse(statuses_count);
			labelTweets.Text = tweet_count.ToString("#,##0");

			// 画像表示
			WebClient cl = new WebClient();
			byte[] pic = cl.DownloadData(profile_image_url);
			MemoryStream st = new MemoryStream(pic);
			pictureBox1.Image = new Bitmap(st);
			st.Close();

			_tweets = new List<Tweet>();
			worker.RunWorkerAsync();
		}
		public class Tweet
		{
			public int Num { get; set; }
			public string ID { get; set; }
			public string created_at { get; set; }
			public string text { get; set; }
			public DateTime CreatedAt
			{
				get { return convDateTime(created_at); }
			}
			private DateTime convDateTime(string created_at)
			{
				string convertStr = "ddd MMM dd HH':'mm':'ss zz'00' yyyy";
				DateTime dt = DateTime.ParseExact(created_at, convertStr,
					System.Globalization.DateTimeFormatInfo.InvariantInfo,
					System.Globalization.DateTimeStyles.None);
				return dt;
			}
		}
		/// <summary>
		/// 指定ユーザ情報を取得
		/// </summary>
		/// <param name="sname">screen_name</param>
		/// <returns></returns>
		private string GetUserInfo(string sname)
		{
			string content = "";
			Dictionary<string, string> param = new Dictionary<string, string>();
			param.Add("screen_name", sname);
			HttpStatusCode code =
			httpCon.GetContent("GET",
							new Uri(string.Format(
								"http://api.twitter.com/1/users/show.xml")),
							param,
							ref content,
							null);


			if (code != HttpStatusCode.OK)
			{
				return "";
			}
			else
			{
				return content;
			}
		}

		/// <summary>
		/// ツイートを取得
		/// </summary>
		/// <param name="sname"></param>
		/// <param name="max_id"></param>
		/// <returns></returns>
		private string GetTweets(string sname, string max_id = "")
		{
			string content = "";
			Dictionary<string, string> param = new Dictionary<string, string>();
			param.Add("screen_name", sname);
			if (max_id != "")
				param.Add("max_id", max_id);
			param.Add("count", "200");
			param.Add("trim_user", "true");
			HttpStatusCode code =
			httpCon.GetContent("GET",
							new Uri(string.Format(
								"http://api.twitter.com/1/statuses/user_timeline.xml")),
							param,
							ref content,
							null);


			if (code != HttpStatusCode.OK)
				return "";
			return content;
		}


		/// <summary>
		/// 初期設定
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSetup_Click(object sender, EventArgs e)
		{
			string pinCode = textCode.Text;
			bool res =
			oauth.AuthenticatePinFlow(
				"https://twitter.com/oauth/access_token",
				requestToken,
				pinCode);
			if (res == true)
			{
				MessageBox.Show("認証成功");
				accessToken = oauth.AccessToken;
				accessSecret = oauth.AccessTokenSecret;

				// プロキシを初期化
				HttpConnection.InitializeConnection(20, HttpConnection.ProxyType.IE, "", 0, "", "");
				httpCon.Initialize(consumerKey, consumerSecret, accessToken, accessSecret, username);


				// username = GetUsername();
				// textMessage.Text = username;

				string xml =
					"<?xml version='1.0' encoding='utf-8'?>\n" +
					"<setting>\n" +
					" <username>" + username + "</username>\n" +
					" <accessToken>" + accessToken + "</accessToken>\n" +
					" <accessSecret>" + accessSecret + "</accessSecret>\n" +
					"</setting>";
				FileStream fs = new FileStream(initfile, FileMode.OpenOrCreate);
				StreamWriter sw = new StreamWriter(fs);
				sw.WriteLine(xml);
				sw.Close();
				fs.Close();

				// 再初期化
				btnGet.Text = "取得";
				textCode.Visible = false;
				btnSetup.Visible = false;

				this.Text = string.Format(
					"{0} - {1}", this.Text, username);
			}
			else
			{
				MessageBox.Show("認証失敗");
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.Text = APPTITLE;
			LoadInit();
			if (accessToken == "")
			{
				btnGet.Text = "初期化";
				textCode.Visible = true;
				btnSetup.Visible = true;
			}
			else
			{
				textCode.Visible = false;
				btnSetup.Visible = false;
			}
		}
		/// <summary>
		/// 設定値を読み込み
		/// </summary>
		/// <returns></returns>
		private bool LoadInit()
		{

			if (File.Exists(initfile))
			{
				XmlDocument doc = new XmlDocument();
				FileStream fs = new FileStream(initfile, FileMode.Open);
				doc.Load(fs);
				XmlElement root = doc.DocumentElement;
				accessToken = root.GetElementsByTagName("accessToken")[0].InnerText;
				accessSecret = root.GetElementsByTagName("accessSecret")[0].InnerText;
				username = root.GetElementsByTagName("username")[0].InnerText;
				fs.Close();

			}
			//プロキシの設定
			HttpConnection.InitializeConnection(20, HttpConnection.ProxyType.IE, "", 0, "", "");
			httpCon.Initialize(consumerKey, consumerSecret, accessToken, accessSecret, username);

			return true;
		}

		/// <summary>
		/// 中断ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			// キャンセル
			worker.CancelAsync();
		}


		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			int err_count = 5;

			EXDocument doc = new EXDocument();
			int count = 0;
			while (count < tweet_count)
			{
				if (worker.CancellationPending == true)
					break;

				// 直近の200件を取得
				string xml = GetTweets(username, max_id);
				if (xml == "")
				{
					err_count--;
					if (err_count < 0)
					{
						MessageBox.Show("取得エラー発生", APPTITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
						worker.CancelAsync();
						return;
					}
					// 1秒待つ
					System.Threading.Thread.Sleep(1000);
					continue;
				}

				doc.LoadXML(xml);
				EXElements items = doc * "status";

				foreach (EXElement status in items)
				{
					string id = status / "id";
					if (max_id == id) continue;

					count++;
					Tweet twi = new Tweet()
					{
						Num = count,
						ID = status / "id",
						text = status / "text",
						created_at = status / "created_at"
					};
					_tweets.Add(twi);
					max_id = twi.ID;
				}
				worker.ReportProgress(count * 100 / tweet_count);
				if (items.Count < 200)
					break;
			}
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			dataGridView1.DataSource = null;
			dataGridView1.DataSource = _tweets;
			labelCrawl.Text = _tweets.Count.ToString("#,##0");
			labelSapn.Text = (DateTime.Now - startTime).ToString(@"mm\:ss");
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			dataGridView1.DataSource = null;
			dataGridView1.DataSource = _tweets;
			labelCrawl.Text = _tweets.Count.ToString("#,##0");
			labelSapn.Text = (DateTime.Now - startTime).ToString(@"mm\:ss");
			if (e.Cancelled == false)
			{
				MessageBox.Show("取得が完了しました", APPTITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			dataGridView1.ClipboardCopyMode =
				DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
			dataGridView1.SelectAll();
			Clipboard.SetDataObject(dataGridView1.GetClipboardContent());
			MessageBox.Show("クリップボードにコピーしました");
		}
	}
}
