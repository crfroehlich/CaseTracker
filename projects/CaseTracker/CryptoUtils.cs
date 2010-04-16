using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.IO;

namespace FogBugzCaseTracker
{
    public class CryptoUtils
    {
        public static void VerifyMd5(string filename, string expectedHash)
        {
            Utils.Log.InfoFormat("Verifying downloaded setup MD5 {0}, {1}", filename, expectedHash);
            string actualHashStr = ComputeFileHash(filename, new System.Security.Cryptography.MD5CryptoServiceProvider());
            if (actualHashStr != expectedHash)
            {
                Utils.Log.WarnFormat("Actual MD5 was: ", actualHashStr);
                File.Delete(filename);
                throw new Exception(String.Format("Bad hash of downloaded version.\nExpected: {0}\n  Actual: {1}", expectedHash, actualHashStr));
            }
        }

        public static string ComputeFileHash(string filename, System.Security.Cryptography.HashAlgorithm alg)
        {
            StringBuilder sb = new StringBuilder();
            byte[] actualHash = alg.ComputeHash(new FileStream(filename, FileMode.Open, FileAccess.Read));
            foreach (Byte b in actualHash)
                sb.Append(String.Format("{0,2:X}", b));
            return sb.ToString();
        }

    }
}
