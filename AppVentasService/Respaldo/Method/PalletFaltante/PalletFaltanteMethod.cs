using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace AppBodegaService.Method
{
    public class PalletFaltanteMethod
    {
        internal static string GetPalletFaltante (int CodigoEntry,int CodigoCarga)
        {
            SqlCommand Command = new SqlCommand(string.Format(@"declare @CodigoEntry as int = {0}
                                                                declare @CodigoCarga as int = {1}

                                                                declare @CodigoDespacho as int = (Select Codigo from Despacho where CodigoCarga = @CodigoCarga)

                                                                declare @EntregasDelEntry as table(
	                                                                Codigo int
                                                                )

                                                                declare @EntregasDelDespacho as table(
	                                                                Codigo int,
	                                                                CodigoPallet varchar(50)
                                                                )

                                                                insert into @EntregasDelEntry
	                                                                Select CodigoEntrega from EntryDetalle where CodigoEntry = @CodigoEntry
	                                                                and CodigoEntrega is not null

                                                                insert into @EntregasDelDespacho
	                                                                Select Codigo,CodigoPallet from Entrega where CodigoDespacho = @CodigoDespacho

                                                                declare @PalletsEnDespacho as int = (Select Count(Entrega.Codigo) from Entrega where CodigoDespacho = @CodigoDespacho)
                                                                declare @PalletAsignados as int   = (Select Count(Codigo) from @EntregasDelEntry where Codigo is not null)
                                                                declare @PalletScanneados as int  = (Select Count(*) from @EntregasDelEntry where Codigo is null)

                                                                declare @ProductoFaltante as table(
	                                                                CodigoProducto varchar(50) collate SQL_Latin1_General_CP850_CI_AS, 
	                                                                PalletsFaltantes int
                                                                )

                                                                insert into @ProductoFaltante
                                                                Select CodigoProducto,COUNT(CodigoProducto) as Faltantes from DespachoDetalle 
	                                                                where DespachoDetalle.CodigoDespacho = @CodigoDespacho
	                                                                and CodigoPallet 
		                                                                in (Select CodigoPallet from Entrega 
				                                                                where Codigo not in 
					                                                                (Select Codigo from @EntregasDelEntry))
	                                                                group by CodigoProducto


                                                                Select Producto.ItemName as NombreProducto ,
		                                                                ProductoFaltante.PalletsFaltantes 
		                                                                from @ProductoFaltante as ProductoFaltante 
			                                                                join Producto on (Producto.ItemCode = ProductoFaltante.CodigoProducto)
		                                                                for xml path ('Data'),Root('Root')", CodigoEntry,CodigoCarga));

            string Resultado = Tools.CommandToJson(Command);
            return Resultado;

        }
    }
}