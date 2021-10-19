using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Evaluacion
{
    public class Defectos
    {
        public int CodigoCultivo { get; set; }
        public int CodigoDefecto { get; set; }
        public int Danio { get; set; }
        public int Serio { get; set; }
        public int MuySerio { get; set; }
        public string Nombre { get; set; }
    }
}