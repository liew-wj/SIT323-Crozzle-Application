using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using System.Linq;

namespace CrozzleApplication
{
    public class WordList
    {
        #region constants
        private static readonly Char[] WordSeparators = new Char[] { '\n' ,'\r' }; // used to be new Char[] { ',' };

        private const Boolean IS_VALIDATOR = true;
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }

        public String FileErrors
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING FILE: " + WordlistFileName + "\r\n";

                foreach (String error in WordList.Errors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING FILE: " + WordlistFileName + "\r\n";

                return (errors);
            }
        }

        public String FileErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING FILE: " + WordlistFileName + "</p>";

                foreach (String error in WordList.Errors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING FILE: " + WordlistFileName + "</p>";

                return (errors);
            }
        }
        #endregion

        #region properties - filenames
        public String WordlistPath { get; set; }
        public String WordlistFileName { get; set; }
        public String WordlistDirectoryName { get; set; }
        #endregion

        #region properties
        public String[] OriginalList { get; set; }
        public Boolean Valid { get; set; } = false;
        public List<SequenceData> List { get; set; }

        public int Count
        {
            get { return (List.Count); }
        }
        #endregion

        #region validation
        public SequenceData SequenceValidator { get; set; }
        #endregion

        #region constructors
        public WordList(String path, Configuration aConfiguration)
        {
            WordlistPath = path;
            WordlistFileName = Path.GetFileName(path);
            WordlistDirectoryName = Path.GetDirectoryName(path);
            List = new List<SequenceData>();
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String path, Configuration aConfiguration, out WordList aWordList)
        {
            StreamReader fileIn = new StreamReader(path);

            Errors = new List<String>();
            aWordList = new WordList(path, aConfiguration);

            /// Split the sequence file, and extract the WordDataValidator.
            aWordList.OriginalList = fileIn.ReadToEnd().Split(WordSeparators).Where(x => x != String.Empty).ToArray();

            /// Validate the SequenceValidator; possible case of invalid fields.
            SequenceData seq;
            SequenceData.TryParse(aWordList.OriginalList.First(), IS_VALIDATOR, out seq);
            if (SequenceData.Errors.Any())
                Errors.AddRange(SequenceData.Errors);
            else
            {
                aWordList.SequenceValidator = seq;
                aWordList.OriginalList = aWordList.OriginalList.Skip(1).ToArray(); /// existence of magic number.

                // Check each field in the wordlist.
                int totalPotentialScore = 0,
                    totalPotentialLength = 0,
                    totalPotentialAsciiSum = 0,
                    totalPotentialHashTotal = 0;
                foreach (String potentialWord in aWordList.OriginalList)
                {
                    SequenceData data;
                    SequenceData.TryParse(potentialWord, !IS_VALIDATOR, out data);
                    if (SequenceData.Errors.Any())
                        Errors.AddRange(SequenceData.Errors);
                    else
                    {
                        /// Check that the sequence is not empty.
                        if (data.Sequence.Length > 0)
                        {
                            /// Check if sequence is alphabetic, then check if sequence length matches its pre-determined length.
                            if (Regex.IsMatch(data.Sequence, aWordList.SequenceValidator.Sequence))
                            {
                                if (data.ReturnLength() != data.SequenceLength)
                                    Errors.Add(String.Format(WordListErrors.NonIdenticalLengthError, data.OriginalData));
                                else
                                {
                                    aWordList.Add(data);
                                    totalPotentialLength += data.SequenceLength;
                                }
                            }
                            else
                                Errors.Add(String.Format(WordListErrors.AlphabeticError, data.Sequence, data.OriginalData));

                            totalPotentialScore += data.Score;

                            /// Check if sequence's ASCII sum matches its pre-determined sum.
                            if (data.ReturnAsciiSum() != data.AsciiSum)
                                Errors.Add(String.Format(WordListErrors.NonIdenticalAsciiSumError, data.OriginalData));
                            else
                                totalPotentialAsciiSum += data.AsciiSum;

                            /// Check if seqence's hash total matches its pre-determined total.
                            if (data.ReturnHashTotal() != data.HashTotal)
                                Errors.Add(String.Format(WordListErrors.NonIdenticalHashTotalError, data.OriginalData));
                            else
                                totalPotentialHashTotal += data.HashTotal;
                        }
                        else
                            Errors.Add(String.Format(WordListErrors.MissingWordError, data.OriginalData));
                    }


                }

                /// Check if totaled potential values match the sequence validator.
                if (totalPotentialScore != aWordList.SequenceValidator.Score)
                    Errors.Add(String.Format(WordListErrors.IncorrectBatchScoreError, totalPotentialScore, aWordList.SequenceValidator.Score));
                if (totalPotentialLength != aWordList.SequenceValidator.SequenceLength)
                    Errors.Add(String.Format(WordListErrors.IncorrectBatchLengthError, totalPotentialLength, aWordList.SequenceValidator.SequenceLength));
                if (totalPotentialAsciiSum != aWordList.SequenceValidator.AsciiSum)
                    Errors.Add(String.Format(WordListErrors.IncorrectBatchAsciiSumError, totalPotentialAsciiSum, aWordList.SequenceValidator.SequenceLength));
                if (totalPotentialHashTotal != aWordList.SequenceValidator.HashTotal)
                    Errors.Add(String.Format(WordListErrors.IncorrectBatchHashTotalError, totalPotentialHashTotal, aWordList.SequenceValidator.HashTotal));

                // Check the minimmum word limit.
                if (aWordList.Count < aConfiguration.MinimumNumberOfUniqueWords)
                    Errors.Add(String.Format(WordListErrors.MinimumSizeError, aWordList.Count, aConfiguration.MinimumNumberOfUniqueWords));

                // Check the maximum word limit.
                if (aWordList.Count > aConfiguration.MaximumNumberOfUniqueWords)
                    Errors.Add(String.Format(WordListErrors.MaximumSizeError, aWordList.Count, aConfiguration.MaximumNumberOfUniqueWords));
            }

            aWordList.Valid = Errors.Count == 0;
            return (aWordList.Valid);
        }
        #endregion

        #region list functions
        public void Add(SequenceData data)
        {
            List.Add(data);
        }

        public Boolean Contains(SequenceData data)
        {
            return (List.Contains(data));
        }

        public Boolean Exists(String data)
        {
            return List.Exists(item => item.Sequence == data);
        }

        public SequenceData FirstResult(String equivalent)
        {
            return List.Where(seq => seq.Sequence == equivalent).First();
        }
        #endregion
    }
}
