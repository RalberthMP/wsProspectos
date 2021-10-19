//using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.Evaluacion
{
    public class EvaluacionFoto
    {
        public string CodigoHeroku { get; set; }
        public string NombreFoto { get; set; }
        public string NombreFotoCompleto { get; set; }
        public string IdEvaluacion { get; set; }
        public string Creador {get;set;}
        public DateTime FechaCreacion { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public bool SincronizadoHeroku { get; set; }
        public string TipoArchivo { get; set; }

    }
}