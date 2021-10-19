using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AppBodegaService.Method;
using TripleH.Erp.Model;
using Newtonsoft.Json;
using AppBodegaService.Models.Ajutes;
using Agility.Sdk.Model.Jobs;
using Agility.Sdk.Model.Processes;
using Agility.Sdk.Model.Users;
using Agility.Sdk.Model.Variables;
using Agility.Sdk.Model.Activities;
using TotalAgility.Sdk;
using AppBodegasService.Global;


namespace AppBodegaService
{
    public class DefinirAfectacionMethod
    {
        static string ConnectionString = Configurations.ConnectionString;
        internal static string GetAjustesXdefinir(int CodigoBodega, int CodigoUsuario)
        {

            SqlCommand Command = new SqlCommand(string.Format(@"exec paGetAjustesXdefinir {0},{1}", CodigoBodega, CodigoUsuario));

            DataTable DefinirAfectacionTable = Tools.CommandTextToDataTable(Command);

            //string Parameters = Tools.JoinParameters(
            //    CodigoBodega,
            //    CodigoUsuario);

            //string Resultado = Tools.StoredProcedureToJsonString("paGetAjustesXdefinir", Parameters);

            List<AjustesAutorizarClass> AjustesXdefinir = new List<AjustesAutorizarClass>();
            AjustesAutorizarClass Ajuste;

            DBDataContext db = new DBDataContext(ConnectionString);
            ParametrosTotalAgility parametro = db.ParametrosTotalAgilities.FirstOrDefault();
            var session_id = "";
            session_id = Tools.GetSessionIdKofax(db);

            ActivityService js = new ActivityService();

            foreach (var row in DefinirAfectacionTable.AsEnumerable())
            {
                Ajuste = new AjustesAutorizarClass();
                Ajuste.Codigo = Convert.ToInt32(row["Codigo"].ToString());
                Ajuste.Bodega = row["Bodega"].ToString();
                Ajuste.CodigoAjuste = Convert.ToInt32(row["CodigoAjuste"].ToString());
                Ajuste.TipoAjuste = row["TipoAjuste"].ToString();
                Ajuste.Observaciones = row["Observaciones"].ToString();
                Ajuste.Fecha = row["Fecha"].ToString();
                Ajuste.JobId = row["JobId"].ToString();
                Ajuste.CodigoTipoAjuste = Convert.ToInt32(row["codigoTipoAjuste"].ToString());

                JobIdentity JI = new JobIdentity
                {
                    Id = Ajuste.JobId
                };
            
                var actNaMe = js.GetActivitiesInJobWithStatus(session_id, JI, 0);
                if (actNaMe.Count > 0)
                {
                    Ajuste.ActivityName = actNaMe.First().NodeName;
                    Ajuste.NodeId = actNaMe.First().NodeId;
                }

                AjustesXdefinir.Add(Ajuste);
            }
            //usrSVC.LogOff(session_id);
            Tools.logOfKofax(session_id);

            return Tools.ObjectToJsonString(AjustesXdefinir);

        }

