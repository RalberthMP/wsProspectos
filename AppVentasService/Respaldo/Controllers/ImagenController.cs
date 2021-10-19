using Newtonsoft.Json;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using EmbarquesAppService.Models;
//using EmbarquesAppService.Method;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using AppBodegaService.Models.Evaluacion;
using AppBodegaService;
using AppBodegaService.Models.Ajutes;
using AppBodegaService.Method;
using AppBodegaService.Method.Ajustes;



namespace EmbarquesAppService.Controllers
{
    public class ImagenController : ApiController
    {
        // GET: api/Imagen
        public IEnumerable<string> Get()
        {
            return null;
        }

        // GET: api/Imagen/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Imagen
        /*public void Post([FromBody]DespachoImagen DespachoImagen)
         {
             int c = Imagen.GuardarImagen(DespachoImagen.data,DespachoImagen.despacho);
         }*/




        [HttpPost, Route("api/upload")]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {

                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    //Do whatever you want with filename and its binary data.
                    var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];

                    File.WriteAllBytes(rutaFoto + "\\" + filename, buffer);
                    //Event.Log(string.Format("Se recibió el archivo {0}", filename));
                }
                return Ok();
            }
            catch (Exception e)
            {
                //Event.Log(e.Message + e.StackTrace);
                throw new Exception(e.Message);
            }


        }


        [HttpPost, Route("api/upload2")]
        public HttpResponseMessage Upload2([FromBody] EvaluacionFoto[] Fotos)
        {
            HttpResponseMessage Response = new HttpResponseMessage();

            try
            {
                Response.StatusCode = HttpStatusCode.OK;
                //EntryMethod.UpdateEntry(Entries);
                var provider = new MultipartMemoryStreamProvider();
                //await Fotos.Content.ReadAsMultipartAsync(provider);
         
                foreach (EvaluacionFoto file in Fotos)
                {
                   if (file.NombreFotoCompleto != null)
                   { 
                        
                        var rutaFoto = ConfigurationManager.AppSettings["RutaFotos"];
                        var filename = file.NombreFotoCompleto.Trim('\"');
                        byte[] imageBytes = Convert.FromBase64String(file.Url);
                        // Convert byte[] to Image
                        using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                        {
                            // Image image = Image.FromStream(ms, true);
                            File.WriteAllBytes(rutaFoto + "\\" + filename, imageBytes);
                            // GuardarNombreArchivos
                            AjusteMethod.GuardarNombreArchivos(filename);
                        }
                    }
                 }
            
                SqlCommand Command2 = new SqlCommand(@"select max (codigo)  from AjusteBodega ");
                DataTable Resultado2 = Tools.CommandTextToDataTable(Command2);
                var cd = JsonConvert.SerializeObject(Resultado2);
                List<DataRow> list = Resultado2.AsEnumerable().ToList();
                var codAjuste = list[0].ItemArray[0];
                AjusteMethod.MandaCorreoAjuste(Convert.ToInt32( codAjuste));
                Tools.SetResponseContent(ref Response, "Ok");
                return Response;
            }
            catch (Exception ex)
            {
                Response.StatusCode = (HttpStatusCode)422;
                string Error = string.Format("No se pudo actualizar el Entry.");

                return Response;
            }

        }

        // PUT: api/Imagen/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/Imagen/5
        public void Delete(int id)
        {
        }
    }
}
