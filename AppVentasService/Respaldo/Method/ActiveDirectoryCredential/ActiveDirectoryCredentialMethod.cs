using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TripleH.Erp.Model;
using AppBodegaService.Method;
using System.Data;

namespace AppBodegaService.Method.ActiveDirectoryCredential
{
    internal class ActiveDirectoryCredentialMethod
    {
        internal static bool UserLogin(string UserName, string Password)
        {
            var usuario = Usuario.Logon(UserName, Password);
            if (usuario != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static string GetUserData(string Username){
            SqlCommand Command = new SqlCommand(string.Format(@"Select * from Usuario where Nombre = '{0}'
                                                                For xml path('Data'),root('Root')", Username));
            //DataTable Usuario = Tools.CommandTextToDataTable(Command);
            //string Resultado = Tools.DataTableToJsonString(Usuario);

            return Tools.CommandToJson(Command);
        }

    
    }
}