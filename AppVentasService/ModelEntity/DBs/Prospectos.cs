//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSprospectos.ModelEntity.DBs
{
    using System;
    using System.Collections.Generic;
    
    public partial class Prospectos
    {
        public decimal id { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Colonia { get; set; }
        public Nullable<int> CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string RFC { get; set; }
        public string Status { get; set; }
        public string Comentarios { get; set; }
        public string NombreArchivo { get; set; }
    }
}