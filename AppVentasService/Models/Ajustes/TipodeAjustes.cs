using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos.Models.TiposdeAjustes
{
    public class TipoAjustesClass
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Comprobante { get; set; }
        public int CodigoTipoAjuste { get; set; }
        public int CodigoTipoOperacion { get; set; }
                   
        public List <AjustesArchivos> ArchivosAjustes { get; set; }
    }
    public class AjustesArchivos
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }


    }

    public class AjustesInfoClass
    {
        public string CodigoAjuste { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public string FechaAutorizado { get; set; }
        public string NombreBodega { get; set; }
        public string CodigoDetalle { get; set; }
        public string CodigoCarga { get; set; }
        public string FolioCarga { get; set; }
        public string TipoAjusteBodega { get; set; }
        public string ClasificacionAjuste { get; set; }
        
            
    }

    public class AjustesInfoPaletClass
    {
        public string CodigoEntrega { get; set; }       
        public string Cantidad { get; set; }
        public string CodigoPallet { get; set; }
        public string CodigoProducto { get; set; }
        public string Producto { get; set; }
        public string Agricultor { get; set; }
        public string CodigoCarga { get; set; }

    }

    public class AjustesVisorClass
    {
        public string Codigo { get; set; }
        public string Bodega { get; set; }
        public string CodigoAjuste { get; set; }
        public string TipoAjuste { get; set; }
        public string Observaciones { get; set; }
        public string Fecha { get; set; }

    }
    public class AjustesInfoArchivoClass
    {
        public string NombreArchivo { get; set; }

    }
    public class AgricultorClass
    {
        public string CodigoAgricultor { get; set; }
        public string NombreAgricultor { get; set; }
    }
}