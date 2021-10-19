using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using AppBodegaService.Method;
using AppBodegaService.Method.GastosDestino;
using System.Configuration;
using AppBodegaService.Models.GastosDestino;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace AppBodegaService.Controllers
{
    public class GastosDestinoController : ApiController
    {
        [HttpGet]
        [Route("api/GastosDestino/GetBodegas")]
        public HttpResponseMessage GetBodegas()
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetBodegas();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetTipoGastos")]
        public HttpResponseMessage GetTipoGastos()
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetTipoGastos();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetMonedas")]
        public HttpResponseMessage GetMonedas()
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetMonedas();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetClientes")]
        public HttpResponseMessage GetClientes([FromUri] int CodigoBodega)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetClientes(CodigoBodega);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetOrdenes")]
        public HttpResponseMessage GetOrdenes([FromUri] string CodigoCliente)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetOrdenes(CodigoCliente);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetPickups")]
        public HttpResponseMessage GetPickups([FromUri] int CodigoOrden)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetPickups(CodigoOrden);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/GastosDestino/GetGastos")]
        public HttpResponseMessage GetGastos([FromUri] int CodigoPickup)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = GastosDestinoMethod.GetGastos(CodigoPickup);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        
        [HttpPost]
        [Route("api/GastosDestino/GuardaGastos")]
        public HttpResponseMessage GuardaGastos([FromBody] GastosPickup GastosPickup)
        {
            try
            {
                var message = GastosDestinoMethod.GuardaGastos(GastosPickup);
                HttpResponseMessage Response = new HttpResponseMessage();
                Response.StatusCode = HttpStatusCode.OK;
                if (message != "")
                    Response.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                return Response;
            }
            catch (Exception ex)
            {
                HttpResponseMessage ResponseE = new HttpResponseMessage();
                ResponseE.StatusCode = HttpStatusCode.BadRequest;
                ResponseE.Content = new StringContent(ex.Message + ex.StackTrace);
                return ResponseE;
            }
        }
    }
}