using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using PoissonSoft.CommonUtils.Security;
using Xunit;

namespace PoissonSoft.CommonUtils.Tests.Security
{
    public class NppCryptDecoderTests
    {
        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void ReadAllFileAsText()
        {
            const string text =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et " +
                "dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " +
                "ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu " +
                "fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt " +
                "mollit anim id est laborum.";
            
            const string password = "123456789";
            
            var encryptedFileContent = 
                "<nppcrypt version=\"1016\">" + Environment.NewLine +
                "<encryption cipher=\"rijndael\" key-length=\"32\" mode=\"cbc\" encoding=\"base64\" />" + Environment.NewLine +
                "<key algorithm=\"scrypt\" N=\"16384\" r=\"8\" p=\"1\" salt=\"PU02bI6MbfBW/7lBeF5hbA==\" />" + Environment.NewLine +
                "<iv value=\"1Hez2mbSM82/sPHYrYs7Fg==\" method=\"random\" />" + Environment.NewLine +
                "</nppcrypt>" + Environment.NewLine +
                "nhujoCFDdAuUoRVm6PxA7982lpOB/VZA0FPfFoRMFAC3vTS5zR5S5vIS2OpTAE57Y3ys9CyrKPjy1iMqW3/ON8CAUY9NvBZjA01Zlt4/+cLddBfv/Z1TF6JmQD9X1OvW" + Environment.NewLine +
                "y1YjSz5iUwVNZ/yLpHaH66+G4xs31+1MkGqCrmVJ03chQAZfuQbld/+ZU4tncJBMYzjSFpGbPay2lP2TS3ChO+7EYOlQn9H+I2cY4PRcqxFAKEvlpAlRomJOnk6Ei2Fy" + Environment.NewLine +
                "V7/QDLpgTMk6FYsJ50ysiigzcEku4DiK5aAw3s+vFW1nzWFwyRlZWzFBwpSXyaK4VOMmTwMjchKH1u4KrikCYXjbnfQ6+RqY1ElKQUfKT/cOuqjaAsHryvVkcqppjII/" + Environment.NewLine +
                "oYGMHlvuTv6hDxTrmhp2adzaV9eYUrNNivLaTYbSGY0jw0eTYaxhalqwJUDAERqPMkMrPn9wNZ4ZkcM3O1yfFwL5k+pCI+/wIw4TRxqIvjmDBGK/y0IPZAHvBN5r4GcW" + Environment.NewLine +
                "7KYO4lThIccWw3gkdVOfzOjZ2T7l6Vwfr+hmGsUxXgROSi4W92byXwE1x3LqjPeRTL4+tyMdvz+xehu74JbefA==" + Environment.NewLine;
            
            const string fileName = "example.nppcrypt";
            File.WriteAllText(fileName, encryptedFileContent);

            var decryptedFileContent = NppCryptDecoder.ReadAllFileAsText(fileName, password);

            Assert.Equal(text, decryptedFileContent);
        }
    }
}
