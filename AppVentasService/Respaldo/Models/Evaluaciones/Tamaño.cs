using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Evaluacion
{
    public class MuestraTamaño
    {
        public int CodigoCultivo { get; set; }
        public int CodigoTamanio { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
    }
}