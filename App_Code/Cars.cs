using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SQLite;
using Igprog;

/// <summary>
/// Cars
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Cars : System.Web.Services.WebService {
    Global G = new Global();
    DataBase db = new DataBase();
    string mainSql = "SELECT id, name, carType, color, plate, seats FROM cars";

    public Cars() {
    }

    public class NewCar {
        public int id;
        public string name;
        public string carType;
        public string color;
        public string plate;
        public int seats;
        public bool isAvailable;
    }

    [WebMethod]
    public string Load() {
        List<NewCar> xx = new List<NewCar>();
        try {
            return JsonConvert.SerializeObject(LoadData(mainSql), Formatting.None);
        } catch (Exception e) { return e.Message; }
    }

    [WebMethod]
    public string GetAvailableCars(TravelPlan.NewTravelPlan x) {
        try {
            List<NewCar> xx = LoadData(mainSql);
            TravelPlan TP = new TravelPlan();
            List<TravelPlan.NewTravelPlan> tPlans = TP.LoadData(TP.mainSql);
            foreach(var c in xx) {
                if (tPlans.Where(a => (a.car.id == c.id) 
                                && ((DateTime.Compare(a.startDate, x.startDate) >= 0 && DateTime.Compare(a.startDate, x.endDate) < 0) 
                                || (DateTime.Compare(a.endDate, x.startDate) >= 0 && DateTime.Compare(a.endDate, x.endDate) <= 0))).Count() > 0) {
                    c.isAvailable = false;
                } else {
                    c.isAvailable = true;
                }
            }
            List<NewCar> availableCars = xx.Where(a => a.isAvailable).ToList();
            return JsonConvert.SerializeObject(availableCars, Formatting.None);
        } catch (Exception e) {
            return JsonConvert.SerializeObject(e.Message, Formatting.None);
        }
    }

     public List<NewCar> LoadData(string sql) {
        List<NewCar> xx = new List<NewCar>();
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                using (SQLiteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        NewCar x = ReadData(reader);
                        xx.Add(x);
                    }
                }
            }
            connection.Close();
        }
        return xx;
    }

    public NewCar Get(string id) {
        NewCar x = new NewCar();
        if (!string.IsNullOrEmpty(id)) {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                connection.Open();
                string sql = string.Format("{0} WHERE id = {1}", mainSql, id);
                using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                    using (SQLiteDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            x = ReadData(reader);
                        }
                    }
                }
                connection.Close();
            }
        }
        return x;
    }

    public NewCar ReadData(SQLiteDataReader reader) {
        NewCar x = new NewCar();
        x.id = G.ReadI(reader, 0);
        x.name = G.ReadS(reader, 1);
        x.carType = G.ReadS(reader, 2);
        x.color = G.ReadS(reader, 3);
        x.plate = G.ReadS(reader, 4);
        x.seats = G.ReadI(reader, 5);
        return x;
    }

}
