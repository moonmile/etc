namespace CrawlTwitter
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTweets = new System.Windows.Forms.Label();
			this.labelCrawl = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.labelSapn = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(76, 32);
			this.button1.TabIndex = 0;
			this.button1.Text = "確認";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBox1.Location = new System.Drawing.Point(95, 19);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(186, 26);
			this.textBox1.TabIndex = 1;
			// 
			// webBrowser1
			// 
			this.webBrowser1.Location = new System.Drawing.Point(12, 51);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(563, 241);
			this.webBrowser1.TabIndex = 2;
			this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(13, 505);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(96, 57);
			this.textBox2.TabIndex = 3;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(287, 12);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(76, 32);
			this.button2.TabIndex = 5;
			this.button2.Text = "実行";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 302);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "ツイート数：";
			// 
			// labelTweets
			// 
			this.labelTweets.AutoSize = true;
			this.labelTweets.Location = new System.Drawing.Point(78, 302);
			this.labelTweets.Name = "labelTweets";
			this.labelTweets.Size = new System.Drawing.Size(31, 12);
			this.labelTweets.TabIndex = 7;
			this.labelTweets.Text = "0,000";
			// 
			// labelCrawl
			// 
			this.labelCrawl.AutoSize = true;
			this.labelCrawl.Location = new System.Drawing.Point(78, 324);
			this.labelCrawl.Name = "labelCrawl";
			this.labelCrawl.Size = new System.Drawing.Size(31, 12);
			this.labelCrawl.TabIndex = 9;
			this.labelCrawl.Text = "0,000";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 324);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 12);
			this.label4.TabIndex = 8;
			this.label4.Text = "クロール数：";
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(115, 298);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.Size = new System.Drawing.Size(460, 264);
			this.dataGridView1.TabIndex = 10;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(369, 12);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(76, 32);
			this.button3.TabIndex = 11;
			this.button3.Text = "中断";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(15, 467);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(76, 32);
			this.button4.TabIndex = 12;
			this.button4.Text = "コピー";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// labelSapn
			// 
			this.labelSapn.AutoSize = true;
			this.labelSapn.Location = new System.Drawing.Point(78, 348);
			this.labelSapn.Name = "labelSapn";
			this.labelSapn.Size = new System.Drawing.Size(31, 12);
			this.labelSapn.TabIndex = 14;
			this.labelSapn.Text = "00:00";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 348);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 13;
			this.label3.Text = "経過時間：";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(587, 574);
			this.Controls.Add(this.labelSapn);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.labelCrawl);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelTweets);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Crawl Twitter";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelTweets;
		private System.Windows.Forms.Label labelCrawl;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Label labelSapn;
		private System.Windows.Forms.Label label3;
	}
}

