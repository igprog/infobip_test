using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SQLite;
using Igprog;

/// <summary>
/// Booking
/// </summary>
[WebService(Namespace = "https://apartmentselvira.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Booking : System.Web.Services.WebService {
    Global G = new Global();
    DataBase db = new DataBase();
    string mainSql = "id, firstName, lastName, email, apartment, arrival, departure, adult, child, note, totPrice, deposit, confirmed";

    public Booking() {
    }

    public class NewBooking {
        public string id;
        public string firstName;
        public string lastName;
        public string email;
        public string apartment;
        public string arrival;
        public string departure;
        public string adult;
        public string child;
        public string note;
        public double totPrice;
        public double deposit;
        public bool confirmed;
        public bool isEdit;
        public Mail.Response response;
    }

    public class Calendar {
        public string apartment;
        public string name;
        public string arrival;
        public string departure;
        public bool confirmed;
    }

    [WebMethod]
    public string Init() {
        NewBooking x = new NewBooking();
        x.id = null;
        x.firstName = null;
        x.lastName = null;
        x.email = null;
        x.apartment = null;
        x.arrival = null;
        x.departure = null;
        x.adult = null;
        x.child = null;
        x.note = null;
        x.totPrice = 0;
        x.deposit = 0;
        x.confirmed = false;
        x.isEdit = false;
        x.response = new Mail.Response();
        x.response.isSent = false;
        x.response.msg = null;
        x.response.msg1 = null;
        return JsonConvert.SerializeObject(x, Formatting.None);
    }

    [WebMethod]
    public string Send(NewBooking x) {
        try {
            string subject = string.Format("Novi upit - A{0} od {1} do {2}", x.apartment, x.arrival, x.departure);
            string body = string.Format(@"
<h3>Novi upit</h3>
<p>Ime: {0} {1}</p>
<p>Email: {2}</p>
<p>Apartman: {3}</p>
<p>Dolazak: {4}</p>
<p>Odlazak: {5}</p>
<p>Odraslih: {6}</p>
<p>Djece: {7}</p>
<p>Poruka: {8}</p>", x.firstName, x.lastName, x.email, string.IsNullOrEmpty(x.apartment) ? null : string.Format("A{0}", x.apartment), x.arrival, x.departure, x.adult, x.child, x.note);
            Mail m = new Mail();
            x.response = m.SendMail(G.email, subject, body, true);

            //TODO: Send mail to client

            return JsonConvert.SerializeObject(SaveData(x), Formatting.None);
        } catch(Exception e) {
            x.response.isSent = false;
            x.response.msg = e.Message;
            x.response.msg1 = e.StackTrace;
            return JsonConvert.SerializeObject(x, Formatting.None);
        }
    }

    [WebMethod]
    public string Load(int year) {
        db.CreateDataBase(G.db.booking);
        List<NewBooking> xx = new List<NewBooking>();
        try {
            return JsonConvert.SerializeObject(LoadData(year), Formatting.None);
        } catch (Exception e) { return e.Message; }
    }

    public List<NewBooking> LoadData(int year) {
        db.CreateDataBase(G.db.booking);
        List<NewBooking> xx = new List<NewBooking>();
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            string sql = string.Format("SELECT {0} FROM booking WHERE strftime('%Y', arrival) = '{1}' ORDER BY rowid DESC", mainSql, year);
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                using (SQLiteDataReader reader = command.ExecuteReader()) {
                    xx = ReadData(reader);
                }
            }
            connection.Close();
        }
        return xx;
    }

    public List<NewBooking> ReadData(SQLiteDataReader reader) {
        List<NewBooking>  xx = new List<NewBooking>();
        while (reader.Read()) {
            NewBooking x = new NewBooking();
            x.id = G.ReadS(reader, 0);
            x.firstName = G.ReadS(reader, 1);
            x.lastName = G.ReadS(reader, 2);
            x.email = G.ReadS(reader, 3);
            x.apartment = G.ReadS(reader, 4);
            x.arrival = G.ReadS(reader, 5);
            x.departure = G.ReadS(reader, 6);
            x.adult = G.ReadS(reader, 7);
            x.child = G.ReadS(reader, 8);
            x.note = G.ReadS(reader, 9);
            x.totPrice = G.ReadD(reader, 10);
            x.deposit = G.ReadD(reader, 11);
            x.confirmed = G.ReadB(reader, 12);
            x.response = new Mail.Response();
            xx.Add(x);
        }
        return xx;
    }

    [WebMethod]
    public string Save(NewBooking x) {
        try {
            return JsonConvert.SerializeObject(SaveData(x), Formatting.None);
        } catch (Exception e) {
            return JsonConvert.SerializeObject(e.Message, Formatting.None);
        }
    }

    public NewBooking SaveData(NewBooking x) {
        db.CreateDataBase(G.db.booking);
        if (string.IsNullOrEmpty(x.id)) {
            x.id = Guid.NewGuid().ToString();
        }
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            string sql = string.Format(@"INSERT OR REPLACE INTO booking (id, firstName, lastName, email, apartment, arrival, departure, adult, child, note, totPrice, deposit, confirmed)
                        VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}')"
                    , x.id, x.firstName, x.lastName, x.email, x.apartment, x.arrival, x.departure, x.adult, x.child, x.note, x.totPrice, x.deposit, x.confirmed);
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return x;
    }

    [WebMethod]
    public string CheckAvailability(NewBooking x) {
        try {
            db.CreateDataBase(G.db.booking);
            List<NewBooking> xx = new List<NewBooking>();
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                connection.Open();
                string sql = string.Format(@"SELECT {0} FROM booking WHERE ((date('{1}') >= date(arrival) AND date('{1}') < date(departure))
                                            OR (date('{2}') > date(arrival) AND date('{2}') <= date(departure))) AND (confirmed = 'True')"
                                            , mainSql, x.arrival, x.departure);
                using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                    using (SQLiteDataReader reader = command.ExecuteReader()) {
                        xx = ReadData(reader);
                    }
                }
                connection.Close();
            }
            bool ap1 = false, ap2 = false, ap3 = false;
            if (xx.Count > 0) {
                foreach(var r in xx) {
                    if (r.apartment.Contains("1")) {
                        ap1 = true;
                    }
                    if (r.apartment.Contains("2")) {
                        ap2 = true;
                    }
                    if (r.apartment.Contains("3")) {
                        ap3 = true;
                    }
                }
            }
            if (ap1 && ap2 && ap3) {
                x.response.isSent = false;
                x.response.msg = "We do not have free accommodation in that period";
                x.response.msg1 = "Change the term";
            } else {
                bool[] apList = new bool[3] { ap1, ap2, ap3 };
                int i = apList.Count(a => a == false);
                if ((i == 1 && Convert.ToInt32(x.adult) + Convert.ToInt32(x.child) <= 4) || (i == 2 && Convert.ToInt32(x.adult) + Convert.ToInt32(x.child) <= 8) || (i == 3 && Convert.ToInt32(x.adult) + Convert.ToInt32(x.child) <= 13)) {
                    x.response.isSent = true;
                    x.response.msg = "We have free accommodation in that period";
                    x.response.msg1 = null;
                } else {
                    x.response.isSent = false;
                    x.response.msg = "We do not have free accommodation in that period";
                    x.response.msg1 = "Change the term";
                }
            }
            return JsonConvert.SerializeObject(x.response, Formatting.None);
        } catch (Exception e) { return JsonConvert.SerializeObject(e.Message, Formatting.None); }
    }

    [WebMethod]
    public string LoadCalendar(int year, bool showDetails) {
        try {
            db.CreateDataBase(G.db.booking);
            List<Calendar> xx = new List<Calendar>();
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                connection.Open();
                string sql = string.Format(@"SELECT firstName, lastName, apartment, arrival, departure, confirmed FROM booking WHERE strftime('%Y', arrival) = '{0}' {1}"
                                        , year, showDetails ? "" : string.Format("AND confirmed = 'True'"));
                using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                    using (SQLiteDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Calendar x = new Calendar();
                            x.name = showDetails ? string.Format("{0} {1}", G.ReadS(reader, 0), G.ReadS(reader, 1)) : "";
                            x.apartment = G.ReadS(reader, 2);
                            x.arrival = G.ReadS(reader, 3);
                            x.departure = G.ReadS(reader, 4);
                            x.confirmed = G.ReadB(reader, 5);
                            xx.Add(x);
                        }
                    }
                }
                connection.Close();
            }
            for (int i=1; i<=3; i++) {
                if (!xx.Exists(a => a.apartment == i.ToString())) {
                    Calendar x = new Calendar();
                    x.name = "";
                    x.apartment = i.ToString();
                    x.arrival = string.Format("{0}-06-15", year);
                    x.departure = string.Format("{0}-06-15", year);
                    x.confirmed = false;
                    xx.Add(x);
                }
            }
            return JsonConvert.SerializeObject(xx, Formatting.None);
        } catch (Exception e) { return JsonConvert.SerializeObject(e.Message, Formatting.None); }
    }

    [WebMethod]
    public string Delete(string id, int year) {
        try {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                connection.Open();
                string sql = string.Format(@"DELETE from booking WHERE id = '{0}'", id);
                using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            } return JsonConvert.SerializeObject(LoadData(year), Formatting.None);
        }
        catch (Exception e) { return JsonConvert.SerializeObject(e.Message, Formatting.None); }
    }


}
