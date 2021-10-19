using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AppBodegaService.Models.Entry;
using AppBodegaService.Method;
using TripleH.Erp.Model;
using Newtonsoft.Json;
using AppBodegasService.Global;

namespace AppBodegaService
{
    internal static class EntryMethod
    {
        static string ConnectionString = Configurations.ConnectionString;
        
        static internal string GetAllEntries()
        {
            SqlCommand Command = new SqlCommand(@"Select
	                                                    Entry.*,
	                                                    (Select EntryDetalle.* from EntryDetalle 
		                                                    where EntryDetalle.CodigoEntry = Entry.CodigoEntry
		                                                    For xml path('EntryDetalle'),type
	                                                    )	
	                                                    from Entry
                                                    For xml path('Data'),root('Root')");
            
            return Tools.CommandToJson(Command);
        }
        static internal string GetAllentriesByDateAndCodigoBodega(int CodigoBodega,DateTime Date)
        {
            SqlCommand Command = new SqlCommand(string.Format(@"Select
		                                                                Entry.*,
		                                                                Bodega.NombreBodega,
		                                                                Frontera.Nombre as NombreFrontera,
		                                                                Agricultor.Nombre as NombreAgricultor,
                                                                        AgenteAduanal.NombreAgenteAduanal as NombreAgenteAduanal,
                                                                        (Select Pallets from dbo.fnObtieneCantidadPalletsEntry(Entry.CodigoEntry)) as CantidadPallets,
						                                                (Select Cargas from dbo.fnObtieneCargasEntry(Entry.CodigoEntry)) as Carga,
			                                                                (Select EntryDetalle.* from EntryDetalle 
				                                                                where EntryDetalle.CodigoEntry = Entry.CodigoEntry
				                                                                For xml path('EntryDetalle'),type
			                                                                )
			
		                                                                from Entry
			                                                                left join Bodega on (Bodega.Codigo = Entry.CodigoBodega)
			                                                                left join Frontera on (Frontera.Codigo = Entry.CodigoFrontera) 
			                                                                left join AgenteAduanal on (AgenteAduanal.CodigoAgenteAduanal = Entry.CodigoAgenteAduanal) 
			                                                                left join Agricultor on (Agricultor.Codigo = Entry.CodigoAgricultor)
		                                                                Where 
			                                                                Entry.CodigoBodega = {0}
			                                                                and Entry.FechaHoraCancelacion is null
                                                                            and Entry.Aprobado = 0
			                                                                and Entry.FechaCreacion >= '{1}'
		                                                                order by FechaCreacion Desc
		                                                                For xml path('Data'),root('Root')", CodigoBodega,Date.ToString("yyyy-MM-dd")));
            string Resultado = Tools.CommandToJson(Command);
            return Resultado;
        }
        static internal string GetAllEntriesByDate(DateTime Date)
        {
            SqlCommand Command = new SqlCommand(string.Format(string.Format(@"Select
		                                                Entry.*,
		                                                    (Select EntryDetalle.* from EntryDetalle 
			                                                    where EntryDetalle.CodigoEntry = Entry.CodigoEntry
			                                                    For xml path('EntryDetalle'),type
		                                                    )	
		                                                from Entry
		                                                Where Entry.FechaCreacion = '{0}'
	                                                    For xml path('Data'),root('Root')"
                                                    ,Date.ToString("yyyy-MM-dd")))
                                                    );

            return Tools.CommandToJson(Command);
        }
        static internal string[] ValidateEntries(EntryClass[] Entries)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            List<EntryDetalle> InvalidEntryDetalleList = new List<EntryDetalle>();

            foreach (EntryClass Entry in Entries)
            {
                if (Entry.EntryDetalle != null)
                {
                    foreach (EntryDetalleClass EntryDetalle in Entry.EntryDetalle)
                    {
                        List<EntryDetalle> InvalidEntryDetalle =
                            data.EntryDetalles.Where(
                                x => x.CodigoEntrega == EntryDetalle.CodigoEntrega
                            ).ToList();

                        if (InvalidEntryDetalle.Count > 0)
                        {
                            InvalidEntryDetalleList.AddRange(InvalidEntryDetalle);
                        }
                    }
                }
            }
            
            return InvalidEntryDetalleList.Select(
                x => x.CodigoPalletBodega
            ).Distinct().ToArray();
            
        }
        static internal void SaveEntries(EntryClass[] Entries)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            
            foreach (EntryClass EntryClass in Entries)
            {
                Bodega Bodega = data.Bodegas.Where(bdg => bdg.Codigo == EntryClass.CodigoBodega).FirstOrDefault();
                                
                //Crear Entry
                Entry Entry = new Entry();                
                Entry.CodigoAgricultor = EntryClass.CodigoAgricultor;
                                
                Entry.FechaHoraCancelacion = EntryClass.FechaHoraCancelacion;
                Entry.FolioEntry = EntryClass.FolioEntry;
                Entry.CodigoBodega = Bodega.Codigo;
                Entry.CodigoAgenteAduanal = EntryClass.CodigoAgenteAduanal;
                Entry.FechaCreacion = DateTime.Now;
                Entry.CodigoFrontera = (int)Bodega.ZonaBodegas.CodigoFrontera;
                Entry.EntryCompleto = EntryClass.EntryCompleto;
                Entry.UsuarioCreador = EntryClass.UsuarioCreador;
                Entry.Aprobado = false;
                                
                if (EntryClass.EntryDetalle != null)
                {
                    foreach(EntryDetalleClass EntryDetalleClass in EntryClass.EntryDetalle)
                    {   
                        //Crear Detalle del Entry
                        EntryDetalle  EntryDetalle = new EntryDetalle();
                        EntryDetalle.CodigoEntry = EntryDetalleClass.CodigoEntry;
                        EntryDetalle.CodigoEntrega = EntryDetalleClass.CodigoEntrega;
                        EntryDetalle.CodigoPalletBodega = EntryDetalleClass.CodigoPallet;
                        Entry.EntryDetalles.Add(EntryDetalle);
                    }
                }

                data.Entries.InsertOnSubmit(Entry);
                data.SubmitChanges();
            }
        }
        static internal string GenerateReception(int CodigoCarga,EntryClass EntryClass)
        {
            DBDataContext data = new DBDataContext(ConnectionString);
            List<long?> Entregas = EntryClass.EntryDetalle.Select(x => x.CodigoEntrega).ToList();

            List<Entrega> Entrega = data.Entregas
                                    .Where(x => Entregas.Contains(x.Codigo)
                                                && x.Despacho.CodigoCarga == CodigoCarga).ToList();


             List<Despacho> Despacho = Entrega.Select(x => x.Despacho).ToList();

            Bodega Bodega = data.Bodegas
                                    .Where(x => x.Codigo == EntryClass.CodigoBodega)
                                    .First();

            var Recepcion = data.RecepcionProduces.Where(x => x.CodigoCarga == CodigoCarga
                                                        && x.CodigoBodegaUS == EntryClass.CodigoBodega
                                                        ).ToList();
            RecepcionProduce RecepcionProduceNew;

            if (Recepcion.Count > 0)
            {
                RecepcionProduceNew = data.RecepcionProduces
                    .Where(x => x.CodigoCarga == CodigoCarga
                            && x.CodigoBodegaUS == EntryClass.CodigoBodega
                            ).FirstOrDefault();
            }
            else
            {
                RecepcionProduceNew = new RecepcionProduce();

                RecepcionProduceNew.CodigoCarga = CodigoCarga;
                RecepcionProduceNew.CodigoBodegaUS = Bodega.Codigo;
                        
                RecepcionProduceNew.CodigoBodega = Bodega.NombreBodega;
                RecepcionProduceNew.Fecha = DateTime.Now;
                RecepcionProduceNew.CodigoZonaBodega = Bodega.CodigoZonaBodega;
            
                RecepcionProduceNew.Provisional = false;
                RecepcionProduceNew.Recinto = false;
                RecepcionProduceNew.Hold = false;
                data.RecepcionProduces.InsertOnSubmit(RecepcionProduceNew);
            }


                        
            string CodigoCliente;
            CodigoCliente = data.Parametros.FirstOrDefault().CodigoClienteDistribuidoraHortimex;

            var RecepcionProduceDetalleNew2 = (from e in data.Entregas
                                    where Entregas.Contains(e.Codigo) 
                                    && e.Despacho.CodigoCarga == CodigoCarga
                                    group e by new
                                    {
                                        CodigoCliente = CodigoCliente,
                                        CodigoProducto = e.DespachoDetalle.CodigoProducto
                                    }
                                    into groupEntrega
                                    select new  {
                                        CodigoCliente = groupEntrega.Key.CodigoCliente,
                                        CodigoProducto = groupEntrega.Key.CodigoProducto,
                                        Cantidad = groupEntrega.Sum(x=> x.DespachoDetalle.CantidadCajas)
                                    })
                                    .ToList();

            foreach (var RecNew in RecepcionProduceDetalleNew2)
            {
                RecepcionProduceDetalle RecepcionProduceDetalleNew = new RecepcionProduceDetalle();
                RecepcionProduceDetalleNew.CodigoCliente = RecNew.CodigoCliente;
                RecepcionProduceDetalleNew.CodigoProducto = RecNew.CodigoProducto;
                RecepcionProduceDetalleNew.Cantidad = RecNew.Cantidad;
                RecepcionProduceDetalleNew.RecepcionProduce = RecepcionProduceNew;
                RecepcionProduceNew.Detalles.Add(RecepcionProduceDetalleNew);
            }
                       
            
            


            var RecepcionProducePalletsNew = (from e in data.Entregas
                                               from entry in e.EntryDetalles
                                               where Entregas.Contains(e.Codigo)
                                               && e.Despacho.CodigoCarga == CodigoCarga
                                               select new
                                                   {
                                                       CodigoEntrega = e.Codigo,
                                                       CodigoPallet = entry.CodigoPalletBodega
                                                   })
                                            .ToList();


            


            foreach (var RecNew in RecepcionProducePalletsNew)
            {
                RecepcionProducePallet RecepcionProducePallestNew = new RecepcionProducePallet();
                RecepcionProducePallestNew.CodigoEntrega = RecNew.CodigoEntrega;
                RecepcionProducePallestNew.FolioPalletBodega = RecNew.CodigoPallet;
                RecepcionProducePallestNew.RecepcionProduce = RecepcionProduceNew;


                var CajasPallets = data.DespachoDetalleCajas
                                    .Where(ddc => ddc.CodigoEntrega == RecNew.CodigoEntrega).ToList();


                foreach (DespachoDetalleCaja ddc in CajasPallets)
                {
                    RecepcionProduceDetallePalletCaja RecepcionProduceDetallePalletCajaNew = new RecepcionProduceDetallePalletCaja();
                    RecepcionProduceDetallePalletCajaNew.DespachoDetalleCaja = ddc;
                    ddc.TieneRecepcion = true;
                    RecepcionProduceDetallePalletCajaNew.RecepcionProducePallet = RecepcionProducePallestNew;
                    RecepcionProducePallestNew.RecepcionProduceDetallePalletCajas.Add(RecepcionProduceDetallePalletCajaNew);
                }
                
                RecepcionProduceNew.RecepcionProducePallets.Add(RecepcionProducePallestNew);
            }

            
            data.SubmitChanges();
                                                
            return "";
        }
        static internal string GetAllEntriesByCodigoEntry(int CodigoEntry)
        {
            SqlCommand Command = new SqlCommand(string.Format(string.Format(@"Select
		                                                Entry.*,
		                                                    (Select EntryDetalle.* from EntryDetalle 
			                                                    where EntryDetalle.CodigoEntry = Entry.CodigoEntry
			                                                    For xml path('EntryDetalle'),type
		                                                    )	
		                                                from Entry
		                                                Where Entry.CodigoEntry = '{0}'
	                                                    For xml path('Data'),root('Root')"
                                                        ,CodigoEntry))
                                                    );

            return Tools.CommandToJson(Command);
        }
        static internal string GetAllEntriesByCodigoBodega(int CodigoBodega)
        {
            SqlCommand Command = new SqlCommand(string.Format(string.Format(@"Select
		                                                                            Entry.*,
		                                                                            Bodega.NombreBodega,
		                                                                            Frontera.Nombre as NombreFrontera,
		                                                                            Agricultor.Nombre as NombreAgricultor,
			                                                                            (Select EntryDetalle.* from EntryDetalle 
				                                                                            where EntryDetalle.CodigoEntry = Entry.CodigoEntry
				                                                                            For xml path('EntryDetalle'),type
			                                                                            )
			
		                                                                            from Entry
			                                                                            join Bodega on (Bodega.Codigo = Entry.CodigoBodega)
			                                                                            join Frontera on (Frontera.Codigo = Entry.CodigoFrontera) 
			                                                                            join AgenteAduanal on (AgenteAduanal.CodigoAgenteAduanal = Entry.CodigoAgenteAduanal) 
			                                                                            join Agricultor on (Agricultor.Codigo = Entry.CodigoAgricultor)
		                                                                            Where 
			                                                                            Entry.CodigoBodega = {0}
			                                                                            and Entry.FechaHoraCancelacion is null
		                                                                            For xml path('Data'),root('Root')"
                                                                                , CodigoBodega))
                                                                            );

            return Tools.CommandToJson(Command);
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
        internal static void ApproveEntryByCodigoEntry(int CodigoEntry,string FolioEntry)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            Entry Entry = data.Entries.SingleOrDefault(x => x.CodigoEntry == CodigoEntry);
            if ((bool)Entry.Aprobado)
            {
                throw new Exception("El Entry ya está aprobado");
            }else if((bool)!Entry.EntryCompleto){
                throw new Exception("El Entry no ha sido completado");
            }
            else
            {
                List<long> ListaEntregas = new List<long>();
                
                EntryClass EntryClass = CreateEntryClassFromEntryModel(Entry);
                foreach (EntryDetalleClass EntryDetalleClass in EntryClass.EntryDetalle)
                {
                    if (EntryDetalleClass.CodigoEntrega != null)
                    {
                        ListaEntregas.Add((long)EntryDetalleClass.CodigoEntrega);
                    }
                }

                List<int> Cargas = data.Entregas
                                .Where(x => ListaEntregas.Contains(x.Codigo))
                                .Select(x => x.Despacho.Carga.Codigo)
                                .Distinct().ToList();
                
                foreach (int Carga in Cargas)
                {
                    GenerateReception(Carga, EntryClass);
                }
                Entry.Aprobado = true;
                Entry.FolioEntry = FolioEntry;
                data.SubmitChanges();
            }
        }
        private static EntryClass CreateEntryClassFromEntryModel(Entry Entry)
        {
            EntryClass EntryClass = new EntryClass();

            EntryClass.CodigoEntry = Entry.CodigoEntry;
            EntryClass.CodigoAgricultor = Entry.CodigoAgricultor;
            //EntryClass.CodigoTransfer = (int)Entry.CodigoTransfer;
            EntryClass.CodigoFrontera = Entry.CodigoFrontera;
            EntryClass.FechaHoraCancelacion = Entry.FechaHoraCancelacion;
            EntryClass.FolioEntry = Entry.FolioEntry;
            EntryClass.CodigoBodega = (int)Entry.CodigoBodega;
            EntryClass.CodigoAgenteAduanal = (int)Entry.CodigoAgenteAduanal;

            List<EntryDetalleClass> ListaEntryDetalleClass = new List<EntryDetalleClass>();

            foreach (EntryDetalle EntryDetalle in Entry.EntryDetalles)
            {
                EntryDetalleClass ObjEntryDetalleClass = new EntryDetalleClass();
                ObjEntryDetalleClass.CodigoEntryDetalle = EntryDetalle.CodigoEntryDetalle;
                ObjEntryDetalleClass.CodigoEntry = EntryDetalle.CodigoEntry;
                ObjEntryDetalleClass.CodigoEntrega = EntryDetalle.CodigoEntrega;
                ObjEntryDetalleClass.CodigoPallet = EntryDetalle.CodigoPalletBodega;

                ListaEntryDetalleClass.Add(ObjEntryDetalleClass);
            }

            EntryClass.EntryDetalle = ListaEntryDetalleClass.ToArray();
            return EntryClass;
        }

        internal static void UpdateEntry(EntryClass[] Entries)
        {
            DBDataContext data = new DBDataContext(ConnectionString);

            EntryClass IncomingEntry = Entries[0];
            Entry EntryInDB = data.Entries.SingleOrDefault(x => x.CodigoEntry == IncomingEntry.CodigoEntry);
            EntryInDB.FolioEntry = IncomingEntry.FolioEntry;
            EntryInDB.EntryCompleto = IncomingEntry.EntryCompleto;
            data.SubmitChanges();

            if (EntryInDB == null)
            {
                throw new Exception("No se Encontró el Entry");
            }

            int CodigoEntry = IncomingEntry.CodigoEntry;

            foreach (EntryDetalleClass EntryDetalleClass in IncomingEntry.EntryDetalle)
            {
                EntryDetalle EntryDetalle = data.EntryDetalles
                                            .SingleOrDefault(x => x.CodigoEntry == CodigoEntry
                                                && x.CodigoEntrega == null
                                                && x.CodigoPalletBodega == EntryDetalleClass.CodigoPallet);
                if (EntryDetalle == null)
                {
                    throw new Exception("No se Encontró el Pallet");
                }
                else
                {
                    EntryDetalle.CodigoEntrega = EntryDetalleClass.CodigoEntrega;
                    data.SubmitChanges();
                }
            }

        }

        internal static void RemoveEntryDetalleByCodigoPalletBodega(ref EntryClass[] Entries,string[] invalid)
        {
            Entries[0].EntryDetalle = Entries[0].EntryDetalle
                                        .Where(x => !invalid.Contains(x.CodigoPallet))
                                        .ToArray();
        }
    }
}