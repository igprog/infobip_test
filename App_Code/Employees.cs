using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SQLite;
using Igprog;

/// <summary>
/// Employees
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Employees : System.Web.Services.WebService {
    Global G = new Global();
    DataBase db = new DataBase();

    public Employees() {
    }

    public class NewEmployee {
        public int id;
        public string name;
        public bool isDriver;
        public bool isSelected;
    }

    [WebMethod]
    public string Load() {
        List<NewEmployee> xx = new List<NewEmployee>();
        try {
            return JsonConvert.SerializeObject(LoadData(), Formatting.None);
        } catch (Exception e) { return e.Message; }
    }

     public List<NewEmployee> LoadData() {
        List<NewEmployee> xx = new List<NewEmployee>();
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
            connection.Open();
            string sql = "SELECT id, name, isDriver FROM employees";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection)) {
                using (SQLiteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        NewEmployee x = ReadData(reader);
                        xx.Add(x);
                    }
                }
            }
            connection.Close();
        }
        return xx;
    }

    public NewEmployee ReadData(SQLiteDataReader reader) {
        //List<NewEmployee>  xx = new List<NewEmployee>();
        //while (reader.Read()) {
            NewEmployee x = new NewEmployee();
            x.id = G.ReadI(reader, 0);
            x.name = G.ReadS(reader, 1);
            x.isDriver = G.ReadB(reader, 2);
            x.isSelected = false;
            //xx.Add(x);
        //}
        return x;
    }


    public NewEmployee Get(string id) {
        NewEmployee x = new NewEmployee();
        if (!string.IsNullOrEmpty(id)) {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db.GetDataBasePath(G.dataBase))) {
                connection.Open();
                string sql = string.Format("SELECT id, name, isDriver FROM employees WHERE id = {0}", id);
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

    public List<NewEmployee> GetSelectedEmployees(string employees) {
        List<NewEmployee> xx = new List<NewEmployee>();
        if (!string.IsNullOrEmpty(employees)) {
            string[] employeesList = employees.Split(';');
            foreach (var e in employeesList) {
                NewEmployee x = Get(e);
                xx.Add(x);
            }
        }
        
        return xx;
    }





}
