using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CrozzleApplication;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            Boolean aBoolean;

            Assert.IsTrue(Validator.IsBoolean(aString, out aBoolean));
        }

        [TestMethod]
        public void Scenario1Case2()
        {
            String aString = "False";
            Boolean aBoolean;

            Assert.IsFalse(Validator.IsBoolean(aString, out aBoolean));
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

            Assert.IsFalse(Validator.IsHexColourCode(hexColour));
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
                keyPattern = "^VALID-CROZZLE-SCORE";
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
            CrozzleSequences crozzleSequences;
            Int32 lowerLimit, upperLimit;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lowerLimit = 0;
            upperLimit = 100;

            crozzleSequences = new CrozzleSequences(rows, columns, new Configuration("../../Resources/UnitTest1.cfg"));

            crozzleSequences.CheckDuplicateWords(lowerLimit, upperLimit);
            Assert.IsTrue(crozzleSequences.ErrorMessages.Count == 0);
        }

        [TestMethod]
        public void Scenario6Case2()
        {
            CrozzleSequences crozzleSequences;
            Int32 lowerLimit, upperLimit;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lowerLimit = 0;
            upperLimit = 0;

            crozzleSequences = new CrozzleSequences(rows, columns, new Configuration(@"..\..\..\..\Test Files\UnitTest1.cfg"));

            crozzleSequences.CheckDuplicateWords(lowerLimit, upperLimit);
            Assert.IsTrue(crozzleSequences.ErrorMessages.Count == 0);
        }

        [TestMethod]
        public void Scenario6Case3()
        {
            CrozzleSequences crozzleSequences;
            Int32 lowerLimit, upperLimit;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow3.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn3.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lowerLimit = 0;
            upperLimit = 0;

            crozzleSequences = new CrozzleSequences(rows, columns, new Configuration(@"..\..\..\..\Test Files\UnitTest1.cfg"));

            crozzleSequences.CheckDuplicateWords(lowerLimit, upperLimit);
            Assert.IsTrue(crozzleSequences.ErrorMessages.Count == 3);
        }
        #endregion

        #region Scenario 8
        [TestMethod]
        public void Scenario8Case1()
        {
            Configuration config;
            WordList wordlist;
            Crozzle crozzle;

            String[] expectedStrings =
            {
                "Configuration file is valid",
                "Crozzle file is valid",
                "Word list file is valid"
            };

            Configuration.TryParse(@"..\..\..\..\Test Files\UnitTest1.cfg", out config);
            WordList.TryParse(@"..\..\..\..\Test Files\UnitTest1.seq", config, out wordlist);
            Crozzle.TryParse(@"..\..\..\..\Test Files\UnitTest1.czl", config, wordlist, out crozzle);

            var htmlString = crozzle.ToStringHTML();
            foreach (String eString in expectedStrings)
                if (!htmlString.Contains(eString))
                    Assert.Fail();
        }

        [TestMethod]
        public void Scenario8Case2()
        {
            Configuration config;
            WordList wordlist;
            Crozzle crozzle;

            String[] expectedStrings =
            {
                "Configuration file is valid",
                "Crozzle file is valid",
                "Word list file is valid"
            };

            Configuration.TryParse(@"..\..\..\..\Test Files\UnitTest2.cfg", out config);
            WordList.TryParse(@"..\..\..\..\Test Files\UnitTest2.seq", config, out wordlist);
            Crozzle.TryParse(@"..\..\..\..\Test Files\UnitTest2.czl", config, wordlist, out crozzle);

            var htmlString = crozzle.ToStringHTML();
            foreach (String eString in expectedStrings)
                if (!htmlString.Contains(eString))
                    Assert.Fail();
        }

        [TestMethod]
        public void Scenario8Case3()
        {
            Configuration config;
            WordList wordlist;
            Crozzle crozzle;

            String[] expectedStrings =
            {
                "Configuration file is invalid",
                "Crozzle file is invalid",
                "Word list file is invalid"
            };

            Configuration.TryParse(@"..\..\..\..\Test Files\UnitTest3.cfg", out config);
            WordList.TryParse(@"..\..\..\..\Test Files\UnitTest3.seq", config, out wordlist);
            Crozzle.TryParse(@"..\..\..\..\Test Files\UnitTest1.czl", config, wordlist, out crozzle);

            var htmlString = crozzle.ToStringHTML();
            foreach (String eString in expectedStrings)
                if (!htmlString.Contains(eString))
                    Assert.Fail();
        }
        #endregion

        #region Scenario 9
        [TestMethod]
        public void Scenario9Case1()
        {
            CrozzleMap map;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            map = new CrozzleMap(rows, columns);
            if (map.GroupCount() != 1)
                Assert.Fail();
        }

        [TestMethod]
        public void Scenario9Case2()
        {
            CrozzleMap map;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow2.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn2.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            map = new CrozzleMap(rows, columns);
            if (map.GroupCount() != 2)
                Assert.Fail();
        }

        [TestMethod]
        public void Scenario9Case3()
        {
            CrozzleMap map;

            List<String[]> rows = new List<String[]>();
            List<String[]> columns = new List<String[]>();

            List<String> lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleRow3.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                rows.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            lines = new StreamReader(@"..\..\..\..\Test Files\CrozzleColumn3.txt").ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (String line in lines)
            {
                var result = Regex.Split(line, string.Empty).ToList();
                result.RemoveAt(0);
                result.RemoveAt(result.Count - 1);
                columns.Add(result.Select(str => (str.Length == 0) ? " " : str).ToArray());
            }

            map = new CrozzleMap(rows, columns);
            if (map.GroupCount() != 4)
                Assert.Fail();
        }
        #endregion

    }
}
