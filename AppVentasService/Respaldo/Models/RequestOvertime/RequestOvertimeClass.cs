using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models
{
    public class RequestOvertimeClass
    {
        public long Codigo { get; set; }
        public int CodigoTipoRequest { get; set; }
        public string DescripcionMotivo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string JOBID { get; set; }
        public RequestOvertimeDetalleClass RequestOvertimeDetalle { get; set; }
    }
}