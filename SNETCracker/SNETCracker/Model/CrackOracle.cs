﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using Tools;

namespace SNETCracker.Model
{
    class CrackOracle : CrackService
    {
        public CrackOracle()
        {

        }
        private String orclName = FileTool.readFileToString(AppDomain.CurrentDomain.BaseDirectory + "/config/oracle.txt");
        public override Server creack(String ip, int port, String username, String password, int timeOut)
        {

            OracleConnection conn = null;
            Server server = new Server();
            try
            {
                String connectionStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ip + ")(PORT=" + port + "))(CONNECT_DATA=(SERVICE_NAME=" + orclName + ")));User ID=" + username + ";PassWord=" + password + ";Connect Timeout=" + timeOut;
                conn = new OracleConnection(connectionStr);
                conn.Open();
                server.isSuccess = ConnectionState.Open.Equals(conn.State);
                if (server.isSuccess)
                {
                    server.banner = conn.ServerVersion;
                }
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("invalid username/password") == -1)
                {
                    throw e;
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return server;
        }

    }
}
