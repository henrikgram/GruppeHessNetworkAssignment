using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    public class DBHandler
    {
        public DBHandler()
        {

        }

        // SKAL KALDES. DON'T BE DUMB. Sørger for at den connecter til databasen som vi opretter.
        public string LoadSQLiteConnectionString()
        {
            return ConfigurationManager.AppSettings["NetvaerkDB"];
        }

        /// <summary>
        /// Builds the database.
        /// </summary>
        public void BuildDatabase()
        {
            //CreateTable("Highscore", "Name STRING, Score INTEGER", new SQLiteConnection(LoadSQLiteConnectionString()));
            //CreateTable("NameTable", "ID INTEGER PRIMARY KEY, Name STRING, UNIQUE (Name)", new SQLiteConnection(LoadSQLiteConnectionString()));
            //CreateTable("ScoreTable", "ID INTEGER PRIMARY KEY, NameID INTEGER, Score INTEGER, FOREIGN KEY (NameID) REFERENCES NameTable(ID)", new SQLiteConnection(LoadSQLiteConnectionString()));
            CreateTable("Highscore", "ID INTEGER PRIMARY KEY, Name STRING, Score INTEGER", new SQLiteConnection(LoadSQLiteConnectionString()));
        }

        private void ExecuteNonQuerySQLiteCommand(SQLiteCommand command, SQLiteConnection connection)
        {
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Hovedforfatter: Stinna
        /// Method for executing SQLiteCommands as Reader and returning them as a List of integers. 
        /// </summary>
        /// <param name="selectedColumn">What you want to select</param>
        /// <param name="fromTable">What table you want to select from</param>
        /// <param name="whereDefinition">Where conidition for selection</param>
        /// <returns>Returns a list of integers</returns>
        //public List<int> ExecuteStringReader()
        //{
        //    List<int> values = new List<int>();
        //    SQLiteConnection connection = new SQLiteConnection(LoadSQLiteConnectionString());
        //    SQLiteCommand command;

        //    using (connection)
        //    {
        //        connection.Open();

        //        command = new SQLiteCommand($"SELECT {selectedColumn} FROM Highscore WHERE {whereDefinition}",
        //                  connection);

        //        SQLiteDataReader reader = command.ExecuteReader();

        //        //For every read while reader is reading, convert to int32 and add it to the return list.
        //        while (reader.Read())
        //        {
        //            values.Add(reader.GetInt32(0));
        //        }
        //    }

        //    return values;
        //}


        public List<string> CreateHighscoreList()
        {
            List<string> highscoreList = new List<string>();
            SQLiteConnection connection = new SQLiteConnection(LoadSQLiteConnectionString());
            SQLiteCommand command;

            using (connection)
            {
                connection.Open();

                command = new SQLiteCommand($"SELECT ID FROM Highscore ORDER BY Score DESC", connection);

                SQLiteDataReader reader = command.ExecuteReader();

                //For every read while reader is reading, convert to int32 and add it to the return list.
                while (reader.Read())
                {
                    int tmpID = reader.GetInt32(0);

                    string tmpName = SelectStringValuesWhere("Name", "Highscore", $"ID={tmpID}", new SQLiteConnection(LoadSQLiteConnectionString()));

                    int tmpScore = SelectIntValuesWhere("Score", "Highscore", $"ID={tmpID}", new SQLiteConnection(LoadSQLiteConnectionString()));

                    highscoreList.Add($"{tmpName}: {tmpScore}");
                }
            }

            return highscoreList;
        }


        private void CreateTable(string tableName, string columns, SQLiteConnection connection)
        {
            ExecuteNonQuerySQLiteCommand(new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {tableName} ({columns})", connection), connection);
        }

        /// <summary>
        /// Inserts a new score into the highscore list.
        /// Needs both a name and a score.
        /// </summary>
        /// <param name="name"> Insert the values into certain colums. First name, then score.</param>
        /// <param name="connection"></param>
        public void InsertIntoTable(string tableName, string definedValues, SQLiteConnection connection)
        {
            ExecuteNonQuerySQLiteCommand(new SQLiteCommand($"INSERT INTO {tableName} VALUES ({definedValues});", connection), connection);
        }


        #region Methods for getting table values.

        private string SelectStringValuesWhere(string selectDefinition, string tableName, string whereDefinition,
                                              SQLiteConnection connection)
        {
            return ExecuteScalarString(new SQLiteCommand($"SELECT {selectDefinition} FROM {tableName} WHERE " +
                                                 $"{whereDefinition};", connection), connection);
        }

        private int SelectIntValuesWhere(string selectDefinition, string tableName, string whereDefinition,
                                       SQLiteConnection connection)
        {
            return ExecuteScalarInt(new SQLiteCommand($"SELECT {selectDefinition} FROM {tableName} WHERE " +
                                 $"{whereDefinition};", connection), connection);
        }



        private string ExecuteScalarString(SQLiteCommand command, SQLiteConnection connection)
        {
            string value;

            using (connection)
            {
                connection.Open();
                value = Convert.ToString(command.ExecuteScalar());
            }

            return value;
        }

        private int ExecuteScalarInt(SQLiteCommand command, SQLiteConnection connection)
        {
            int value;

            using (connection)
            {
                connection.Open();
                value = Convert.ToInt32(command.ExecuteScalar());
            }

            return value;
        }

        #endregion
    }
}
