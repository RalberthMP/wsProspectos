using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AppBodegaService.Models.Entry;
using AppBodegaService.Method;
using System.Text;
using Newtonsoft.Json;

namespace AppBodegaService.EntryController
{
    
    public class EntryController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get([FromUri] int CodigoEntry = 0,
            [FromUri] DateTime? FechaCreacion = null,
            [FromUri] int CodigoBodega=0)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            if (FechaCreacion != null && CodigoBodega != 0)
            {
                Response.StatusCode = HttpStatusCode.OK;
                Tools.SetResponseContent(ref Response,
                    EntryMethod.GetAllentriesByDateAndCodigoBodega(CodigoBodega,(DateTime)FechaCreacion));
                return Response;
            }

            //if (FechaCreacion != null)
            //{                
            //    Response.StatusCode = HttpStatusCode.OK;
            //    Tools.SetResponseContent(ref Response, 
            //        EntryMethod.GetAllEntriesByDate((DateTime)FechaCreacion));
            //    return Response;
            //}
            //else if (CodigoEntry != 0)
            //{
            //    Response.StatusCode = HttpStatusCode.OK;
            //    Tools.SetResponseContent(ref Response,
            //        EntryMethod.GetAllEntriesByCodigoEntry(CodigoEntry));
            //    return Response;
            //}
            //else if (CodigoBodega != 0)
            //{
            //    Response.StatusCode = HttpStatusCode.OK;
            //    Tools.SetResponseContent(ref Response,
            //        EntryMethod.GetAllEntriesByCodigoBodega(CodigoBodega));
            //    return Response;
            //}

            Response.StatusCode = HttpStatusCode.BadRequest;
            Tools.SetResponseContent(ref Response,"No se encontraron los parametros FechaCreacion y CodigoBodega");
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

        [Route("api/Entry/Actualizar")]
        [HttpPost]
        public HttpResponseMessage Actualizar([FromBody] EntryClass[]Entries){
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                EntryMethod.UpdateEntry(Entries);
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar el Entry.");
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
    }
}
