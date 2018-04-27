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

        [Route("docs/upload/")]
        [HttpPost]
        public async Task<IHttpActionResult> uploadDocument()
        {

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
                                return BadRequest("Invalid parameter");
                            }

                        }
                    }
                }

                if(attributes.Count() != 5)
                    return BadRequest("Missing parameters");

                //Read Ubyquo Import Index.xml File

                var fullUbyquoImportPath = ubyquoImportPath + "\\Index.xml";

                if (!File.Exists(fullUbyquoImportPath))
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Error loading Index.xml from " + ubyquoImportPath + "\\Index.xml", EventLogEntryType.Error);
                    }

                    return InternalServerError();
                }

                string ubyquoFoldersInfoString = File.ReadAllText(fullUbyquoImportPath);

                var ubyquoFoldersInfo = XmlParser.toObject<Index>(ubyquoFoldersInfoString);

               var foldersLevel1 = ubyquoFoldersInfo.FolderLevel1.FirstOrDefault( p => p.MasterId.Equals(attributes["companyID"]) 
                                                                              && p.Detail.Equals(attributes["companyCIF"])
                                                                              && p.Name.Equals(attributes["companyName"])
                                                                              );
                string folderPath = foldersLevel1.FolderLevel2.FirstOrDefault(p => p.Name.Equals(attributes["fiscalExercice"])).Path;

                if (attributes["documentType"].Equals("Factura Recibida"))
                {
                    folderPath = folderPath + "\\Factura Recibida\\";
                }
                else
                {
                    folderPath = folderPath + "\\Factura Emitida\\";
                }

                folderPath = ConfigurationManager.AppSettings.Get("ubyquoRootPath") + "\\"+ folderPath;
                var newFileName = Guid.NewGuid().ToString() + ".pdf";

                File.WriteAllBytes(folderPath + newFileName, files.FirstOrDefault().Value);

                return Ok();
                
            }
            catch (Exception Ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(Ex.Message.ToString(), EventLogEntryType.Error);
                }
                return InternalServerError();
            }
        }
    }
}
