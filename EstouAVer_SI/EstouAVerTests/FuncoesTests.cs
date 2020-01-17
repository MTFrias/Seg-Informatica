using Microsoft.VisualStudio.TestTools.UnitTesting;
using EstouAVer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EstouAVer.Tests
{
    [TestClass()]
    public class FuncoesTests
    {
        [TestMethod()]
        public void GenerateFromTextTestCorrect()
        {
            string returned = Funcoes.GenerateSalt().ToString();
            string a = Funcoes.GenerateSalt().ToString();
            Assert.AreEqual(a, returned);
        }
    }
}