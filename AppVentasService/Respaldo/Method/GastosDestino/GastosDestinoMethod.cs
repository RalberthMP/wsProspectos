using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using AppBodegaService.Models.TiposdeAjustes;
using AppBodegaService.Method;
using TripleH.Erp.Model;
using Newtonsoft.Json;
using Agility.Sdk.Model.Jobs;
using Agility.Sdk.Model.Processes;
using Agility.Sdk.Model.Users;
using Agility.Sdk.Model.Variables;
using TotalAgility.Sdk;
using AppBodegaService.Models.GastosDestino;
using AppBodegasService.Global;

namespace AppBodegaService.Method.GastosDestino
{
    public class GastosDestinoMethod
    {
        static string ConnectionString = Configurations.ConnectionString;
        internal static string GetBodegas()
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(@"select Codigo, NombreBodega as Nombre from Bodega  ");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetTipoGastos()
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(@"select Codigo as CodigoTipoGasto, Descripcion as Gasto from TipoGasto");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetMonedas()
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(@"select CurrCode as CodigoMoneda, CurrName as Moneda from Moneda");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetClientes(int CodigoBodega)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(string.Format(@"SELECT [CodigoCliente] as Codigo,[CardName] as Nombre FROM [getClientesPorBodegaAgrupado] WHERE (( [CodigoBodega] = {0} )) ORDER BY [CardName]", CodigoBodega));

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetOrdenes(string CodigoCliente)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(string.Format(@"SELECT [CodigoPromesa] as CodigoOrden,[FolioPromesa] FROM [getClientesPorBodega] WHERE (( [CodigoCliente] = '{0}')) ORDER BY [FolioPromesa] asc", CodigoCliente));

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }


        internal static string GetPickups(int CodigoOrden)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(string.Format(@"SELECT [Codigo],[Folio] FROM [PickupOrder] WHERE [CodigoPromesaVenta] = {0} AND [FechaHoraCancelacion] IS NULL", CodigoOrden));

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetGastos(int CodigoPickup)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(string.Format(@"select g.Codigo as CodigoGasto, g.CodigoTipoGasto, tp.Descripcion as Gasto , MontoEstimado as Importe, g.CodigoMoneda
                                                                from GastoPorVenta g
                                                                inner join TipoGasto tp on tp.Codigo = g.CodigoTipoGasto
                                                                where g.CodigoPickup = {0}", CodigoPickup));

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GuardaGastos(GastosPickup GastosPickup)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            var codigoPickup = GastosPickup.CodigoPickup;
            var usuario = GastosPickup.Usuario;
            var Gastos = GastosPickup.Gastos;
            var GastosEliminados = GastosPickup.GastosEliminados;
            var mensaje = "OK";
            try 
            { 
                string gastostring = "";
                if (Gastos.Count() > 0)
                {
                    foreach (var gasto in Gastos)
                    {
                        if (string.IsNullOrEmpty(gastostring))
                            gastostring += gasto.CodigoGasto.ToString() + "," + gasto.CodigoTipoGasto.ToString() + "," + gasto.Gasto + "," + gasto.Importe.ToString() + "," + gasto.CodigoMoneda;
                        else
                            gastostring += "|" + gasto.CodigoGasto.ToString() + "," + gasto.CodigoTipoGasto.ToString() + "," + gasto.Gasto + "," + gasto.Importe.ToString() + "," + gasto.CodigoMoneda;
                    }

                    SqlCommand Command = new SqlCommand(string.Format(@"exec paGuardaGastosPickup {0},'{1}',{2}", codigoPickup, gastostring, usuario));
                    DataTable Resultado = Tools.CommandTextToDataTable(Command);
                }

                var qwEliminar = "";
                if (GastosEliminados.Count() > 0) 
                {
                    foreach (var gasto in GastosEliminados)
                    {
                        qwEliminar += "delete from GastoPorVenta where Codigo = " + gasto.CodigoGasto.ToString();
                    }

                    SqlCommand CommandEliminar = new SqlCommand(string.Format(qwEliminar));
                    DataTable Resultado = Tools.CommandTextToDataTable(CommandEliminar);
                }

                if (GastosEliminados.Count() <= 0 && Gastos.Count() <= 0)
                {

                    SqlCommand CommandEliminar = new SqlCommand(string.Format(@"delete from GastoPorVenta where CodigoPickup = {0}", codigoPickup.ToString()));
                    DataTable Resultado = Tools.CommandTextToDataTable(CommandEliminar);

                }

                var qw = "";
                qw = "exec paCalculaGastosDestino " + codigoPickup.ToString();
                SqlCommand CommandCalcula = new SqlCommand(qw);
                DataTable ResultadoCalcula = Tools.CommandTextToDataTable(CommandCalcula);

            }
            catch (Exception e)
            {
                mensaje = e.Message + e.StackTrace;
            }

            return mensaje;
        }

    }
}