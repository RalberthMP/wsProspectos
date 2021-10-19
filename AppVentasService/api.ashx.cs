using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSprospectos
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    public class api : IHttpHandler
    {
        class VariableUrl
        {
            public static string Catalogo { get { return "Catalogo"; } }
            public static string FechaCreacion { get { return "FechaCreacion"; } }
            public static string FechaModificacion { get { return "FechaModificacion"; } }
            public static string CodigoEmpaque { get { return "CodigoEmpaque"; } }
        }

        public void ProcessRequest(HttpContext context)
        {
            ChangeContentType(ref context);

            string Catalogo = context.Request.QueryString[VariableUrl.Catalogo];
            string FechaCreacion = context.Request.QueryString[VariableUrl.FechaCreacion];
            string FechaModificacion = context.Request.QueryString[VariableUrl.FechaModificacion];
            string CodigoEmpaque = context.Request.QueryString[VariableUrl.CodigoEmpaque];

 //           DateTime? FechaCreacionDate = GetDate(FechaCreacion);
 //           DateTime? FechaModificacionDate = GetDate(FechaModificacion);

 //           if (Catalogo == "Bodega")
 //           {
 //               GetBodega(context);
 //           }
 //           else if (Catalogo == "Carga")
 //           {
 //               GetCargas(context);
 //           }
 //           else if (Catalogo == "Recepcion")
 //           {
 //               GetRecepcion(context);
 //           }
 //           else if (Catalogo == "Pallet")
 //           {
 //               GetPallets(context);
 //           }
 //           else if (Catalogo == "UsuarioBodega")
 //           {
 //               GetUsuarioBodega(context);
 //           }
 //           else if (Catalogo == "AgenteAduanal")
 //           {
 //               GetAgenteAduanal(context);
 //           }
 //           else if (Catalogo == "Afectacion")
 //           {
 //               GetAfectacion(context);
 //           }
 //           else if (Catalogo == "CausasOvertime")
 //           {
 //               GetCausasOvertime(context);
 //           }
 //           else if (Catalogo == "ProductoCarga")
 //           {
 //               GetProductoCarga(context);
 //           }
 //           else if (Catalogo == "GetCargaBodegaAjuste")
 //           {
 //               GetCargaBodegaAjuste(context);
 //           }
 //           else if (Catalogo == "GetCargaBodegaRecepcion")
 //           {
 //               GetCargaBodegaRecepcion(context);
 //           }
 //           else if (Catalogo == "GetProductoCargaBodegaAjuste")
 //           {
 //               GetProductoCargaBodegaAjuste(context);
 //           }
 //           else if (Catalogo == "GetProductoCargaRecepcion")
 //           {
 //               GetProductoCargaRecepcion(context);
 //           }
 //           else if (Catalogo == "GetTipoAjuste")
 //           {
 //               GetTipoAjuste(context);
 //           }
 //           else if (Catalogo == "GetAgenteAduenal")
 //           {
 //               GetAgenteAduenal(context);
 //           }
 //           else if (Catalogo == "GetAgricultorCarga")
 //           {
 //               GetAgricultorCarga(context);
 //           }
 //           else if (Catalogo == "GetOvertime")
 //           {
 //               GetOvertime(context);
 //           }
 //           else if (Catalogo == "GetDatosBodegaOvertime")
 //           {
 //               GetDatosBodegaOvertime(context);
 //           }
 //           else if (Catalogo == "GetTipoAfectacion")
 //           {
 //               GetTipoAfectacion(context);
 //           }
 //           else if (Catalogo == "GetCausaOverTime")
 //           {
 //               GetCausaOverTime(context);
 //           }
 //           else if (Catalogo == "GetCompras")
 //           {
 //               GetCompras(context);
 //           }
 //           else if (Catalogo == "GetComprasDetalle")
 //           {
 //               GetComprasDetalle(context);
 //           }

       }
        
 //       private void GetRecepcion(HttpContext context)
 //       {
 //           string Resultado = Method.Catalogo.GetRecepcionaMetodo(int.Parse(context.Request.Params["CodigoBodega"]));
 //           context.Response.Write(Resultado);
 //       }
 //       private void GetProductoCarga(HttpContext context)
 //       {
 //           string Resultado = Method.Catalogo.GetProductoCargaMetodo(int.Parse(context.Request.Params["CodigoCarga"]));
 //           context.Response.Write(Resultado);
 //       }

 //       private void GetCausasOvertime(HttpContext context)
 //       {
 //           string Resultado = Method.Catalogo.GetCausasOvertime();
 //           context.Response.Write(Resultado);
 //       }

 //       private void GetAfectacion(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetAfectacion();
 //           context.Response.Write(Resultado);
 //       }

 //       private void GetAgenteAduanal(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetAgenteAduanal();
 //           context.Response.Write(Resultado);
 //       }

        private void ChangeContentType(ref HttpContext context)
        {
            context.Response.ContentType = "application/json";
        }
        
 //       private void GetBodega(HttpContext context)
 //       {
 //    //       string Resultado = Method.Catalogo.GetBodegaMetodo();
 //  //         context.Response.Write(Resultado);
 //       }
 //       private void GetCargas(HttpContext context)
 //       {
 //     //      string Resultado = Method.Catalogo.GetCargaMetodo(int.Parse(context.Request.Params["CodigoBodega"]));
 //   //        context.Response.Write(Resultado);
 //       }
 //       private void GetCargaBodegaAjuste(HttpContext context)
 //       {
 //     //      string Resultado = Method.Catalogo.GetCargaBodegaAjusteMetodo((context.Request.Params["CodigoAgricultor"]));
 //       //    context.Response.Write(Resultado);
 //       }

 //       private void GetCargaBodegaRecepcion(HttpContext context)
 //       {
 //      //     string Resultado = Method.Catalogo.GetCargaBodegaRecepcionMetodo((context.Request.Params["CodigoBodega"]));
 //    //       context.Response.Write(Resultado);
 //       }
 //       private void GetDatosBodegaOvertime(HttpContext context)
 //       {
 //       //    string Resultado = Method.Catalogo.GetDatosBodegaOvertime((context.Request.Params["CodigoBodega"]));
 //     //      context.Response.Write(Resultado);
 //       }
        
 //       private void GetTipoAfectacion(HttpContext context)
 //       {
 //   //        string Resultado = Method.Catalogo.GetTipoAfectacion();
 //   //        context.Response.Write(Resultado);
 //       }
 //       private void GetCausaOverTime(HttpContext context)
 //       {
 //   //        string Resultado = Method.Catalogo.GetCausaOverTime();
 //    //       context.Response.Write(Resultado);
 //       }
 //       private void GetProductoCargaBodegaAjuste(HttpContext context)
 //       {
 //    //       string Resultado = Method.Catalogo.GetProductoCargaBodegaAjuste(int.Parse(context.Request.Params["CodigoCarga"]));
 //   //        context.Response.Write(Resultado);
 //       }
 //       private void GetProductoCargaRecepcion(HttpContext context)
 //       {
 ////           string Resultado = Method.Catalogo.GetProductoCargaRecepcion(int.Parse(context.Request.Params["CodigoCarga"]));
 ////           context.Response.Write(Resultado);
 //       }

 //       private void GetTipoAjuste(HttpContext context)
 //       {
 //  //         string Resultado = Method.Catalogo.GetTipoAjuste();
 // //          context.Response.Write(Resultado);
 //       }
 //       private void GetAgricultorCarga(HttpContext context)
 //       {
 //   ////        string Resultado = Method.Catalogo.GetAgricultorCarga(int.Parse(context.Request.Params["CodigoCarga"]));
 //   //        context.Response.Write(Resultado);
 //       }
 //       private void GetOvertime(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetOvertime(int.Parse(context.Request.Params["CodigoCarga"]));
 //           //context.Response.Write(Resultado);
 //       }
 //       private void GetAgenteAduenal(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetAgenteAduenal();
 //           //context.Response.Write(Resultado);
 //       }
 //       private void GetPallets(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetPalletMetodo();
 //           //context.Response.Write(Resultado);
 //       }
 //       private void GetUsuarioBodega(HttpContext context)
 //       //{
 //       //    string Resultado = Method.Catalogo.GetUsuarioBodegaMetodo();
 //       //    context.Response.Write(Resultado);
 //       }
 //       private void GetCompras(HttpContext context)
 //       {
 //           //string Resultado = Method.Catalogo.GetCompras();
 //           //context.Response.Write(Resultado);
 //       }
 //       private void GetComprasDetalle(HttpContext context)
 //       {
 //           string Resultado ==""M
 //           context.Response.Write(Resultado);
 //       }

        #region Metodos de las Variables en la URL
        public DateTime? GetDate(string FechaConvertir)
        {
            DateTime date = new DateTime();
            if (DateTime.TryParseExact(FechaConvertir,
                         "yyyyMMdd",
                         System.Globalization.CultureInfo.InvariantCulture,
                         System.Globalization.DateTimeStyles.None,
                         out date))
            {
                return date;
            }
            else
            {
                return null;
            }


        }
        private int? GetCodigoEmpaque(string CodigoEmpaqueString)
        {
            int CodigoEmpaque;
            if (Int32.TryParse(CodigoEmpaqueString, out CodigoEmpaque))
            {
                return CodigoEmpaque;
            }
            else
            {
                return null;
            }

        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}