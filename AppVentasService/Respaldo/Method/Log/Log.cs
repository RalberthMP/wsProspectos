using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TripleH.Erp.Model;
using AppBodegasService.Global;
namespace AppBodegaService.Method
{
    public class Log
    {
        static string ConnectionString = Configurations.ConnectionString;
        public static void SaveLog(string text)
        {
            DBDataContext data = new DBDataContext(Configurations.ConnectionString);

            HerokuLog HerokuLogs = new HerokuLog();
            HerokuLogs.Mensaje = text;
            HerokuLogs.Fecha = DateTime.Now;
            data.HerokuLogs.InsertOnSubmit(HerokuLogs);
            
            data.SubmitChanges();
        }

        internal static void SaveJsonInLog(Object Entries)
        {
            string Json = Newtonsoft.Json.JsonConvert.SerializeObject(Entries);
            SaveLog(Json);
        }
    }
}