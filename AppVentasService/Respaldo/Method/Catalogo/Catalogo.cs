using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AppBodegasService.Global;

namespace AppBodegaService.Method
{
    public class Catalogo
    {
        static string ConnectionString = Configurations.ConnectionString;

        internal static string GetBodegaMetodo()
        {
            SqlCommand Command = new SqlCommand(@"Select 
                                                    Bodega.Codigo as CodigoBodega,
                                                    Bodega.NombreBodega,
                                                    Bodega.* 
                                                    from Bodega");
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetCargaMetodo(int CodigoBodega)
        {

            SqlCommand Command = new SqlCommand(@"
                                                declare @DateEnd datetime = GetDate()
                                                declare @DateStart datetime = dateadd(day,-10,@DateEnd)
                                                declare @CodigoDespachoInicial as int = (select top 1 Despacho.Codigo from Despacho 
								                                                where Despacho.FechaEmbarque between @DateStart and @DateEnd
								                                                order by Codigo asc)

									                                            Select distinct(dd.CodigoProducto) , D.CodigoCarga,  c.Nombre, 
									                                            P.ItemName as NombreProducto, d.FechaEmbarque, c.CodigoBodega, DD.CodigoAgricultor, A.Nombre as NombreAgricultor, p.U_Cultivo as CodigoCultivo  
									                                            from Despacho as D
										                                            inner join Carga as C on C.Codigo=D.CodigoCarga
										                                            inner join DespachoDetalle DD on DD.CodigoDespacho=d.Codigo
										                                            inner join Producto P on p.ItemCode=dd.CodigoProducto
										                                            inner join Entrega E on E.CodigoDespacho = DD.CodigoDespacho
										                                            inner join DespachoDetalleCaja DDC on DDC.CodigoEntrega=E.Codigo
										                                            inner join Agricultor A on A.Codigo=DD.CodigoAgricultor
										                                            --left join RecepcionProduceDetallePalletCaja RDPC on RDPC.CodigoDespachoDetalleCaja=DDC.Codigo
									                                            Where  D.FechaEmbarque>=(select GETDATE()-DiasSyncEmbaques from ParametrosTotalAgility)  and c.TipoMercado=1 
									                                            --and RDPC.Codigo is null
									                                            and D.Codigo>=@CodigoDespachoInicial
									                                            and C.CodigoBodega = " + CodigoBodega.ToString() + " order by D.CodigoCarga desc");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetCargaBodegaAjusteMetodo(string CodigoAgricultor)
        {

            SqlCommand Command = new SqlCommand(@"
                                                SELECT FolioCarga,Codigo FROM getCargasAjusteBodega WHERE 
                                (( CodigoAgricultor = " + CodigoAgricultor + ")) ORDER BY Codigo asc");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetProductoCargaBodegaAjuste(int CodigoCarga)
        {

            SqlCommand Command = new SqlCommand(@"
                                              SELECT [CodigoAgricultor],[Agricultor],[CodigoEntrega],[CodigoProducto],[Producto],[Cantidad],[CodigoCarga],[CodigoPallet],[CantidadAjustar],[CodigoInspeccion],[Folio],[CantidadPickup], [CodigoEntrega]  
FROM [getProductoCarga] WHERE (( [CodigoCarga] = " + CodigoCarga + " )) ORDER BY [Producto] asc");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }

        internal static string GetTipoAjuste()
        {

            SqlCommand Command = new SqlCommand(@"
                                              SELECT[Codigo],[Nombre] FROM[TipoAjusteBodega]");

            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        
        internal static string GetProductoCargaMetodo(int CodigoCarga)
        {


            SqlCommand Command = new SqlCommand(@"
                select distinct(CodigoProducto),p.ItemName, p.U_Cultivo from DespachoDetalle dd
	                inner join Producto p on p.ItemCode=dd.CodigoProducto
                where dd.CodigoDespacho in 
                (select codigo from Despacho where CodigoCarga in (" + CodigoCarga + "))"

     

);




            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetRecepcionaMetodo(int CodigoBodega)
        {


            SqlCommand Command = new SqlCommand(@"

                    select recep.Codigo,
                    recep.CodigoCarga,
					recep.NombreAgricultor + ' - ' + recep.Nombre as Nombre ,
					
					recep.CodigoAgricultor,
                    recep.CodigoBodega, 
					recep.CodigoBodegaUS,
                    recep.Fecha,
                    recep.CierreEvaluaciones,
					
                    LlevaColoresTarimas =  case when
                    (select count(ms.Codigo)
                    from Muestra ms
                    inner join Cultivo c on c.Codigo = ms.CodigoCultivo 
                    where ms.CodigoCarga = recep.CodigoCarga
                    and (c.Nombre like '%CHILE%' or c.Nombre = 'PEPINO' or c.Nombre = 'CALABAZA' or c.Nombre = 'MANGO')) > 0 then 'N' else 'S' end,
                    TieneFotosRecepcion = case when (
                    select count(mf.Codigo) 
                    from Muestra ms
                    left join MuestraFoto mf on mf.CodigoMuestra = ms.Codigo 
                    where ms.CodigoCarga =recep.CodigoCarga and ms.CodigoTipoMuestra = 5) > 0 then cast(1 as bit) else cast(0 as bit) end,
                    Nota = case when exists(select Codigo from Muestra where CodigoCarga = recep.CodigoCarga and  CodigoTipoMuestra = 5 and Nota != '')
                    then (select top 1 Nota from Muestra where CodigoCarga = recep.CodigoCarga and CodigoTipoMuestra = 5 and Nota != '') else '' end
                    from (select rp.Codigo, rp.CodigoCarga,rp.CodigoBodega,Fecha =cast(rp.Fecha as Date),
                    CierreEvaluaciones = case  when c.EvaluacionesRecepcionCerradas = 0 then 'N' else 'S' end, rp.CodigoBodegaUS, 
					c.Nombre, dd.CodigoAgricultor, a.Nombre as NombreAgricultor
                    --into #recepciones
                    from DespachoDetalleCaja ddc
                    inner join Entrega e on e.Codigo = ddc.CodigoEntrega
                    inner join Despacho d on d.Codigo = e.CodigoDespacho
					inner join DespachoDetalle dd on dd.CodigoDespacho = e.CodigoDespacho
					inner join Agricultor a on a.Codigo= dd.CodigoAgricultor
                    inner join Carga c on c.Codigo = d.CodigoCarga
                    inner join RecepcionProduceDetallePalletCaja rpc on rpc.CodigoDespachoDetalleCaja = ddc.Codigo
                    inner join RecepcionProducePallets rpp on rpp.Codigo = rpc.CodigoRecepcionProducePallet
                    inner join RecepcionProduce rp on rp.Codigo = rpp.CodigoRecepcion
                    where cast(rp.Fecha as date) >= cast(getdate()-40 as date) and
                    c.EvaluacionesRecepcionCerradas = 0  
                    group by rp.Codigo, rp.CodigoCarga,rp.CodigoBodega,rp.Fecha,c.EvaluacionesRecepcionCerradas,rp.CodigoBodegaUS,
					c.Nombre,dd.CodigoAgricultor,a.Nombre) recep

");




            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetPalletMetodo()
        {
            string StringCommand = @"select c.Codigo as CodigoCarga, dd.CodigoDespacho, dd.CodigoPallet, E.Codigo as CodigoEntrega, p.ItemCode as CodigoProducto, p.ItemName as NombreProducto, dd.NumeroPallet, dd.CodigoAgricultor from DespachoDetalle dd 
                                            inner join Despacho d on d.Codigo = dd.CodigoDespacho 
                                            inner join Entrega E on E.CodigoPallet=dd.CodigoPallet
                                            inner join Carga C on C.Codigo=d.CodigoCarga 
                                            inner join Bodega B on B.Codigo=C.CodigoBodega 
                                            inner join Producto P on p.ItemCode = dd.CodigoProducto
                                            left join RecepcionProducePallets rp on rp.CodigoEntrega= E.Codigo
                                            left join EntryDetalle ed on ed.CodigoEntrega= E.Codigo
											left join Entry en on en.CodigoEntry= ed.CodigoEntry and en.FechaHoraCancelacion is null
                                            Where  D.FechaEmbarque >= (select GETDATE()-DiasSyncEmbaques from ParametrosTotalAgility) and c.TipoMercado = 1 and en.CodigoEntry is null and rp.Codigo is null
                                            order by D.CodigoCarga desc, dd.CodigoPallet ";
                        
            SqlCommand Command = new SqlCommand(StringCommand);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetUsuarioBodegaMetodo()
        {
            string StringCommand = @"Select U.Codigo as CodigoUsuario, U.Nombre as NombreUsuario, B.Codigo as CodigoBodega, B.NombreBodega from UsuarioBodega  UB
                                            inner join Usuario U on U.Codigo=UB.CodigoUsuario
                                            inner join Bodega B on B.Codigo=UB.CodigoBodega";

            SqlCommand Command = new SqlCommand(StringCommand);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetAgenteAduanal()
        {
            string StringCommand = @"Select * from AgenteAduanal";

            SqlCommand Command = new SqlCommand(StringCommand);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetAfectacion()
        {
            string StringCommand = @"Select CodigoTipoAfectacion, 
		                                    Afectacion = (Case 
			                                    when CodigoTipoAfectacion = 4 
				                                    then 'Cargo Triple H'
			                                    else
			                                    (Case 
				                                    when CodigoTipoAfectacion = 0 
					                                    then 'Cargo Comprador'
				                                    else
				                                    (Case
					                                    when CodigoTipoAfectacion = 24 
						                                    then 'Cargo Chofer comprador' 
					                                    else
				                                    (Case 
					                                    when CodigoTipoAfectacion = 1 
						                                    then 'Cargo Agricultor' 
					                                    else ''
					                                    end)
				                                    end)
			                                    end)
		                                    end)
                                    from TipoAfectacionGasto where CodigoTipoAfectacion in (4,0,24,1)";

            SqlCommand Command = new SqlCommand(StringCommand);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
        internal static string GetCausasOvertime()
        {
            string StringCommand = @"Select Codigo,Descripcion from CausasOvertime";

            SqlCommand Command = new SqlCommand(StringCommand);
            DataTable Resultado = Tools.CommandTextToDataTable(Command);
            return JsonConvert.SerializeObject(Resultado);
        }
    }
}