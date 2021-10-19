
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TripleH.Erp.Model;

namespace AppBodegaService.Models.Evaluacion
{
    //public class ParametrosMuestra
    //{
    //    public MuestraClase[] Muestras { get; set; }
    //    public MuestraCargas[] MuestrasCarga { get; set; }
    //    public FotoMuestra[] FotosMuestras { get; set; }
    //}

    public class MuestraClase
    {
        public string CodigoHeroku { get; set; }
        public bool Sel { get; set; }
        public int CodigoCarga { get; set; }
        public int CantidadUnidadCaja { get; set; }
        public int CantidadMuestra { get; set; }
        public decimal Peso { get; set; }
        public string Nota { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreAgricultor { get; set; }
        public string NombreProducto { get; set; }
        public int TipoMuestra { get; set; }
        public int CodigoCultivo { get; set; }
        public int CodigoAgricultor { get; set; }
        public int CodigoEmpaque { get; set; }
        public int CodigoRecepcion { get; set; }
        public bool EsIntegra { get; set; }
        public bool SinFotos { get; set; }
        public DateTime FechaMuestra { get; set; }
        public EvaluacionFoto[] Fotos { get; set; }
        public Defectos[] Defectos { get; set; }
        public MuestraTamaño[] Tamanos { get; set; }
        public bool ListoSincronizar { get; set; }
        public bool SincronizadoHeroku { get; set; }
        public List<MuestraColor> Colores { get; set; }
        public List<MuestraColoresTarima> ColoresTarimas { get; set; }
    }

    //public class MuestraFotos
    //{
    //    public string CodigoHeroku { get; set; }
    //    public string IdEvaluacion { get; set; }
    //    public DateTime FechaCreacion { get; set; }
    //    public string Data { get; set; }
    //    public string URL { get; set; }
    //}


    //public class FotoMuestra
    //{
    //    public string _id { get; set; }
    //    public string CodigoHeroku { get; set; }
    //    public string IdEvaluacion { get; set; }
    //    public DateTime FechaCreacion { get; set; }
    //    public string Data { get; set; }
    //    public string URL { get; set; }
    //    public string NombreFoto { get; set; }
    //}

    //public class MuestraCargas
    //{
    //    public string _id { get; set; }
    //    public int CodigoCarga { get; set; }
    //    public string CodigoMuestra { get; set; }
    //    public int CodigoEmpaque { get; set; }
    //    public string CodigoHeroku { get; set; }
    //}

    
        
    
}