        internal static string GetDetalleAjusteXdefinir(int CodigoAjuste, string JobId)
        {

            SqlCommand Command = new SqlCommand(string.Format(@"SELECT CodigoCarga
            FROM [getDetallePalletAjustesBodegaAfectacionAgricultor] 
            WHERE CodigoAjusteBodega= {0} and JobId = '{1}' 
            group by CodigoCarga", CodigoAjuste, JobId));

            DataTable CargasTable = Tools.CommandTextToDataTable(Command);

            Command = new SqlCommand(string.Format(@"SELECT CodigoAgricultor,Agricutltor
            FROM [getDetallePalletAjustesBodegaAfectacionAgricultor] 
            WHERE CodigoAjusteBodega= {0} and JobId = '{1}' 
            group by  CodigoAgricultor,Agricutltor", CodigoAjuste, JobId));

            DataTable AgricultoresTable = Tools.CommandTextToDataTable(Command);

            Command = new SqlCommand(string.Format(@"SELECT [CodigoAjusteBodega],[Agricutltor],[CodigoCarga],[CodigoEntrega],[CodigoPallet],[CodigoProducto],
            [Producto],[Cantidad],[CodigoAgricultor],[TienePrecioPublicado],CodigoBodega 
            FROM [getDetallePalletAjustesBodegaAfectacionAgricultor] 
            WHERE CodigoAjusteBodega= {0} and JobId = '{1}'", CodigoAjuste, JobId));

            DataTable DEtalleAjusteTable = Tools.CommandTextToDataTable(Command);

            AjusteXdefinirClass ajuste = new AjusteXdefinirClass();
            List<AjustesDEtalleProductoPalletClass> detalleAjusteList;
            AjustesDEtalleProductoPalletClass detalleAjuste;
            List<AjustesDetalleCargaClass> detalleCargasList;
            AjustesDetalleCargaClass detalleCargas;
            List<AjustesDetalleAgricultorClass> detalleAgricultoresList;
            AjustesDetalleAgricultorClass detalleAgricultores;

            var ajusteGrouped = DEtalleAjusteTable.AsEnumerable().Select(x => new { CodigoAjuste = Convert.ToInt32(x["CodigoAjusteBodega"].ToString()), CodigoBodega = Convert.ToInt32(x["CodigoBodega"].ToString()) }).FirstOrDefault();

            ajuste.CodigoAjuste = ajusteGrouped.CodigoAjuste;
            ajuste.CodigoBodega = ajusteGrouped.CodigoBodega;

            detalleCargasList = new List<AjustesDetalleCargaClass>();
            foreach (var rowCarga in CargasTable.AsEnumerable())
            {
                detalleCargas = new AjustesDetalleCargaClass();
                detalleCargas.CodigoCarga = Convert.ToInt32(rowCarga["CodigoCarga"].ToString());

                detalleCargasList.Add(detalleCargas);

            }

            ajuste.DetalleCargas = detalleCargasList;

            detalleAgricultoresList = new List<AjustesDetalleAgricultorClass>();
            foreach (var rowAgricultor in AgricultoresTable.AsEnumerable())
            {
                detalleAgricultores = new AjustesDetalleAgricultorClass();
                detalleAgricultores.CodigoAgricultor = Convert.ToInt32(rowAgricultor["CodigoAgricultor"].ToString());
                detalleAgricultores.Agricultor = rowAgricultor["Agricutltor"].ToString();

                detalleAgricultoresList.Add(detalleAgricultores);

            }

            ajuste.DetalleAgricultores = detalleAgricultoresList;

            detalleAjusteList = new List<AjustesDEtalleProductoPalletClass>();
            foreach (var row in DEtalleAjusteTable.AsEnumerable())
            {
                detalleAjuste = new AjustesDEtalleProductoPalletClass();
                detalleAjuste.Seleccionado = false;
                detalleAjuste.CodigoAgricultor = Convert.ToInt32(row["CodigoAgricultor"].ToString());
                detalleAjuste.Agricultor = row["Agricutltor"].ToString();
                detalleAjuste.CodigoCarga = Convert.ToInt32(row["CodigoCarga"].ToString());
                detalleAjuste.CodigoPallet = row["CodigoPallet"].ToString();
                detalleAjuste.CodigoProducto = row["CodigoProducto"].ToString();
                detalleAjuste.Producto = row["Producto"].ToString();
                detalleAjuste.Cantidad = Convert.ToInt32(row["Cantidad"].ToString());
                detalleAjuste.CantidadAjustar = Convert.ToInt32(row["Cantidad"].ToString());
                detalleAjuste.CodigoEntrega = Convert.ToInt32(row["CodigoEntrega"].ToString());

                detalleAjusteList.Add(detalleAjuste);
            }

            ajuste.ProductoPallet = detalleAjusteList;

            return JsonConvert.SerializeObject(ajuste);

        }

        internal static void GuardaAfectacionAjuste(AjusteXdefinirClass AjusteXdefinir)
        {
            List<string> listaAjuste = new List<string>();

            var detalle = AjusteXdefinir.ProductoPallet.AsQueryable().Select(x => new { x.CodigoCarga, x.CodigoAgricultor, x.Agricultor }).Distinct().ToList();
            var detalleAjustes = "";
            var ajuste = "";
            foreach (var item in detalle)
            {
                var palletsAjuste = AjusteXdefinir.ProductoPallet.AsQueryable().Where(x => x.CodigoCarga == item.CodigoCarga && x.CodigoAgricultor == item.CodigoAgricultor).ToList();
                foreach (var row in palletsAjuste)
                {
                    if(string.IsNullOrEmpty(detalleAjustes))
                        detalleAjustes += row.CodigoEntrega + ",''" + row.CodigoProducto + "''," + row.CantidadAjustar;
                    else
                        detalleAjustes += "|"+row.CodigoEntrega + ",''" + row.CodigoProducto + "''," + row.CantidadAjustar;
                }

                SqlCommand Command = new SqlCommand(string.Format(@"exec InsertAjusteMermaAgricultorBodegaApp   '{0}',{1},{2},{3},{4},'{5}','{6}'", AjusteXdefinir.JobId, item.CodigoCarga, item.CodigoAgricultor, AjusteXdefinir.CodigoAjuste,
                AjusteXdefinir.CodUsuario, AjusteXdefinir.Observaciones, detalleAjustes));
                //ajuste = (string.Format(@"exec InsertAjusteMermaAgricultorBodegaApp   '{0}',{1},{2},{3},{4},'{5}','{6}'", AjusteXdefinir.JobId, item.CodigoCarga, item.CodigoAgricultor, AjusteXdefinir.CodigoAjuste,
                //     AjusteXdefinir.CodUsuario, AjusteXdefinir.Observaciones, detalleAjustes));

                //listaAjuste.Add(ajuste);

                detalleAjustes = "";
                ajuste = "";

                DataTable DefinirAfectacionTable = Tools.CommandTextToDataTable(Command);
            }

            var respuesta = AvanzarActividad(AjusteXdefinir.JobId, AjusteXdefinir.afectaAgricultor, AjusteXdefinir.Observaciones);

            //if(respuesta == 1)
            //{
            //    foreach(var item in listaAjuste)
            //    {
            //        SqlCommand Command = new SqlCommand(item);

            //        DataTable DefinirAfectacionTable = Tools.CommandTextToDataTable(Command);


            //    }
            //}
        }

        public static int AvanzarActividad(string JobId, bool afectaAgricutlor, string ObservacionesAjuste)
        {
            DBDataContext db = new DBDataContext(ConnectionString);
            ParametrosTotalAgility parametro = db.ParametrosTotalAgilities.FirstOrDefault();
            var session_id = "";
            session_id = Tools.GetSessionIdKofax(db);

            //Accedemos al servicio de Trabajos SOA
            ActivityService ja = new ActivityService();
            ProcessIdentity pi = new ProcessIdentity { Id = "726F7C3061224D4EB150519624F20193", Name = "AjustesEnBodega_App" };

            //Indicamos el nombre del Proceso y id, puedes obtenerlo mediante base de datos u buscarlo por medio del SDk
            //Instanciamos un objeto de coleccion de variables
            OutputVariableCollection ovc = new OutputVariableCollection();


            ovc.Add(new OutputVariable { Id = "AFECTANALAGRICULTOR", Value = afectaAgricutlor });
            ovc.Add(new OutputVariable { Id = "OBSERVACIONESAJUSTEAGRICULTOR", Value = ObservacionesAjuste });

            JobActivityIdentity JA = new JobActivityIdentity
            {
                NodeId = 10,
                EmbeddedProcessCount = 0,
                JobId = JobId,
                ActivityName = "Decidir Afectacion Agricultor"

            };

            //JA.NodeId = 33;
            //JA.EmbeddedProcessCount = 0;
            //JA.JobId = JobId;
            //JA.ActivityName = "Registrar Ajustes";

            //JobActivityOutput JAO = new JobActivityOutput();

            JobActivityOutput JAO = new JobActivityOutput
            {
                OutputVariables = ovc
            };

            //JAO.OutputVariables = ovc;

            ja.TakeActivity(session_id, JA);

            ja.CompleteActivity(session_id, JA, JAO);
            Tools.logOfKofax(session_id);
            //usrSVC.LogOff(session_id);
            return 1; //Creamos un JOb del Proceso y los prarmetros necesarios 
        }        
    }
}