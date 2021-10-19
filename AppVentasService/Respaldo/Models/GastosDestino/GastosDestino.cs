using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBodegaService.Models.GastosDestino
{
    public class GastosPickup
    {
        public int CodigoPickup { get; set; }
        public int Usuario { get; set; }
        public GastosDestino[] Gastos  { get; set; }
        public GastosDestino[] GastosEliminados { get; set; }
    }

    public class GastosDestino
    {
        public int CodigoGasto { get; set; }
        public int CodigoTipoGasto { get; set; }
        public string CodigoMoneda { get; set; }
        public decimal Importe { get; set; }
        public string Gasto { get; set; }
    }
}