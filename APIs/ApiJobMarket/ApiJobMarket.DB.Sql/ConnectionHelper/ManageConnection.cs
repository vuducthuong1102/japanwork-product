using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ApiJobMarket.ShareLibs.Exceptions;

namespace ApiJobMarket.DB.Sql
{
    public class ManageConnection : IDisposable
    {
        protected ConnectionStringOptions connectionStringOptions = null;
        protected string connectionString = null;
        protected SqlConnection sqlConnection = null;

        #region Initialization

        public ManageConnection(string connectionString)
        {
            SetConnectionString(connectionString);
        }

        public ManageConnection(ConnectionStringOptions options)
        {
            SetConnectionString(options);
        }

        public void SetConnectionString(ConnectionStringOptions options)
        {
            this.connectionStringOptions = options;
            //this.connectionString = options.ToString();
            this.connectionString = options.CreateConnectionString();

            this.SetConnectionString(this.connectionString);
        }

        public void SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            this.sqlConnection = new SqlConnection(connectionString);
        }


        public ConnectionStringOptions GetConnectionOptions()
        {
            return this.connectionStringOptions;
        }

        public string GetConnectionString()
        {
            return this.connectionString;
        }

        #endregion

        /// <summary>
        /// Validates that the connection is opened and kept open
        /// </summary>
        public bool OpenConnection()
        {
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
            }
            catch (SqlException ex)
            {
                var strError = "Failed to open connection:  " + ex.Message;                
                CloseConnection();
                throw new CustomSQLException(strError);
            }
            finally
            {

            }

