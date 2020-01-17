using Microsoft.VisualStudio.TestTools.UnitTesting;
using EstouAVer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EstouAVer.Tests
{
    [TestClass()]
    public class HashCodeSHA256Tests
    {
        [TestMethod()]
        public void GenerateFromTextTestCorrect()
        {
            string expected = "a8a2f6ebe286697c527eb35a58b5539532e9b3ae3b64d4eb0a46fb657b41562c";
            string returned = SHA256Code.GenerateFromText("This is a test.");

            Assert.AreEqual(expected, returned);
        }

        [TestMethod()]
        public void GenerateFromTextTestNonASCII()
        {
            string expected = "4e026be40a0d0c07c8e218640c2770010943b0c3c94ededd2742787a6732e000";
            string returned = SHA256Code.GenerateFromText("Olá, isto é denovo apenas outro teste.");

            Assert.AreEqual(expected, returned);
        }

        [TestMethod()]
        public void GenerateFromTextTestEmpty()
        {
            string expected = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
            string returned = SHA256Code.GenerateFromText(string.Empty);

            Assert.AreEqual(expected, returned);
        }
    }
}