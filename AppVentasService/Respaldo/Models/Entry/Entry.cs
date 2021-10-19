using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Entry
{
    public class EntryClass
    {        
        public int CodigoEntry { get; set; }
        public int CodigoAgricultor { get; set; }
        public int CodigoTransfer { get; set; }
        public int CodigoFrontera { get; set; }
        public DateTime? FechaHoraCancelacion { get; set; }
        public string FolioEntry { get; set; }
        public int CodigoBodega { get; set; }
        public int CodigoAgenteAduanal { get; set; }
        public string UsuarioCreador { get; set; }
        public bool EntryCompleto { get; set; }
        public EntryDetalleClass[] EntryDetalle { get; set; }
    }
}