using System.Net.Security;

namespace BuildingBlocks;

public static class CertificateValidationHandler
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/previous-versions/office/developer/exchange-server-2010/dd633677(v=exchg.80)?redirectedfrom=MSDN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    /// <param name="sslPolicyErrors"></param>
    /// <returns></returns>
    public static bool ValidationCallBack(
        object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        // If the certificate is a valid, signed certificate, return true.
        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            return true;
        }

        if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
        {
            Console.WriteLine("Certificate name mismatch for certificate: {0}", certificate.Subject);
            // Optionally, implement further validation or accept the mismatch in specific cases
            return true; // Accept the mismatch for this example
        }

        // If there are errors in the certificate chain, look at each error to determine the cause.
        if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
        {
            if (chain != null && chain.ChainStatus != null)
            {
                foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                {
                    if ((certificate.Subject == certificate.Issuer) &&
                        (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags
                            .UntrustedRoot))
                    {
                        // Self-signed certificates with an untrusted root are valid. 
                        continue;
                    }
                    else
                    {
                        if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags
                                .NoError)
                        {
                            Console.WriteLine("Certificate error: Subject:{0} | Code:{1} | Details:{2}",
                                certificate.Subject, status.Status, status.StatusInformation);
                            // If there are any other errors in the certificate chain, the certificate is invalid,
                            // so the method returns false.
                            return false;
                        }
                    }
                }
            }

            // When processing reaches this line, the only errors in the certificate chain are 
            // untrusted root errors for self-signed certificates. These certificates are valid
            // for default Exchange server installations, so return true.
            return true;
        }
        else
        {
            // In all other cases, return false.
            return false;
        }
    }
}