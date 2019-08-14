namespace Bra3MapAdminTools
{
    partial class ConfigDlg
    {
        /// <summary>
        /// 必要なデザイナ変数です。
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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.saveBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.loginPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.delLoginDataBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.sv = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pwd = new System.Windows.Forms.TextBox();
            this.id = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.alli_name = new System.Windows.Forms.TextBox();
            this.alli_owner = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.setDefY = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.setDefX = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.tabBigMap = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dgvBigMapColor = new System.Windows.Forms.DataGridView();
            this.colChk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colBkColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colAlliance = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWood = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIron = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resetBtn = new System.Windows.Forms.Button();
            this.downBtn = new System.Windows.Forms.Button();
            this.upBtn = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.backupPathBtn = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.backupPath = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.resetDbPath = new System.Windows.Forms.Button();
            this.setDbDirBtn = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.setDbPath = new System.Windows.Forms.TextBox();
            this.msg = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.loginPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sv)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDefY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setDefX)).BeginInit();
            this.tabBigMap.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBigMapColor)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(226, 236);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 1000;
            this.saveBtn.Tag = "1000";
            this.saveBtn.Text = "保存(&A)";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.Location = new System.Drawing.Point(307, 236);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 1100;
            this.closeBtn.Text = "閉じる(&C)";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.loginPage);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabBigMap);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(394, 233);
            this.tabControl1.TabIndex = 2;
            // 
            // loginPage
            // 
            this.loginPage.Controls.Add(this.groupBox1);
            this.loginPage.Location = new System.Drawing.Point(4, 21);
            this.loginPage.Name = "loginPage";
            this.loginPage.Padding = new System.Windows.Forms.Padding(3);
            this.loginPage.Size = new System.Drawing.Size(386, 208);
            this.loginPage.TabIndex = 0;
            this.loginPage.Text = "ログイン";
            this.loginPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.delLoginDataBtn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.sv);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pwd);
            this.groupBox1.Controls.Add(this.id);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "mixi ログイン情報";
            // 
            // delLoginDataBtn
            // 
            this.delLoginDataBtn.Location = new System.Drawing.Point(214, 100);
            this.delLoginDataBtn.Name = "delLoginDataBtn";
            this.delLoginDataBtn.Size = new System.Drawing.Size(150, 23);
            this.delLoginDataBtn.TabIndex = 4;
            this.delLoginDataBtn.Tag = "4";
            this.delLoginDataBtn.Text = "ログイン情報削除";
            this.delLoginDataBtn.UseVisualStyleBackColor = true;
            this.delLoginDataBtn.Click += new System.EventHandler(this.delLoginDataBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "鯖：";
            // 
            // sv
            // 
            this.sv.Location = new System.Drawing.Point(72, 69);
            this.sv.Name = "sv";
            this.sv.Size = new System.Drawing.Size(76, 19);
            this.sv.TabIndex = 3;
            this.sv.Tag = "3";
            this.sv.Value = new decimal(new int[] {
            33,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "パスワード：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ログインID：";
            // 
            // pwd
            // 
            this.pwd.Location = new System.Drawing.Point(72, 43);
            this.pwd.Name = "pwd";
            this.pwd.PasswordChar = '*';
            this.pwd.Size = new System.Drawing.Size(292, 19);
            this.pwd.TabIndex = 1;
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(72, 18);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(292, 19);
            this.id.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(386, 208);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "同盟";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.alli_name);
            this.groupBox2.Controls.Add(this.alli_owner);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(374, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "同盟情報";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "同盟名：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "盟主：";
            // 
            // alli_name
            // 
            this.alli_name.Location = new System.Drawing.Point(73, 44);
            this.alli_name.Name = "alli_name";
            this.alli_name.Size = new System.Drawing.Size(150, 19);
            this.alli_name.TabIndex = 1;
            // 
            // alli_owner
            // 
            this.alli_owner.Location = new System.Drawing.Point(73, 18);
            this.alli_owner.Name = "alli_owner";
            this.alli_owner.Size = new System.Drawing.Size(150, 19);
            this.alli_owner.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(386, 208);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "共通";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.setDefY);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.setDefX);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(377, 65);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "初期値";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(148, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "Y";
            // 
            // setDefY
            // 
            this.setDefY.Location = new System.Drawing.Point(165, 17);
            this.setDefY.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.setDefY.Minimum = new decimal(new int[] {
            800,
            0,
            0,
            -2147483648});
            this.setDefY.Name = "setDefY";
            this.setDefY.Size = new System.Drawing.Size(50, 19);
            this.setDefY.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(71, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "X";
            // 
            // setDefX
            // 
            this.setDefX.Location = new System.Drawing.Point(88, 17);
            this.setDefX.Maximum = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.setDefX.Minimum = new decimal(new int[] {
            800,
            0,
            0,
            -2147483648});
            this.setDefX.Name = "setDefX";
            this.setDefX.Size = new System.Drawing.Size(50, 19);
            this.setDefX.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "基準座標：";
            // 
            // tabBigMap
            // 
            this.tabBigMap.Controls.Add(this.groupBox6);
            this.tabBigMap.Location = new System.Drawing.Point(4, 21);
            this.tabBigMap.Name = "tabBigMap";
            this.tabBigMap.Size = new System.Drawing.Size(386, 208);
            this.tabBigMap.TabIndex = 5;
            this.tabBigMap.Text = "広域マップ";
            this.tabBigMap.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.dgvBigMapColor);
            this.groupBox6.Controls.Add(this.resetBtn);
            this.groupBox6.Controls.Add(this.downBtn);
            this.groupBox6.Controls.Add(this.upBtn);
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(380, 202);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "背景色";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 187);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(176, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "上にある方が優先度が高くなります。";
            // 
            // dgvBigMapColor
            // 
            this.dgvBigMapColor.AllowUserToResizeRows = false;
            this.dgvBigMapColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBigMapColor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBigMapColor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChk,
            this.colBkColor,
            this.colType,
            this.colAlliance,
            this.colName,
            this.colLevel,
            this.colWood,
            this.colStone,
            this.colIron,
            this.colRice,
            this.colComment,
            this.colSort});
            this.dgvBigMapColor.Location = new System.Drawing.Point(5, 18);
            this.dgvBigMapColor.MultiSelect = false;
            this.dgvBigMapColor.Name = "dgvBigMapColor";
            this.dgvBigMapColor.RowHeadersWidth = 25;
            this.dgvBigMapColor.RowTemplate.Height = 21;
            this.dgvBigMapColor.Size = new System.Drawing.Size(312, 166);
            this.dgvBigMapColor.TabIndex = 3;
            this.dgvBigMapColor.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBigMapColor_CellClick);
            this.dgvBigMapColor.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvBigMapColor_DataError);
            this.dgvBigMapColor.SelectionChanged += new System.EventHandler(this.dgvBigMapColor_SelectionChanged);
            // 
            // colChk
            // 
            this.colChk.HeaderText = "有効";
            this.colChk.Name = "colChk";
            this.colChk.Width = 40;
            // 
            // colBkColor
            // 
            this.colBkColor.HeaderText = "背景色";
            this.colBkColor.Name = "colBkColor";
            this.colBkColor.ReadOnly = true;
            this.colBkColor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colBkColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colBkColor.Width = 50;
            // 
            // colType
            // 
            this.colType.HeaderText = "種類";
            this.colType.Items.AddRange(new object[] {
            "領地&拠点",
            "領地",
            "拠点",
            "NCP城&砦",
            "空き地"});
            this.colType.Name = "colType";
            this.colType.Width = 90;
            // 
            // colAlliance
            // 
            this.colAlliance.HeaderText = "同盟";
            this.colAlliance.Name = "colAlliance";
            this.colAlliance.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colName
            // 
            this.colName.HeaderText = "君主";
            this.colName.Name = "colName";
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "戦力";
            this.colLevel.MaxInputLength = 1;
            this.colLevel.Name = "colLevel";
            this.colLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLevel.ToolTipText = "領地レベルを半角数字の 1 ～ 9 で入力してください。";
            this.colLevel.Width = 40;
            // 
            // colWood
            // 
            this.colWood.HeaderText = "木";
            this.colWood.MaxInputLength = 2;
            this.colWood.Name = "colWood";
            this.colWood.Width = 20;
            // 
            // colStone
            // 
            this.colStone.HeaderText = "石";
            this.colStone.MaxInputLength = 2;
            this.colStone.Name = "colStone";
            this.colStone.Width = 20;
            // 
            // colIron
            // 
            this.colIron.HeaderText = "鉄";
            this.colIron.MaxInputLength = 2;
            this.colIron.Name = "colIron";
            this.colIron.Width = 20;
            // 
            // colRice
            // 
            this.colRice.HeaderText = "糧";
            this.colRice.MaxInputLength = 2;
            this.colRice.Name = "colRice";
            this.colRice.Width = 20;
            // 
            // colComment
            // 
            this.colComment.HeaderText = "コメント";
            this.colComment.Name = "colComment";
            this.colComment.Width = 200;
            // 
            // colSort
            // 
            this.colSort.HeaderText = "sort";
            this.colSort.Name = "colSort";
            this.colSort.Visible = false;
            // 
            // resetBtn
            // 
            this.resetBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetBtn.Location = new System.Drawing.Point(323, 161);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(51, 23);
            this.resetBtn.TabIndex = 2;
            this.resetBtn.Text = "初期化";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.resetBtn_Click);
            // 
            // downBtn
            // 
            this.downBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downBtn.Location = new System.Drawing.Point(323, 47);
            this.downBtn.Name = "downBtn";
            this.downBtn.Size = new System.Drawing.Size(51, 23);
            this.downBtn.TabIndex = 1;
            this.downBtn.Text = "▼";
            this.downBtn.UseVisualStyleBackColor = true;
            this.downBtn.Click += new System.EventHandler(this.downBtn_Click);
            // 
            // upBtn
            // 
            this.upBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.upBtn.Location = new System.Drawing.Point(323, 18);
            this.upBtn.Name = "upBtn";
            this.upBtn.Size = new System.Drawing.Size(51, 23);
            this.upBtn.TabIndex = 0;
            this.upBtn.Text = "▲";
            this.upBtn.UseVisualStyleBackColor = true;
            this.upBtn.Click += new System.EventHandler(this.upBtn_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Location = new System.Drawing.Point(4, 21);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(386, 208);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "データベース";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.backupPathBtn);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.backupPath);
            this.groupBox7.Location = new System.Drawing.Point(1, 112);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(377, 76);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "データベースバックアップ先";
            // 
            // backupPathBtn
            // 
            this.backupPathBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backupPathBtn.Location = new System.Drawing.Point(296, 40);
            this.backupPathBtn.Name = "backupPathBtn";
            this.backupPathBtn.Size = new System.Drawing.Size(75, 23);
            this.backupPathBtn.TabIndex = 2;
            this.backupPathBtn.Text = "参照";
            this.backupPathBtn.UseVisualStyleBackColor = true;
            this.backupPathBtn.Click += new System.EventHandler(this.backupPathBtn_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 12);
            this.label12.TabIndex = 1;
            this.label12.Text = "※空白の場合は何もしません";
            // 
            // backupPath
            // 
            this.backupPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.backupPath.Location = new System.Drawing.Point(6, 18);
            this.backupPath.Name = "backupPath";
            this.backupPath.Size = new System.Drawing.Size(365, 19);
            this.backupPath.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.resetDbPath);
            this.groupBox5.Controls.Add(this.setDbDirBtn);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.setDbPath);
            this.groupBox5.Location = new System.Drawing.Point(6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(377, 100);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "データベース作成先";
            // 
            // resetDbPath
            // 
            this.resetDbPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetDbPath.Location = new System.Drawing.Point(296, 69);
            this.resetDbPath.Name = "resetDbPath";
            this.resetDbPath.Size = new System.Drawing.Size(75, 23);
            this.resetDbPath.TabIndex = 3;
            this.resetDbPath.Text = "リセット";
            this.resetDbPath.UseVisualStyleBackColor = true;
            this.resetDbPath.Click += new System.EventHandler(this.resetDbPath_Click);
            // 
            // setDbDirBtn
            // 
            this.setDbDirBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.setDbDirBtn.Location = new System.Drawing.Point(296, 40);
            this.setDbDirBtn.Name = "setDbDirBtn";
            this.setDbDirBtn.Size = new System.Drawing.Size(75, 23);
            this.setDbDirBtn.TabIndex = 2;
            this.setDbDirBtn.Text = "参照";
            this.setDbDirBtn.UseVisualStyleBackColor = true;
            this.setDbDirBtn.Click += new System.EventHandler(this.setDbDirBtn_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(206, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "※空白の場合はアプリフォルダに作成します";
            // 
            // setDbPath
            // 
            this.setDbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.setDbPath.Location = new System.Drawing.Point(6, 18);
            this.setDbPath.Name = "setDbPath";
            this.setDbPath.ReadOnly = true;
            this.setDbPath.Size = new System.Drawing.Size(365, 19);
            this.setDbPath.TabIndex = 0;
            // 
            // msg
            // 
            this.msg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.msg.AutoSize = true;
            this.msg.Location = new System.Drawing.Point(12, 241);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(0, 12);
            this.msg.TabIndex = 3;
            // 
            // ConfigDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 266);
            this.Controls.Add(this.msg);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.saveBtn);
            this.Name = "ConfigDlg";
            this.Text = "設定";
            this.Load += new System.EventHandler(this.ConfigDlg_Load);
            this.tabControl1.ResumeLayout(false);
            this.loginPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sv)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setDefY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setDefX)).EndInit();
            this.tabBigMap.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBigMapColor)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage loginPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pwd;
        private System.Windows.Forms.TextBox id;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button delLoginDataBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown sv;
        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox alli_name;
        private System.Windows.Forms.TextBox alli_owner;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown setDefY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown setDefX;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button setDbDirBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox setDbPath;
        private System.Windows.Forms.Button resetDbPath;
        private System.Windows.Forms.TabPage tabBigMap;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.Button downBtn;
        private System.Windows.Forms.Button upBtn;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView dgvBigMapColor;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChk;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBkColor;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private System.Windows.Forms.DataGridViewComboBoxColumn colAlliance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWood;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIron;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSort;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button backupPathBtn;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox backupPath;
    }
}