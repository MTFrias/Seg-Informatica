using Microsoft.VisualStudio.TestTools.UnitTesting;
using EstouAVer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EstouAVer.Tests
{
    [TestClass()]
    public class AjudanteParaBDTests
    {
        [TestMethod()]
        public void SelectUserNoUser()
        {
            Assert.IsNull(AjudanteParaBD.SelectUserWithUsername("noUser"));
        }

        [TestMethod()]
        public void SelectUserUserS4nd()
        {
            Assert.IsNotNull(AjudanteParaBD.SelectUserWithUsername("S4nd"));
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