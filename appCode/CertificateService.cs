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

namespace ssowebapp.appCode
{
    public class CertificateService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate, string arn)
        {
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

            //var cert = new X509Certificate2(Path.Combine("ccissocertificate"), "admin123");
            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }

            return false;
        }

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
