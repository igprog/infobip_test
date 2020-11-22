using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Data.SQLite;
using Igprog;

/// <summary>
/// Global
/// </summary>
namespace Igprog {
    public class Global {
        public Global() {
        }

        public DB db = new DB();
        public string dataBase = ConfigurationManager.AppSettings["dataBase"];

        public class DB {
            public string travelplan = "travelplan";
        }

        public string ReadS(SQLiteDataReader reader, int i) {
            return reader.GetValue(i) == DBNull.Value ? null : reader.GetString(i);
        }

        public int ReadI(SQLiteDataReader reader, int i) {
            return reader.GetValue(i) == DBNull.Value ? 0 : reader.GetInt32(i);
        }

        public double ReadD(SQLiteDataReader reader, int i) {
            return reader.GetValue(i) == DBNull.Value ? 0 : Convert.ToDouble(reader.GetString(i));
        }

        public bool ReadB(SQLiteDataReader reader, int i) {
            return reader.GetValue(i) == DBNull.Value ? false : Convert.ToBoolean(reader.GetString(i));
        }

        public DateTime ReadDT(SQLiteDataReader reader, int i) {
            return reader.GetValue(i) == DBNull.Value ? new DateTime() : Convert.ToDateTime(reader.GetString(i));
        }
    }
}