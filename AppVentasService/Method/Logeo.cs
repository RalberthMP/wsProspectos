using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WSprospectos.Models.Prospecto;
using WSprospectos.ModelEntity.DBs;
//using TripleH.Erp.Model;

namespace WSprospectos.Method
{
    public class Logeo
    {
        //internal static bool UserLogin(string UserName, string Password)
        //{
        //    //var usuario = new DBDataContext(Global.Configuration.ConnectionString).Usuarios.FirstOrDefault(x => x.Nombre == UserName);
        //    List<PaspectoClass> ListaProspecto = new List<PaspectoClass>();

        //    ListaProspecto = (from p in db.Prospectos
        //                      select new PaspectoClass
        //                      {
        //                          id = p.id,
        //                          Nombre = p.Nombre,
        //                          PrimerApellido = p.PrimerApellido,
        //                          SegundoApellido = p.SegundoApellido,
        //                          Status = p.Status



        //                      }).ToList();
        //    var usuario = Usuario.Logon(UserName, Password);
        //    if (usuario != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        internal static string Autenticar(string UserName, string Password)
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {




                List<UsersClass> Lista = new List<UsersClass>();

                var Lista2 =(db.Users.Where(u => u.Usuario == UserName).Select(u => u.Id).FirstOrDefault());

                //                  select new UsersClass
                //                  {
                //                      Id = u.Id,
                //                      Usuario = u.Usuario,
                //                      Pass = p.PrimerApellido




                //Lista2}).ToList();
                return (Lista2.ToString());
            }
        }

        internal static string GetUserData(string Username, string token)
        {
            SqlCommand Command = new SqlCommand(string.Format(@"Select 
                                                u.*,                                                                                               
                                                Token='" + token +
                                                @"' from Usuario  u
                                                where u.Nombre = '{0}' 
                                                For xml path('Data')", Username));
            return Tools.CommandToJson(Command);
        }
    }
}