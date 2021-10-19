using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos.Models
{
    public class ProspectosClass
    {
        public decimal id { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Colonia { get; set; }
        public int CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string RFC { get; set; }
        public string Status { get; set; }
        public string Comentarios { get; set; }
    }
    public class FileClass
    {
        public string NombreFoto { get; set; }
        public string NombreFotoCompleto { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public string TipoArchivo { get; set; }
        
         public int IdProspecto { get; set; }
    }
}