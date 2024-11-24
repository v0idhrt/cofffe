using Microsoft.Data.Sqlite;
using SupportSystemCofe.Shared.Models;
using System.Security.Cryptography.Xml;

namespace Server.Controllers
{
    public class DatabaseController
    {
        private SqliteConnection _con;
        private SqliteCommand _command;
        public DatabaseController()
        {
            _con = new SqliteConnection("Data Source=coffee.db;Mode=ReadWriteCreate");
            _con.Open();
            _command = new SqliteCommand();
            _command.Connection = _con;
            _command.CommandText = "CREATE TABLE IF NOT EXISTS Users(" +
                                    "_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                                    "fullName     TEXT NOT NULL, " +
                                    "email        TEXT NOT NULL, " +
                                    "phone        TEXT NOT NULL, " +
                                    "password     TEXT NOT NULL, " +
                                    "businessName TEXT NOT NULL, " +
                                    "industry     TEXT NOT NULL, " +
                                    "region       TEXT NOT NULL, " +
                                    "scale        TEXT NOT NULL, " +
                                    "activity     TEXT NOT NULL, " +
                                    "details      TEXT NOT NULL," +
                                    "isActivated  INTEGER NOT NULL" +
                                    ")";
            _command.ExecuteNonQuery();
        }

        public int user_writeRegInfo(RegistrationRequest regInfo)
        {
            int inserted = 0;

            _command.Connection = _con;

            _command.CommandText = $"SELECT * FROM Users WHERE email='{regInfo.Email}'";

            using (SqliteDataReader reader = _command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    inserted = 0;
                    return inserted;
                }
            }

            _command.CommandText = "INSERT INTO Users " +
                "(fullName, email, phone, password, businessName, industry, region, scale, activity, details, isActivated)" +
               $"VALUES ('{regInfo.FullName}', '{regInfo.Email}', '{regInfo.Phone}', '{regInfo.Password}', '{regInfo.BusinessName}', " +
               $"'{regInfo.Industry}', '{regInfo.Region}', '{regInfo.Scale}', '{regInfo.Activity}', '{regInfo.Details}', 0)";

            inserted = _command.ExecuteNonQuery();
            return inserted;
        }

        public RegistrationRequest user_getRegInfo(string userEmail)
        {
            RegistrationRequest regInfo = new RegistrationRequest();

            _command.Connection = _con;

            _command.CommandText = $"SELECT * FROM Users WHERE email='{userEmail}'";

            using (SqliteDataReader reader = _command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    regInfo.FullName     = (string)reader["fullName"];
                    regInfo.Email        = (string)reader["email"];
                    regInfo.Phone        = (string)reader["phone"];
                    regInfo.Password     = String.Empty;
                    regInfo.BusinessName = (string)reader["businessName"];
                    regInfo.Industry     = (string)reader["industry"];
                    regInfo.Region       = (string)reader["region"];
                    regInfo.Scale        = (string)reader["scale"];
                    regInfo.Activity     = (string)reader["activity"];
                    regInfo.Details      = (string)reader["details"];
                }
                else
                {
                    return regInfo;
                }
            }

            return regInfo;
        }

        public bool user_activateAccount(string userEmail)
        {
            int linesUpdated = 0;

            _command.Connection = _con;

            _command.CommandText = $"UPDATE Users SET isActivated=1 WHERE email='{userEmail}'";
            linesUpdated = _command.ExecuteNonQuery();

            return linesUpdated != 0;
        }

        public bool user_isActivated(string userEmail)
        {
            bool isActivated = false;

            _command.Connection = _con;

            _command.CommandText = $"SELECT isActivated FROM Users WHERE email='{userEmail}'";
            using (SqliteDataReader reader = _command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    isActivated = ((Int64)reader["isActivated"] == 1);
                }
                else
                {
                    return false;
                }
            }

            return isActivated;
        }

        ~DatabaseController()
        {
            _con.Close();
        }
    }
}