            return sqlConnection != null && sqlConnection.State == ConnectionState.Open;
        }


        /// <summary>
        /// Closes the connection that will generate.
        /// </summary>
        public void CloseConnection()
        {
            ReleaseManagedConnection();
        }


        /// <summary>
        /// Method used to execute SQL statements that are about to enter, modify or delete
        /// </summary>
        /// <param name="sqlStatement">Entire SQL statement to be executed</param>
        /// <param name="parameters">Number of parameters needed to use</param>
        /// <returns>Returns true if no exception does not occur</returns>
        public bool ExecuteNonQuery(string sqlStatement, CommandType cmdType, params SqlParameter[] parameters)
        {
            bool accept = true;

            try
            {
                if (!OpenConnection()) return false;

                SqlCommand sqlComando = new SqlCommand();
                sqlComando.CommandText = sqlStatement;
                sqlComando.CommandType = cmdType;
                sqlComando.Connection = sqlConnection;

                AttachParameters(sqlComando, parameters);

                sqlComando.ExecuteNonQuery();
                sqlComando.Dispose();
                accept = true;
            }
            catch (SqlException e)
            {
                accept = false;

                var strError = "Failed to ExecuteNonQuery: " + e.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                CloseConnection();
            }

            return accept;
        }


        /// <summary>
        /// Main method to access the database returns a reader using a stored procedure
        /// </summary>
        /// <param name="sqlStatement">Entire SQL statement to be executed</param>
        /// <param name="parameters">Number of parameters needed to use</param>
        /// <param name="parseReaderFunc">The function to extract data reader to object</param>
        /// <returns>Returns reader data if no exception does not occur, otherwise return null</returns>
        public object ExecuteReader(string sqlStatement, CommandType cmdType,
            Func<SqlDataReader, object> parseReaderFunc,
            params SqlParameter[] parameters
            )
        {
            object readerResult = null;
            try
            {
                if (!OpenConnection()) return null;
                SqlCommand sqlComando = new SqlCommand();
                sqlComando.CommandText = sqlStatement;
                sqlComando.CommandType = cmdType;
                sqlComando.Connection = sqlConnection;

                AttachParameters(sqlComando, parameters);

                using (var reader = sqlComando.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (parseReaderFunc != null)
                        readerResult = parseReaderFunc(reader);
                }

                sqlComando.Dispose();
            }
            catch (SqlException e)
            {
                var strError = "Failed to ExecuteReader: " + e.Message;
                readerResult = null;
                throw new CustomSQLException(strError);
            }
            finally
            {
                CloseConnection();
            }
            return readerResult;
        }




        public object ExecuteScalar(string sqlStatement, CommandType cmdType, params SqlParameter[] parameters)
        {
            object scalarResult = null;
            try
            {
                if (!OpenConnection()) return null;

                SqlCommand sqlComando = new SqlCommand();
                sqlComando.CommandText = sqlStatement;
                sqlComando.CommandType = cmdType;
                sqlComando.Connection = sqlConnection;

                AttachParameters(sqlComando, parameters);

                scalarResult = sqlComando.ExecuteScalar();
                sqlComando.Dispose();
            }
            catch (SqlException e)
            {
                var strError = "Failed to ExecuteScalar: " + e.Message;
                scalarResult = null;
                throw new CustomSQLException(strError);               
            }
            finally
            {
                CloseConnection();
            }
            return scalarResult;
        }


        /// <summary>
        /// Method takes a SQL statement and executes it so you can bring a dataset table I use it to put it on the grid or in combos     
        /// </summary>
        /// <param name="sqlStatement">Entire SQL statement to be executed</param>
        /// <param name="parameters">Number of parameters needed to use</param>
        /// <param name="parseReaderFunc">The function to extract data reader to object</param>
        /// <returns>Returns dataset if no exception does not occur, otherwise return null</returns>        
        public DataSet ExecuteDataSet(string sqlStatement, CommandType cmdType, params SqlParameter[] parameters)
        {
            DataSet miDataSet = new DataSet();
            bool ban = false;
            try
            {
                if (!OpenConnection()) return null;

                SqlCommand sqlComando = new SqlCommand();
                sqlComando.CommandText = sqlStatement;
                sqlComando.CommandType = cmdType;
                sqlComando.Connection = sqlConnection;

                // Assign the provided values to these parameters based on parameter order
                //AssignParameterValues(parameters, parameters);
                AttachParameters(sqlComando, parameters);

                SqlDataAdapter myAdap = new SqlDataAdapter(sqlComando);
                myAdap.Fill(miDataSet);
                sqlComando.Dispose();
                ban = true;
            }
            catch (SqlException e)
            {
                var strError = "Failed to ExecuteDataSet: " + e.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                CloseConnection();
            }

            if (ban)
                return miDataSet;
            else
                return null;
        }


        public DataTable ExecuteDataTable(string sqlStatement, CommandType cmdType, params SqlParameter[] parameters)
        {
            var dataSet = ExecuteDataSet(sqlStatement, cmdType, parameters);
            if (dataSet != null && dataSet.Tables.Count > 0)
                return dataSet.Tables[0];
            else
                return null;
        }




        /// <summary>
        /// execute a trascation include one or more sql sentence(author:donne yin)
        /// </summary>        
        /// <returns>execute trascation result(success: true | fail: false)</returns>
        public bool ExecuteTransaction(string[] cmdTexts, CommandType cmdType, params SqlParameter[] parameters)
        {

            if (!OpenConnection()) return false;

            //Begin transaction
            SqlTransaction myTrans = sqlConnection.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.Transaction = myTrans;
            cmd.CommandType = cmdType;

            try
            {
                for (int i = 0; i < cmdTexts.Length; i++)
                {
                    //Attach new command text
                    cmd.CommandText = cmdTexts[i];

                    //Attach new parameters
                    AttachParameters(cmd, parameters);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                //Commit transaction
                myTrans.Commit();
            }
            catch (SqlException e)
            {
                var strError = "Failed to ExecuteTransaction: " + e.Message;
                //Rollback transaction
                myTrans.Rollback();

                throw new CustomSQLException(strError);
            }
            finally
            {
                CloseConnection();
            }
            return true;
        }


        #region Parameter Helpers


        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }




        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="ParamName">parameter name</param>
        /// <param name="DbType">data type</param>
        /// <param name="Size">type size</param>
        /// <param name="Direction">input or output</param>
        /// <param name="Value">set the value</param>
        /// <returns>Return parameters that has been assigned</returns>
        public static SqlParameter CreateParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;


            if (Size > 0)
            {
                param = new SqlParameter(ParamName, DbType, Size);
            }
            else
            {

                param = new SqlParameter(ParamName, DbType);
            }


            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
            {
                param.Value = Value;
            }


            return param;
        }



        /// <summary>
        /// set Input parameters
        /// </summary>
        /// <param name="ParamName">parameter names, such as:@ id </param>
        /// <param name="DbType">parameter types, such as: SqlDbType.Int</param>
        /// <param name="Size">size parameters, such as: the length of character type for the 100</param>
        /// <param name="Value">parameter value to be assigned</param>
        /// <returns>Parameters</returns>
        public static SqlParameter CreateInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// Output parameters 
        /// </summary>
        /// <param name="ParamName">parameter names, such as:@ id</param>
        /// <param name="DbType">parameter types, such as: SqlDbType.Int</param>
        /// <param name="Size">size parameters, such as: the length of character type for the 100</param>
        /// <param name="Value">parameter value to be assigned</param>
        /// <returns>Parameters</returns>
        public static SqlParameter CreateOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Set return parameter value 
        /// </summary>
        /// <param name="ParamName">parameter names, such as:@ id</param>
        /// <param name="DbType">parameter types, such as: SqlDbType.Int</param>
        /// <param name="Size">size parameters, such as: the length of character type for the 100</param>
        /// <param name="Value">parameter value to be assigned<</param>
        /// <returns>Parameters</returns>
        public static SqlParameter CreateReturnParam(string ParamName, SqlDbType DbType, int Size)
        {
            return CreateParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }

        public static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }


        #endregion

        #region IDisposable Interface

        private void ReleaseManagedConnection()
        {
            Console.WriteLine("Releasing Managed Connection");
            if (sqlConnection != null)
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();

                sqlConnection.Dispose();
            }
        }

        void ReleaseUnmangedConnection()
        {
            Console.WriteLine("Releasing Unmanaged Connection");
        }



        public void Dispose()
        {
            Console.WriteLine("Dispose called from outside");
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }



        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine("Actual Dispose called with a " + disposing.ToString());
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
                ReleaseManagedConnection();
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            ReleaseUnmangedConnection();
        }



        ~ManageConnection()
        {
            Console.WriteLine("Finalizer called");
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion
    }





    /// <summary>
    /// Helper for generating connection string
    /// </summary>
    public class ConnectionStringOptions
    {
        /// <summary>
        /// When true, multiple SQL statements can be sent with one command execution. 
        /// Note: starting with Sql 4.1.1, batch statements should be separated by the server-defined separator character. 
        /// Statements sent to earlier versions of Sql should be separated by ';'
        /// </summary>
        public bool allowBatch = true;


        /// <summary>
        /// Setting this to true indicates that the provider expects user variables in the SQL. 
        /// This option was added in Connector/Net version 5.2.2.
        /// </summary>
        public bool allowUserVariables = true;


        /// <summary>
        /// If set to True, SqlDataReader.GetValue() returns a SqlDateTime object for date or datetime columns that have disallowed values,
        /// such as zero datetime values, and a System.DateTime object for valid values. 
        /// If set to False (the default setting) it causes a System.DateTime object to be returned for all valid values 
        /// and an exception to be thrown for disallowed values, such as zero datetime values.
        /// </summary>
        public bool allowZeroDatetime = false;


        /// <summary>
        /// Setting this option to true enables compression of packets exchanged between the client and the server. 
        /// This exchange is defined by the Sql client/server protocol. 
        /// Compression is used if both client and server support ZLIB compression, and the client has requested compression using this option.
        /// A compressed packet header is: packet length (3 bytes), packet number (1 byte), and Uncompressed Packet Length (3 bytes). 
        /// The Uncompressed Packet Length is the number of bytes in the original, uncompressed packet. 
        /// If this is zero, the data in this packet has not been compressed. 
        /// When the compression protocol is in use, either the client or the server may compress packets. 
        /// However, compression will not occur if the compressed length is greater than the original length. 
        /// Thus, some packets will contain compressed data while other packets will not.
        /// </summary>
        public bool compress = false;


        public int connectionAttemps = 3;

        public int connectionSleep = 50;



        /// <summary>
        /// True to have SqlDataReader.GetValue() and SqlDataReader.GetDateTime() return DateTime.MinValue for date or datetime columns
        /// that have disallowed values.
        /// </summary>
        public bool convertZeroDateTime = true;







        /// <summary>
        /// The name or network address of the instance of Sql to which to connect. 
        /// Multiple hosts can be specified separated by commas. 
        /// This can be useful where multiple Sql servers are configured for replication and you are not concerned about the precise server 
        /// you are connecting to. No attempt is made by the provider to synchronize writes to the database, so take care when using this option. 
        /// In Unix environment with Mono, this can be a fully qualified path to a Sql socket file.
        /// With this configuration, the Unix socket is used instead of the TCP/IP socket. 
        /// Currently, only a single socket name can be given, so accessing Sql in a replicated environment using Unix sockets is not currently supported.
        /// </summary>
        public string Server;


        /// <summary>
        /// The port Sql is using to listen for connections. This value is ignored if Unix socket is used.
        /// </summary>
        public uint Port = 1433;


        /// <summary>
        /// Name of the database
        /// </summary>
        public string Database;

        /// <summary>
        /// The Sql login account being used.
        /// </summary>
        public string Username;

        /// <summary>
        /// The password for the Sql account being used.
        /// </summary>
        public string Password;


        /// <summary>
        /// When true, the SqlConnection object is drawn from the appropriate pool, 
        /// or if necessary, is created and added to the appropriate pool. 
        /// Recognized values are true, false, yes, and no.
        /// </summary>
        public bool UsePooling = true;

        /// <summary>
        /// Gets or sets the minimum of connections that will be in the connection pool;
        /// </summary>
        public uint MinPoolCount = 0;

        /// <summary>
        /// Gets or sets the maximum of connections that wil be in the connection pool
        /// </summary>
        public uint MaxPoolCount = 100;

        /// <summary>
        /// The length of time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error.
        /// </summary>
        public uint ConnectionTimeout = 5000;


        /// <summary>
        /// Sets the default value of the command timeout to be used. 
        /// This does not supersede the individual command timeout property on an individual command object. 
        /// If you set the command timeout property, that will be used. This option was added in Connector/Net 5.1.4
        /// </summary>
        public uint DefaultCommandTimeout = 5000;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionStringOptions(string server, string uid, string pwd, uint port = 1433)
        {
            this.Server = server;
            this.Username = uid;
            this.Password = pwd;
            this.Port = port;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>Returns a valid connection string</returns>
        public override string ToString()
        {
            return
            "Server=" + Server +
            ";Port=" + Port.ToString() +
            ";Uid=" + Username.ToString() +
            ";Pwd=" + Password.ToString() +
            ";AllowUserVariables=" + allowUserVariables.ToString() +
            ";ConnectionTimeout=" + ConnectionTimeout.ToString() +
            ";DefaultCommandTimeout=" + DefaultCommandTimeout.ToString() +
            ";ConvertZeroDateTime=" + convertZeroDateTime.ToString() +
            ";Pooling=" + UsePooling.ToString() +
            ";Compress=" + compress.ToString() +
            ";AllowBatch=" + allowBatch.ToString() +
            ";AllowZeroDateTime=" + allowZeroDatetime.ToString();
        }


        public string CreateConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            //builder.ConnectionProtocol = Sql.Data.SqlClient.SqlConnectionProtocol.Tcp;
            //builder.Server = Server;
            //builder.Port = Port;
            //builder.UserID = Username;
            //builder.Password = Password;
            //builder.Database = Database;

            //builder.Pooling = UsePooling;
            //builder.MaximumPoolSize = MaxPoolCount;
            //builder.MinimumPoolSize = MinPoolCount;

            //builder.ConnectionTimeout = ConnectionTimeout;
            //builder.DefaultCommandTimeout = DefaultCommandTimeout;

            //var connectionString = builder.GetConnectionString(true);
            //return connectionString;

            return string.Empty;
        }

        #endregion Methods
    }


    public static class ManageConnectionExtensions
    {
        public static SqlParameter[] ToSqlParams(this Dictionary<string, object> paramDic)
        {
            SqlParameter[] paramArr = null;
            List<SqlParameter> paramList = new List<SqlParameter>();

            if (paramDic != null && paramDic.Count > 0)
            {
                foreach (var param in paramDic)
                {
                    var parameter = new SqlParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value;
                    paramList.Add(parameter);
                }
            }

            if (paramList.Count > 0)
            {
                paramArr = paramList.ToArray();
            }

            return paramArr;
        }
    }
}