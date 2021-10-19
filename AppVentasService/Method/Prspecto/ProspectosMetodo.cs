using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using System.Configuration;


using WSprospectos.Models;
using Newtonsoft.Json;
using WSprospectos.Models.Prospecto;
using WSprospectos.ModelEntity.DBs;
using WSprospectos.Global;
namespace WSprospectos.Method.Prospectos

{
    internal static class ProspectosMetodo
    {
        public static string GlobalJobId { get; set; } = string.Empty;
        public static object Lista { get; private set; }

        static string ConnectionString = Configurations.ConnectionString;
        //Cambio de producto
        
 
        internal static string Prueba()
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                ModelEntity.DBs.Prospectos  NP = new ModelEntity.DBs.Prospectos();
                NP.Nombre = "Test";
                NP.PrimerApellido= "Test";
                NP.SegundoApellido = "Test";
                NP.Calle = "Test";
                NP.Numero = "Test";
                NP.Colonia = "Test";

                NP.CodigoPostal = 80000;
                NP.Telefono = "Test";
                NP.RFC = "Test";
                NP.Status = "Test";



                db.Prospectos.Add(NP);
                db.SaveChanges();



            List<PaspectoClass> ListaProspecto = new List<PaspectoClass>();

                ListaProspecto = (from p in db.Prospectos
                         select new PaspectoClass
                         {
                             id= p.id,
                             Nombre = p.Nombre,
                             PrimerApellido = p.PrimerApellido,
                             SegundoApellido=p.SegundoApellido,
                             Status=p.Status



                         }).ToList();
                return JsonConvert.SerializeObject(ListaProspecto);
            }
        }

        internal static bool Autenticar(string UserName, string Password)
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {

                List<UsersClass> Lista = new List<UsersClass>();

                //var Existe = (db.Users.Where(u => u.Usuario == UserName && u.Pass= Password).Select(u => u.Id).FirstOrDefault());
                var Existe = db.Users.Where(p => p.Usuario == UserName
                                       && p.Pass == Password).Select(u=>u.Id).FirstOrDefault();
                if (Existe == 0)
                    return (false);
                else
                    return (true);
            }
        }

        internal static string GetUserData(string Username, string token)
        {
            //SqlCommand Command = new SqlCommand(string.Format(@"Select 
            //                                    u.*,                                                                                               
            //                                    Token='" + token +
            //                                    @"' from Usuario  u
            //                                    where u.Nombre = '{0}' 
            //                                    For xml path('Data')", Username))

            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                List<UsersClass> ListUsuario = new List<UsersClass>();

                ListUsuario = (from u in db.Users.Where(x=>x.Usuario==Username)
                               select new UsersClass
                               {
                                   Usuario=u.Usuario,
                                   Pass=u.Pass,
                                   Token=token

                        

                                  }).ToList();
                return JsonConvert.SerializeObject(ListUsuario);
            }
        }
        internal static string Guardar(ProspectosClass Prospecto)
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                ModelEntity.DBs.Prospectos NP = new ModelEntity.DBs.Prospectos();
                NP.Nombre = Prospecto.Nombre;
                NP.PrimerApellido = Prospecto.PrimerApellido;
                NP.SegundoApellido = Prospecto.SegundoApellido;
                NP.Calle = Prospecto.Calle;
                NP.Numero = Prospecto.Numero;
                NP.Colonia = Prospecto.Colonia;

                NP.CodigoPostal = Prospecto.CodigoPostal;
                NP.Telefono = Prospecto.Telefono;
                NP.RFC = Prospecto.RFC;
                NP.Status = "Enviado";



                db.Prospectos.Add(NP);
                db.SaveChanges();



                List<PaspectoClass> ListaProspecto = new List<PaspectoClass>();

                ListaProspecto = (from p in db.Prospectos
                                  select new PaspectoClass
                                  {
                                      id = p.id,
                                      Nombre = p.Nombre,
                                      PrimerApellido = p.PrimerApellido,
                                      SegundoApellido = p.SegundoApellido,
                                      Status = p.Status



                                  }).ToList();
                return JsonConvert.SerializeObject(ListaProspecto);
            }
        }

        internal static string GetProspectos()
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                List<PaspectoClass> ListaProspecto = new List<PaspectoClass>();

                ListaProspecto = (from p in db.Prospectos.Where(x=>x.Status=="Enviado")
                                  select new PaspectoClass
                                  {
                                      id = p.id,
                                      Nombre = p.Nombre,
                                      PrimerApellido = p.PrimerApellido,
                                      SegundoApellido = p.SegundoApellido,
                                      Status = p.Status



                                  }).ToList();
                return JsonConvert.SerializeObject(ListaProspecto);
            }
        }

        internal static string GetProspectoTodoss()
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                List<PaspectoClass> ListaProspecto = new List<PaspectoClass>();

                ListaProspecto = (from p in db.Prospectos
                                  select new PaspectoClass
                                  {
                                      id = p.id,
                                      Nombre = p.Nombre,
                                      PrimerApellido = p.PrimerApellido,
                                      SegundoApellido = p.SegundoApellido,
                                      Status = p.Status



                                  }).ToList();
                return JsonConvert.SerializeObject(ListaProspecto);
            }
        }

        internal static string AutorizaRechaza(ProspectosClass Prospecto)
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                ModelEntity.DBs.Prospectos NP = new ModelEntity.DBs.Prospectos();

   
                var result = db.Prospectos.SingleOrDefault(p => p.id == Prospecto.id);
                if (result != null)
                {
                    result.Status = Prospecto.Status;
                    result.Comentarios = Prospecto.Comentarios;
                    db.SaveChanges();
                }

                // db.Prospectos.Add(NP);
                db.SaveChanges();

               return JsonConvert.SerializeObject(result);
            }
        }

        internal static string GuardarNombreArchivo(string NombreArchivo, int IdProspecto)
        {
            using (var db = new ModelEntity.DBs.DBProEntitie())
            {
                ModelEntity.DBs.Prospectos NP = new ModelEntity.DBs.Prospectos();


                var result = db.Prospectos.SingleOrDefault(p => p.id == IdProspecto);
                if (result != null)
                {
                    result.NombreArchivo = NombreArchivo;
               
                    db.SaveChanges();
                }

                // db.Prospectos.Add(NP);
                db.SaveChanges();




                return JsonConvert.SerializeObject(result);
            }
        }

        //return Tools.CommandToJson(Command);


    }
}