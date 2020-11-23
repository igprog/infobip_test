using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SQLite;
using Igprog;

/// <summary>
/// TravelPlan
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class TravelPlan : System.Web.Services.WebService {
    Global G = new Global();
    DataBase db = new DataBase();
    public string mainSql = "SELECT id, startLoacation, endLocation, startDate, endDate, car, employees FROM travelplan";

    public TravelPlan() {
    }

    public class NewTravelPlan {
        public string id;
        public string startLoacation;
        public string endLocation;
        public DateTime startDate;
        public DateTime endDate;
        public Cars.NewCar car;
        public List<Employees.NewEmployee> employees;
    }

    public class Response {
        public List<NewTravelPlan> data;
        public string msg;
    }

    [WebMethod]
    public string Init() {
        return JsonConvert.SerializeObject(cInit(), Formatting.None);
    }

    public NewTravelPlan cInit() {
        NewTravelPlan x = new NewTravelPlan();
        x.id = null;
        x.startLoacation = null;
        x.endLocation = null;
        x.startDate = DateTime.Today;
        x.endDate = DateTime.Today;
        x.car = new Cars.NewCar();
        x.employees = new List<Employees.NewEmployee>();
        return x;
    }

    [WebMethod]
    public string Load() {
        Response x = new Response();
        try {
            x.data = LoadData(mainSql);
            x.msg = null; 
            return JsonConvert.SerializeObject(x, Formatting.None);
        } catch (Exception e) {
            x.msg = e.Message;
            return JsonConvert.SerializeObject(x, Formatting.None);
        }
    }

    [WebMethod]
    public string LoadMonth(string month, string year) {
        Response x = new Response();
        try {
            List<NewTravelPlan> xx = new List<NewTravelPlan>();
            xx = LoadData(mainSql);
            if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year)) {
                x.data = xx.Where(a => a.startDate.Month == Convert.ToInt32(month) && a.startDate.Year == Convert.ToInt32(year)).ToList();
            } else {
                x.data = xx;
            }

            x.msg = null; 
            return JsonConvert.SerializeObject(x, Formatting.None);
        } catch (Exception e) {
            x.msg = e.Message;
            return JsonConvert.SerializeObject(x, Formatting.None);
        }
    }

    [WebMethod]
    public string Get(string id) {
        NewTravelPlan x = new NewTravelPlan();
        try {
            if (!string.IsNullOrEmpty(id)) {
                db.CreateDataBase(G.db.travelplan);
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                    connection.Open();
                    string sql = string.Format("{0} WHERE id = '{1}'", mainSql, id);
                    using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                        using (SQLiteDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                x = ReadData(reader);
                            }
                        }
                    }
                    connection.Close();
                }
            } else {
                x = cInit();
            }
            return JsonConvert.SerializeObject(x, Formatting.None);
        } catch (Exception e) {
            return JsonConvert.SerializeObject(x, Formatting.None);
        }
    }

     public List<NewTravelPlan> LoadData(string sql) {
        List<NewTravelPlan> xx = new List<NewTravelPlan>();
        db.CreateDataBase(G.db.travelplan);
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                using (SQLiteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        NewTravelPlan x = ReadData(reader);
                        xx.Add(x);
                    }
                }
            }
            connection.Close();
        }
        return xx;
    }

    public NewTravelPlan ReadData(SQLiteDataReader reader) {
        NewTravelPlan x = new NewTravelPlan();
        x.id = G.ReadS(reader, 0);
        x.startLoacation = G.ReadS(reader, 1);
        x.endLocation = G.ReadS(reader, 2);
        x.startDate = G.ReadDT(reader, 3);
        x.endDate = G.ReadDT(reader, 4);
        Cars C = new Cars();
        x.car = C.Get(G.ReadS(reader, 5));
        Employees E = new Employees();
        x.employees = E.GetSelectedEmployees(G.ReadS(reader, 6));
        return x;
    }

    [WebMethod]
    public string Save(NewTravelPlan x) {
        try {
            return JsonConvert.SerializeObject(SaveData(x), Formatting.None);
        } catch (Exception e) {
            return JsonConvert.SerializeObject(e.Message, Formatting.None);
        }
    }

    public NewTravelPlan SaveData(NewTravelPlan x) {
        db.CreateDataBase(G.db.travelplan);
        if (string.IsNullOrEmpty(x.id)) {
            x.id = Guid.NewGuid().ToString();
        }
        string employees = null;
        if (x.employees != null) {
            List<int> selectedEmployees = new List<int>();
            foreach (var e in x.employees) {
                selectedEmployees.Add(e.id);
            }
            employees = string.Join(";", selectedEmployees);
        }
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            string sql = string.Format(@"INSERT OR REPLACE INTO travelplan (id, startLoacation, endLocation, startDate, endDate, car, employees)
                        VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')"
                    , x.id, x.startLoacation, x.endLocation, x.startDate, x.endDate, x.car.id, employees);
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return x;
    }

    [WebMethod]
    public string Delete(string id) {
        try {
            string msg = null;
            if (!string.IsNullOrEmpty(id)) {
                string sql = string.Format(@"DELETE FROM travelplan WHERE id = '{0}'", id);
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                        command.ExecuteReader();
                    }
                    connection.Close();
                }
                msg = "Obrisano";
            } else {
                msg = "Pozajmica ne može biti obrisana jer nije spremljena";
            }
            return JsonConvert.SerializeObject(msg, Formatting.None);
        } catch (Exception e) {
            return JsonConvert.SerializeObject(e.Message, Formatting.None);
        }
    }

}
