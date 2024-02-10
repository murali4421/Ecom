using System;
using System.Data;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using NpgsqlTypes;

namespace EcomCore.DB
{
    public class ECADB
    {
        private static NpgsqlConnection sqlCon;

        public ECADB()
        {
            sqlCon = new NpgsqlConnection();
        }

        public static NpgsqlConnection Connection()
        {
            if(sqlCon  == null)
            {
                sqlCon = new NpgsqlConnection();
                sqlCon.ConnectionString = "Server=localhost;port=5432;User ID=postgres;password=postgre;database=EC_DB;";
            }
            return sqlCon;
        }

        private static NpgsqlCommand SqlCommend()
        {
            try
            {
                ECADB.Connection();
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                NpgsqlCommand sqlCmd = new NpgsqlCommand();
                sqlCmd.Connection = sqlCon;
                sqlCmd.CommandType = CommandType.Text;

                return sqlCmd;
            }
            catch
            {
                return null;
            }
        }

        public static string Insert(string SqlQuery, byte[] bytes=null)
        {
            try
            {
                var sqlcmd = ECADB.SqlCommend();

                sqlcmd.CommandText = SqlQuery;
                if (bytes != null && bytes.Length > 0)
                {
                    NpgsqlParameter imageParameter = sqlcmd.Parameters.Add("@image", NpgsqlDbType.Bytea);
                    imageParameter.Value = bytes;
                    imageParameter.Size = bytes.Length;
                }
                sqlcmd.ExecuteNonQuery();
                return AppMessages.MSG_SAVE_SUCCESS;
            }
            catch
            {
                return AppMessages.MSG_SAVE_FAILED;
            }            
        }

        public static string Delete(string SqlQuery)
        {
            try
            {
                var sqlcmd = ECADB.SqlCommend();
                sqlcmd.CommandText = SqlQuery;                
                sqlcmd.ExecuteNonQuery();
                return AppMessages.MSG_DELETE_SUCCESS;
            }
            catch
            {
                return AppMessages.MSG_DELETE_FAILED;
            }
        }

        public static string Update(string SqlQuery, byte[] bytes = null)
        {
            try
            {
                var sqlcmd = ECADB.SqlCommend();

                sqlcmd.CommandText = SqlQuery;

                if(bytes !=null && bytes.Length > 0)
                {
                    NpgsqlParameter imageParameter = sqlcmd.Parameters.Add("@image", NpgsqlDbType.Bytea);
                    imageParameter.Value = bytes;
                    imageParameter.Size = bytes.Length;
                }                

                sqlcmd.ExecuteNonQuery();
                return AppMessages.MSG_UPDATE_SUCCESS;
            }
            catch
            {
                return AppMessages.MSG_UPDATE_FAILED;
            }
        }

        public static object GetRecords(string SqlQuery)
        {
            try
            {
                var sqlcmd = ECADB.SqlCommend();
                DataTable datas = new DataTable();

                sqlcmd.CommandText = SqlQuery;
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(sqlcmd);
                
                ad.Fill(datas);
                return datas;
            }
            catch
            {
                return new object();
            }
        }
    }
}
