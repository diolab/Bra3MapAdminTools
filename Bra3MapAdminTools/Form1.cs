using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Text.RegularExpressions;

namespace Bra3MapAdminTools
{
    public partial class MainForm : Form
    {
        //flags
        bool loginFlag = false;
        bool lording = false;
        bool craw = false;

        //const params
        const int LOG_LENGTH = 10000;

        //member
        SQLiteConnection _mapdata_connection = null;
        Dictionary<string, string> sqlList = new Dictionary<string, string>();

        public MainForm()
        {
            InitializeComponent();
        }

        ~MainForm()
        {
            this.closeDBConnect();
        }

        private void closeDBConnect()
        {
            if (null != _mapdata_connection)
            {
                _mapdata_connection.Close();
                _mapdata_connection = null;
            }
        }

        private void CloseMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.setSqlList();
            if (selectSql.Items.Count > 0) { selectSql.SelectedIndex = 0; }

            this.setAllianceList();
            this.setMapList();

            Config conf = new Config();
            defX.Value = conf.getInt(Config.DEF_X);
            defY.Value = conf.getInt(Config.DEF_Y);

            casDefX.Value = conf.getInt(Config.DEF_X);
            casDefY.Value = conf.getInt(Config.DEF_Y);

            mapposX.Value = conf.getInt(Config.DEF_X);
            mapposY.Value = conf.getInt(Config.DEF_Y);

            cmbEnviron.SelectedIndex = 0;

            wb.Navigate(conf.get(Config.BURA3_URL));

            Thread trd = new Thread(new ThreadStart(this.loginThread));
            trd.IsBackground = true;
            trd.Start();
        }
        private DataTable freeSqlDataTable = new DataTable();
        private DataTable casDataTable = new DataTable();
        private DataTable mapDataTable = new DataTable();
        private DataTable mapLogDataTable = new DataTable();
        private DataTable mapLogFindDataTable = new DataTable();
        protected override void OnLoad(EventArgs e)
        {
            dgvFreeSql.DataSource = freeSqlDataTable;
            dgvCas.DataSource = casDataTable;
            dgvMap.DataSource = mapDataTable;
            dgvMapLog.DataSource = mapLogDataTable;
            dgvLog.DataSource = mapLogFindDataTable;
            base.OnLoad(e);
        }

