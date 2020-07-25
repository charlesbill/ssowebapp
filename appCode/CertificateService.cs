using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.CertificateManager;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using Amazon.CertificateManager.Model;
using Amazon.CertificateManager.Model.Internal.MarshallTransformations;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using AWS.Logger.SeriLog;
using AWS.Logger;

namespace ssowebapp.appCode
{
    // Certificate service using Amazon Certificate Manager
    public class CertificateService
    {
        /// <summary>
        /// Validate the certificate on ARN. The ARN is stored in the appsettings.json. 
        /// 
        /// The X509Certificate2 is injected into this class.
        /// 
        /// </summary>
        /// <param name="clientCertificate"></param>
        /// <param name="arn"></param>
        /// <returns></returns>
        public bool ValidateCertificate(X509Certificate2 clientCertificate, string arn)
        {
            bool validCert = false;

            try
            {
                Log.Information("Before Validate Certificate");
                AmazonCertificateManagerClient client = new AmazonCertificateManagerClient();
                var certificates = client.GetCertificateAsync(arn).Result;

                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12
                };

                byte[] toBytes = Encoding.ASCII.GetBytes(certificates.Certificate);
                var cert = new X509Certificate2(toBytes);
                
                handler.ClientCertificates.Add(cert);
                var httpClient = new HttpClient(handler);
                    
                // I removed the issuer name for security reasons
                if (cert.IssuerName.Name == "Dummy Issues Name")
                {
                    validCert = true;
                    Log.Information("Valid Certificate Found!");
                }
            }
            catch(Exception ex)
            {
                Log.Error("Error in Validate Certificate: " + ex.Message);
            }

            return validCert;

        }

        // This is an alternative method how to validate certificates. This should be implemented by using an Interface. 

        //public GetCertificateResponse GetCertificate(string certificateArn)
        //{
        //    var request = new GetCertificateRequest();
        //   request.CertificateArn = certificateArn;

        //    return GetCertificate(request.CertificateArn);
        //}

        //public GetCertificateResponse GetCertificate(GetCertificateRequest request)
        //{
        //    var marshaller = new GetCertificateRequestMarshaller();
        //    var unmarshaller = GetCertificateResponseUnmarshaller.Instance;

        //    return Invoke<GetCertificateRequest, GetCertificateResponse>(request, marshaller, unmarshaller);
        //}

        //public IAsyncResult BeginGetCertificate(GetCertificateRequest request, AsyncCallback callback, object state)
        //{
        //    var marshaller = new GetCertificateRequestMarshaller();
        //    var unmarshaller = GetCertificateResponseUnmarshaller.Instance;

        //    return BeginInvoke<GetCertificateRequest>(request, marshaller, unmarshaller,
        //        callback, state);
        //}

        //public GetCertificateResponse EndGetCertificate(IAsyncResult asyncResult)
        //{
        //    return EndInvoke<GetCertificateResponse>(asyncResult);
        //}
    }
}
