using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Evaluacion
{
    public class MuestraColoresTarima
    {
        public int id { get; set; }
        public int CodigoColor { get; set; }
        public string NombreColor { get; set; }
        public string urlImg { get; set; }
        public int RangoPallets { get; set; }        
    }
}