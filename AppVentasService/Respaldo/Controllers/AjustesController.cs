 using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using AppBodegaService.Models.Entry;
using AppBodegaService.Models.TiposdeAjustes;
using AppBodegaService.Models.Ajutes;
using AppBodegaService.Method;
using AppBodegaService.Method.Ajustes;
using System.Configuration;


namespace AppBodegaService.AjusteController
{
    
    public class AjustesController : ApiController
    {
        [HttpGet]
        [Route("api/Ajuste/GetClasiTiposAjuste")]
        public HttpResponseMessage GetClaseTipoAjuste([FromUri]string CodigoTipoAjuste="")
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetClasiTiposAjuste(CodigoTipoAjuste);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }
        [HttpGet]
        [Route("api/Ajuste/GetClasiTiposAjusteDetalle")]
        public HttpResponseMessage GetClasiTiposAjusteDetalle([FromUri]string CodigoTipArchivo)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetClasiTiposAjusteDetalle(CodigoTipArchivo);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }
        [HttpGet]
        [Route("api/Ajuste/GetInfoAjuste")]
        public HttpResponseMessage GetInfoAjuste([FromUri]string CodigoAjuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetAjuste(CodigoAjuste);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/Ajuste/GetInfoArchivos")]
        public HttpResponseMessage GetAjusteArchivos([FromUri]string CodigoAjuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            string Mensaje = AjusteMethod.GetAjusteArchivos(CodigoAjuste);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/Ajuste/GetInfoAjustePalet")]
        public HttpResponseMessage GetInfoAjustePalet([FromUri]string CodigoAjuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetAjustePallet(CodigoAjuste);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/Ajuste/GetClasiArchivos")]
        public HttpResponseMessage GetClaseArchivos([FromUri]string CodigoTipoAjuste = "")
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetClasiArchivos();

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

        [HttpGet]
        [Route("api/Ajuste/GetAgricultores")]
        public HttpResponseMessage GetAgricultores([FromUri]string CodigoBodega = "")
        {
            HttpResponseMessage Response = new HttpResponseMessage();


            string Mensaje = AjusteMethod.GetAgricultores(CodigoBodega);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }
        // POST: api/Entry

        [HttpPost]
        public HttpResponseMessage Post([FromBody] EntryClass[] Entries)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            string[] InvalidCodigoPalletBodega = EntryMethod.ValidateEntries(Entries);
            Log.SaveJsonInLog(Entries);            

            if (InvalidCodigoPalletBodega.Length > 0)
            {
                EntryMethod.RemoveEntryDetalleByCodigoPalletBodega(ref Entries, InvalidCodigoPalletBodega);
                string Mensaje = string.Format("Los siguientes Pallets ya están registrados: {0} ",
                                 string.Join(",", InvalidCodigoPalletBodega));
                Log.SaveLog(Mensaje);
            }

            if (Entries[0].EntryDetalle.Length == 0)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Mensaje = "Todos los pallets ya fueron registrados";
                Tools.SetResponseContent(ref Response,Mensaje);
                return Response;
            }

            try
            {
                EntryMethod.SaveEntries(Entries);
                Response.StatusCode = HttpStatusCode.OK;
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                if (ex.Message.Contains("No se pudo registrar el Entry"))
                {
                    Tools.SetResponseContent(ref Response, ex.ToString());
                }
                else
                {
                    Method.Log.SaveLog(ex.ToString());
                    //Method.Log.SaveJsonInLog(Entries);
                    Tools.SetResponseContent(ref Response, "Ha ocurrido un Error Interno");
                }
                return Response;
            }
            
        }

        [Route("api/Ajuste/SaveTipoAjuste")]
        [HttpPost]
        public HttpResponseMessage Actualizar([FromBody] TipoAjustesClass[]TipoAjustes){
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                AjusteMethod.GuardarTiposdeAjustes(TipoAjustes);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el tipo de ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }

        [Route("api/Ajuste/SaveClasifArchivos")]
        [HttpPost]
        public HttpResponseMessage SaveClasifArchivos([FromBody] TipoAjustesClass[] TipoAjustes)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                AjusteMethod.GuardarClasificacionArchivos(TipoAjustes);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el tipo de ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }


        [Route("api/Ajuste/SaveAjusteMerma")]
        [HttpPost]
        public HttpResponseMessage GuardarAjustesMerma([FromBody] AjustesClass[] Ajuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                AjusteMethod.GuardarAjustesMerma(Ajuste);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }
        
        [Route("api/Ajuste/SaveAjustePalletCaido")]
        [HttpPost]
        public HttpResponseMessage GuardarAjustesPalletCaido([FromBody] AjustesClass[] Ajuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                AjusteMethod.GuardarAjustesPalletCaido(Ajuste);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }

        [Route("api/Ajuste/SaveAjuste")]
        [HttpPost]
        public HttpResponseMessage GuardarAjustes([FromBody] AjustesClass[] Ajuste)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                AjusteMethod.GuardarAjustes(Ajuste);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar guardar el ajuste.");
                Log.SaveLog(Error);
                Log.SaveLog(ex.ToString());
                Tools.SetResponseContent(ref Response, Error);
                return Response;
            }

        }

        // PUT: api/Entry/5
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {

        }

        
        [Route("api/Entry/Approve")]
        [HttpPost]
        public HttpResponseMessage Approve(
            [FromUri] string FolioEntry,
            [FromUri]int CodigoEntry = 0
            )
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            if (CodigoEntry != 0)
            {
                try
                {
                    Response.StatusCode = HttpStatusCode.OK;
                    EntryMethod.ApproveEntryByCodigoEntry(CodigoEntry,FolioEntry);
                    return Response;
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (HttpStatusCode)422;
                    string Error = string.Format("No se pudo aprobar el Entry. CodigoEntry:{0}", CodigoEntry);
                    Log.SaveLog(Error);
                    Log.SaveLog(ex.ToString());
                    Tools.SetResponseContent(ref Response, Error);
                    return Response;
                }
            }

            Response.StatusCode = HttpStatusCode.BadRequest;
            Tools.SetResponseContent(ref Response, "No se encontró el parametro CodigoEntry");
            return Response;
        }

        // DELETE: api/Entry/5
        [HttpPost]
        [Route("api/Entry/Delete")]
        public HttpResponseMessage Delete([FromUri]int CodigoEntry=0)
        {
            HttpResponseMessage Response = new HttpResponseMessage();
            if (CodigoEntry != 0)
            {
                try
                {
                    Response.StatusCode = HttpStatusCode.OK;
                    EntryMethod.DeleteEntryByCodigoEntry(CodigoEntry);
                    return Response;
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (HttpStatusCode)422;
                    Log.SaveLog(ex.ToString());
                    string Error = string.Format("No se pudo eliminar el Entry CodigoEntry:{0}", CodigoEntry);
                    Log.SaveLog(Error);
                    Tools.SetResponseContent(ref Response, Error);
                    return Response;
                }
            }

            Response.StatusCode = HttpStatusCode.BadRequest;
            Tools.SetResponseContent(ref Response, "No se encontró el parametro CodigoEntry");
            return Response;
        }
        [HttpGet]
        [Route("api/Ajuste/getFile")]
        public HttpResponseMessage GetFile(string Archivo)
        {
            if (Archivo == "")
                return this.Request.CreateResponse(HttpStatusCode.BadRequest);
            string path = "Gasto.PDF";
             var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];
            //string nombrePdfArchivo = AjusteMethod.GetNombreArchivo(Archivo);
            string nombreArchivo = rutaFoto + Archivo;
            FileStream fileStream = new FileStream(nombreArchivo, FileMode.Open, FileAccess.Read);
            byte[] content = AjusteMethod.ReadFully((Stream)fileStream);
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = (HttpContent)new ByteArrayContent(content)
            };
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(Archivo);
            //httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            httpResponseMessage.Content.Headers.ContentLength = new long?(fileStream.Length);
            return httpResponseMessage;
        }
        [HttpGet]
        [Route("api/Ajuste/GetAjustes")]
        public HttpResponseMessage GetAjustes([FromUri]string Codigo,int TipoConsulta )
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            string Mensaje = AjusteMethod.GetAjustesVisor(Codigo, TipoConsulta);

            Tools.SetResponseContent(ref Response, Mensaje);

            return Response;
        }

    }


}
