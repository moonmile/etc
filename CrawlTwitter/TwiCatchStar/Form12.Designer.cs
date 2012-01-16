namespace TwiCatchStar
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
			this.btnGet = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.labelSapn = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnCopy = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.labelCrawl = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelTweets = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSetup = new System.Windows.Forms.Button();
			this.textCode = new System.Windows.Forms.TextBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.worker = new System.ComponentModel.BackgroundWorker();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnGet
			// 
			this.btnGet.Location = new System.Drawing.Point(204, 11);
			this.btnGet.Name = "btnGet";
			this.btnGet.Size = new System.Drawing.Size(76, 32);
			this.btnGet.TabIndex = 7;
			this.btnGet.Text = "取得";
			this.btnGet.UseVisualStyleBackColor = true;
			this.btnGet.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(186, 26);
			this.textBox1.TabIndex = 6;
			// 
			// labelSapn
			// 
			this.labelSapn.AutoSize = true;
			this.labelSapn.Location = new System.Drawing.Point(82, 157);
			this.labelSapn.Name = "labelSapn";
			this.labelSapn.Size = new System.Drawing.Size(31, 12);
			this.labelSapn.TabIndex = 23;
			this.labelSapn.Text = "00:00";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 157);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 12);
			this.label3.TabIndex = 22;
			this.label3.Text = "経過時間：";
			// 
			// btnCopy
			// 
			this.btnCopy.Location = new System.Drawing.Point(12, 280);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(101, 32);
			this.btnCopy.TabIndex = 21;
			this.btnCopy.Text = "コピー";
			this.btnCopy.UseVisualStyleBackColor = true;
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(119, 49);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.Size = new System.Drawing.Size(514, 264);
			this.dataGridView1.TabIndex = 20;
			// 
			// labelCrawl
			// 
			this.labelCrawl.AutoSize = true;
			this.labelCrawl.Location = new System.Drawing.Point(82, 133);
			this.labelCrawl.Name = "labelCrawl";
			this.labelCrawl.Size = new System.Drawing.Size(31, 12);
			this.labelCrawl.TabIndex = 19;
			this.labelCrawl.Text = "0,000";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 133);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 12);
			this.label4.TabIndex = 18;
			this.label4.Text = "クロール数：";
			// 
			// labelTweets
			// 
			this.labelTweets.AutoSize = true;
			this.labelTweets.Location = new System.Drawing.Point(82, 111);
			this.labelTweets.Name = "labelTweets";
			this.labelTweets.Size = new System.Drawing.Size(31, 12);
			this.labelTweets.TabIndex = 17;
			this.labelTweets.Text = "0,000";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 111);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 12);
			this.label1.TabIndex = 16;
			this.label1.Text = "ツイート数：";
			// 
			// btnSetup
			// 
			this.btnSetup.Location = new System.Drawing.Point(558, 15);
			this.btnSetup.Name = "btnSetup";
			this.btnSetup.Size = new System.Drawing.Size(75, 23);
			this.btnSetup.TabIndex = 25;
			this.btnSetup.Text = "設定";
			this.btnSetup.UseVisualStyleBackColor = true;
			this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(427, 19);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(125, 19);
			this.textCode.TabIndex = 24;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(286, 10);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(76, 32);
			this.btnStop.TabIndex = 26;
			this.btnStop.Text = "中断";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(12, 49);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(48, 48);
			this.pictureBox1.TabIndex = 27;
			this.pictureBox1.TabStop = false;
			// 
			// worker
			// 
			this.worker.WorkerReportsProgress = true;
			this.worker.WorkerSupportsCancellation = true;
			this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
			this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
			this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(645, 324);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnSetup);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.labelSapn);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.labelCrawl);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelTweets);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnGet);
			this.Controls.Add(this.textBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGet;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label labelSapn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label labelCrawl;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelTweets;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSetup;
		private System.Windows.Forms.TextBox textCode;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.ComponentModel.BackgroundWorker worker;
	}
}

