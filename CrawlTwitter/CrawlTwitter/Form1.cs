using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CrawlTwitter
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private int countTweets = 0;
		private string screenName = "";
		private bool stoped = false;
		private DateTime startTime;

		/// <summary>
		/// 確認ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e)
		{
			screenName = textBox1.Text;
			string url = "http://twitter.com/" + screenName;
			webBrowser1.Navigate(url);
		}

		/// <summary>
		/// 読み込み完了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			HtmlDocument doc = webBrowser1.Document;
			textBox2.Text = doc.Body.InnerHtml;
			var links = doc.GetElementsByTagName("a");
			string tweets = "";
			foreach (HtmlElement el in links)
			{
				string val = el.GetAttribute("className");
				Debug.Print("class:" + val);
				if (val == "user-stats-count user-stats-tweets")
				{
					tweets = el.InnerText;
					break;
				}
			}
			labelTweets.Text = tweets;
			countTweets = int.Parse(tweets.Replace(",", "").Replace("ツイート",""));
			labelTweets.Text = countTweets.ToString();
		}

		/// <summary>
		/// 実行ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e)
		{
			string js = @"
var links = document.getElementsByTagName('a');
for(i=0; i<links.length; i++ ) {
  var el = links[i];
  if ( el.className == 'more-button' ) {
    el.click();
  }
}
";
			startTime = DateTime.Now;
			button2.Enabled = false;
			button3.Enabled = true;

			int max = countTweets;
			int count = 0;
			HtmlDocument doc = webBrowser1.Document;
			while (true)
			{
				webBrowser1.Url = new Uri("javascript:" + Uri.EscapeDataString(js) + ";");
				// 1秒待つ
				for (int i = 0; i < 10; i++)
				{
					Application.DoEvents();
					System.Threading.Thread.Sleep(100);
				}
				doc = webBrowser1.Document;
				if (isAllTweets(doc) == true)
				{
					stoped = true;
				}
				// count = getTweetCount(doc);
				// labelCrawl.Text = count.ToString();
				count += 20;
				labelCrawl.Text = count.ToString();
				labelSapn.Text = (DateTime.Now - startTime).ToString(@"mm\:ss");
				if (stoped == true) break;
			}
			textBox2.Text = doc.Body.InnerHtml;
			List<Tweet> items = getTweets(doc);
			dataGridView1.DataSource = items;

			button2.Enabled = true;
			button3.Enabled = false;
		}

		/// <summary>
		/// 表示されている自分のツイート数をカウントする
		/// 非常に遅いので未使用
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public int getTweetCount(HtmlDocument doc)
		{
			int count = 0;
			string href0 = "";
			foreach (HtmlElement el in doc.GetElementsByTagName("a"))
			{
				string cname = el.GetAttribute("className");
				if (cname == "tweet-timestamp js-permalink")
				{
					string href = el.GetAttribute("href");
					if ( href0 != href &&
						href.StartsWith("http://twitter.com/#!/" + screenName)== true) {
						count++;
						href0 = href;
					}
				}
			}
			return count;
		}
		/// <summary>
		/// 終端をチェックする
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public bool isAllTweets(HtmlDocument doc )
		{
			foreach (HtmlElement el in doc.Links)
			{
				if (el.InnerText == "TOPへ戻る↑")
				{
					return true;
				}

			}
			return false;
		}

		/// <summary>
		/// 自分のツイートを取得する
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public List<Tweet> getTweets( HtmlDocument doc )
		{
			List<Tweet> items = new List<Tweet>();

			Tweet tw = null;
			bool skip = false;
			foreach (HtmlElement el in doc.All)
			{
				string cname = el.GetAttribute("className");
				if ( 	cname == "tweet-screen-name user-profile-link js-action-profile-name" ) {
					skip = el.InnerText != screenName? true: false;
				}
				// 対象のツイートのみ取得
				if (skip == true)
					continue;

				switch (cname)
				{
					case "tweet-text js-tweet-text":
						if (tw == null || tw.Text != el.InnerText)
						{
							tw = new Tweet();
							tw.Text = el.InnerText;
							items.Add(tw);
						}
						break;
					case "tweet-actions js-actions":
						tw.ID = el.GetAttribute("data-tweet-id");
						break;
					case "tweet-timestamp js-permalink":
						tw.Permalink = el.GetAttribute("href");
						break;
					case  "_timestamp js-tweet-timestamp":
						tw.Timestamp = el.GetAttribute("data-time");
						if (el.GetAttribute("title") != "")
						{
							tw.TimestampText = el.GetAttribute("title");
						}
						break;
				}
			}
			return items;
		}

		/// <summary>
		/// 中断
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{
			stoped = true;
			button3.Enabled = false;
		}

		/// <summary>
		/// クリップボードへコピー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button4_Click(object sender, EventArgs e)
		{
			dataGridView1.ClipboardCopyMode = 
				DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
			dataGridView1.SelectAll();
			Clipboard.SetDataObject(dataGridView1.GetClipboardContent());
			MessageBox.Show("クリップボードにコピーしました");
		}
	}

	public class Tweet
	{
		public string ID { get; set; }
		public string Text { get; set; }
		public string Timestamp { get; set; }
		public string TimestampText { get; set; }
		public string Permalink { get; set; }
	}
}