        public delegate void showLogDelegate(string msg);
        public void showLog(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new showLogDelegate(showLog), new Object[]{msg});
                return;
            }
            log.Text = msg;
        }

        public delegate void showLogLnDelegate(string msg);
        public void showLogLn(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new showLogLnDelegate(showLogLn), new Object[] { msg });
                return;
            }
            string newmsg = string.Format("{0}\r\n{1}", log.Text, msg);
            if (newmsg.Length > LOG_LENGTH)
            {
                newmsg = newmsg.Substring(newmsg.Length - LOG_LENGTH);
            }
            log.Text = newmsg;
            log.SelectionStart = log.Text.Length;
            log.Focus();
            log.ScrollToCaret();
        }

        private void getMapBtn_Click(object sender, EventArgs e)
        {
            getMapBtn.Enabled = false;
            defX.Enabled = false;
            defY.Enabled = false;

            sleepBtn.Enabled = true;
            stopBtn.Enabled = true;

            //run
            if (false == craw)
            {
                craw = true;
                Thread trd = new Thread(new ThreadStart(this.startGetMapThread));
                trd.IsBackground = true;
                trd.Start();
            }
            
        }

        private void startGetMapThread()
        {
            craw = true;

            //鯖確認
            Config conf = new Config();
            int sv = 0;
            if (!int.TryParse(conf.get(Config.LOGIN_SV), out sv))
            {
                MessageBox.Show("設定画面で鯖にはマップデータを取得する鯖を半角数値で設定してね。");
                craw = false;
                return;
            }

            //巡回順設定
            List<Point> arealist = new List<Point>();
            for (int x = -800; x <= 800; x += 50)
            {
                for (int y = -800; y <= 800; y += 50)
                {
                    Point p;
                    p.x = x;
                    p.y = y;
                    p.base_x = (int)defX.Value;   //center
                    p.base_y = (int)defY.Value;   //center
                    arealist.Add(p);
                }
            }
            arealist.Sort(delegate(Point a, Point b) { return (((a.x - a.base_x) * (a.x - a.base_x)) + ((a.y - a.base_y) * (a.y - a.base_y))) - (((b.x - b.base_x) * (b.x - b.base_x)) + ((b.y - b.base_y) * (b.y - b.base_y))); });
            int round = 1;      //周回
            string progStr = "周回： {0}週目 - マップ進捗：{1}/{2}";

            while (craw)
            {
                int map_count = 1;  //周回済みマップ数

                foreach (Point p in arealist)
                {
                    this.progress.Text = string.Format(progStr, round, map_count, arealist.Count);

                    while (true == sleepBtn.Checked && true == craw)
                    {
                        roundStatus.Text = "巡回一時停止中";
                        Thread.Sleep(500);
                    }
                    if (false == craw)
                    {
                        this.showLogLn(string.Format("巡回を停止しました。"));
                        break;
                    }

                    roundStatus.Text = "巡回中";
                    this.getMapData(p.x, p.y);

                    map_count++;
                }

                //DBコピー
                this.backupDbFile();

                //周回間隔
                int rTime = 60 * (int)roundSpan.Value;
                while (true == craw && 0 < rTime)
                {
                    rTime--;
                    stripMsg.Text = string.Format("次回周回開始まで後 {0:00}:{1:00}:{2:00}", rTime / 3600, (rTime % 3600) / 60, rTime % 60);
                    Thread.Sleep(1000);
                }

                round++;
            }
            roundStatus.Text = "巡回停止中";
            stripMsg.Text = string.Format("巡回を停止しました");
            craw = false;
        }

        private bool getMapData(int cx, int cy)
        {
            Config conf=new Config();
            int sv = conf.getInt(Config.LOGIN_SV);

            //DBコネクション取得
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return false;
            }
            SQLiteCommand command = con.CreateCommand();

            //正規表現
            Regex regX = new Regex("x=(?<x>[-0-9]*?)&amp;y=");
            Regex regY = new Regex("y=(?<y>[-0-9]*?)#ptop");
            Regex regWood = new Regex("木(?<wood>[0-9]*?)&amp;nbsp;岩");
            Regex regSton = new Regex("岩(?<ston>[0-9]*?)&amp;nbsp;鉄");
            Regex regIron = new Regex("鉄(?<iron>[0-9]*?)&amp;nbsp;糧");
            Regex regRice = new Regex("糧(?<rice>[0-9]*?)</dd>");
            Regex regName = new Regex("<dt class=&quot;bigmap-caption&quot;>(?<name>.*?)</dt>");
            Regex regOwne = new Regex("(<dt.*>|<dt>)君主名</dt><dd>(?<owner>.*?)</dd>");
            Regex regAll1 = new Regex("<dt>同盟名</dt><dd>(?<alliance>.*?)</dd>");      //領地用
            Regex regAll2 = new Regex("<dt.*>同盟名</dt><dd.*>(?<alliance>.*?)</dd>");  //拠点・NCP用
            Regex regPopu = new Regex("<dt.*>人口</dt><dd.*>(?<population>[0-9]*?)</dd>");
            Regex regCasL = new Regex("<dt>戦力</dt><dd><span.*>(?<level>[★]*?)</span></dd>"); //城レベル取得
                        
            //領地取得
            {
                //this.showLogLn(string.Format("中心座標({0},{1})のマップデータ取得を開始します。", cx, cy));

                //map page open
                string url = string.Format(conf.get(Config.BURA3_MAP_URL), conf.get(Config.LOGIN_SV), cx, cy, 4);
                stripMsg.Text = url;
                lording = true;
                wb.Navigate(url);
                while (lording) { 
                    System.Threading.Thread.Sleep(500);
                }

                //停止確認
                if (!craw) { return craw; }

                //get current time
                DateTime dt = DateTime.Now;
                string time = dt.ToString("yyyy-MM-dd HH:mm:ss");

                //マップリスト取得                
                HtmlElementCollection list = this.getMapList();
                if (null == list) { return false; }
                using (SQLiteTransaction transaction = con.BeginTransaction())
                {

                    int ret = 0;
                    string sql = string.Empty;
                    string sql_log = string.Empty;
                    foreach (HtmlElement el in this.getMapList())
                    {
                        //領域外チェック
                        string level = el.InnerText;
                        if (null == level || level.Length <= 0)
                        {
                            string mapdata2 = el.InnerHtml;
                            string x2 = regX.Match(mapdata2).Groups["x"].Value;
                            string y2 = regY.Match(mapdata2).Groups["y"].Value;
                            this.showLogLn(string.Format("マップレベル取得エラー({0},{1})", x2, y2));
                            continue;
                        }

                        //初期化
                        string mapdata = el.InnerHtml;
                        int type = 0;
                        string x = regX.Match(mapdata).Groups["x"].Value;
                        string y = regY.Match(mapdata).Groups["y"].Value;
                        if (false == int.TryParse(x, out ret) || false == int.TryParse(y, out ret))
                        {
                            stripMsg.Text = string.Format("座標取得失敗しました。x={0}, y={1})", x, y);
                            continue;
                        }
                        stripMsg.Text = string.Format("座標({0},{1})のマップデータ取得中", x, y);

                        string wood = string.Empty;
                        string ston = string.Empty;
                        string iron = string.Empty;
                        string rice = string.Empty;
                        string owne = string.Empty;
                        string name = string.Empty;
                        string alli = string.Empty;
                        string popu = "0";

                        //DBのマップ情報取得
                        int cur_type = 0;
                        string cur_owne = string.Empty;
                        string cur_name = string.Empty;
                        string cur_alli = string.Empty;
                        int cur_popu = 0;
                        int row = 0;

                        SQLiteDataReader rs = null;
                        try
                        {
                            command.CommandText = string.Format("select type, name, owner, alliance, population from MAP where x = {0} and y = {1} limit 1", x, y);
                            rs = command.ExecuteReader();
                            row = 0;
                            while (rs.Read())
                            {
                                cur_type = rs.GetInt32(0);
                                cur_name = rs.GetString(1);
                                cur_owne = rs.GetString(2);
                                cur_alli = rs.GetString(3);
                                cur_popu = rs.GetInt32(4);
                                row++;
                            }

                            if (0 == row)
                            {
                                this.showLogLn(string.Format("データベースにマップデータがありませんでした。次のマップ情報へ進みます。\r\nSQL = {0}", command.CommandText));
                                continue;
                            }
                        }
                        catch (SQLiteException e)
                        {
                            this.showLogLn(string.Format("データベースからマップデータの取得に失敗しました。次のマップ情報へ進みます。\r\nSQL = {0}", command.CommandText));
                            this.showLogLn(e.Message);
                            continue;
                        }
                        finally
                        {
                            rs.Close();
                        }


                        if (int.TryParse(level, out ret))
                        {
                            //空き地・領地
                            if (0 == cur_type)
                            {
                                //初回のみ領地情報更新
                                wood = regWood.Match(mapdata).Groups["wood"].Value;
                                ston = regSton.Match(mapdata).Groups["ston"].Value;
                                iron = regIron.Match(mapdata).Groups["iron"].Value;
                                rice = regRice.Match(mapdata).Groups["rice"].Value;

                                try
                                {
                                    command.CommandText = string.Format("update MAP set wood = {0}, stone = {1}, iron = {2}, rice = {3}, level = {4} where x = {5} and y = {6}"
                                        , wood, ston, iron, rice, level, x, y);
                                    command.ExecuteNonQuery();
                                }
                                catch (SQLiteException e)
                                {
                                    this.showLogLn(string.Format("領地情報の更新に失敗しました。次のマップ情報へ進みます。\r\nSQL = {0}", command.CommandText));
                                    this.showLogLn(e.Message);
                                    continue;
                                }
                            }

                            owne = regOwne.Match(mapdata).Groups["owner"].Value;
                            if (owne.Equals(string.Empty))
                            {
                                //空き地
                                type = 1;
                            }
                            else
                            {
                                //領地
                                type = 2;
                                name = regName.Match(mapdata).Groups["name"].Value;
                                alli = regAll1.Match(mapdata).Groups["alliance"].Value;
                                if (name.Equals(string.Empty) || alli.Equals(string.Empty))
                                {
                                    this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            //城・村・砦
                            if (level.Equals("村"))
                            {
                                //村
                                type = 3;
                                owne = regOwne.Match(mapdata).Groups["owner"].Value;
                                name = regName.Match(mapdata).Groups["name"].Value;
                                alli = regAll2.Match(mapdata).Groups["alliance"].Value;
                                popu = regPopu.Match(mapdata).Groups["population"].Value;
                                if (owne.Equals(string.Empty) || name.Equals(string.Empty) || alli.Equals(string.Empty) || popu.Equals(string.Empty))
                                {
                                    this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                    continue;
                                }
                            }
                            else if (level.Equals("砦"))
                            {
                                popu = regPopu.Match(mapdata).Groups["population"].Value;
                                if (!popu.Equals(string.Empty))
                                {
                                    //拠点砦
                                    type = 4;
                                    owne = regOwne.Match(mapdata).Groups["owner"].Value;
                                    name = regName.Match(mapdata).Groups["name"].Value;
                                    alli = regAll2.Match(mapdata).Groups["alliance"].Value;
                                    if (owne.Equals(string.Empty) || name.Equals(string.Empty) || alli.Equals(string.Empty))
                                    {
                                        this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (0 == cur_type)
                                    {
                                        string casLevel = regCasL.Match(mapdata).Groups["level"].Value;
                                        if (!string.Empty.Equals(casLevel))
                                        {
                                            try
                                            {
                                                command.CommandText = string.Format("update MAP set level = {0} where x = {1} and y = {2}"
                                                    , casLevel.Length, x, y);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (SQLiteException e)
                                            {
                                                this.showLogLn(string.Format("砦レベルの更新に失敗しました。次のマップ情報へ進みます。\r\nSQL = {0}", command.CommandText));
                                                this.showLogLn(e.Message);
                                                continue;
                                            }
                                        }
                                    }
                                    //NCP砦
                                    type = 5;
                                    owne = regOwne.Match(mapdata).Groups["owner"].Value;
                                    name = regName.Match(mapdata).Groups["name"].Value;
                                    popu = "0";
                                    if (owne.Equals(string.Empty) || name.Equals(string.Empty))
                                    {
                                        this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                        continue;
                                    }
                                }
                            }
                            else if (level.Equals("城"))
                            {
                                popu = regPopu.Match(mapdata).Groups["population"].Value;
                                if (!popu.Equals(string.Empty))
                                {
                                    //拠点城
                                    type = 6;
                                    owne = regOwne.Match(mapdata).Groups["owner"].Value;
                                    name = regName.Match(mapdata).Groups["name"].Value;
                                    alli = regAll2.Match(mapdata).Groups["alliance"].Value;
                                    if (owne.Equals(string.Empty) || name.Equals(string.Empty) || alli.Equals(string.Empty) || popu.Equals(string.Empty))
                                    {
                                        this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (0 == cur_type)
                                    {
                                        string casLevel = regCasL.Match(mapdata).Groups["level"].Value;
                                        if (!string.Empty.Equals(casLevel))
                                        {
                                            try
                                            {
                                                command.CommandText = string.Format("update MAP set level = {0} where x = {1} and y = {2}"
                                                    , casLevel.Length, x, y);
                                                command.ExecuteNonQuery();
                                            }
                                            catch (SQLiteException e)
                                            {
                                                this.showLogLn(string.Format("城レベルの更新に失敗しました。次のマップ情報へ進みます。\r\nSQL = {0}", command.CommandText));
                                                this.showLogLn(e.Message);
                                                continue;
                                            }
                                        }
                                    }
                                    //NCP城
                                    type = 7;
                                    owne = regOwne.Match(mapdata).Groups["owner"].Value;
                                    name = regName.Match(mapdata).Groups["name"].Value;
                                    popu = "0";
                                    if (owne.Equals(string.Empty) || name.Equals(string.Empty))
                                    {
                                        this.showLogLn(string.Format("領地({0},{1})の情報取得に失敗しました。(Type = {2})", x, y, type));
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                //Do Nothing
                                this.showLogLn(string.Format("分類不明領地({0},{1})", x, y)); ;
                                continue;
                            }
                        }

                        //データベース更新
                        //変更有無の確認
                        if ((type != cur_type) || !cur_owne.Equals(owne) || !cur_name.Equals(name) || !cur_alli.Equals(alli) || !cur_popu.ToString().Equals(popu))
                        {
                            try
                            {
                                command.CommandText = string.Format("update MAP set type = {0}, name = '{1}', owner = '{2}', alliance = '{3}', population = {4}, time = '{5}' where x = {6} and y = {7}"
                                                                   , type, name, owne, alli, popu, time, x, y);
                                command.ExecuteNonQuery();
                                command.CommandText = string.Format("insert into MAP_LOG ( type, name, owner, alliance, population, time, x, y ) values ({0}, '{1}', '{2}', '{3}', {4}, '{5}', {6}, {7})"
                                                                   , type, name, owne, alli, popu, time, x, y);
                                command.ExecuteNonQuery();
                            }
                            catch (SQLiteException e)
                            {
                                this.showLogLn(string.Format("領地({0},{1})の情報更新に失敗しました。(Type = {2})\r\nSQL = {3}", x, y, type, command.CommandText));
                                this.showLogLn(e.Message);
                                continue;
                            }
                        }

                        //停止確認
                        if (!craw) { break; }
                    }
                    transaction.Commit();
                }

                //取得後処理
                //this.showLogLn(string.Format("中心座標({0},{1})のマップデータ取得完了しました。", cx, cy));             

            }
            return true;
        }

        public delegate HtmlElementCollection getMapListDelegate();
        private HtmlElementCollection getMapList()
        {
            if (InvokeRequired)
            {
                return (HtmlElementCollection)Invoke(new getMapListDelegate(getMapList));
            }

            HtmlElement map = wb.Document.GetElementById("map51-content");
            if (null == map)
            {
                this.showLogLn(string.Format("マップデータの取得に失敗しました。"));
                return null;
            }
            return map.GetElementsByTagName("li");
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            lording = false;
            wb.Document.Window.ScrollTo(new System.Drawing.Point(0, wb.Document.Body.ScrollRectangle.Size.Height / 2));
        }

        private string getDbName()
        {
            Config conf = new Config();
            int sv = conf.getInt(Config.LOGIN_SV);
            return string.Format("mapdata.w{0}.db", sv);
        }

        private string getDbPath()
        {
            Config conf = new Config();
            string dbDir = conf.get(Config.TOOL_DB_DIR);
            if (string.Empty.Equals(dbDir))
            {
                dbDir = string.Format("{0}\\mapdata", System.IO.Directory.GetCurrentDirectory());
            }

            if (!System.IO.Directory.Exists(dbDir))
            {
                System.IO.Directory.CreateDirectory(dbDir);
            }
            return string.Format("{0}\\{1}", dbDir, this.getDbName());
        }

        private SQLiteConnection getDBConnect()
        {
            if (null != _mapdata_connection)
            {
                return _mapdata_connection;
            }

            Config conf = new Config();

            string dbPath = this.getDbPath();
            string connectionStr = string.Format("Data Source={0};Version=3;", dbPath);

            if (!System.IO.File.Exists(dbPath))
            {
                //db構築確認
                if (DialogResult.Yes != MessageBox.Show(string.Format("{0}鯖のデータベースが見つかりませんでした。新規作成してもいいですか？\r\n※この処理は10分ぐらいかかります。", conf.getInt(Config.LOGIN_SV))
                    , "データベース構築確認", MessageBoxButtons.YesNo))
                {
                    return null;
                }

                SQLiteConnection con = null;
                try
                {
                    con = new SQLiteConnection();
                    con.ConnectionString = connectionStr;
                    con.Open();
                    SQLiteCommand command = con.CreateCommand();

                    string sql = "create table MAP (";
                    sql += " x INTEGER NOT NULL";
                    sql += ",y INTEGER NOT NULL";
                    sql += ",wood INTEGER NOT NULL";
                    sql += ",stone INTEGER NOT NULL";
                    sql += ",iron INTEGER NOT NULL";
                    sql += ",rice INTEGER NOT NULL";
                    sql += ",level INTEGER NOT NULL";
                    sql += ",type INTEGER NOT NULL";
                    sql += ",name text";
                    sql += ",owner text";
                    sql += ",alliance text";
                    sql += ",population INTEGER";
                    sql += ",time text";
                    sql += ",comment text";
                    sql += ")";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    sql = "create table MAP_LOG (";
                    sql += " x INTEGER NOT NULL";
                    sql += ",y INTEGER NOT NULL";
                    sql += ",type INTEGER NOT NULL";
                    sql += ",name text";
                    sql += ",owner text";
                    sql += ",alliance text";
                    sql += ",population INTEGER";
                    sql += ",time text";
                    sql += ")";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    //初期データ作成
                    int cnt = 0;
                    for (int x = -800; x <= 800; x++)
                    {
                        using (SQLiteTransaction transaction = con.BeginTransaction())
                        {
                            for (int y = -800; y <= 800; y++)
                            {
                                command.CommandText = string.Format("insert into MAP ( x  ,y  ,wood ,stone ,iron ,rice ,level ,type ,name ,owner ,alliance ,population ,time ,comment )"
                                                                          + " values ( {0},{1},0    ,0     ,0    ,0    ,0     ,0    ,''   ,''    ,''       ,0          ,0    ,''      );", x, y);
                                command.ExecuteNonQuery();

                                this.showLog(string.Format("新規データベース構築中です。この処理は初回のみ発生します。しばらくお待ちください。\r\n ({0}/{1})", ++cnt, 1600 * 1600));
                            }
                            transaction.Commit();
                        }
                    }

                    this.showLog(string.Format("データベースのインデックスを構築中です。"));
                    command.CommandText = "create index mapindex on MAP(x, y);";
                    command.ExecuteNonQuery();
                    command.CommandText = "create index maplogindex on MAP_LOG(x, y);";
                    command.ExecuteNonQuery();

                    this.showLog(string.Format("データベース構築完了"));
                }
                catch (SQLiteException e)
                {
                    this.showLog(string.Format("データベースの構築に失敗しました。\r\n{0}", e.Message));
                    return null;
                }
                finally
                {
                    if (null != con) { con.Close(); }
                }
            }

            SQLiteConnection connect = null;
            try
            {
                connect = new SQLiteConnection();
                connect.ConnectionString = connectionStr;
                connect.Open();
            }
            catch (SQLiteException e)
            {
                this.showLogLn(e.Message);
                return null;
            }
            return connect;

        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("マップデータ取得巡回を停止します。よろしいですか？",
                "巡回停止確認", MessageBoxButtons.YesNo))
            {
                getMapBtn.Enabled = true;
                defX.Enabled = true;
                defY.Enabled = true;
                sleepBtn.Enabled = false;
                sleepBtn.Checked = false;
                stopBtn.Enabled = false;

                craw = false;
            }
        }

        private void runFreeSql_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return;
            }

            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlTextBox.Text, con))
            {
                try
                {
                    freeSqlDataTable.Clear();
                    freeSqlDataTable.Columns.Clear();
                    freeSlqCount.Text = string.Format("データ件数：{0}", 0);
                    adapter.Fill(freeSqlDataTable);
                }
                catch (SQLiteException ex)
                {
                    this.showLogLn(ex.Message);
                    return;
                }
            }
            dgvFreeSql.AutoResizeColumns();
            dgvFreeSql.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            stripMsg.Text = string.Format("データ件数：{0}", dgvFreeSql.Rows.Count);
            freeSlqCount.Text = string.Format("データ件数：{0}", dgvFreeSql.Rows.Count);
        }

        private void sqlSave_Click(object sender, EventArgs e)
        {
            string title = selectSql.Text;
            if (string.Empty.Equals(title))
            {
                MessageBox.Show("タイトルを入力してください。");
                return;
            }

            Config conf = new Config();
            conf.set(title, sqlTextBox.Text, 1);

            this.setSqlList();
            stripMsg.Text = string.Format("「{0}」を保存しました。", title);
        }

        private void sqlDelete_Click(object sender, EventArgs e)
        {
            string title = selectSql.Text;
            int idx = selectSql.Items.IndexOf(title);
            if (idx < 0)
            {
                stripMsg.Text = string.Format("「{0}」は見つかりませんでした。", title);
                return;
            }

            if (DialogResult.Yes != MessageBox.Show(string.Format("「{0}」を削除します。よろしいですか？", title),"削除確認",MessageBoxButtons.YesNo))
            {
                return;
            }
            Config conf = new Config();
            conf.del(title);

            this.setSqlList();
            sqlTextBox.Text = string.Empty;
            if (selectSql.Items.Count > 0) { selectSql.SelectedIndex = 0; } else { selectSql.Text = string.Empty; }
            stripMsg.Text = string.Format("「{0}」を削除しました。", title);
        }

        private void setSqlList()
        {
            Config conf = new Config();
            selectSql.Items.Clear();
            sqlList.Clear();

            foreach (ConfigItem it in conf.getList(1))
            {
                selectSql.Items.Add(it.key);
                sqlList[it.key] = it.value;
            }
        }

        private void selectSql_SelectedIndexChanged(object sender, EventArgs e)
        {
            sqlTextBox.Text = sqlList[selectSql.Text];
        }

        private void dgvFreeSql_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvFreeSql.Rows.Count <= 0) { return; }
            const int OVI = 10000;
            int x = OVI;
            int y = OVI;
            for (int i = 0; i < dgvFreeSql.ColumnCount; i++)
            {
                if ("x".Equals(dgvFreeSql.Columns[i].HeaderText))
                {
                    string sx = dgvFreeSql.Rows[dgvFreeSql.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out x);
                }
                if ("y".Equals(dgvFreeSql.Columns[i].HeaderText))
                {
                    string sx = dgvFreeSql.Rows[dgvFreeSql.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out y);
                }
            }
            if (x < OVI && y < OVI)
            {
                Config conf = new Config();
                System.Diagnostics.Process.Start(string.Format(conf.get(Config.BURA3_MAP_URL_MINI), conf.get(Config.LOGIN_SV), x, y, 3));
            }

        }

        private void loginThread()
        {
            while (!loginFlag)
            {
                Thread.Sleep(500);
                this.loginFunc();
            }
        }

        public delegate void loginFuncDelegate();
        private void loginFunc()
        {
            if (InvokeRequired)
            {
                Invoke(new loginFuncDelegate(loginFunc));
                return;
            }

            try
            {
                Config conf = new Config();

                int cnt = 0;
                while (string.Empty.Equals(conf.get(Config.LOGIN_ID)) || string.Empty.Equals(conf.get(Config.LOGIN_PW)))
                {
                    ConfigDlg dlg = new ConfigDlg();
                    dlg.ShowDialog();
                    cnt++;
                    if (cnt > 2) { break; }
                }

                wb.Document.All.GetElementsByName("email")[0].InnerText = conf.get(Config.LOGIN_ID);
                wb.Document.All.GetElementsByName("password")[0].InnerText = conf.get(Config.LOGIN_PW);
                wb.Document.Forms[0].InvokeMember("submit");

                loginFlag = true;

                this.showLog(string.Format("必ず {0}鯖にログインしてくださいね！\r\n{0}鯖のログインリンクをクリックして、ブラウザ三国志のトップ画面が表示されたら準備完了です！"
                    , conf.get(Config.LOGIN_SV)));
            }
            catch (Exception e)
            {
                //do nothing
                //this.showLogLn(e.Message.ToString());
                System.Console.WriteLine(e.Message.ToString());
            }
            
        }

        private void 設定OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigDlg dlg = new ConfigDlg();
            dlg.ShowDialog();
        }

        private void runCasBtn_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return;
            }
            
            string casMsgF = "データ件数： {0}件";

            string sql = "select m.name as 城・砦名, m.x, m.y, m.level as 戦力, (((m.x-({4}))*(m.x-({4})))+((m.y-({5}))*(m.y-({5})))) as 距離, m.owner as 盟主名 ";
            sql += ", case when (m.name <> replace( m.owner, '守衛', '砦' )) And (m.owner not in ('献帝', '袁紹', '曹操', '劉備', '関羽', '董卓', '孫権', '公孫瓚', '呂布', '袁術', '馬超', '孟獲', '孫策', '荀彧', '于禁', '馬謖', '張飛', '張魯', '沙摩柯', '朱霊', '程普', '夏侯惇', '蔡瑁', '甘寧', '周瑜', '張郃', '張遼', '呂蒙', '龐統')) then '済' else '未' end as 攻略 ";
                sql += ", case when n.alliance = '{2}' and nw.alliance = '{2}' and w.alliance = '{2}' and sw.alliance = '{2}' and s.alliance = '{2}' and se.alliance = '{2}' and e.alliance = '{2}' and ne.alliance = '{2}' then '包囲' else ";
                    sql += " case when n.alliance <> '{2}' and nw.alliance <> '{2}' and w.alliance <> '{2}' and sw.alliance <> '{2}' and s.alliance <> '{2}' and se.alliance <> '{2}' and e.alliance <> '{2}' and ne.alliance <> '{2}' then '隣無' else ";
                    sql += " case when ( n.alliance = '{2}' or n.alliance = '' ) and ( nw.alliance = '{2}' or nw.alliance = '' ) and ( w.alliance = '{2}' or w.alliance = '' ) and ( sw.alliance = '{2}' or sw.alliance = '' ) and ( s.alliance = '{2}' or s.alliance = '' ) and ( se.alliance = '{2}' or se.alliance = '' ) and ( e.alliance = '{2}' or e.alliance = '' ) and ( ne.alliance = '{2}' or ne.alliance = '' ) then '隣接' else  ";
                    sql += " case when n.alliance = '{2}' or nw.alliance = '{2}' or w.alliance = '{2}' or sw.alliance = '{2}' or s.alliance = '{2}' or se.alliance = '{2}' or e.alliance = '{2}' or ne.alliance = '{2}' then '競合' else ";
                    sql += " '不明' end end end end as 状況";
                sql += ", case when n.alliance  = '' then 1 else 0 end ";
                sql += "+ case when nw.alliance = '' then 1 else 0 end ";
                sql += "+ case when w.alliance  = '' then 1 else 0 end ";
                sql += "+ case when sw.alliance = '' then 1 else 0 end ";
                sql += "+ case when s.alliance  = '' then 1 else 0 end ";
                sql += "+ case when se.alliance = '' then 1 else 0 end ";
                sql += "+ case when e.alliance  = '' then 1 else 0 end ";
                sql += "+ case when ne.alliance = '' then 1 else 0 end ";
                sql += " as 空隣接";
                sql += ",  n.alliance || case when  n.type > 1 then '(' ||  n.owner || ')' else '' end 隣接_北";
                sql += ", nw.alliance || case when nw.type > 1 then '(' || nw.owner || ')' else '' end 隣接_北東";
                sql += ",  w.alliance || case when  w.type > 1 then '(' ||  w.owner || ')' else '' end 隣接_東";
                sql += ", sw.alliance || case when sw.type > 1 then '(' || sw.owner || ')' else '' end 隣接_南東";
                sql += ",  s.alliance || case when  s.type > 1 then '(' ||  s.owner || ')' else '' end 隣接_南";
                sql += ", se.alliance || case when se.type > 1 then '(' || se.owner || ')' else '' end 隣接_南西";
                sql += ",  e.alliance || case when  e.type > 1 then '(' ||  e.owner || ')' else '' end 隣接_西";
                sql += ", ne.alliance || case when ne.type > 1 then '(' || ne.owner || ')' else '' end 隣接_北西";
                sql += " from map  m ";
                sql += "left join map n  on m.x = n.x and m.y + 1 = n.y ";
                sql += "left join map nw on m.x + 1 = nw.x and m.y + 1 = nw.y ";
                sql += "left join map w  on m.x + 1 = w.x and m.y = w.y ";
                sql += "left join map sw on m.x + 1 = sw.x and m.y - 1 = sw.y ";
                sql += "left join map s   on m.x = s.x and m.y - 1 = s.y ";
                sql += "left join map se on m.x - 1 = se.x and m.y  - 1 = se.y ";
                sql += "left join map e  on m.x - 1 = e.x and m.y = e.y ";
                sql += "left join map ne on m.x - 1 = ne.x and m.y + 1 = ne.y ";
                sql += "where ( m.type = 7 or m.type = 5 ) ";
                sql += " and m.level in ( {0} )";
                sql += " and 攻略 in ( {1} ) ";
                sql += " and 状況 in ( {3} ) ";
                sql += " and (m.x between {6} and {7}) and (m.y between {8} and {9}) ";
                sql += " {10} ";
                sql += " order by m.level desc limit 2000;";

            //レベル抽出
                string level = "0";
                if (lev1.Checked) { level += ", 1"; }
                if (lev2.Checked) { level += ", 2"; }
                if (lev3.Checked) { level += ", 3"; }
                if (lev4.Checked) { level += ", 4"; }
                if (lev5.Checked) { level += ", 5"; }
                if (lev6.Checked) { level += ", 6"; }
                if (lev7.Checked) { level += ", 7"; }
                if (lev8.Checked) { level += ", 8"; }
                if (lev9.Checked) { level += ", 9"; }
            //隣接状況
                string sts = "'不明'";
                if (casSts1.Checked) { sts += " , '包囲'"; }
                if (casSts2.Checked) { sts += " , '競合'"; }
                if (casSts3.Checked) { sts += " , '隣接'"; }
                if (casSts4.Checked) { sts += " , '隣無'"; }
                
            //攻略状況
                string sm = "'-'";
                string ownnm = string.Empty;
                if (sm_s.Checked)
                {
                    sm += ", '済'";
                    ownnm = string.Format("and m.owner like '%{0}%'", casOwner.Text);
                }
                if (sm_m.Checked) { sm += ", '未'"; }

                sql = string.Format(sql, level, sm, alliListBox.Text, sts, (int)casDefX.Value, (int)casDefY.Value
                                       , (int)casLTX.Value, (int)casRBX.Value, (int)casRBY.Value, (int)casLTY.Value, ownnm);
                

                this.showLog(sql);
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, con))
            {
                try
                {
                    casSqlMsg.Text = string.Format(casMsgF, 0);
                    casDataTable.Clear();
                    casDataTable.Columns.Clear();
                    adapter.Fill(casDataTable);
                }
                catch (SQLiteException ex)
                {
                    this.showLogLn(ex.Message);
                    return;
                }
            }
            dgvCas.AutoResizeColumns();
            dgvCas.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            this.showLog(sql);

            casSqlMsg.Text = string.Format(casMsgF, dgvCas.Rows.Count);
            
        }

        private void dgvCas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvCas.Rows.Count <= 0) { return; }
            const int OVI = 10000;
            int x = OVI;
            int y = OVI;
            for (int i = 0; i < dgvCas.ColumnCount; i++)
            {
                if ("x".Equals(dgvCas.Columns[i].HeaderText))
                {
                    string sx = dgvCas.Rows[dgvCas.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out x);
                }
                if ("y".Equals(dgvCas.Columns[i].HeaderText))
                {
                    string sx = dgvCas.Rows[dgvCas.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out y);
                }
            }
            if (x < OVI && y < OVI)
            {
                Config conf = new Config();
                System.Diagnostics.Process.Start(string.Format(conf.get(Config.BURA3_MAP_URL_MINI), conf.get(Config.LOGIN_SV), x, y, 3));
            }
        }

        private void dgvCas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (4 == e.ColumnIndex)
            {
                int x = 0;
                int y = 0;
                if (int.TryParse(dgvCas.Rows[e.RowIndex].Cells[1].Value.ToString(), out x)
                    && int.TryParse(dgvCas.Rows[e.RowIndex].Cells[2].Value.ToString(), out y))
                {
                    x = x - (int)casDefX.Value;
                    y = y - (int)casDefY.Value;
                    e.Value = Math.Round(Math.Sqrt((x * x) + (y * y)), 2);
                }
            }
            else if (7 == e.ColumnIndex)
            {//包囲状況
                switch (e.Value.ToString())
                {
                    case "包囲":
                        e.CellStyle.BackColor = Color.LawnGreen; ;
                        break;
                    case "競合":
                        e.CellStyle.BackColor = Color.LightPink;
                        break;
                    case "隣接":
                        e.CellStyle.BackColor = Color.Gold;
                        break;
                    case "隣無":
                        e.CellStyle.BackColor = Color.LightSkyBlue;
                        break;
                    default:
                        e.CellStyle.BackColor = Color.Tomato;
                        break;
                }
            }
        }

        private void levAll_CheckedChanged(object sender, EventArgs e)
        {
            lev1.Checked = levAll.Checked;
            lev2.Checked = levAll.Checked;
            lev3.Checked = levAll.Checked;
            lev4.Checked = levAll.Checked;
            lev5.Checked = levAll.Checked;
            lev6.Checked = levAll.Checked;
            lev7.Checked = levAll.Checked;
            lev8.Checked = levAll.Checked;
            lev9.Checked = levAll.Checked;
        }

        private void dgvCas_SelectionChanged(object sender, EventArgs e)
        {

            if(0<dgvCas.SelectedRows.Count)
            {
                //if (true == chkEnviron.Checked)
                {
                    Config conf = new Config();
                    int row = dgvCas.SelectedRows[0].Index;

                    this.showLog(this.getPointEnviron(int.Parse(dgvCas.Rows[row].Cells[1].Value.ToString())
                        , int.Parse(dgvCas.Rows[row].Cells[2].Value.ToString())));
                }
            }
            
        }

        private string getPointEnviron(int x, int y)
        {
            if (x < -800 || 800 < x || y < -800 || 800 < y) { return string.Empty; }
            
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return string.Empty;
            }

            SQLiteDataReader rs = null;
            string ret = string.Empty;
            try
            {
                SQLiteCommand command = con.CreateCommand();
                command.CommandText = string.Format("select x, y, level, type, name, owner, alliance from map where x between {0} - 1 and {0} + 1 and y between {1} - 1 and {1} + 1 order by x, y", x, y);
                rs = command.ExecuteReader();
                while (rs.Read())
                {
                    if (x == rs.GetInt32(0) && y == rs.GetInt32(1))
                    {
                        ret = string.Format("{0}({1},{2})★{3}\r\n{4}", rs.GetString(4), x, y, rs.GetInt32(2), ret);
                    }
                    else
                    {
                        string buf = string.Empty;
                        if (1 < rs.GetInt32(3))
                        {
                            buf = string.Format("{0} {1}", rs.GetString(5), rs.GetString(6));
                            if (cmbEnviron.SelectedIndex == 0 || cmbEnviron.SelectedIndex == 1)
                            {
                                ret += string.Format(" ({0},{1})★{2} {3}\r\n", rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(2), buf);
                            }
                        }
                        else
                        {
                            if (cmbEnviron.SelectedIndex == 0 || cmbEnviron.SelectedIndex == 2)
                            {
                                ret += string.Format(" ({0},{1})★{2} {3}\r\n", rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(2), buf);
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                this.showLogLn(ex.Message);
            }
            finally
            {
                if (null != rs) { rs.Close(); }
            }
            return ret;
        }

        private void freeOutputFile_Click(object sender, EventArgs e)
        {
            if (dgvFreeSql.Rows.Count <= 0) { return; }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = selectSql.Text;
            dlg.Filter = "CSVファイル(*.csv)|*.csv|TSVファイル(*.tsv)|*.tsv|すべてのファイル(*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.Title = "保存先のファイルを選択してください";
            dlg.RestoreDirectory = true;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                try
                {
                    using (StreamWriter w = new StreamWriter(dlg.FileName))
                    {
                        string delim = ",";
                        if (2 == dlg.FilterIndex) { delim = "\t"; }

                        for (int col = 0; col < dgvFreeSql.ColumnCount; col++)
                        {
                            if (0 != col)
                            {
                                w.Write(delim);
                            }
                            w.Write(dgvFreeSql.Columns[col].HeaderText);
                        }
                        w.Write(Environment.NewLine);

                        for (int line = 0; line < dgvFreeSql.Rows.Count; line++)
                        {
                            for (int col = 0; col < dgvFreeSql.Columns.Count; col++)
                            {
                                if (0 != col)
                                {
                                    w.Write(delim);
                                }
                                w.Write(dgvFreeSql.Rows[line].Cells[col].Value.ToString());
                            }
                            w.Write(Environment.NewLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.showLog(ex.Message.ToString());
                }
            }

        }


        private void findMapBtn_Click(object sender, EventArgs e)
        {
            
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return;
            }
            
            string mapMsgF = "データ件数： {0}件";

            string sql = "select m.x, m.y, m.level as 戦力, '木' || m.wood || ' 岩' || m.stone || ' 鉄' || m.iron || ' 糧' || m.rice as 資源";
            sql += ", (((m.x-({0}))*(m.x-({0})))+((m.y-({1}))*(m.y-({1})))) as 距離 ";
            sql += ", case m.type when 1 then '未取得' when 2 then '取得' when 3 then '拠点村' when 4 then '拠点砦' else '不明' end as 状態";
            sql += " , m.name as 名称, m.owner as 君主名, m.alliance as 同盟名, m.population as 人口 ";
                sql += ", case when n.alliance = '{2}' and nw.alliance = '{2}' and w.alliance = '{2}' and sw.alliance = '{2}' and s.alliance = '{2}' and se.alliance = '{2}' and e.alliance = '{2}' and ne.alliance = '{2}' then '包囲' else ";
                    sql += " case when n.alliance <> '{2}' and nw.alliance <> '{2}' and w.alliance <> '{2}' and sw.alliance <> '{2}' and s.alliance <> '{2}' and se.alliance <> '{2}' and e.alliance <> '{2}' and ne.alliance <> '{2}' then '隣無' else ";
                    sql += " case when ( n.alliance = '{2}' or n.alliance = '' ) and ( nw.alliance = '{2}' or nw.alliance = '' ) and ( w.alliance = '{2}' or w.alliance = '' ) and ( sw.alliance = '{2}' or sw.alliance = '' ) and ( s.alliance = '{2}' or s.alliance = '' ) and ( se.alliance = '{2}' or se.alliance = '' ) and ( e.alliance = '{2}' or e.alliance = '' ) and ( ne.alliance = '{2}' or ne.alliance = '' ) then '隣接' else  ";
                    sql += " case when n.alliance = '{2}' or nw.alliance = '{2}' or w.alliance = '{2}' or sw.alliance = '{2}' or s.alliance = '{2}' or se.alliance = '{2}' or e.alliance = '{2}' or ne.alliance = '{2}' then '競合' else ";
                    sql += " '不明' end end end end as 状況";
                sql += ", n.alliance 隣接_北";
                sql += ", nw.alliance 隣接_北東";
                sql += ", w.alliance 隣接_東"; 
                sql += ", sw.alliance 隣接_南東";
                sql += ", s.alliance 隣接_南";
                sql += ", se.alliance 隣接_南西";
                sql += ", e.alliance 隣接_西";
                sql += ", ne.alliance 隣接_北西";
                sql += " from map  m ";
                sql += "left join map n  on m.x = n.x and m.y + 1 = n.y ";
                sql += "left join map nw on m.x + 1 = nw.x and m.y + 1 = nw.y ";
                sql += "left join map w  on m.x + 1 = w.x and m.y = w.y ";
                sql += "left join map sw on m.x + 1 = sw.x and m.y - 1 = sw.y ";
                sql += "left join map s   on m.x = s.x and m.y - 1 = s.y ";
                sql += "left join map se on m.x - 1 = se.x and m.y  - 1 = se.y ";
                sql += "left join map e  on m.x - 1 = e.x and m.y = e.y ";
                sql += "left join map ne on m.x - 1 = ne.x and m.y + 1 = ne.y ";
                sql += "where ( m.type in ( {3} ) ) ";
                sql += " and m.x between {4} and {5} ";
                sql += " and m.y between {6} and {7} ";
                sql += " and not ( m.level = 0 and m.wood = 0 and m.stone = 0 and m.iron = 0 and m.rice = 0 )";
                sql += " and ( {8} )";
                sql += " and 状況 in ( {10} ) ";
                sql += " and m.alliance like '%{11}%' ";
                sql += " and m.owner like '%{12}%' ";
                sql += " order by m.level desc, 距離 desc limit {9};";
            
            //領地分類
                string type = "0";
                if (mapType1.Checked) { type += ", 1"; }
                if (mapType2.Checked) { type += ", 2"; }
                if (mapType3.Checked) { type += ", 3"; }
                if (mapType4.Checked) { type += ", 4"; }
            
            //隣接状況
                string sts = "'不明'";
                if (mapSts1.Checked) { sts += " , '包囲'"; }
                if (mapSts2.Checked) { sts += " , '競合'"; }
                if (mapSts3.Checked) { sts += " , '隣接'"; }
                if (mapSts4.Checked) { sts += " , '隣無'"; }

            //領地
                string mt = " ( 0 <> 0 ) ";
                if (0 >= mapTypeList.SelectedItems.Count)
                {
                    MessageBox.Show("対象領地を選択してください。");
                    return;
                }
                for (int i = 0; i < mapTypeList.SelectedItems.Count; i++)
                {
                    MapListItem item = (MapListItem)mapTypeList.SelectedItems[i];
                    mt += string.Format(" or ( m.level = {0} and m.wood = {1} and m.stone = {2} and m.iron = {3} and m.rice = {4} )"
                        , item.level, item.wood, item.stone, item.iron, item.rice );
                }

                sql = string.Format(sql, (int)mapDefX.Value, (int)mapDefY.Value, mapAlliBox.Text, type
                                    , (int)mapLTX.Value, (int)mapRBX.Value, (int)mapRBY.Value, (int)mapLTY.Value
                                    , mt, (int)mapLimit.Value, sts, mapOwnAlliBox.Text, mapOwner.Text);
                this.showLog(sql);
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, con))
            {
                try
                {
                    mapSqlMsg.Text = string.Format(mapMsgF, 0);
                    mapDataTable.Clear();
                    mapDataTable.Columns.Clear();
                    adapter.Fill(mapDataTable);
                }
                catch (SQLiteException ex)
                {
                    this.showLogLn(ex.Message);
                    return;
                }
            }
            dgvMap.AutoResizeColumns();
            dgvMap.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            mapSqlMsg.Text = string.Format(mapMsgF, dgvMap.Rows.Count);
             
             
        }

        private void dgvMap_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (4 == e.ColumnIndex)
            {
                int x = 0;
                int y = 0;
                if (int.TryParse(dgvMap.Rows[e.RowIndex].Cells[0].Value.ToString(), out x)
                    && int.TryParse(dgvMap.Rows[e.RowIndex].Cells[1].Value.ToString(), out y))
                {
                    x = x - (int)mapDefX.Value;
                    y = y - (int)mapDefY.Value;
                    e.Value = Math.Round(Math.Sqrt((x * x) + (y * y)), 2);
                }
            }
            else if (10 == e.ColumnIndex)
            {//包囲状況
                switch (e.Value.ToString())
                {
                    case "包囲":
                        e.CellStyle.BackColor = Color.LawnGreen; ;
                        break;
                    case "競合":
                        e.CellStyle.BackColor = Color.LightPink;
                        break;
                    case "隣接":
                        e.CellStyle.BackColor = Color.Gold;
                        break;
                    case "隣無":
                        e.CellStyle.BackColor = Color.LightSkyBlue;
                        break;
                    default:
                        e.CellStyle.BackColor = Color.Tomato;
                        break;
                }
            }
        }

        private void dgvMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvMap.Rows.Count <= 0) { return; }
            const int OVI = 10000;
            int x = OVI;
            int y = OVI;
            for (int i = 0; i < dgvMap.ColumnCount; i++)
            {
                if ("x".Equals(dgvMap.Columns[i].HeaderText))
                {
                    string sx = dgvMap.Rows[dgvMap.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out x);
                }
                if ("y".Equals(dgvMap.Columns[i].HeaderText))
                {
                    string sx = dgvMap.Rows[dgvMap.SelectedRows[0].Index].Cells[i].Value.ToString();
                    int.TryParse(sx, out y);
                }
            }
            if (x < OVI && y < OVI)
            {
                Config conf = new Config();
                System.Diagnostics.Process.Start(string.Format(conf.get(Config.BURA3_MAP_URL_MINI), conf.get(Config.LOGIN_SV), x, y, 3));
            }
        }

        private void createBigMapBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("ちょっと時間かかるけど覚悟はいいかい？", "広域マップ構築確認", MessageBoxButtons.YesNo)) { return; }
            createBigMapBtn.Enabled = false;
            Application.DoEvents();

            Config conf = new Config();

            //背景色設定取得
            List<BigMapBackColorSetting> bgColor = new List<BigMapBackColorSetting>();
            for (int row = 0; row < conf.getInt(Config.BIGMAP_COLOR_TABLE_SIZE); row++)
            {
                string keyF = string.Format("{0}_{1}", Config.BIGMAP_COLOR_TABLE, row);
                if (bool.Parse(conf.get(string.Format("{0}_{1}", keyF, 0))))
                {
                    BigMapBackColorSetting bmbcs = new BigMapBackColorSetting();
                    bmbcs.flag     = true;
                    bmbcs.color    = Color.FromArgb(conf.getInt(string.Format("{0}_{1}", keyF, 1)));
                    bmbcs.type     = conf.get(string.Format("{0}_{1}", keyF, 2));
                    bmbcs.alliance = conf.get(string.Format("{0}_{1}", keyF, 3));
                    bmbcs.owner    = conf.get(string.Format("{0}_{1}", keyF, 4));
                    bmbcs.level    = conf.getInt(string.Format("{0}_{1}", keyF, 5));
                    bmbcs.wood     = conf.getInt(string.Format("{0}_{1}", keyF, 6));
                    bmbcs.stone    = conf.getInt(string.Format("{0}_{1}", keyF, 7));
                    bmbcs.iron     = conf.getInt(string.Format("{0}_{1}", keyF, 8));
                    bmbcs.rice     = conf.getInt(string.Format("{0}_{1}", keyF, 9));
                    bmbcs.comment  = conf.get(string.Format("{0}_{1}", keyF, 10));

                    bgColor.Add(bmbcs);
                }     
             
            }

            int progCount = 0;
            progLoading.Maximum = 1200 * 1200 + 60000;
            progLoading.Value = 0;
            progLoading.Visible = true;

            dgvBigMap.Rows.Clear();
            dgvBigMap.Columns.Clear();
            for (int x = -800; x <= 800; x++)
            {
                DataGridViewColumn col = new DataGridViewColumn();
                col.FillWeight = 1;
                col.Width = (int)mapSize.Value;
                col.CellTemplate = new DataGridViewTextBoxCell();
                dgvBigMap.Columns.Add(col);
            }

            progCount = 1000;
            progLoading.Value = progCount;
            progLoading.Update();

            for (int y = -800; y <= 800; y++)
            {
                dgvBigMap.Rows.Add();
                dgvBigMap.Rows[y + 800].Height = (int)mapSize.Value;

                if (0 == y % 100)
                {
                    progCount += 5000;
                    progLoading.Value = progCount;
                    progLoading.Update();
                }
            }

            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                createBigMapBtn.Enabled = true;
                return;
            }

            SQLiteCommand command = con.CreateCommand();
            SQLiteDataReader rs = null;

            string alliance = conf.get(Config.ALLI_NM);
            bool showLevelFlag = chkShowLevel.Checked;
            List<string> mapTypeName = new List<string>();
            mapTypeName.Add("不明");
            mapTypeName.Add("空き地");
            mapTypeName.Add("取得済み領地");
            mapTypeName.Add("拠点(村)");
            mapTypeName.Add("拠点(砦)");
            mapTypeName.Add("NPC砦");
            mapTypeName.Add("本拠");
            mapTypeName.Add("NPC城");

            try
            {
                string sql = "select x, y, type, alliance, owner, level, wood, stone, iron, rice, population, name from map where type in ( 1, 2, 3, 4, 5, 6, 7 ) ";

                command.CommandText = sql;
                rs = command.ExecuteReader();
                while (rs.Read())
                {
                    progCount++;

                    int x = rs.GetInt32(0);
                    int y = rs.GetInt32(1);
                    if (-800 <= x && x <= 800 && -800 <= y && y <= 800)
                    {
                        x = 800 + x;
                        y = Math.Abs(y - 800);

                        StringBuilder level = new StringBuilder(9);
                        for (int i = 0; i < rs.GetInt32(5); i++)
                        {
                            level.Append("★");
                        }

                       
                        //領地表記
                        if (showLevelFlag)
                        {
                            switch (rs.GetInt32(2))
                            {
                                case 1:
                                case 2:
                                case 5:
                                case 7:
                                    dgvBigMap[x, y].Value = rs.GetInt32(5).ToString();
                                    break;
                                case 3:
                                    dgvBigMap[x, y].Value = "村";
                                    break;
                                case 4:
                                    dgvBigMap[x, y].Value = "砦";
                                    break;
                                case 6:
                                    dgvBigMap[x, y].Value = "城";
                                    break;
                                default:
                                    break;
                            }
                        }

                        //領地概要
                        if (0 <= rs.GetInt32(2) && rs.GetInt32(2) < mapTypeName.Count)
                        {
                            dgvBigMap[x, y].ToolTipText = string.Format("分類：{0}\r\n", mapTypeName[rs.GetInt32(2)]);
                        }
                        switch (rs.GetInt32(2))
                        {
                            case 1: 
                                dgvBigMap[x, y].ToolTipText += string.Format("座標：({0},{1})\r\n資源：木{2} 石{3} 鉄{4} 糧{5}\r\n戦力：{6}"
                                    , rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(6), rs.GetInt32(7), rs.GetInt32(8), rs.GetInt32(9), level.ToString(), rs.GetString(4), rs.GetString(3), rs.GetInt32(10), rs.GetString(11));
                                break;
                            case 2:
                                dgvBigMap[x, y].ToolTipText += string.Format("座標：({0},{1})\r\n名称：{10}\r\n資源：木{2} 石{3} 鉄{4} 糧{5}\r\n戦力：{6}\r\n君主：{7}\r\n同盟：{8}"
                                    , rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(6), rs.GetInt32(7), rs.GetInt32(8), rs.GetInt32(9), level.ToString(), rs.GetString(4), rs.GetString(3), rs.GetInt32(10), rs.GetString(11));
                                break;
                            case 3:
                            case 4:
                            case 6:
                                dgvBigMap[x, y].ToolTipText += string.Format("座標：({0},{1})\r\n名称：{10}\r\n君主：{7}\r\n同盟：{8}\r\n人口：{9}"
                                    , rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(6), rs.GetInt32(7), rs.GetInt32(8), rs.GetInt32(9), level.ToString(), rs.GetString(4), rs.GetString(3), rs.GetInt32(10), rs.GetString(11));
                                break;
                            case 5:
                            case 7:
                                dgvBigMap[x, y].ToolTipText += string.Format("座標：({0},{1})\r\n名称：{10}\r\n戦力：{6}\r\n君主：{7}"
                                    , rs.GetInt32(0), rs.GetInt32(1), rs.GetInt32(6), rs.GetInt32(7), rs.GetInt32(8), rs.GetInt32(9), level.ToString(), rs.GetString(4), rs.GetString(3), rs.GetInt32(10), rs.GetString(11));
                                break;
                            default:
                                break;
                        }

                        //背景色設定
                        dgvBigMap[x, y].Style.BackColor = Color.White;
                        dgvBigMap[x, y].Style.SelectionBackColor = Color.White;
                        foreach (BigMapBackColorSetting bgc in bgColor)
                        {
                            bool f = false;

                            //領地分類確認
                            switch (bgc.type)
                            {
                                case "領地&拠点":
                                    switch (rs.GetInt32(2))
                                    {
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 6:
                                            f = true;
                                            break;
                                    }
                                    break;
                                case "領地":
                                    if (2 == rs.GetInt32(2))
                                    {
                                        f = true;
                                    }
                                    break;
                                case "拠点":
                                    switch (rs.GetInt32(2))
                                    {
                                        case 3:
                                        case 4:
                                        case 6:
                                            f = true;
                                            break;
                                    }
                                    break;
                                case "NCP城&砦":
                                    switch (rs.GetInt32(2))
                                    {
                                        case 5:
                                        case 7:
                                            f = true;
                                            break;
                                    }
                                    break;
                                case "空き地":
                                    if (1 == rs.GetInt32(2))
                                    {
                                        f = true;
                                    }
                                    break;
                            }
                            if (!f) { continue; }

                            //同盟名
                            f = false;
                            if (string.Empty.Equals(bgc.alliance)) { f = true; }
                            else if (bgc.alliance.Equals(rs.GetString(3))) { f = true; }
                            if (!f) { continue; }

                            //君主名
                            f = false;
                            if (string.Empty.Equals(bgc.owner)) { f = true; }
                            else if (bgc.owner.Equals(rs.GetString(4))) { f = true; }
                            if (!f) { continue; }

                            //戦力
                            f = false;
                            if (0 == bgc.level) { f = true; }
                            else if (bgc.level == rs.GetInt32(5)) { f = true; }
                            if (!f) { continue; }

                            //資源
                            f = false;
                            if (0 == (bgc.wood + bgc.stone + bgc.iron + bgc.rice)) { f = true; }
                            else if (bgc.wood == rs.GetInt32(6) && bgc.stone == rs.GetInt32(7)
                                    && bgc.iron == rs.GetInt32(8) && bgc.rice == rs.GetInt32(9)) { f = true; }
                            if (!f) { continue; }

                            dgvBigMap[x, y].Style.BackColor = bgc.color;
                            dgvBigMap[x, y].Style.SelectionBackColor = bgc.color;
                            break;

                        }//背景色設定 End
                       
                    }
                    if (0 == progCount % 10000)
                    {
                        if (progCount < progLoading.Maximum)
                        {
                            progLoading.Value = progCount;
                            progLoading.Update();
                        }
                    }
                }
                progLoading.Visible = false;
                dgvBigMap[((int)mapposX.Value) + 800, (-1 * (int)mapposY.Value) + 800].Selected = true;

            }
            catch (SQLiteException ex)
            {
                this.showLogLn(string.Format("データベースからマップデータの取得に失敗しました。"));
                this.showLogLn(ex.Message);
            }
            finally
            {
                rs.Close();
            }

            this.moveBigMapBtn_Click(sender, e);
            createBigMapBtn.Enabled = true;
        }

        private void dgvBigMap_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            mapPoint.Text = dgvBigMap[e.ColumnIndex, e.RowIndex].ToolTipText;
        }

        private void moveBigMapBtn_Click(object sender, EventArgs e)
        {
            if (0 != dgvBigMap.ColumnCount && 0 != dgvBigMap.RowCount)
            {
                dgvBigMap.FirstDisplayedScrollingColumnIndex = 800 + (int)mapposX.Value - dgvBigMap.Width / 2 / (int)mapSize.Value;
                dgvBigMap.FirstDisplayedScrollingRowIndex = (-1 * (int)mapposY.Value) + 800 - dgvBigMap.Height / 2 / (int)mapSize.Value;
            }
        }

        private void dgvBigMap_SelectionChanged(object sender, EventArgs e)
        {

            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return;
            }

            int x = 0;
            int y = 0;
            foreach (DataGridViewCell cell in dgvBigMap.SelectedCells)
            {
                x = cell.ColumnIndex - 800;
                y = (-1 * cell.RowIndex) + 800;

                mapPoint.Text = dgvBigMap[cell.ColumnIndex, cell.RowIndex].ToolTipText;
            }

            string sql = string.Format("select time as 日時, case type when 1 then '空地' when 2 then '領地' when 3 then '拠村' when 4 then '拠砦'  when 5 then 'Ｎ砦' when 6 then '本拠' when 7 then 'Ｎ城' end as 種類 , name as 名称, owner as 君主, alliance as 同盟, population as 人口 from map_log where x = {0} and y = {1} order by time"
            , x, y);
            this.showLog(sql);
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, con))
            {
                try
                {
                    mapLogTitle.Text = string.Format("座標({0},{1})の履歴", x, y);
                    mapLogDataTable.Clear();
                    mapLogDataTable.Columns.Clear();
                    adapter.Fill(mapLogDataTable);
                }
                catch (SQLiteException ex)
                {
                    this.showLogLn(ex.Message);
                    return;
                }
            }
            dgvMapLog.AutoResizeColumns();
            dgvMapLog.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;
        }

        private void dgvBigMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvBigMap.Rows.Count <= 0) { return; }
            int x = 0;
            int y = 0;
            foreach (DataGridViewCell cell in dgvBigMap.SelectedCells)
            {
                x = cell.ColumnIndex - 800;
                y = (-1 * cell.RowIndex) + 800;
            }

            if ( -800 <= x && x <= 800 && -800 <= y && y <= 800)
            {
                Config conf = new Config();
                System.Diagnostics.Process.Start(string.Format(conf.get(Config.BURA3_MAP_URL_MINI), conf.get(Config.LOGIN_SV), x, y, 3));
            }
        }

        private void updateMapListBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("最新のマップデータを元に領地一覧を更新します。よろしいですか？", "更新確認", MessageBoxButtons.YesNo)) { return; }
            if (this.updateMapList())
            {
                this.setMapList();
                MessageBox.Show("領地一覧を更新しました！");
            }
            else
            {
                MessageBox.Show("領地一覧の更新に失敗しました！");
            }
        }

        private bool updateMapList()
        {
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return false;
            }

            Config conf = new Config();
            SQLiteCommand command = con.CreateCommand();
            SQLiteDataReader rs = null;

            try
            {
                string sql = "select level , wood, stone, iron, rice from map where type in ( 1, 2, 3, 4, 5 ) ";
                sql += "  and  0 < level ";
                sql += " group by level, wood, stone, iron, rice order by level desc, wood, stone, iron, rice desc limit 100 ";

                command.CommandText = sql;
                rs = command.ExecuteReader();

                int count = 0;
                while (rs.Read())
                {
                    conf.set(string.Format("{0}_{1}_level", Config.MAP_LIST, count), rs.GetInt32(0).ToString());
                    conf.set(string.Format("{0}_{1}_wood" , Config.MAP_LIST, count), rs.GetInt32(1).ToString());
                    conf.set(string.Format("{0}_{1}_stone", Config.MAP_LIST, count), rs.GetInt32(2).ToString());
                    conf.set(string.Format("{0}_{1}_iron" , Config.MAP_LIST, count), rs.GetInt32(3).ToString());
                    conf.set(string.Format("{0}_{1}_rice" , Config.MAP_LIST, count), rs.GetInt32(4).ToString());

                    count++;
                }
                conf.set(Config.MAP_LIST_SIZE, count.ToString());
            }
            catch (SQLiteException e)
            {
                this.showLogLn(string.Format("領地一覧の更新に失敗しました。"));
                this.showLogLn(e.Message);
                return false;
            }
            finally
            {
                rs.Close();
            }
            return true;
        }

        private void setMapList()
        {
            Config conf = new Config();

            mapTypeList.Items.Clear();
            for (int i = 0; i < conf.getInt(Config.MAP_LIST_SIZE); i++)
            {
                MapListItem mlt = new MapListItem();
                mlt.level = conf.getInt(string.Format("{0}_{1}_level", Config.MAP_LIST, i));
                mlt.wood  = conf.getInt(string.Format("{0}_{1}_wood" , Config.MAP_LIST, i));
                mlt.stone = conf.getInt(string.Format("{0}_{1}_stone", Config.MAP_LIST, i));
                mlt.iron  = conf.getInt(string.Format("{0}_{1}_iron" , Config.MAP_LIST, i));
                mlt.rice  = conf.getInt(string.Format("{0}_{1}_rice" , Config.MAP_LIST, i));

                mapTypeList.Items.Add(mlt);
            }
            if (0 < mapTypeList.Items.Count) { mapTypeList.SelectedIndex = 0; }
        }

        private void updateAllianceListBtn1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("最新のマップデータから、領地取得数TOP50の同盟を取得します。よろしいですか？", "更新確認", MessageBoxButtons.YesNo)) { return; }
            if (this.updateAllianceList())
            {
                this.setAllianceList();
                MessageBox.Show("同盟一覧を更新しました！");
            }
            else
            {
                MessageBox.Show("同盟一覧の更新に失敗しました！");
            }
        }

        private bool updateAllianceList()
        {
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return false;
            }

            Config conf = new Config();
            SQLiteCommand command = con.CreateCommand();
            SQLiteDataReader rs = null;
            try
            {
                command.CommandText = string.Format("SELECT count(*) as cnt , alliance FROM map group by alliance having cnt > 10 and alliance <> '' order by cnt desc limit 50");
                rs = command.ExecuteReader();
                int count = 0;
                while (rs.Read())
                {
                    conf.set(string.Format("{0}_{1}", Config.ALLIANCE_LIST, count), rs.GetString(1));
                    count++;
                }
                conf.set(Config.ALLIANCE_LIST_SIZE, count.ToString());
            }
            catch (SQLiteException e)
            {
                this.showLogLn(string.Format("データベースから同盟一覧の取得に失敗しました。"));
                this.showLogLn(e.Message);
                return false;
            }
            finally
            {
                rs.Close();
            }
            return true;
        }

        private void setAllianceList()
        {
            Config conf = new Config();
            
            alliListBox.Items.Clear();
            mapAlliBox.Items.Clear();
            mapOwnAlliBox.Items.Clear();

            mapOwnAlliBox.Items.Add(string.Empty);
            for (int i = 0; i < conf.getInt(Config.ALLIANCE_LIST_SIZE); i++)
            {
                string alliance = conf.get(string.Format("{0}_{1}", Config.ALLIANCE_LIST, i));

                alliListBox.Items.Add(alliance);
                mapAlliBox.Items.Add(alliance);
                mapOwnAlliBox.Items.Add(alliance);

            }
            alliListBox.Text = conf.get(Config.ALLI_NM);
            mapAlliBox.Text = conf.get(Config.ALLI_NM);
            
        }

        private void マップ区分コードCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("0：不明\r\n1：空き地\r\n2：取得領地\r\n3：拠点(村)\r\n4：拠点(砦)\r\n5：NPC砦\r\n6：本拠\r\n7：NPC城", "マップ区分コード");
        }

        private void このツールについてToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help dlg = new Help();
            dlg.ShowDialog();
        }

        private void backupDbFile()
        {
            Config conf = new Config();

            if (string.Empty.Equals(conf.get(Config.TOOL_DB_BACKUP_DIR))) { return; }

            try
            {
                stripMsg.Text = string.Format("データベースファイルのバックアップ中");
                if (!System.IO.Directory.Exists(conf.get(Config.TOOL_DB_BACKUP_DIR)))
                {
                    System.IO.Directory.CreateDirectory(conf.get(Config.TOOL_DB_BACKUP_DIR));
                }
                System.IO.File.Copy(this.getDbPath(), string.Format("{0}\\{1}", conf.get(Config.TOOL_DB_BACKUP_DIR), this.getDbName()), true);
            }
            catch (Exception ex)
            {
                this.showLog(ex.Message.ToString());
            }
        }

        private void sm_s_CheckedChanged(object sender, EventArgs e)
        {
            casOwner.Enabled = sm_s.Checked;
        }

        private void btnEnviron_Click(object sender, EventArgs e)
        {
            btnEnviron.Enabled = false;
            
            string outStr = string.Empty;

            for (int row = 0; row < dgvCas.Rows.Count; row++)
            {
                int x = int.Parse(dgvCas.Rows[row].Cells[1].Value.ToString());
                int y = int.Parse(dgvCas.Rows[row].Cells[2].Value.ToString());

                outStr += string.Format("\r\n{0}",this.getPointEnviron(x,y));

            }

            this.showLog(outStr);

            btnEnviron.Enabled = true;
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            SQLiteConnection con = this.getDBConnect();
            if (null == con)
            {
                this.showLogLn(string.Format("データベースコネクションの取得に失敗しました。処理を中断します。"));
                return;
            }

            string sql = string.Format("select time as データ取得日時"
                                        + ", case type "
                                        + " when 1 then '空き地' "
                                        + " when 2 then '領地' "
                                        + " when 3 then '村' "
                                        + " when 4 then '砦' "
                                        + " when 5 then 'NPC砦' "
                                        + " when 6 then '本拠' "
                                        + " when 7 then 'NPC城' "
                                        + " else '不明' end as 状態 "
                                        + ", owner as 所有者, alliance as 同盟, name as 領地名, population as 人口 "
                                        + " from map_log where x = {0} and y = {1} order by time"
                , logX.Value.ToString(), logY.Value.ToString());
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, con))
            {
                try
                {
                    mapLogFindDataTable.Clear();
                    mapLogFindDataTable.Columns.Clear();
                    //freeSlqCount.Text = string.Format("データ件数：{0}", 0);
                    adapter.Fill(mapLogFindDataTable);
                }
                catch (SQLiteException ex)
                {
                    this.showLogLn(ex.Message);
                    return;
                }
            }
            dgvLog.AutoResizeColumns();
            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            //stripMsg.Text = string.Format("データ件数：{0}", dgvFreeSql.Rows.Count);
            //freeSlqCount.Text = string.Format("データ件数：{0}", dgvFreeSql.Rows.Count);
        }

    }

    struct Point
    {
        public int x;
        public int y;
        public int base_x;
        public int base_y;

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }

    struct MapListItem
    {
        public int level;
        public int wood;
        public int stone;
        public int iron;
        public int rice;

        public override string ToString()
        {
            return string.Format("☆{0} - 木{1} 石{2} 鉄{3} 糧{4}", level, wood, stone, iron, rice);
        }
    }

    struct BigMapBackColorSetting
    {
        public bool flag;
        public Color color;
        public string type;
        public string alliance;
        public string owner;
        public int level;
        public int wood;
        public int stone;
        public int iron;
        public int rice;
        public string comment;
    }

}