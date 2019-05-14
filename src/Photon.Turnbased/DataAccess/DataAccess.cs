using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Configuration;

namespace SQLite_DataAccess
{
    //////SQLite Edition
    class DataAccess
    {

        // Connection String for  SQlite Edition
        //static string _ConnectionString = @"Data Source=SqliteDBtest;Version=3;New=False;Compress=True";
        //Data Source=DemoT.db;Version=3;New=False;Compress=True;

        // Use for ..exe.config file 
        public static string _ConnectionString = ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString;

        public static SQLiteConnection _Connection = null;
        public static SQLiteConnection SQLiteConnection;
        /*{
            get
            {
                if (_Connection == null)
                {
                    _Connection = new SQLiteConnection(_ConnectionString);
                    _Connection.Open();

                    return _Connection;
                }
                else if (_Connection.State != System.Data.ConnectionState.Open)
                {
                    _Connection.Open();

                    return _Connection;
                }
                else
                {
                    return _Connection;
                }
            }
        }*/
        public DataAccess()
        {
            _Connection = new SQLiteConnection(_ConnectionString);
            _Connection.Open();
            SQLiteConnection = _Connection;
        }
        public static DataSet GetDataSet(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, SQLiteConnection);
            SQLiteDataAdapter adp = new SQLiteDataAdapter(cmd);

            DataSet ds = new DataSet();
            adp.Fill(ds);
            SQLiteConnection.Close();

            return ds;
        }

        public static DataTable GetDataTable(string sql)
        {
            Console.WriteLine(sql);
            DataSet ds = GetDataSet(sql);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        public static int ExecuteSQL(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, SQLiteConnection);
            return cmd.ExecuteNonQuery();
        }
    }

}
