using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos.Models.Prospecto
{

    public class PaspectoClass
    {
        public decimal id { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Colonia { get; set; }
        public Nullable<decimal> CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string RFC { get; set; }
        public string Status { get; set; }

    }
    public class UsersClass {
        public decimal Id { get; set; }
        public string Usuario { get; set; }
        public string Pass { get; set; }
        public string Token { get; set; }


    }
}