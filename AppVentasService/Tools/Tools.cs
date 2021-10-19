using Agility.Sdk.Model.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using TotalAgility.Sdk;
using TripleH.Erp.Model;
using WSprospectos.Global;

namespace WSprospectos
{
    public class Tools
    {
        static string ConnectionString = Configurations.ConnectionString;

        internal static DataTable CommandTextToDataTable(SqlCommand Command)
        {
            SqlConnection cn = new SqlConnection(ConnectionString);
           
            Command.Connection = cn;
            Command.CommandTimeout = 0;
            SqlDataAdapter adp = new SqlDataAdapter(Command);
            DataTable Resultado = new DataTable();
            adp.Fill(Resultado);
            return Resultado;
        }
        internal static string CommandTextToJsonString(string CommandString)
        {
            SqlCommand Command = new SqlCommand();
            Command.CommandText = CommandString;
            SqlConnection cn = new SqlConnection(ConnectionString);
            Command.CommandType = CommandType.Text;
            Command.Connection = cn;
            SqlDataAdapter adp = new SqlDataAdapter(Command);
            DataTable Resultado = new DataTable();
            adp.Fill(Resultado);

            return Tools.ObjectToJsonString(Resultado);
        }
        internal static string DataTableToJsonString(DataTable Table)
        {
            return JsonConvert.SerializeObject(Table);
        }
        internal static string CommandToJson(SqlCommand Command)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                XDocument XMLDocument = new XDocument();
                Command.Connection = Connection;
                Connection.Open();

                XmlReader reader = Command.ExecuteXmlReader();

