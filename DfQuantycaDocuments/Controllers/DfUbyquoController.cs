using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Configuration;
using DfQuantycaDocuments.Helpers;
using DfQuantycaDocuments.Models;
using System.IO;
using System.Web.Http.Tracing;

namespace DfQuantycaDocuments.Controllers
{
    [RoutePrefix("api/DfUbyquo")]
    public class DfUbyquoController : ApiController
    {
        private readonly string ubyquoImportPath = String.Empty;

        public DfUbyquoController()
        {
            ubyquoImportPath = ConfigurationManager.AppSettings.Get("ubyquoImportPath");

        }

        
        private void WriteToTrace(string category, string log, System.Web.Http.Tracing.TraceLevel level)

        {

            ITraceWriter trceriter = Configuration.Services.GetTraceWriter();
            
            if (trceriter != null)
            {
                trceriter.Trace(
                    Request, category, level,
                    (traceRecord) =>
                    {
                        traceRecord.Message =
                            String.Format("DfQuantycaDocuments Log: {0}", log);
                    });
            }
        }

        [Route("docs/upload/")]
        [HttpPost]
        public async Task<IHttpActionResult> uploadDocument()
        {
            WriteToTrace("Controllers", "Start upload document", System.Web.Http.Tracing.TraceLevel.Info);


            Dictionary<string, string> attributes = new Dictionary<string, string>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    if (file.Headers.ContentDisposition.FileName != null)
                    {
                        var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                        var buffer = await file.ReadAsByteArrayAsync();

                        if (files.Count() == 0)
                            files.Add(filename, buffer);
                        else
                        {
                            WriteToTrace("Controllers", "Only accept single file", System.Web.Http.Tracing.TraceLevel.Error);
                            return BadRequest("Only accept single file");
                        }
                    }
                    else
                    {
                        foreach (NameValueHeaderValue p in file.Headers.ContentDisposition.Parameters)
                        {
                            string paramName = p.Value;
                            if (paramName.StartsWith("\"") && paramName.EndsWith("\"")) paramName = paramName.Substring(1, paramName.Length - 2);
                            string value = await file.ReadAsStringAsync();
                            if (paramName.Equals("companyID")
                                    || paramName.Equals("companyCIF")
                                    || paramName.Equals("companyName")
                                    || paramName.Equals("fiscalExercice")
                                    || paramName.Equals("documentType"))
                            {

                                if (attributes.Keys.Contains(paramName))
                                    return BadRequest("Duplicate parameter");

                                attributes.Add(paramName, value);
                            }
                            else
                            {
                                WriteToTrace("Controllers", "Invalid parameter", System.Web.Http.Tracing.TraceLevel.Error);
                                return BadRequest("Invalid parameter");
                            }

                        }
                    }
                }

                if(attributes.Count() != 5)
                {
                    WriteToTrace("Controllers", "Invalid parameter", System.Web.Http.Tracing.TraceLevel.Error);
                    return BadRequest("Missing parameters");
                }
                    

                //Read Ubyquo Import Index.xml File

                var fullUbyquoImportPath = ubyquoImportPath + "\\Index.xml";

                if (!File.Exists(fullUbyquoImportPath))
                {
                    WriteToTrace("Controllers", "Error loading Index.xml from " + ubyquoImportPath + "\\Index.xml", System.Web.Http.Tracing.TraceLevel.Error);
                    return InternalServerError();
                }

                string ubyquoFoldersInfoString = File.ReadAllText(fullUbyquoImportPath);

                var ubyquoFoldersInfo = XmlParser.toObject<Index>(ubyquoFoldersInfoString);

               var foldersLevel1 = ubyquoFoldersInfo.FolderLevel1.FirstOrDefault( p => p.MasterId.Equals(attributes["companyID"]) 
                                                                              && p.Detail.Equals(attributes["companyCIF"])
                                                                              && p.Name.Equals(attributes["companyName"])
                                                                              );
                if (foldersLevel1 == null)
                {
                    WriteToTrace("Controllers", "Invalid input data for companyID, companyCIF or companyName. Path not found in Index.xml", System.Web.Http.Tracing.TraceLevel.Error);
                    return BadRequest("Invalid input data for companyID, companyCIF or companyName. ");
                }

                string folderPath = foldersLevel1.FolderLevel2.FirstOrDefault(p => p.Name.Equals(attributes["fiscalExercice"])).Path;

                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    WriteToTrace("Controllers", "Invalid input data for fiscalExercice. Path not found in Index.xml", System.Web.Http.Tracing.TraceLevel.Error);
                    return BadRequest("Invalid input data for fiscalExercice.");
                }

                if (attributes["documentType"].Equals("Factura Recibida"))
                {
                    folderPath = folderPath + "\\Factura Recibida\\";
                }
                else
                {
                    folderPath = folderPath + "\\Factura Emitida\\";
                }

                folderPath = ConfigurationManager.AppSettings.Get("ubyquoImportPath") + "\\"+ folderPath;
                //var newFileName = Guid.NewGuid().ToString() + ".pdf";
                //var file = files.FirstOrDefault().Value;


                var fileName = files.FirstOrDefault().Key;

                WriteToTrace("Controllers", "Try to create file in: " + folderPath + fileName, System.Web.Http.Tracing.TraceLevel.Info);
                File.WriteAllBytes(folderPath + fileName, files.FirstOrDefault().Value);

                WriteToTrace("Controllers","File "+ fileName + " upload success", System.Web.Http.Tracing.TraceLevel.Info);
                return Ok();
                
            }
            catch (Exception Ex)
            {

                WriteToTrace("Controllers", Ex.Message.ToString(), System.Web.Http.Tracing.TraceLevel.Error);
                return InternalServerError();
            }
        }

        [Route("docs/test/")]
        [HttpGet]
        public IHttpActionResult TestApi()
        {

            WriteToTrace("Controllers", "Test Success", System.Web.Http.Tracing.TraceLevel.Info);
            
            return Ok("Service online");
        }
             
    }
}
