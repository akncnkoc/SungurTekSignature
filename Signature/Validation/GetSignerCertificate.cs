using System;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.asn.util;

/**
 * Gets signer certificate from BasedSignedData
 * @author orcun.ertugrul
 *
 */

namespace SungurTekSignature.Signature.Sign.Validation
{
    [TestFixture]
    internal class GetSignerCertificate
    {
        /***
	 * Gets certificate of the first signature.
	 * @throws Exception
	 */

        [Test]
        public void testGetCertificate()
        {
            byte[] sign = AsnIO.dosyadanOKU(TestConstants.getDirectory() + @"\testVerileri\BES-1.p7s");
            BaseSignedData bs = new BaseSignedData(sign);
            Console.WriteLine("Certificate Owner: " +
                              bs.getSignerList()[0].getSignerCertificate().getSubject().getCommonNameAttribute());
        }
    }
}