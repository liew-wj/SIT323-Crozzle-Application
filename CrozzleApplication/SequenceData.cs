using System;
using System.Collections.Generic;
using System.Linq;

namespace CrozzleApplication
{
    public class SequenceData
    {
        public String OriginalData { get; set; }
        public String Sequence { get; set; }
        public Int32 Score { get; set; }
        public Int32 SequenceLength { get; set; }
        public Int32 AsciiSum { get; set; }
        public Int32 HashTotal { get; set; }

        public SequenceData() { }

        public SequenceData(String originalSequenceData)
        {
            OriginalData = originalSequenceData;
            
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
