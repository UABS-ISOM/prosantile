// ------------------------------------------------------------------------------------------------
//  <copyright file="NotImplemented.cs" company="Exit Games GmbH">
//    Copyright (c) Exit Games GmbH.  All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------------

namespace Photon.Webhooks.Turnbased.DataAccess
{
    using System.Collections.Generic;
    using System.Data;    
    using System.Data.SQLite;
    using System.Configuration;
    using log4net;
    using System;

    public class SQLite : IDataAccess
    {
        public static string _ConnectionString = ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString;
        public static SQLiteConnection _Connection = null;
        public static SQLiteConnection SQLiteConnection
        {
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
        }

        private static readonly ILog log = log4net.LogManager.GetLogger("MyLogger");

        public SQLite()
        {
            if (log.IsDebugEnabled) log.DebugFormat("Connecting SQL LITE");
            if (_Connection == null)
            {
                if (log.IsDebugEnabled) log.DebugFormat("Connection Was NULL");
                _Connection = new SQLiteConnection(_ConnectionString);
                _Connection.Open();                
            }
            else if (_Connection.State != System.Data.ConnectionState.Open)
            {
                if (log.IsDebugEnabled) log.DebugFormat("Connection WAS CLOSED");
                _Connection.Open();                
            }
            else
            {
                if (log.IsDebugEnabled) log.DebugFormat("Connection already established");                
            }
            if (log.IsDebugEnabled) log.DebugFormat("Function End");
        }

        public bool StateExists(string appId, string key)
        {
            if (log.IsDebugEnabled) log.DebugFormat("StateExists-SQLite, state {0}/{1} searching", appId, key);

            string sql = "SELECT state FROM game_state WHERE appId='"+appId+"' AND key='"+key+"'";
            ExecuteSQL(sql);
            DataTable dt = GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                if (log.IsDebugEnabled) log.DebugFormat("StateExists-SQLite, state {0}/{1} found", appId, key);
                return true;
            }
            else
            {
                if (log.IsDebugEnabled) log.DebugFormat("StateExists-SQLite, state {0}/{1} not found", appId, key);
                return false;

            }
            
        }

        public void StateSet(string appId, string key, string state)
        {
            if (log.IsDebugEnabled) log.DebugFormat("StateSet-SQLite, state {0}/{1} : state {2} setting ", appId, key,state);

            try
            {
                string sql = "INSERT INTO game_state VALUES('" + appId + "','" + key + "','" + state + "')";
                ExecuteSQL(sql);
                if (log.IsDebugEnabled) log.DebugFormat("StateSet-SQLite, state {0}/{1} : state {2} setting success ", appId, key, state);
            }
            catch (Exception e)
            {
                if (log.IsDebugEnabled) log.DebugFormat("StateSet-SQLite, state {0}/{1} : state {2} setting failure "+e.ToString(), appId, key, state);
            }

        }

        public string StateGet(string appId, string key)
        {

            if (log.IsDebugEnabled) log.DebugFormat("StateGet-SQLite, state {0}/{1} ", appId, key);
            string state = "Not Found";
            try
            {
                string sql = "SELECT state FROM game_state WHERE appId='" + appId + "' AND key='" + key + "'";                
                ExecuteSQL(sql);
                DataTable dt = GetDataTable(sql);
                if(dt.Rows.Count > 0)
                {
                    state = dt.Rows[0].Field<string>("state");
                    
                }
                if (log.IsDebugEnabled) log.DebugFormat("StateGet-SQLite, state {0}/{1} : state {2} get success ", appId, key, state);
                return state;
            }
            catch (Exception e)
            {
                if (log.IsDebugEnabled) log.DebugFormat("StateSet-SQLite, state {0}/{1} : state {2} setting failure " + e.ToString(), appId, key, state);
            }
            return state;
        }

        public void StateDelete(string appId, string key)
        {
            if (log.IsDebugEnabled) log.DebugFormat("StateDelete-SQLite, state {0}/{1} ", appId, key);            
            try
            {
                string sql = "DELETE FROM game_state WHERE appId='" + appId + "' AND key='" + key + "'";
                ExecuteSQL(sql);
                if (log.IsDebugEnabled) log.DebugFormat("StateGet-SQLite, state {0}/{1} : delete success ", appId, key);
            
            }
            catch (Exception e)
            {
                if (log.IsDebugEnabled) log.DebugFormat("StateSet-SQLite, state {0}/{1} : state delete failure " + e.ToString(), appId, key);
            }
            
        }

        public void GameInsert(string appId, string key, string gameId, int actorNr)
        {
            string sql = "INSERT INTO game VALUES('" + appId + "','" + key + "','" + gameId + "','" + actorNr + "')";
            ExecuteSQL(sql);
            
           // throw new System.NotImplementedException();
        }

        public void GameDelete(string appId, string key, string gameId)
        {
            string sql = "DELETE FROM game WHERE appId='" + appId + "' AND key='" + key + "'  AND gameId='" + gameId + "'";
            ExecuteSQL(sql);
        }

        public Dictionary<string, string> GameGetAll(string appId, string key)
        {
            var result = new Dictionary<string, string>();
            string sql = "SELECT gameId,actorNr FROM game WHERE appId='" + appId + "' AND key='" + key + "'";

            DataTable dt = GetDataTable(sql);
            DataRow dr = dt.Rows[0];

            result.Add(dr.Field<string>("gameId"), dr.Field<string>("actorNr"));

            return result;
            //throw new System.NotImplementedException();
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