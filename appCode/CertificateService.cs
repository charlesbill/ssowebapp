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
                clientCertificate = cert;
                handler.ClientCertificates.Add(cert);
                var httpClient = new HttpClient(handler);

                if (clientCertificate.Thumbprint == cert.Thumbprint)
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
