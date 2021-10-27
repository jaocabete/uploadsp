using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using PnP.Framework;
using File = Microsoft.SharePoint.Client.File;

namespace UploadSP 
{
    class Program
    {
        static void Main(string[] args)
        {
            string SiteUrl = "https://contoso.sharepoint.com/";
            string DocumentLibrary = "Documents";
            string FileName = @"C:\top-50-security-threats.pdf";
            string CustomerFolder = "test";
            string client = "";
            string secret = "";
            UploadFileToSharePoint(SiteUrl, DocumentLibrary, CustomerFolder, FileName, client, secret );

        }

        private static void UploadFileToSharePoint(string SiteUrl, string DocLibrary, string ClientSubFolder, string FileName, string client, string secret)
        {
            try
            {
                #region Insert the data
                using (ClientContext CContext = new AuthenticationManager().GetACSAppOnlyContext(SiteUrl, client, secret))
                {
                    
                    Web web = CContext.Web;
                    FileCreationInformation newFile = new FileCreationInformation();
                    byte[] FileContent = System.IO.File.ReadAllBytes(FileName);
                    newFile.ContentStream = new MemoryStream(FileContent);
                    newFile.Url = Path.GetFileName(FileName);
                    List DocumentLibrary = web.Lists.GetByTitle(DocLibrary);
                    //SP.Folder folder = DocumentLibrary.RootFolder.Folders.GetByUrl(ClientSubFolder);
                    Folder Clientfolder = DocumentLibrary.RootFolder.Folders.Add(ClientSubFolder);
                    Clientfolder.Update();
                    File uploadFile = Clientfolder.Files.Add(newFile);

                    CContext.Load(DocumentLibrary);
                    CContext.Load(uploadFile);
                    CContext.ExecuteQuery();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("The File has been uploaded" + Environment.NewLine + "FileUrl -->" + SiteUrl + "/" + DocLibrary + "/" + ClientSubFolder + "/" + Path.GetFileName(FileName));
                }
                #endregion
            }
            catch (Exception exp)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exp.Message + Environment.NewLine + exp.StackTrace);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
