using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WSprospectos.Global
{
    internal class Configurations
    {
        static public string ConnectionString
        {
            get
            {
                return Connection();
            }
        }

        static public string RutaReporte
        {
            get
            {
                return GetRutaReporte();
            }
        }
        static public string RutaReporteOrdenVenta
        {
            get
            {
                return GetRutaReporteOrdenVenta();
            }
        }

        static public string CorreoGrupoVentas
        {
            get
            {
                return GetCorreoGrupoVentas();
            }
        }

        static private string Connection()
        {
            return ConfigurationManager.AppSettings["CString"];
        }

        static private string GetRutaReporte()
        {
            return ConfigurationManager.AppSettings["RutaReporte"];
        }
        static private string GetRutaReporteOrdenVenta()
        {
            return ConfigurationManager.AppSettings["RutaReporteOrdenVenta"];
        }
        static private string GetCorreoGrupoVentas()
        {
            return ConfigurationManager.AppSettings["CorreoGrupoVentas"];
        }
    }
}