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
using AppBodegaService.Models.Ajutes;
using AppBodegasService.Global;
//AppBodegaService.Method.ActiveDirectoryCredential
namespace AppBodegaService.Method.Ajustes
//namespace AppBodegaService
{
    internal static class AjusteMethod
    {
        public static string GlobalJobId { get; set; } = string.Empty;

        static string ConnectionString = Configurations.ConnectionString;
        //Cambio de producto
        static internal void GuardarAjustes(AjustesClass[] Ajuste)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            var qw = "";
            var qwDetalle = "";
            var qwCajas = "";
            var qwSPAutoriza = "";
            if (Ajuste[0].FolioCertificado == "")
            {
                Ajuste[0].FolioCertificado = "0";
            }

            if (Ajuste[0].NombreArchivo == "")
            {
                Ajuste[0].NombreArchivo = "0";
            }

            qw += " declare @JobId NVARCHAR(100) \n ";
            qw += " declare @CodAjuste int \n";
            qw += " set @JobId = (select NewId from getnewid) \n";
            qw += " insert into AjusteBodega values( " +
            Ajuste[0].CodigoBodega + "," +
            "1" + "," +
            "'AJUSTE CAMBIO DE PRODUCTO'" + "," +
            "0" + ",'" +
            null + "'," +
            Ajuste[0].CodUsuario + "," +
            "GETDATE()" + "," +
            "@JobId" + ",'" +
            Ajuste[0].Observaciones + "'," +
            "0" + ","
            + "0," +
            Ajuste[0].CodigoClasificacionAjuste + ",'" +
            Ajuste[0].FolioCertificado + "','" +
            Ajuste[0].NombreArchivo + "') \n";


            qw += " select @CodAjuste = IDENT_CURRENT( 'AjusteBodega' ) \n ";


            foreach (var a in Ajuste[0].ProductoPallet)
            {
                qwDetalle += " insert into AjusteBodegaDetalleCambioProducto values (@CodAjuste " + "," + a.CodigoEntrega + ",'" + a.CodigoProducto + "'," + a.CantidadAjustar + ",'" + a.CodigoProductoNvo + "'," + a.CodigoPallet + "," + a.CodigoAgricultor + "," + a.Cantidad + ") \n";
            }



            qwCajas += " if OBJECT_ID(N'tempdb.dbo.#Entregas') is null \n ";
            qwCajas += "  create table #Entregas( Codigo int identity(1,1),CodigoEntrega int, Cantidad int) \n ";
            qwCajas += "else \n ";
            qwCajas += "  truncate table #Entregas \n ";
            qwCajas += "insert into #Entregas \n ";
            qwCajas += "select t.CodigoEntrega,t.Cantidad \n ";
            qwCajas += "from AjusteBodegaDetalleCambioProducto t \n ";
            qwCajas += "inner join Entrega e on e.Codigo = t.CodigoEntrega \n ";
            qwCajas += "where CodigoAjusteBodega = @CodAjuste and t.Codigo not in            	 \n ";
            qwCajas += "(select CodigoDetalleCambioProducto from AjusteBodegaDetalleCambioProductoCaja where  CodigoAjusteBodega = @CodAjuste )          	 \n ";
            qwCajas += "declare @cont int, @maxcont int, @codigoEntrega int, @cantidad int \n ";
            qwCajas += "  set @cont = 1 \n ";
            qwCajas += "select @maxcont = max(Codigo) from #Entregas 	 \n ";
            qwCajas += "while @cont <= @maxcont \n ";
            qwCajas += "begin \n ";
            qwCajas += "  select @codigoEntrega = CodigoEntrega , @cantidad = Cantidad from #Entregas where Codigo=@cont                	 \n ";
            qwCajas += " insert into AjusteBodegaDetalleCambioProductoCaja \n ";
            qwCajas += "select top(@cantidad) t.Codigo,ddc.Codigo \n ";
            qwCajas += "from AjusteBodegaDetalleCambioProducto t \n ";
            qwCajas += "  inner join Entrega e on e.Codigo = t.CodigoEntrega \n ";
            qwCajas += "  inner join DespachoDetalleCaja ddc on ddc.CodigoEntrega = e.Codigo \n ";
            qwCajas += "  left join DespachoPickupDetalleCaja pdc on pdc.CodigoDespachoDetalleCaja = ddc.Codigo \n ";
            qwCajas += "  left join AjusteBodegaDetalleCambioProductoCaja tc on tc.CodigoDespachoDetalleCaja = ddc.Codigo \n ";
            qwCajas += "  where t.CodigoAjusteBodega = @CodAjuste  and t.CodigoEntrega = @codigoEntrega and pdc.Codigo is null and tc.Codigo is null \n ";
            qwCajas += "  and t.Codigo not in           	 \n ";
            qwCajas += "  (select CodigoDetalleCambioProducto from AjusteBodegaDetalleCambioProductoCaja where CodigoAjusteBodega = @CodAjuste )            	 \n ";
            qwCajas += "  set @cont = @cont + 1 \n ";
            qwCajas += "end \n ";

            //qwSPAutoriza += "autorizaAjustesBodega @JobId,@CodAjuste  ";



            qw += qwDetalle + qwCajas + qwSPAutoriza;

            SqlCommand Command = new SqlCommand(qw);

            SqlCommand Command2 = new SqlCommand(@"select max (codigo)  from AjusteBodega ");
            DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);
            var cd = JsonConvert.SerializeObject(Resultado2);

            List<DataRow> list = Resultado2.AsEnumerable().ToList();
            var cod = list[0].ItemArray[0];

            int CodigoAjuste = Convert.ToInt32(cod) + 1;
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            //string Job =
            CrearTrabajoAjusteBodega(CodigoAjuste, data);

            string Proced = "exec autorizaAjustesBodega @JobId = '" + GlobalJobId + "',@codigosAjustes = " + CodigoAjuste.ToString();
            Command2 = new SqlCommand(Proced);
            DataTable Resultado3 = Tools.CommandTextToDataTable(Command2);


        }

        static internal void GuardarAjustesMerma(AjustesClass[] Ajuste)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            var qw = "";
            var qwAjuste = "";
            var qwMerma = "";
            var qwDetalle = "";
            var qwPallet = "";
            var qwCajas = "";
            var qwSPAutoriza = "";

            var descripcion = "";
            if (Ajuste[0].CodigoTipoAjuste == 5)
            {
                descripcion = "AJUSTE DE MERMA";
            }
            if (Ajuste[0].CodigoTipoAjuste == 4)
            {
                descripcion = "AJUSTE DE PRODUCTO FALTANTE";
            }


            qwAjuste += " declare @JobId NVARCHAR(100) \n ";
            qwAjuste += " declare @CodAjuste int \n";
            qwAjuste += " declare @CodMermaBodega int \n";
            qwAjuste += " declare @CodMermaBodegaDetalle int \n";
            qwAjuste += " declare @CodMermaBodegaDetallePallet int \n";
            qwAjuste += " set @JobId = (select NewId from getnewid) \n";
            qwAjuste += " insert into AjusteBodega values( " +
            Ajuste[0].CodigoBodega + "," +
            Ajuste[0].CodigoTipoAjuste.ToString() + ",'" +
            descripcion + "'," +
            "1" + "," +
            "GETDATE()" + "," +
            Ajuste[0].CodUsuario + "," +
            "GETDATE()" + "," +
            "@JobId" + ",'" +
            Ajuste[0].Observaciones + "'," +
            "0" + "," +
            "0 ," +
            Ajuste[0].CodigoClasificacionAjuste + ",'" +
            Ajuste[0].FolioCertificado + "','" +
            Ajuste[0].NombreArchivo + "'" +

            ") \n";
            qwAjuste += " select @CodAjuste = IDENT_CURRENT( 'AjusteBodega' ) \n ";

            //INSERTA MERMA BODEGA
            qwMerma += " INSERT INTO [MermaBodega] (Fecha,CodigoBodega,CodigoAjusteBodega)  ";
            qwMerma += " values(GETDATE() " + "," + Ajuste[0].CodigoBodega + ",@CodAjuste) \n";
            qwMerma += " select @CodMermaBodega = IDENT_CURRENT( 'MermaBodega' ) \n ";

            qwMerma += "if OBJECT_ID(N'tempdb.dbo.#Entregas') is null \n ";
            qwMerma += "create table #Entregas( Codigo int identity(1,1),CodigoEntrega int, Cantidad int)        \n ";
            qwMerma += "  else  \n ";
            qwMerma += "  truncate table #Entregas  \n ";
            qwMerma += "  declare @cont int, @maxcont int, @codigoEntrega int, @cantidad int  \n ";



            //Codigo,Fecha,CodigoBodega,CodigoAjusteBodega

            foreach (var a in Ajuste[0].ProductoPallet)
            {
                //    qwDetalle = "";
                //    qwPallet = "";
                qwPallet += " INSERT INTO [MermaBodegaDetalle] (CodigoMermaBodega, CodigoCarga, CodigoProducto, Cantidad, CodigoAgricultor)  ";
                qwPallet += " values(@CodMermaBodega " + "," + a.CodigoCarga + ",'" + a.CodigoProducto + "'," + a.CantidadAjustar + "," + a.CodigoAgricultor + ") \n";
                qwPallet += " select @CodMermaBodegaDetalle = IDENT_CURRENT( 'MermaBodegaDetalle' ) \n ";
                qwPallet += " INSERT INTO [MermaBodegaDetallePallet] (CodigoMermaBodegaDetalle, CodigoEntrega, Cantidad)  ";
                qwPallet += " values(@CodMermaBodegaDetalle " + "," + a.CodigoEntrega + "," + a.CantidadAjustar + ") \n";
                qwPallet += " select @CodMermaBodegaDetallePallet = IDENT_CURRENT( 'MermaBodegaDetallePallet' ) \n ";
                qwPallet += "    set @cont = 1;  \n ";

                qwPallet += "--AS  \n ";

                qwPallet += "  insert into #Entregas   \n ";
                qwPallet += "  select t.CodigoEntrega,t.Cantidad  \n ";
                qwPallet += "  from MermaBodegaDetallePallet t  \n ";
                qwPallet += "  inner join Entrega e on e.Codigo = t.CodigoEntrega  \n ";
                qwPallet += "  where CodigoMermaBodegaDetalle = @CodMermaBodegaDetalle  \n ";


                qwPallet += "            select @maxcont = max(Codigo) from #Entregas                        \n ";
                qwPallet += "  while @cont <= @maxcont \n ";
                qwPallet += "  begin \n ";
                qwPallet += "  select @codigoEntrega = CodigoEntrega , @cantidad = Cantidad from #Entregas where Codigo=@cont                       \n ";
                qwPallet += "  insert into MermaBodegaDetalleCaja \n ";
                qwPallet += "   select top(@cantidad) t.Codigo,ddc.Codigo \n ";
                qwPallet += "                                                            from MermaBodegaDetallePallet t \n ";
                qwPallet += "   inner join Entrega e on e.Codigo = t.CodigoEntrega \n ";
                qwPallet += "   inner join DespachoDetalleCaja ddc on ddc.CodigoEntrega = e.Codigo \n ";
                qwPallet += "   left join DespachoPickupDetalleCaja pdc on pdc.CodigoDespachoDetalleCaja = ddc.Codigo \n ";
                qwPallet += "   left join   MermaBodegaDetalleCaja mc on mc.CodigoDespachoDestalleCaja = ddc.Codigo \n ";
                qwPallet += "   where t.CodigoMermaBodegaDetalle = @CodMermaBodegaDetalle  and t.CodigoEntrega = @codigoEntrega and pdc.Codigo is null \n ";
                qwPallet += "   and mc.Codigo is null \n ";
                qwPallet += "  set @cont = @cont + 1 \n ";
                qwPallet += "  end \n ";






            }
            qw += qwAjuste + qwMerma + qwDetalle + qwPallet + qwCajas;

            SqlCommand Command = new SqlCommand(qw);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            //SqlCommand Command = new SqlCommand(qw);

            SqlCommand Command2 = new SqlCommand(@"select max (codigo)  from AjusteBodega ");
            DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);
            var cd = JsonConvert.SerializeObject(Resultado2);

            List<DataRow> list = Resultado2.AsEnumerable().ToList();
            var cod = list[0].ItemArray[0];

            int CodigoAjuste = Convert.ToInt32(cod);
            //DataTable Resultado = Tools.CommandTextToDataTable(Command);
            //string Job =
            CrearTrabajoAjusteBodega(CodigoAjuste, data);

            //           string Proced = "exec autorizaAjustesBodega @JobId = '" + GlobalJobId + "',@codigosAjustes = " + CodigoAjuste.ToString();
            //           Command2 = new SqlCommand(Proced);
            //           DataTable Resultado3 = Tools.CommandTextToDataTable(Command2);


        }

        static internal void GuardarNombreArchivos(string NombreArchivo)
        {
            //estraer el codigoajuste
            SqlCommand Command2 = new SqlCommand(@"select max (codigo)  from AjusteBodega ");
            DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);
            var cd = JsonConvert.SerializeObject(Resultado2);
            List<DataRow> list = Resultado2.AsEnumerable().ToList();
            var codAjuste = list[0].ItemArray[0];

            DBDataContext data = new DBDataContext(ConnectionString);
            var qw = "";
            qw += " INSERT INTO [AjustesClasificacionArchivosReg] values (" + codAjuste + ",'" + NombreArchivo + "', 2 )";
            SqlCommand Command = new SqlCommand(qw);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
        }

        static internal void GuardarAjustesPalletCaido(AjustesClass[] Ajuste)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            var qw = "";

            var qwDetalle = "";
            var qwPallet = "";
            var qwCajas = "";
            var qwSPAutoriza = "";
            if (Ajuste[0].FolioCertificado == "")
                Ajuste[0].FolioCertificado = "SinFolio";
            qw += " declare @JobId NVARCHAR(100) \n ";
            qw += " declare @CodAjuste int \n";
            qw += " declare @CodMermaBodega int \n";
            qw += " declare @CodMermaBodegaDetalle int \n";
            qw += " declare @CodMermaBodegaDetallePallet int \n";
            qw += " set @JobId = (select NewId from getnewid) \n";
            qw += " insert into AjusteBodega values( " +
            Ajuste[0].CodigoBodega + "," +
            "2" + "," +
            "'AJUSTE DE MERMA'" + "," +
            "0" + ",'" +
            null + "'," +
            Ajuste[0].CodUsuario + "," +
            "GETDATE()" + "," +
            "@JobId" + ",'" +
            Ajuste[0].Observaciones + "'," +
            "0" + ","
            + "0," +
            Ajuste[0].CodigoClasificacionAjuste + ",'" +
            Ajuste[0].FolioCertificado + "','" +
            Ajuste[0].NombreArchivo + "'" +
            ") \n";
            qw += " select @CodAjuste = IDENT_CURRENT( 'AjusteBodega' ) \n ";


            //Codigo,Fecha,CodigoBodega,CodigoAjusteBodega

            foreach (var a in Ajuste[0].ProductoPallet)
            {
                qwDetalle += "INSERT INTO[AjusteBodegaDetalle]  ";
                qwDetalle += "(CodigoAjusteBodega, CodigoEntrega,  CantidadEntrada, CodigoProducto, CantidadSalida, CantidadMermada, Observaciones)  ";
                qwDetalle += "VALUES(@CodAjuste," + a.CodigoEntrega + "," + a.Cantidad + " , '" + a.CodigoProducto + "'," + a.Cantidad + "," + 0 + ",'N'" + ")  ";
            }


            qwCajas += " if OBJECT_ID(N'tempdb.dbo.#Entregas') is null \n ";
            qwCajas += " create table #Entregas( Codigo int identity(1,1),CodigoEntrega int, Cantidad int)  \n ";
            qwCajas += " else  \n ";
            qwCajas += "    truncate table #Entregas  \n ";
            qwCajas += "  insert into #Entregas  \n ";
            qwCajas += "  select t.CodigoEntrega,t.CantidadEntrada \n ";
            qwCajas += "  from AjusteBodegaDetalle t \n ";
            qwCajas += "  inner join Entrega e on e.Codigo = t.CodigoEntrega \n ";
            qwCajas += "  where CodigoAjusteBodega = @CodAjuste and t.Codigo not in      \n ";
            qwCajas += "  (select CodigoAjusteBodegaDetalle from AjusteBodegaDetalleCaja where  CodigoAjusteBodega = @CodAjuste )  \n ";
            qwCajas += "  declare @cont int, @maxcont int, @codigoEntrega int, @cantidad int \n ";
            qwCajas += "  set @cont = 1 \n ";
            qwCajas += "  select @maxcont = max(Codigo) from #Entregas 	  \n ";
            qwCajas += "  while @cont <= @maxcont \n ";
            qwCajas += "  begin \n ";
            qwCajas += "  select @codigoEntrega = CodigoEntrega , @cantidad = Cantidad from #Entregas where Codigo=@cont      \n ";
            qwCajas += "  insert into AjusteBodegaDetalleCaja \n ";
            qwCajas += "     select top(@cantidad) t.Codigo,ddc.Codigo \n ";
            qwCajas += "     from AjusteBodegaDetalle t \n ";
            qwCajas += "    inner join Entrega e on e.Codigo = t.CodigoEntrega \n ";
            qwCajas += "    inner join DespachoDetalleCaja ddc on ddc.CodigoEntrega = e.Codigo \n ";
            qwCajas += "    left join DespachoPickupDetalleCaja pdc on pdc.CodigoDespachoDetalleCaja = ddc.Codigo \n ";
            qwCajas += "    left join AjusteBodegaDetalleCaja tc on tc.CodigoDespachoDetalleCaja = ddc.Codigo \n ";
            qwCajas += "    where t.CodigoAjusteBodega = @CodAjuste  and t.CodigoEntrega = @codigoEntrega and pdc.Codigo is null and tc.Codigo is null \n ";
            qwCajas += "    and t.Codigo not in           	  \n ";
            qwCajas += "    (select CodigoAjusteBodegaDetalle from AjusteBodegaDetalleCaja where CodigoAjusteBodega = @CodAjuste )       \n ";
            qwCajas += "    set @cont = @cont + 1 \n ";
            qwCajas += "  end \n ";


            //qwSPAutoriza += "exec autorizaAjusteBodegaMerma @codAjuste,@JobId,1  ";





            qw += qwDetalle + qwPallet + qwCajas + qwSPAutoriza;

            SqlCommand Command = new SqlCommand(qw);

            SqlCommand Command2 = new SqlCommand(@"select max (codigo)  from AjusteBodega ");
            DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);
            var cd = JsonConvert.SerializeObject(Resultado2);

            List<DataRow> list = Resultado2.AsEnumerable().ToList();
            var cod = list[0].ItemArray[0];

            int CodigoAjuste = Convert.ToInt32(cod) + 1;
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            //string Job =
            CrearTrabajoAjusteBodega(CodigoAjuste, data);

            string Proced = "exec autorizaAjustesBodega @JobId = '" + GlobalJobId + "',@codigosAjustes = " + CodigoAjuste.ToString();
            Command2 = new SqlCommand(Proced);
            DataTable Resultado3 = Tools.CommandTextToDataTable(Command2);
        }

        static internal void GuardarTiposdeAjustes(TipoAjustesClass[] TipoAjuste)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            var Comprobante = 0;
            if (TipoAjuste[0].Comprobante.ToString() == "True")
                Comprobante = 1;

            var qw = "";
            var archivos = TipoAjuste[0].ArchivosAjustes;
            if (TipoAjuste[0].Codigo == 0) // es nuevo tipo de ajuste
            {
                qw += "insert into AjustesClasificacionTipos values('" + TipoAjuste[0].Descripcion.ToString() + "'," + Comprobante + "," + TipoAjuste[0].CodigoTipoAjuste + "," + TipoAjuste[0].CodigoTipoOperacion + ")";
                qw += " DECLARE @CodigoMaxTA int ";
                qw += " set @CodigoMaxTA = (select max(codigo) as CodigoTA from AjustesClasificacionTipos) ";
                foreach (var a in TipoAjuste[0].ArchivosAjustes)
                {

                    qw += " insert into AjustesClasificacionAjustesArchivos values(@CodigoMaxTA," + a.Codigo + " )";
                }


            }
            else
            { // actualiza ajuste
                qw = " DECLARE @CodigoMaxTA int ";
                qw += " update AjustesClasificacionTipos set Descripcion= '" + TipoAjuste[0].Descripcion.ToString() + "', Comprobante= " + Comprobante + " Where Codigo=" + TipoAjuste[0].Codigo;
                qw += " delete from AjustesClasificacionAjustesArchivos where CodigoClasificacionTipoAjuste= " + TipoAjuste[0].Codigo;
                foreach (var a in TipoAjuste[0].ArchivosAjustes)
                {
                    qw += " insert into AjustesClasificacionAjustesArchivos values(" + TipoAjuste[0].Codigo + "," + a.Codigo + " )";
                }
            }


            //SqlCommand Command = new SqlCommand(@"
            //                                  insert into AjustesTipos values ('" + TipoAjuste[0].Descripcion.ToString() + "'," + Comprobante + ")");

            SqlCommand Command = new SqlCommand(qw);


            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            //return JsonConvert.SerializeObject(Resultado);
        }

        static internal void GuardarClasificacionArchivos(TipoAjustesClass[] TipoAjuste)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            var Comprobante = 0;
            if (TipoAjuste[0].Comprobante.ToString() == "True")
                Comprobante = 1;

            var qw = "";
            if (TipoAjuste[0].Codigo == 0) // es nuevo tipo de ajuste
            {
                qw = "insert into AjustesClasificacionArchivos values('" + TipoAjuste[0].Descripcion.ToString() + "')";

            }
            else
            { // actualiza ajuste
                qw = "update AjustesClasificacionArchivos set Descripcion= '" + TipoAjuste[0].Descripcion.ToString() + "' Where Codigo=" + TipoAjuste[0].Codigo;

            }


            SqlCommand Command = new SqlCommand(qw);


            DataTable Resultado = Tools.CommandTextToDataTable(Command);
        }

        internal static string GetTiposdeAjustexs()
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            SqlCommand Command = new SqlCommand(@"
                                              Select * from AjustesClasificacionTipos order by Codigo desc");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetClasiTiposAjuste(string CodigoTipoAjuste)
        {
            SqlCommand query = new SqlCommand
                (@"Select * from AjustesClasificacionTipos where CodigoTipoAjuste= " + CodigoTipoAjuste + "order by Codigo desc ");

            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<TipoAjustesClass> listaTipoAjuste = new List<TipoAjustesClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                TipoAjustesClass itemTA = new TipoAjustesClass();
                itemTA.Codigo = Convert.ToInt32(row["Codigo"].ToString());
                itemTA.Descripcion = row["Descripcion"].ToString();
                itemTA.Comprobante = Convert.ToBoolean(row["Comprobante"]);
                itemTA.CodigoTipoAjuste = Convert.ToInt32(row["CodigoTipoAjuste"]);



                listaTipoAjuste.Add(itemTA);
            }

            return JsonConvert.SerializeObject(listaTipoAjuste);
        }

        internal static string GetClasiTiposAjusteDetalle(string CodigoTipArchivo)
        {
            SqlCommand query = new SqlCommand
                (@"
                Select CA.Codigo,CA.Descripcion from AjustesClasificacionAjustesArchivos ACA
                inner join AjustesClasificacionArchivos CA on CA.Codigo = ACA.CodigoClasificacionTipoArchivo
                where ACA.CodigoClasificacionTipoAjuste =" + CodigoTipArchivo);

            DataTable ArchivosTable = Tools.CommandTextToDataTable(query);
            List<TipoAjustesClass> listaTipoArchivo = new List<TipoAjustesClass>();
            foreach (DataRow row in ArchivosTable.Rows)
            {
                TipoAjustesClass itemTA = new TipoAjustesClass();
                itemTA.Codigo = Convert.ToInt32(row["Codigo"].ToString());
                itemTA.Descripcion = row["Descripcion"].ToString();
                listaTipoArchivo.Add(itemTA);
            }

            return JsonConvert.SerializeObject(listaTipoArchivo);
        }

        internal static string GetClasiArchivos()
        {
            SqlCommand query = new SqlCommand
                (@"Select * from AjustesClasificacionArchivos order by Codigo desc ");

            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<TipoAjustesClass> listaTipoAjuste = new List<TipoAjustesClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                TipoAjustesClass itemTA = new TipoAjustesClass();
                itemTA.Codigo = Convert.ToInt32(row["Codigo"].ToString());
                itemTA.Descripcion = row["Descripcion"].ToString();


                listaTipoAjuste.Add(itemTA);
            }

            return JsonConvert.SerializeObject(listaTipoAjuste);
        }

        static internal void DeleteEntryByCodigoEntry(int CodigoEntry)
        {
            DBDataContext Data = new DBDataContext(ConnectionString);
            Entry ObjEntry = Data.Entries
                                    .SingleOrDefault(x => x.CodigoEntry == CodigoEntry);

            if (ObjEntry == null)
            {
                Tools.ThrowException("No se encontró el Entry con ese Codigo");
            }
            else if (ObjEntry.FechaHoraCancelacion != null)
            {
                Tools.ThrowException("El Entry se encuentra cancelado");
            }
            else
            {
                ObjEntry.FechaHoraCancelacion = DateTime.Now;
                List<EntryDetalle> ObjEntryDetalle = ObjEntry.EntryDetalles.ToList();
                Data.EntryDetalles.DeleteAllOnSubmit(ObjEntryDetalle);
                Data.SubmitChanges();
            }
        }

        public static JobIdentity CrearTrabajoAjusteBodega(int CodigoAjuste, DBDataContext db)
        {

            ParametrosTotalAgility parametro = db.ParametrosTotalAgilities.FirstOrDefault();
            var session_id = "";
            session_id = Tools.GetSessionIdKofax(db);




            //Accedemos al servicio de Trabajos SOA
            JobService js = new JobService();

            //Indicamos el nombre del Proceso y id, puedes obtenerlo mediante base de datos u buscarlo por medio del SDk
            ProcessIdentity pi = new ProcessIdentity { Id = "726F7C3061224D4EB150519624F20193", Name = "AjustesEnBodega_App" };
            //Instanciamos un objeto de coleccion de variables
            InputVariableCollection ivc = new InputVariableCollection();

            TripleH.Erp.Model.Log log = new TripleH.Erp.Model.Log();
            log.MsgStatus = "paso por ProcessIdentity " + CodigoAjuste;
            db.Logs.InsertOnSubmit(log);
            db.SubmitChanges();

            //var usuarioActual = db.Usuarios.FirstOrDefault(x => x.Codigo == ventaJson.CodigoUsuarioModificacion);

            ivc.Add(new InputVariable { Id = "CodigoAjuste", Value = Convert.ToInt64(CodigoAjuste) });


            JobInitialization ji = new JobInitialization { InputVariables = ivc, StartDate = DateTime.Now, };// instanciamos nuestros parametros de incializacion del job

            log = new TripleH.Erp.Model.Log();
            log.MsgStatus = "paso por JobInitialization " + CodigoAjuste;
            db.Logs.InsertOnSubmit(log);
            db.SubmitChanges();

            var jb = js.CreateJob(session_id, pi, ji);

            //Venta.JOBID = jb.Id;

            //if (exp != null)
            //    exp.JobId = jb.Id;
            log = new TripleH.Erp.Model.Log();
            log.MsgStatus = string.Format("Creo el job Id {0} ", jb.Id);
            db.Logs.InsertOnSubmit(log);
            db.SubmitChanges();
            //usrSVC.LogOff(session_id);
            Tools.logOfKofax(session_id);

            GlobalJobId = jb.Id;

            CodigoAjuste = CodigoAjuste;

            SqlCommand Command2 = new SqlCommand(@"update AjusteBodega set JobId='" + GlobalJobId + "' where Codigo = " + CodigoAjuste);
            DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);

            return jb; //Creamos un JOb del Proceso y los prarmetros necesarios 
        }

        static internal void MandaCorreoAjuste(int CodigoAjuste)
        {

            SqlCommand Command = new SqlCommand(@"
                                              exec  getDetalleAjusteBodegaHtmlApp " + CodigoAjuste);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            List<DataRow> list = Resultado.AsEnumerable().ToList();
            //Extrae datos para correo


            Command = new SqlCommand(@" select NombreArchivo from AjustesClasificacionArchivosReg where CodigoAjuste= " + CodigoAjuste);
            Resultado = Tools.CommandTextToDataTable(Command);
            List<DataRow> listArchivo = Resultado.AsEnumerable().ToList();
            var detalleimagen = "";
            var path = "//SERVERWEB//calidad//Pruebas//";
            foreach (var a in listArchivo)
            {
                //detalleimagen+=  path + a.ItemArray[0] + ";";
                string archivoAbrir = path + a.ItemArray[0];
                //string archivoDestino = path + Path.GetRandomFileName() + "-" + codigoRecepcion.ToString() + Path.GetExtension(archivo.NombreArchivoOriginal);
                string archivoDestino = Path.GetTempPath() + "" + a.ItemArray[0];

                try
                {
                    File.Delete(archivoDestino);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                }
                if (File.Exists(archivoAbrir) && !File.Exists(archivoDestino))
                {
                    File.Copy(archivoAbrir, archivoDestino);

                }
                detalleimagen += archivoDestino + ";";
            }

            var detalleMensaje = "";
            var res = "";
            //ENVIAR CORREO
            detalleMensaje = "<!DOCTYPE html>";
            detalleMensaje += " <html><body>";
            detalleMensaje += " <font face=verdana <font size=4  color=black><strong>AJUSTE DE MERMA </font></strong>";
            detalleMensaje += " <font face=verdana <font size=3  color=black><strong>CODIGO AJUSTE " + CodigoAjuste + " </font></strong>";
            detalleMensaje += " <font face=verdana <font size=3  color=black>";
            detalleMensaje += list[0].ItemArray[0];

            detalleMensaje += " <font face=verdana <font size=3  color=black>";
            //detalleMensaje += detalleimagen;
            detalleMensaje += "</font></body></html>";


            MensajeroEmail.EnviarCorreoCC("Triple H", "tripleh.soft@tripleh.com.mx", "rmendoza@tripleh.com.mx", detalleMensaje, "Ajuste de Bodega", detalleimagen, "");

            //res = Resultado.ToString();



            // return JsonConvert.SerializeObject(res);
        }

        internal static string GetAjuste(string CodigoAjuste)
        {
            SqlCommand query = new SqlCommand
                (@"
                select top 1 ab.Codigo, ab.Descripcion,ab.Observaciones, ab.FechaAutorizado, 
                b.NombreBodega, MB.Codigo, MBD.CodigoCarga,
                CONCAT(c.FolioCarga,' - ', MBD.CodigoCarga) as FolioCarga, ACT.Descripcion as ClasificacionAjuste,
				tab.Nombre as TipoAjusteBodega
                from AjusteBodega AB 
                inner join MermaBodega MB on ab.Codigo=MB.CodigoAjusteBodega
                inner join MermaBodegaDetalle MBD on MBD.CodigoMermaBodega=MB.Codigo
                inner join Carga C on c.Codigo= MBD.CodigoCarga
                inner join Bodega B on b.Codigo=mb.CodigoBodega				
				inner join AjustesClasificacionTipos ACT on act.Codigo=ab.CodigoClasificacionAjuste
				inner join TipoAjusteBodega TAB on tab.Codigo=ACT.CodigoTipoAjuste

                where ab.Codigo= " + CodigoAjuste);

            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<AjustesInfoClass> listaTipoAjuste = new List<AjustesInfoClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                AjustesInfoClass itemTA = new AjustesInfoClass();
                itemTA.CodigoAjuste = (row["Codigo"].ToString());
                itemTA.Descripcion = row["Descripcion"].ToString();
                itemTA.Observaciones = (row["Observaciones"].ToString());
                itemTA.FechaAutorizado = row["FechaAutorizado"].ToString();
                itemTA.NombreBodega = row["NombreBodega"].ToString();
                itemTA.CodigoCarga = row["CodigoCarga"].ToString();
                itemTA.FolioCarga = row["FolioCarga"].ToString();
                itemTA.TipoAjusteBodega = row["TipoAjusteBodega"].ToString();
                itemTA.ClasificacionAjuste = row["ClasificacionAjuste"].ToString();
                listaTipoAjuste.Add(itemTA);
            }


            return JsonConvert.SerializeObject(listaTipoAjuste);
        }

        internal static string GetAjustePallet(string CodigoAjuste)
        {
            SqlCommand query = new SqlCommand
                (@"select MBDP.CodigoEntrega, MBDP.Cantidad, E.CodigoPallet, D.CodigoProducto, P.ItemName as NombreProducto, A.Nombre as NombreAgricultor,  MBD.CodigoCarga  from MermaBodegaDetallePallet MBDP 
                    inner join MermaBodegaDetalle MBD on MBD.Codigo=MBDP.CodigoMermaBodegaDetalle 
                    INNER JOIN MermaBodega MB on MB.Codigo =  MBD.CodigoMermaBodega
                    inner join Entrega E on E.Codigo=MBDP.CodigoEntrega
                    inner join DespachoDetalle D on D.CodigoPallet= E.CodigoPallet
                    inner join Producto P on P.ItemCode=D.CodigoProducto
                    inner join Agricultor A on A.Codigo=D.CodigoAgricultor
                    where MB.CodigoAjusteBodega= " + CodigoAjuste);


            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<AjustesInfoPaletClass> listaTipoAjuste = new List<AjustesInfoPaletClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                AjustesInfoPaletClass itemTA = new AjustesInfoPaletClass();
                itemTA.CodigoPallet = (row["CodigoPallet"].ToString());
                itemTA.CodigoProducto = row["CodigoProducto"].ToString();
                itemTA.Producto = (row["NombreProducto"].ToString());
                itemTA.Agricultor = row["NombreAgricultor"].ToString();
                itemTA.Cantidad = row["Cantidad"].ToString();
                itemTA.CodigoCarga = row["CodigoCarga"].ToString();
                listaTipoAjuste.Add(itemTA);
            }


            return JsonConvert.SerializeObject(listaTipoAjuste);
        }

        internal static string GetAjusteArchivos(string CodigoAjuste)
        {
            var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];
            SqlCommand query = new SqlCommand
                (@" select NombreArchivo from AjustesClasificacionArchivosReg where CodigoAjuste= " + CodigoAjuste);


            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<AjustesInfoArchivoClass> listaTipoAjuste = new List<AjustesInfoArchivoClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                AjustesInfoArchivoClass itemTA = new AjustesInfoArchivoClass();
                itemTA.NombreArchivo = (row["NombreArchivo"].ToString());

                listaTipoAjuste.Add(itemTA);
            }


            return JsonConvert.SerializeObject(listaTipoAjuste);
        }

        internal static string GetAgricultores(string CodigoBodega)
        {

            SqlCommand query = new SqlCommand
                (@" exec paGetAgricultoresConProductoBodega " + CodigoBodega);


            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<AgricultorClass> listaAgricultor = new List<AgricultorClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                AgricultorClass itemTA = new AgricultorClass();
                itemTA.CodigoAgricultor = (row["CodigoAgricultor"].ToString());
                itemTA.NombreAgricultor = (row["Agricultor"].ToString());

                listaAgricultor.Add(itemTA);
            }


            return JsonConvert.SerializeObject(listaAgricultor);
        }

        internal static string GetNombreArchivo(string CodigoAjuste)
        {
            var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];

            DataTable dataTable = Tools.CommandTextToDataTable(new SqlCommand(@" select NombreArchivo from AjustesClasificacionArchivosReg where CodigoAjuste= " + CodigoAjuste));
            string empty = string.Empty;
            string[] strArray = new string[50];
            for (int index = 0; index < dataTable.Rows.Count; ++index)
            {
                DataRow row = dataTable.Rows[index];
                for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; ++columnIndex)
                {
                    empty += rutaFoto + (string)row[columnIndex];
                    strArray[index] = row[columnIndex].ToString();
                    if (columnIndex == dataTable.Columns.Count - 1)
                    {
                        if (index != dataTable.Rows.Count - 1)
                            empty += "|";
                    }
                    else
                        empty += "|";
                }
            }
            return empty;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16384];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, count);
                return memoryStream.ToArray();
            }
        }

        internal static string GetAjustesVisor(string Codigo, int tipoConsulta)
        {
            var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];

            string Consulta = "";
            if (tipoConsulta == 1)
            {
                Consulta = "paGetAjustesBodega " + Codigo;
            }
            if (tipoConsulta == 2)
            {
                Consulta = "paGetAjustesBodegaAgricultor " + Codigo;
            }
            if (tipoConsulta == 3)
            {
                Consulta = "paGetAjustesBodegaCarga " + Codigo;
            }

            SqlCommand query = new SqlCommand(Consulta);

            DataTable AjustesTable = Tools.CommandTextToDataTable(query);
            List<AjustesVisorClass> ListaAjustes = new List<AjustesVisorClass>();
            foreach (DataRow row in AjustesTable.Rows)
            {
                AjustesVisorClass itemTA = new AjustesVisorClass();
                //itemTA.Codigo = (row["Codigo"].ToString());
                itemTA.Bodega = (row["Bodega"].ToString());
                itemTA.CodigoAjuste = (row["CodigoAjuste"].ToString());
                itemTA.TipoAjuste = (row["TipoAjuste"].ToString());
                itemTA.Observaciones = (row["Observaciones"].ToString());
                itemTA.Fecha = (row["Fecha"].ToString());

                ListaAjustes.Add(itemTA);
            }


            return JsonConvert.SerializeObject(ListaAjustes);
        }

    }
}