                while (reader.Read())
                {
                    XMLDocument = XDocument.Load(reader);
                }
                Connection.Close();
                string Resultado = JsonConvert.SerializeXNode(XMLDocument, Newtonsoft.Json.Formatting.None, true);
                if (string.IsNullOrEmpty(Resultado))
                {
                    Resultado = "[]";
                }
                return Resultado;
            }
        }
        internal static void SetResponseContent(ref HttpResponseMessage Response, string JsonString)
        {
            Response.Content = new StringContent(
                JsonString,
                Encoding.UTF8,
                "application/json"
            );
        }
        internal static void ThrowException(String Message)
        {
            throw new Exception(Message);
        }
        internal static string ObjectToJsonString(Object Object)
        {
            string JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(Object);
            return JsonString;
        }
        internal static string StoredProcedureToJsonString(string CommandString, string Params)
        {

            SqlCommand Command = new SqlCommand();
            Command.CommandText = CommandString;
            Command.CommandType = CommandType.StoredProcedure;
            Command.Connection = new SqlConnection(ConnectionString);

            SqlCommand ParametersInStoredProcedure = new SqlCommand(string.Format(@"
                                                Select PARAMETER_NAME,DATA_TYPE from information_schema.parameters
		                                                where specific_name='{0}'", Command.CommandText));

            DataTable Parameters = Tools.CommandTextToDataTable(ParametersInStoredProcedure);

            if (Parameters.Rows.Count > 0)
            {
                string[] ParmasArray = Params.Split(',');
                int counter = 0;
                foreach (DataRow DataRow in Parameters.Rows)
                {
                    //if (ParmasArray.ElementAtOrDefault(counter) == null)
                    //    continue;
                    string Param = DataRow["PARAMETER_NAME"].ToString();
                    //Command.Parameters.AddWithValue(Param, ParmasArray[counter]);
                    if (ParmasArray.ElementAtOrDefault(counter) == "")
                        Command.Parameters.AddWithValue(Param, DBNull.Value);
                    else
                        Command.Parameters.AddWithValue(Param, ParmasArray.ElementAtOrDefault(counter));
                    counter++;
                }
            }

            DataTable ResultadoTable = Tools.CommandTextToDataTable(Command);
            return Tools.ObjectToJsonString(ResultadoTable);
        }

        internal static DataTable StoredProcedureToDataTable(string CommandString, string Params)
        {

            SqlCommand Command = new SqlCommand();
            Command.CommandText = CommandString;
            Command.CommandType = CommandType.StoredProcedure;
            Command.Connection = new SqlConnection(ConnectionString);

            SqlCommand ParametersInStoredProcedure = new SqlCommand(string.Format(@"
                                                Select PARAMETER_NAME,DATA_TYPE from information_schema.parameters
		                                                where specific_name='{0}'", Command.CommandText));

            DataTable Parameters = Tools.CommandTextToDataTable(ParametersInStoredProcedure);

            if (Parameters.Rows.Count > 0)
            {
                string[] ParmasArray = Params.Split(',');
                int counter = 0;
                foreach (DataRow DataRow in Parameters.Rows)
                {
                    //if (ParmasArray.ElementAtOrDefault(counter) == null)
                    //    continue;
                    string Param = DataRow["PARAMETER_NAME"].ToString();
                    //Command.Parameters.AddWithValue(Param, ParmasArray[counter]);
                    if (ParmasArray.ElementAtOrDefault(counter) == "")
                        Command.Parameters.AddWithValue(Param, DBNull.Value);
                    else
                        Command.Parameters.AddWithValue(Param, ParmasArray.ElementAtOrDefault(counter));
                    counter++;
                }
            }

            DataTable ResultadoTable = Tools.CommandTextToDataTable(Command);
            return ResultadoTable;
        }
        internal static string JoinParameters(params Object[] Params)
        {
            if (Params.Length == 0)
            {
                Tools.ThrowException("No se encontró ningún parametro");
            }
            return string.Join(",", Params);
        }

        internal static string GetSessionIdKofax(DBDataContext db)
        {
            ParametrosTotalAgility parametro = db.ParametrosTotalAgilities.FirstOrDefault();
            var SId = "";
            var db2 = new DBDataContext(Global.Configurations.ConnectionString);
            var sesionValida = false;
            var skf = db2.SesionKofaxes.FirstOrDefault();
            if (skf != null)
                SId = skf.SessionId;
            UserService usrSVC = new UserService();
            string session_id = "";
            bool tipoconexionKofax = parametro.TipoConexionKofax.Value;
            if (tipoconexionKofax)
            {
                session_id = parametro.SessionIdKofax;
                sesionValida = true;
            }
            else
            {
                UserIdentityWithPassword user = new UserIdentityWithPassword();
                Agility.Sdk.Model.Users.Session ses;
                try
                {
                    ses = usrSVC.ValidateSession(SId);
                    sesionValida = ses.IsValid;
                    if (sesionValida)
                        session_id = ses.SessionId;
                    else
                        session_id = SId;
                }
                catch (Exception ex) { sesionValida = false; }
                try
                {
                    if (!sesionValida)
                        usrSVC.LogOff(session_id);
                }
                catch (Exception e) { sesionValida = false; }
                if (!sesionValida)
                {
                    try
                    {
                        user.LogOnProtocol = 7;
                        user.Password = parametro.PasswordKofax;
                        user.UserId = parametro.UserNameKofax;
                        Agility.Sdk.Model.Users.Session2 sessionw2 = usrSVC.LogOnWithPassword2(user);
                        session_id = sessionw2.SessionId;


                        db2.SesionKofaxes.DeleteAllOnSubmit(db2.SesionKofaxes);
                        db2.SesionKofaxes.InsertOnSubmit(new SesionKofax { SessionId = session_id });
                        db2.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        TripleH.Erp.Model.Log log1 = new TripleH.Erp.Model.Log();
                        log1.MsgStatus = e.Message + e.StackTrace;
                        db.Logs.InsertOnSubmit(log1);
                        db.SubmitChanges();
                    }
                    sesionValida = true;
                }
            }
            return session_id;
        }

        internal static void logOfKofax(string session_id)
        {
            UserService usrSVC = new UserService();
            usrSVC.LogOff(session_id);
        }
    }
}