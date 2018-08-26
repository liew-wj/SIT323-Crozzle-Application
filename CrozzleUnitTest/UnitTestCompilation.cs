using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CrozzleApplication;

namespace CrozzleUnitTest
{
    [TestClass]
    public class UnitTestCompilation
    {
        #region Scenario 1
        [TestMethod]
        public void Scenario1Case1()
        {
            String aString = "false";
            Boolean aBoolean = true;

            Assert.IsTrue(Validator.IsBoolean(aString, out aBoolean));
        }

        [TestMethod]
        public void Scenario1Case2()
        {
            String aString = "true";
            Boolean aBoolean = true;

            Assert.IsTrue(Validator.IsBoolean(aString, out aBoolean));
        }
        #endregion

        #region Scenario 2
        [TestMethod]
        public void Scenario2Case1()
        {
            String aString = "500";
            Int32 anInteger = -1;

            Assert.IsTrue(Validator.IsInt32(aString, out anInteger));
        }

        [TestMethod]
        public void Scenario2Case2()
        {
            String aString = "498";
            Int32 anInteger = -1;

            Assert.IsTrue(Validator.IsInt32(aString, out anInteger));
        }

        [TestMethod]
        public void Scenario2Case3()
        {
            String aString = "499.5";
            Int32 anInteger = -1;

            Assert.IsFalse(Validator.IsInt32(aString, out anInteger));
        }
        #endregion

        #region Scenario 3
        [TestMethod]
        public void Scenario3Case1()
        {
            String hexColour = "000000";

            Assert.IsFalse(Validator.IsHexColourCode(hexColour));
        }

        [TestMethod]
        public void Scenario3Case2()
        {
            String hexColour = "#56ab7f";

            Assert.IsTrue(Validator.IsHexColourCode(hexColour));
        }

        [TestMethod]
        public void Scenario3Case3()
        {
            String hexColour = "#4EF";

            Assert.IsTrue(Validator.IsHexColourCode(hexColour));
        }
        #endregion

        #region Scenario 4
        [TestMethod]
        public void Scenario4Case1()
        {
            String originalKeyValueData = "INVALID-CROZZLE-SCORE=\"INVALID CROZZLE\"",
                keyPattern = "INVALID-CROZZLE-SCORE";
            KeyValue aKeyValue = null;

            Assert.IsTrue(KeyValue.TryParse(originalKeyValueData, keyPattern, out aKeyValue));
        }

        [TestMethod]
        public void Scenario4Case2()
        {
            String originalKeyValueData = "INVALID-CROZZLE-SCORE=\"INVALID CROZZLE\"",
                keyPattern = "VALID-CROZZLE-SCORE";
            KeyValue aKeyValue = null;

            Assert.IsFalse(KeyValue.TryParse(originalKeyValueData, keyPattern, out aKeyValue));
        }

        [TestMethod]
        public void Scenario4Case3()
        {
            String originalKeyValueData = "VALID-CROZZLE-SCORE=\"INVALID CROZZLE\"",
                keyPattern = "VALID-CROZZLE-SCORE";
            KeyValue aKeyValue = null;

            Assert.IsTrue(KeyValue.TryParse(originalKeyValueData, keyPattern, out aKeyValue));
        }
        #endregion

        #region Scenario 6
        [TestMethod]
        public void Scenario6Case1()
        {

        }

        [TestMethod]
        public void Scenario6Case2()
        {

        }

        [TestMethod]
        public void Scenario6Case3()
        {

        }
        #endregion

        #region Scenario 8
        #endregion

        #region Scenario 9
        #endregion
    }
}
