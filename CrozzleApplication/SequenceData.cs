using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    public class SequenceData
    {
        public static List<String> Errors { get; set; }

        public String OriginalData { get; set; }
        public String Sequence { get; set; }
        public Int32 Score { get; set; }
        public Int32 SequenceLength { get; set; }
        public Int32 AsciiSum { get; set; }
        public Int32 HashTotal { get; set; }

        public SequenceData()
        {
            // OriginalData = originalSequenceData;
            
            /*
            String[] data = originalSequenceData.Split(',');
            if (data.Length != 5)
            {
                Sequence = (data[0] + data[1]).Trim('"');
                Score = Int32.Parse(data[2]);
                SequenceLength = Int32.Parse(data[3]);
                AsciiSum = Int32.Parse(data[4]);
                HashTotal = Int32.Parse(data[5]);
            }
            else
            {
                Sequence = data[0];
                Score = Int32.Parse(data[1]);
                SequenceLength = Int32.Parse(data[2]);
                AsciiSum = Int32.Parse(data[3]);
                HashTotal = Int32.Parse(data[4]);
            }
            */
        }

        public static Boolean TryParse(String originalSequenceData, Boolean isSequenceValidator, out SequenceData sequenceData)
        {
            sequenceData = new SequenceData();
            Errors = new List<string>();

            if (String.IsNullOrWhiteSpace(originalSequenceData))
                Errors.Add(String.Format(""));
            else
            {
                sequenceData.OriginalData = originalSequenceData;

                if (isSequenceValidator)
                {
                    /// Separate the regex specified in the sequence validator from the other fields.
                    sequenceData.Sequence = originalSequenceData.Substring(0, originalSequenceData.LastIndexOf('"') + 1);
                    originalSequenceData = originalSequenceData.Remove(0, sequenceData.Sequence.Length);
                    sequenceData.Sequence = sequenceData.Sequence.Trim('"');
                }
                else
                {
                    sequenceData.Sequence = originalSequenceData.Substring(0, originalSequenceData.IndexOf(','));
                    originalSequenceData = originalSequenceData.Remove(0, sequenceData.Sequence.Length);
                }

                String[] data = originalSequenceData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length != 4)
                    Errors.Add(String.Format(""));
                else
                {
                    int parsedValue;
                    for (int i = 0; i < data.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (!Int32.TryParse(data[i], out parsedValue))
                                    Errors.Add(String.Format(""));
                                else
                                    sequenceData.Score = parsedValue;
                                break;
                            case 1:
                                if (!Int32.TryParse(data[i], out parsedValue))
                                    Errors.Add(String.Format(""));
                                else
                                    sequenceData.SequenceLength = parsedValue;
                                break;
                            case 2:
                                if (!Int32.TryParse(data[i], out parsedValue))
                                    Errors.Add(String.Format(""));
                                else
                                    sequenceData.AsciiSum = parsedValue;
                                break;
                            case 3:
                                if (!Int32.TryParse(data[i], out parsedValue))
                                    Errors.Add(String.Format(""));
                                else
                                    sequenceData.HashTotal = parsedValue;
                                break;
                        }
                    }
                }
            }

            return Errors.Count == 0;
        }
        
        public Boolean IsExactHashTotal()
        {
            return (ReturnHashTotal() == HashTotal) ? true : false;
        }

        public Int32 ReturnHashTotal()
        {
            return (Score + SequenceLength + AsciiSum);
        }

        public Int32 ReturnAsciiSum()
        {
            Int32 sum = 0;
            foreach (char character in Sequence)
                sum += (int)character;

            return sum;
        }

        public Int32 ReturnLength()
        {
            return Sequence.Length;
        }

        public override String ToString()
        {
            return String.Format("{0},{1},{2},{3},{4}", Sequence, Score, SequenceLength, AsciiSum, HashTotal);
        }
    }
}
