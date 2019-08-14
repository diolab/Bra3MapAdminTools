using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bra3MapAdminTools
{
    public partial class ConfigDlg : Form
    {
        public ConfigDlg()
        {
            InitializeComponent();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfigDlg_Load(object sender, EventArgs e)
        {
            Config conf = new Config();

            //ログイン
            id.Text = conf.get(Config.LOGIN_ID);
            pwd.Text = conf.get(Config.LOGIN_PW);
            if (!string.Empty.Equals(conf.get(Config.LOGIN_SV)))
            {
                sv.Text = conf.get(Config.LOGIN_SV);
            }

            //同盟
            alli_owner.Text = conf.get(Config.ALLI_OWN);
            alli_name.Text = conf.get(Config.ALLI_NM);

            //共通
            setDefX.Value = conf.getInt(Config.DEF_X);
            setDefY.Value = conf.getInt(Config.DEF_Y);


            //広域マップ
            colAlliance.Items.Clear();
            colAlliance.Items.Add(string.Empty);
            for (int i = 0; i < conf.getInt(Config.ALLIANCE_LIST_SIZE); i++)
            {
                colAlliance.Items.Add(conf.get(string.Format("{0}_{1}", Config.ALLIANCE_LIST, i)));                
            }
            
            dgvBigMapColor.Rows.Clear();
            for (int row = 0; row < conf.getInt(Config.BIGMAP_COLOR_TABLE_SIZE); row++)
            {
                string keyF = string.Format("{0}_{1}", Config.BIGMAP_COLOR_TABLE, row);
                this.addColorSetting( bool.Parse(conf.get(string.Format("{0}_{1}", keyF, 0)))
                                , Color.FromArgb(conf.getInt(string.Format("{0}_{1}", keyF, 1)))
                                , conf.get(string.Format("{0}_{1}", keyF, 2))
                                , conf.get(string.Format("{0}_{1}", keyF, 3))
                                , conf.get(string.Format("{0}_{1}", keyF, 4))
                                , conf.get(string.Format("{0}_{1}", keyF, 5))
                                , conf.get(string.Format("{0}_{1}", keyF, 6))
                                , conf.get(string.Format("{0}_{1}", keyF, 7))
                                , conf.get(string.Format("{0}_{1}", keyF, 8))
                                , conf.get(string.Format("{0}_{1}", keyF, 9))
                                , conf.get(string.Format("{0}_{1}", keyF, 10))
                );
            }
            if (1 >= dgvBigMapColor.Rows.Count)
            {
                this.resetColors();
            }

            //データベース
            setDbPath.Text = conf.get(Config.TOOL_DB_DIR);
            backupPath.Text = conf.get(Config.TOOL_DB_BACKUP_DIR);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            saveBtn.Enabled = false;

            Config conf = new Config();

            conf.set(Config.LOGIN_ID, id.Text);
            conf.set(Config.LOGIN_PW, pwd.Text);
            conf.set(Config.LOGIN_SV, sv.Text);

            conf.set(Config.ALLI_OWN, alli_owner.Text);
            conf.set(Config.ALLI_NM, alli_name.Text);

            conf.set(Config.DEF_X, setDefX.Value.ToString());
            conf.set(Config.DEF_Y, setDefY.Value.ToString());

            conf.set(Config.TOOL_DB_DIR, setDbPath.Text);
            conf.set(Config.TOOL_DB_BACKUP_DIR, backupPath.Text);

            conf.set(Config.BIGMAP_COLOR_TABLE_SIZE, (dgvBigMapColor.Rows.Count - 1).ToString());
            for (int row = 0; row < dgvBigMapColor.Rows.Count - 1; row++)
            {
                for (int col = 0; col < dgvBigMapColor.Columns.Count - 1; col++)
                {
                    string key = string.Format("{0}_{1}_{2}", Config.BIGMAP_COLOR_TABLE, row, col);
                    if (1 == col)
                    {
                        conf.set(key, dgvBigMapColor.Rows[row].Cells[col].Style.BackColor.ToArgb().ToString());
                    }
                    else
                    {
                        if (null == dgvBigMapColor.Rows[row].Cells[col].Value)
                        {
                            conf.set(key, string.Empty);
                        }
                        else
                        {
                            conf.set(key, dgvBigMapColor.Rows[row].Cells[col].Value.ToString());
                        }
                    }
                }
            }

            msg.Text = "保存しました。";

            saveBtn.Enabled = true;
        }

        private void delLoginDataBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("ログイン情報を完全に削除します。よろしいですか？"
                , "削除確認", MessageBoxButtons.YesNo))
            {
                Config conf = new Config();

                conf.del(Config.LOGIN_ID);
                conf.del(Config.LOGIN_PW);

                id.Text = string.Empty;
                pwd.Text = string.Empty;

                msg.Text = "ログイン情報を削除しました。";
            }
        }

        private void setDbDirBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "データベースの構築先を選択してください。";
            folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = System.IO.Directory.GetCurrentDirectory();

            // ダイアログを表示し、戻り値が [OK] の場合は、選択したディレクトリを表示する
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                setDbPath.Text = folderBrowserDialog.SelectedPath;
            }

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            folderBrowserDialog.Dispose();
        }

        private void resetDbPath_Click(object sender, EventArgs e)
        {
            setDbPath.Text = string.Empty;
        }

        private void dgvBigMapColor_SelectionChanged(object sender, EventArgs e)
        {
            if (1 != dgvBigMapColor.SelectedCells.Count) { return; }
            foreach (DataGridViewCell cell in dgvBigMapColor.SelectedCells)
            {
                if (1 == cell.ColumnIndex)
                {
                    ColorDialog dlg = new ColorDialog();
                    dlg.Color = cell.Style.BackColor;
                    if (DialogResult.OK == dlg.ShowDialog())
                    {
                        cell.Style.BackColor = dlg.Color;
                        cell.Style.SelectionBackColor = dlg.Color;
                    }
                }
            }
        }
        private void dgvBigMapColor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvBigMapColor.Columns[e.ColumnIndex].GetType().Equals(typeof(DataGridViewComboBoxColumn)))
                {
                    // 編集モードにする
                    dgvBigMapColor.BeginEdit(true);
                    // 編集モードにしたので現在の編集コントロールを取得
                    DataGridViewComboBoxEditingControl edt = dgvBigMapColor.EditingControl as DataGridViewComboBoxEditingControl;
                    // ドロップダウンさせる
                    edt.DroppedDown = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("現在の設定を削除し、初期設定に戻します。よろしいですか？"
                , "初期化確認", MessageBoxButtons.YesNo))
            {
                this.resetColors();
            }
        }

        private void resetColors()
        {

            dgvBigMapColor.Rows.Clear();

            this.addColorSetting(true, Color.DarkViolet, "NCP城&砦", string.Empty, string.Empty, string.Empty
                , string.Empty, string.Empty, string.Empty, string.Empty
                , "NCP城&砦");

            this.addColorSetting(true, Color.LightGreen, "拠点", alli_name.Text, string.Empty, string.Empty
                , string.Empty, string.Empty, string.Empty, string.Empty
                , "同盟員の拠点");

            this.addColorSetting(true, Color.Green, "領地&拠点", alli_name.Text, string.Empty, string.Empty
                , string.Empty, string.Empty, string.Empty, string.Empty
                , "同盟員の領地");

            this.addColorSetting(true, Color.Red, "拠点", string.Empty, string.Empty, string.Empty
                , string.Empty, string.Empty, string.Empty, string.Empty
                , "他同盟の拠点");

            this.addColorSetting(true, Color.Tomato, "領地&拠点", string.Empty, string.Empty, string.Empty
                , string.Empty, string.Empty, string.Empty, string.Empty
                , "他同盟の領地");

        }

        private void addColorSetting(bool flag
                                    , Color color
                                    , string type
                                    , string alliance
                                    , string owner
                                    , string level
                                    , string wood
                                    , string stone
                                    , string iron
                                    , string rice 
                                    , string comment )
        {
            try
            {
                int row = dgvBigMapColor.Rows.Count - 1;

                dgvBigMapColor.Rows.Add();
                dgvBigMapColor.Rows[row].Cells[0].Value = flag;
                dgvBigMapColor.Rows[row].Cells[1].Style.BackColor = color;
                dgvBigMapColor.Rows[row].Cells[1].Style.SelectionBackColor = color;
                dgvBigMapColor.Rows[row].Cells[2].Value = type;
                dgvBigMapColor.Rows[row].Cells[3].Value = alliance;
                dgvBigMapColor.Rows[row].Cells[4].Value = owner;
                dgvBigMapColor.Rows[row].Cells[5].Value = level;
                dgvBigMapColor.Rows[row].Cells[6].Value = wood;
                dgvBigMapColor.Rows[row].Cells[7].Value = stone;
                dgvBigMapColor.Rows[row].Cells[8].Value = iron;
                dgvBigMapColor.Rows[row].Cells[9].Value = rice;
                dgvBigMapColor.Rows[row].Cells[10].Value = comment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        private void upBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (0 >= dgvBigMapColor.SelectedCells.Count) { return; }
                DataGridViewCell cell = dgvBigMapColor.SelectedCells[0];
                if (1 > cell.RowIndex) { return; }

                string format = "Sort{0}";

                for (int r = 0; r < dgvBigMapColor.Rows.Count; r++)
                {
                    dgvBigMapColor.Rows[r].Cells[11].Value = string.Format(format, r.ToString());
                }

                dgvBigMapColor.Rows[cell.RowIndex].Cells[11].Value = string.Format(format, cell.RowIndex - 1);
                dgvBigMapColor.Rows[cell.RowIndex - 1].Cells[11].Value = string.Format(format, cell.RowIndex);

                dgvBigMapColor.Sort(dgvBigMapColor.Columns[11], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void downBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (0 >= dgvBigMapColor.SelectedCells.Count) { return; }
                DataGridViewCell cell = dgvBigMapColor.SelectedCells[0];
                if (cell.RowIndex >= dgvBigMapColor.Rows.Count) { return; }

                string format = "Sort{0}";

                for (int r = 0; r < dgvBigMapColor.Rows.Count; r++)
                {
                    dgvBigMapColor.Rows[r].Cells[11].Value = string.Format(format, r.ToString());
                }

                dgvBigMapColor.Rows[cell.RowIndex].Cells[11].Value = string.Format(format, cell.RowIndex + 1);
                dgvBigMapColor.Rows[cell.RowIndex + 1].Cells[11].Value = string.Format(format, cell.RowIndex);

                dgvBigMapColor.Sort(dgvBigMapColor.Columns[11], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void backupPathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "データベースのバックアップ先を選択してください。";
            folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = System.IO.Directory.GetCurrentDirectory();

            // ダイアログを表示し、戻り値が [OK] の場合は、選択したディレクトリを表示する
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                backupPath.Text = folderBrowserDialog.SelectedPath;
            }

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            folderBrowserDialog.Dispose();
        }

        private void dgvBigMapColor_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Do Nothing
        }
       
        
    }
}