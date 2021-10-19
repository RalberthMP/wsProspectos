using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Entry
{
    public class EntryDetalleClass
    {
        public int CodigoEntryDetalle { get; set; }
        public int CodigoEntry { get; set; }
        public long? CodigoEntrega { get; set; }
        public string CodigoPallet { get; set; }
    }
}