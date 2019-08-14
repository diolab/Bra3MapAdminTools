using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Bra3MapAdminTools
{
    public struct ConfigItem
    {
        public string key;
        public string value;
        public int flag;
    }

    public class Config
    {
        private string configDBPath = null;
        private string connectionStr = null;
        private const string squat = "&quot;";

        public const string BURA3_URL = "bura3url";
        public const string BURA3_MAP_URL = "bura3mapurl";
        public const string BURA3_MAP_URL_MINI = "bura3mapurlmini";

        public const string LOGIN_ID = "bura3loginid";
        public const string LOGIN_PW = "bura3loginpw";
        public const string LOGIN_SV = "bura3loginsv";

        public const string ALLI_OWN = "bura3allianceowner";
        public const string ALLI_NM = "bura3alliancename";

        public const string DEF_X = "bura3defaultx";
        public const string DEF_Y = "bura3defaulty";

        public const string CAS_CandP_F = "bura3copuandpastformat";

        public const string TOOL_DB_DIR = "bura3databasefolder";
        public const string TOOL_DB_BACKUP_DIR = "bura3databasebackupfolder";

        public const string BIGMAP_COLOR_TABLE = "bura3bigmapcolotablesettings";
        public const string BIGMAP_COLOR_TABLE_SIZE = "bura3bigmapcolotablesettings_count";

        public const string MAP_LIST_SIZE = "bura3maplist_size";
        public const string MAP_LIST = "bura3maplist";

        public const string ALLIANCE_LIST_SIZE = "bura3alliancelist_size";
        public const string ALLIANCE_LIST = "bura3alliancelist";

        public Config()
        {
            configDBPath = Application.ExecutablePath.Replace("EXE", "config");
            configDBPath = configDBPath.Replace("exe", "config");
            connectionStr = string.Format("Data Source={0};Version=3;", configDBPath);
            if (!System.IO.File.Exists(configDBPath))
            {
                SQLiteConnection con = new SQLiteConnection();
                con.ConnectionString = connectionStr;
                con.Open();

                SQLiteCommand command = con.CreateCommand();
                command.CommandText = "CREATE TABLE CONFIGS (key text, value text, flag INTEGER);";
                command.ExecuteNonQuery();

                con.Close();

                this.set(BURA3_URL, "http://mixi.jp/run_appli.pl?id=6598");
                this.set(BURA3_MAP_URL, "http://w{0}.3gokushi.jp/big_map.php?x={1}&y={2}&type={3}");
                this.set(BURA3_MAP_URL_MINI, "http://w{0}.3gokushi.jp/map.php?x={1}&y={2}&type={3}");
                //this.set(BURA3_MAP_URL, "http://w4.3gokushi.jp/big_map.php?x={1}&y={2}&type={3}");
                //this.set(BURA3_MAP_URL_MINI, "http://w4.3gokushi.jp/map.php?x={1}&y={2}&type={3}");


                this.set(Config.CAS_CandP_F, "\t{0}\t{1}\r\n{2}\tëœãv\t\r\nïÂèWíÜ");
            }
        }

        public bool set(string key, string value)
        {
            return this.set(key, value, 0);
        }

        public bool set(string key, string value, int flag)
        {
            value = value.Replace("'", squat);
            SQLiteConnection con = null;
            try
            {
                con = new SQLiteConnection();
                con.ConnectionString = connectionStr;
                con.Open();

                //check key value
                SQLiteCommand command = con.CreateCommand();
                command.CommandText = string.Format("select key from CONFIGS where key = '{0}';", key);
                SQLiteDataReader rs = command.ExecuteReader();
                int count = 0;
                while (rs.Read()) { count++; }
                rs.Close();

                if (0 == count)
                {
                    //no key
                    command.CommandText = string.Format("insert into CONFIGS (key, value, flag) values ('{0}', '{1}', {2});", key, value, flag);
                    command.ExecuteNonQuery();
                }
                else
                {
                    //active key
                    command.CommandText = string.Format("update configs set value = '{0}' where key = '{1}';", value, key);
                    command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException e)
            {
                string str = e.Message;
                return false;
            }
            finally
            {
                con.Close();
            }
            return true;
        }

        public string get(string key)
        {
            SQLiteConnection con = null;
            string value = string.Empty;
            try
            {
                con = new SQLiteConnection();
                con.ConnectionString = connectionStr;
                con.Open();

                //check key value
                SQLiteCommand command = con.CreateCommand();
                command.CommandText = string.Format("select value from CONFIGS where key = '{0}';", key);
                SQLiteDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    value = rs.GetString(0);
                }
                rs.Close();
            }
            catch (SQLiteException e)
            {
                string str = e.Message;
                return string.Empty;
            }
            finally
            {
                con.Close();
            }
            return value.Replace(squat, "'");
        }

        public int getInt(string key)
        {
            string value = this.get(key);
            int result = 0;
            if (int.TryParse(value,out result))
            {
                return int.Parse(value);
            }
            return 0;
        }

        public void del(string key)
        {
            SQLiteConnection con = null;
            try
            {
                con = new SQLiteConnection();
                con.ConnectionString = connectionStr;
                con.Open();

                //check key value
                SQLiteCommand command = con.CreateCommand();
                command.CommandText = string.Format("delete from CONFIGS where key = '{0}';", key);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                string str = e.Message;             
            }
            finally
            {
                con.Close();
            }
        }

        public List<ConfigItem> getList(int flag)
        {
            List<ConfigItem> ret = new List<ConfigItem>();

            SQLiteConnection con = null;
            try
            {
                con = new SQLiteConnection();
                con.ConnectionString = connectionStr;
                con.Open();

                //check key value
                SQLiteCommand command = con.CreateCommand();
                command.CommandText = string.Format("select key, value from CONFIGS where flag = '{0}' order by key;", flag);
                SQLiteDataReader rs = command.ExecuteReader();
                while (rs.Read())
                {
                    ConfigItem item = new ConfigItem();
                    item.key = rs.GetString(0).Replace(squat, "'");
                    item.value = rs.GetString(1).Replace(squat, "'");
                    item.flag = flag;

                    ret.Add(item);
                }
                rs.Close();
            }
            catch (SQLiteException e)
            {
                string str = e.Message;
                ret.Clear();
                return ret;
            }
            finally
            {
                con.Close();
            }
            return ret;
        }
        
    }
}
