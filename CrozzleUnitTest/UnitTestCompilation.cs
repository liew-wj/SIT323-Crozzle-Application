using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CrozzleApplication;

namespace CrozzleUnitTest
{
    [TestClass]
    public class UnitTestCompilation
    {
        [TestMethod]
        public void Validator_IsBoolean_FalseResult()
        {
            /// Arrange.
            String aString = "false";
            Boolean aBoolean = true;

            /// Act.
            Validator.IsBoolean(aString, out aBoolean);

            /// Assert.
            Assert.IsFalse(aBoolean);
        }

        [TestMethod]
        public void Validator_IsInt32_TrueResult()
        {
            /// Arrange.
            String aString = "500";
            Int32 aInteger = -1;
            Boolean aBoolean = true;

            /// Act.
            aBoolean = Validator.IsInt32(aString, out aInteger);

            /// Assert.
            Assert.IsTrue(aBoolean);
        }

        [TestMethod]
        public void Validator_IsHexColourCode_FalseResult()
        {
            /// Arrange
            String aString = "#4EF"; /// This is a shorthand hex code.
            Boolean aBoolean = true;

            /// Act.
            aBoolean = Validator.IsHexColourCode(aString);

            /// Assert.
            Assert.IsFalse(aBoolean);
        }

        [TestMethod]
        public void KeyValue_TryParse_TrueResult()
        {
            /// Arrange
            String aOriginalKeyValueData = "INVALID-CROZZLE-SCORE=\"INVALID CROZZLE\"",
                aKeyPattern = "INVALID-CROZZLE-SCORE";
            Boolean aBoolean = true;

            KeyValue aKeyValue = new KeyValue(null)
            {
                OriginalKeyValue = "INVALID-CROZZLE-SCORE=\"INVALID CROZZLE\"",
                Valid = true,
                Key = "INVALID-CROZZLE-SCORE",
                Value = "\"INVALID CROZZLE\""
            },
            bKeyValue = null;

            /// Act.
            KeyValue.TryParse(aOriginalKeyValueData, aKeyPattern, out bKeyValue);
            if (aKeyValue.OriginalKeyValue != bKeyValue.OriginalKeyValue ||
                aKeyValue.Key != bKeyValue.Key ||
                aKeyValue.Valid != bKeyValue.Valid ||
                aKeyValue.Value != bKeyValue.Value)
                aBoolean = false;

            /// Assert.
            Assert.IsTrue(aBoolean);
        }

        [TestMethod]
        public void Crozzle_Score_FalseResult()
        {
        }

        [TestMethod]
        public void CrozzleSequence_CheckDuplicate_TrueResult()
        {
            // ...
        }

        [TestMethod]
        public void Crozzle_Validate_FalseResult()
        {
            // ...
        }

        [TestMethod]
        public void Crozzle_ToStringHTML_TrueResult()
        {
            // ...
        }

        [TestMethod]
        public void CrozzleMap_GroupCount_FalseResult()
        {
            // ...
        }

        [TestMethod]
        public void Configuration_TryParse_TrueResult()
        {
            /// Arrange.
            Configuration aConfig;
            String path = @"..\..\Resources\Test1.cfg";
            Boolean result;

            /// Act.
            result = Configuration.TryParse(path, out aConfig);

            /// Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Wordlist_TryParse_FalseResult()
        {
            // ...
        }

        [TestMethod]
        public void Crozzle_TryParse_TrueResult()
        {
            // ...
        }
    }
}
