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
    /// <summary>
    /// For handling SQLite/database methods.
    /// </summary>

    public class DBHandler
    {
        /// <summary>
        /// Builds the database.
        /// </summary>
        public void BuildDatabase()
        {        
            CreateTable("Highscore", "ID INTEGER PRIMARY KEY, Name STRING, Score INTEGER", new SQLiteConnection(LoadSQLiteConnectionString()));
        }

        /// <summary>
        /// Creates the highscore list. Is only called once the game ends.
        /// Return a List<string> of the highscores in the database.
        /// </summary>
        public List<string> CreateHighscoreList()
        {
            //Create a tmp list.
            List<string> highscoreList = new List<string>();

            //Create a connection.
            SQLiteConnection connection = new SQLiteConnection(LoadSQLiteConnectionString());
            SQLiteCommand command;

            using (connection)
            {
                //Open connection.
                connection.Open();

                //Selec all ID's from the Highscore table. Basically all the saved scores in the database.
                //Then sort the ID's from the highest score to lowest score.
                command = new SQLiteCommand($"SELECT ID FROM Highscore ORDER BY Score DESC", connection);

                //Execute a reader command. This allows us to return multiple values from a selec statement.
                SQLiteDataReader reader = command.ExecuteReader();

                // For every value read while reader is reading, convert to int32 and add it to the return list.
                // Basically every time the reader finds a value that fits the command statement, add it to the tmp list.
                while (reader.Read())
                {
                    // Gets the read ID from the table.
                    int tmpID = reader.GetInt32(0);

                    // Gets the name from the table at the given ID.
                    string tmpName = SelectStringValuesWhere("Name", "Highscore", $"ID={tmpID}", new SQLiteConnection(LoadSQLiteConnectionString()));

                    // Gets the score from the table at the given ID.
                    int tmpScore = SelectIntValuesWhere("Score", "Highscore", $"ID={tmpID}", new SQLiteConnection(LoadSQLiteConnectionString()));

                    // Adds the and score corresponding to the ID to the list of highscores.
                    highscoreList.Add($"{tmpName}: {tmpScore}");
                }
            }

            //Return the list once no more values can be read.
            return highscoreList;
        }


        #region Misc. command methods.

        /// <summary>
        /// Makes sure we connect to the database.
        /// </summary>
        public string LoadSQLiteConnectionString()
        {
            return ConfigurationManager.AppSettings["NetvaerkDB"];
        } 


        /// <summary>
        /// For commands that don't need to return anything, fx INSERT.
        /// </summary>
        /// <param name="command">The command you want to peform.</param>
        /// <param name="connection">The connection string used to connect to the database</param>
        private void ExecuteNonQuerySQLiteCommand(SQLiteCommand command, SQLiteConnection connection)
        {
            using (connection)
            {
                //Open connection and execute command.
                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Creates a table in the database.
        /// </summary>
        /// <param name="tableName"> What we want to name the new table. </param>
        /// <param name="columns"> What columns we would like to add to the table. </param>
        /// <param name="connection"> The connection to the database.</param>
        private void CreateTable(string tableName, string columns, SQLiteConnection connection)
        {
            ExecuteNonQuerySQLiteCommand(new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {tableName} ({columns})", connection), connection);
        }


        /// <summary>
        /// Inserts a new score into the highscore list.
        /// Needs both a name and a score.
        /// </summary>
        /// <param name="tableName"> The name of the table we wish to insert values into. </param>
        /// <param name="definedValues"> What values we would like to add into the table. </param>
        /// <param name="connection">The connection to the database.</param>
        public void InsertIntoTable(string tableName, string definedValues, SQLiteConnection connection)
        {
            ExecuteNonQuerySQLiteCommand(new SQLiteCommand($"INSERT INTO {tableName} VALUES ({definedValues});", connection), connection);
        }

        #endregion


        #region Methods for getting table values.

        /// <summary>
        /// Use this method when your command needs to return a string value, Fx with Select.
        /// </summary>
        /// <param name="command">The SQLite command.</param>
        /// <param name="connection">The connection to the database.</param>
        /// <returns>Returns the defined value.</returns>
        private string ExecuteScalarString(SQLiteCommand command, SQLiteConnection connection)
        {
            //Value to return.
            string value;

            using (connection)
            {
                //Open connection.
                connection.Open();

                //Convert the value output from the command to string.
                value = Convert.ToString(command.ExecuteScalar());
            }

            //Return the value.
            return value;
        }


        /// <summary>
        /// Use this method when your command needs to return an integer value, Fx with Select.
        /// </summary>
        /// <param name="command">The SQLite command.</param>
        /// <param name="connection">The connection to the database.</param>
        /// <returns>Returns the defined value.</returns>
        private int ExecuteScalarInt(SQLiteCommand command, SQLiteConnection connection)
        {
            //Value to return.
            int value;

            using (connection)
            {
                //Open connection.
                connection.Open();

                //Convert the value output from the command to Int32.
                value = Convert.ToInt32(command.ExecuteScalar());
            }

            return value;
        }


        /// <summary>
        /// This is used with ExecuteScalarString to select a specific value from a table,
        /// and then return that value as a string.
        /// </summary>
        /// <param name="selectDefinition">What to select.</param>
        /// <param name="tableName">What table to select from.</param>
        /// <param name="whereDefinition">When to select something. A select condition.</param>
        /// <param name="connection">The connection to the database.</param>
        /// <returns>Returns the defined value.</returns>
        private string SelectStringValuesWhere(string selectDefinition, string tableName, string whereDefinition,
                                              SQLiteConnection connection)
        {
            return ExecuteScalarString(new SQLiteCommand($"SELECT {selectDefinition} FROM {tableName} WHERE " +
                                                 $"{whereDefinition};", connection), connection);
        }


        /// <summary>
        /// This is used with ExecuteScalarInt to select a specific value from a table,
        /// and then return that value as an integer.
        /// </summary>
        /// <param name="selectDefinition">What to select.</param>
        /// <param name="tableName">What table to select from.</param>
        /// <param name="whereDefinition">When to select something. A select condition.</param>
        /// <param name="connection">The connection to the database.</param>
        /// <returns>Returns the defined value.</returns>
        private int SelectIntValuesWhere(string selectDefinition, string tableName, string whereDefinition,
                                       SQLiteConnection connection)
        {
            return ExecuteScalarInt(new SQLiteCommand($"SELECT {selectDefinition} FROM {tableName} WHERE " +
                                 $"{whereDefinition};", connection), connection);
        }

        #endregion
    }
}
