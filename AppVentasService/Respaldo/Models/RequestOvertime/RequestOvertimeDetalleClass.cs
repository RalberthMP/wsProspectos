using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models
{
    public class RequestOvertimeDetalleClass
    {
        public long Codigo { get; set; }
        public long CodigoRequestOvertime { get; set; }
        public int? CodigoCarga { get; set; }
        public int? CodigoPickup { get; set; }
        public int? CodigoCausaOvertime { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public int? HorasOvertime { get; set; }
        public decimal? ImporteOvertime { get; set; }
        public int? CodigoAfectacionOvertime { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string FolioFactura { get; set; }
        public string DescripcionMotivoCancelacion { get; set; }
        public string NombreArchivo { get; set; }
        public string NombreArchivoOriginal { get; set; }
        public string DescripcionMotivoOverTime { get; set; }



    }
}