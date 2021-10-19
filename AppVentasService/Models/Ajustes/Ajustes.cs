using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos.Models.Ajutes
{
    
    public class AjustesClass
    {        
        public int CodigoBodega { get; set; }
        public int CodUsuario { get; set; }
        public string Observaciones { get; set; }
        public int CodigoTipoAjuste { get; set; }
        public int CodigoClasificacionAjuste { get; set; }
        public string FolioCertificado { get; set; }
        public string NombreArchivo { get; set; }
        public AjustesProductoClass[] ProductoPallet { get; set; }
    }
    
    public class AjustesProductoClass
    {
        public int CodigoAgricultor { get; set; }
        public int Agricultor { get; set; }
        public string CodigoPallet { get; set; }
        public string CodigoProducto { get; set; }
        public string CodigoProductoNvo { get; set; }
        public int CodigoInspeccion { get; set; }
        public int CantidadAjustar { get; set; }
        public int Cantidad { get; set; }
        public int CodigoEntrega { get; set; }
        public int CodigoCarga { get; set; }

    }

    public class AjustesAutorizarClass
    {
        public int Codigo { get; set; }
        public string Bodega { get; set; }
        public int CodigoAjuste { get; set; }
        public string TipoAjuste { get; set; }
        public string Observaciones { get; set; }
        public string Fecha { get; set; }
        public string JobId { get; set; }
        public int CodigoTipoAjuste { get; set; }
        public string ActivityName { get; set; }
        public int NodeId { get; set; }

    }
        public class AjusteXdefinirClass
    {
        public int CodigoBodega { get; set; }
        public int CodigoAjuste { get; set; }
        public int CodUsuario { get; set; }
        public string Observaciones { get; set; }
        public int CodigoTipoAjuste { get; set; }
        public string TipoAjuste { get; set; }
        public string JobId { get; set; }
        public bool afectaAgricultor { get; set; }

        public List<AjustesDEtalleProductoPalletClass> ProductoPallet { get; set; }
        public List<AjustesDetalleCargaClass> DetalleCargas { get; set; }
        public List<AjustesDetalleAgricultorClass> DetalleAgricultores { get; set; }
    }

    public class AjustesDEtalleProductoPalletClass
    {
        public bool Seleccionado { get; set; }
        public int CodigoAgricultor { get; set; }
        public string Agricultor { get; set; }
        public int CodigoCarga { get; set; }
        public string CodigoPallet { get; set; }
        public string CodigoProducto { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public int CantidadAjustar { get; set; }
        public int CodigoEntrega { get; set; }

    }

    public class AjustesDetalleCargaClass
    {
        public int CodigoCarga { get; set; }

    }

    public class AjustesDetalleAgricultorClass
    {
        public int CodigoAgricultor { get; set; }
        public String Agricultor { get; set; }

    }
}
