using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Linq;

namespace CrozzleApplication
{
    public class WordData
    {
        #region constants
        public const String OrientationRow = "HORIZONTAL-SEQUENCES";
        public const String OrientationColumn = "VERTICAL-SEQUENCES";
        public const int NumberOfFields = 4;
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        private String[] OriginalWordData { get; set; }
        public Boolean Valid { get; set; }
        public Orientation Orientation { get; set; }
        public Coordinate Location { get; set; }
        public String Letters { get; set; }
        #endregion

        #region properties - testing
        public Boolean IsHorizontal
        {
            get { return (Orientation.IsHorizontal); }
        }

        public Boolean IsVertical
        {
            get { return (Orientation.IsVertical); }
        }
        #endregion

        #region constructors
        public WordData()
        {
            OriginalWordData = null;
            Orientation = null;
            Location = null;
            Letters = null;

            Valid = false;
        }

        public WordData(String direction, int row, int column, String sequence)
        {
            OriginalWordData = new String[] { direction, row.ToString(), column.ToString(), sequence};
            Orientation anOrientation;
            if (!Orientation.TryParse(direction, out anOrientation))
                Errors.AddRange(CrozzleApplication.Orientation.Errors);
            Orientation = anOrientation;
            Location = new Coordinate(row, column);
            Letters = sequence;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(SequenceFragment fragment, Crozzle aCrozzle, out WordData aWordData)
        {
            Errors = new List<String>();

            aWordData = new WordData();

            Orientation anOrientation;
            if (!Orientation.TryParse(fragment.DirectionIdentifier, out anOrientation))
                Errors.AddRange(Orientation.Errors);
            aWordData.Orientation = anOrientation;

            String[] fields = fragment.OriginalWordData.Split(new char[] { ',' }, 2);
            if (fields.Length != 2)
                Errors.Add(String.Format(WordDataErrors.FieldCountError, fields.Length, (fields.Length == 1) ? String.Empty : "s", fragment.OriginalWordData));
            else
            {
                if (fields.Where(i => i.Contains("SEQUENCE")).Any() && fields.Where(i => i.Contains("LOCATION")).Any())
                {
                    String[] wordField = fields.Where(item => item.Contains("SEQUENCE")).First().Split('=');
                    if (String.IsNullOrEmpty(wordField[1]))
                        Errors.Add(String.Format(WordDataErrors.BlankFieldError, fragment.OriginalWordData, wordField[0]));
                    else
                    {
                        if (Regex.IsMatch(wordField[1], Configuration.allowedCharacters))
                            aWordData.Letters = wordField[1];
                        else
                            Errors.Add(String.Format(WordDataErrors.AlphabeticError, wordField[1]));
                    }

                    String[] locationField = fields.Where(item => item.Contains("LOCATION")).First().Split('=');
                    if (String.IsNullOrEmpty(locationField[1]))
                        Errors.Add(String.Format(WordDataErrors.BlankFieldError, fragment.OriginalWordData, wordField[0]));
                    else
                    {
                        String[] values = locationField[1].Split(',');
                        if (values.Length != 2)
                            Errors.Add(String.Format(WordDataErrors.SequencePositionIncompleteError, fragment.OriginalWordData));
                        else
                        {
                            int posx, posy;
                            if (!Validator.IsInt32(values[0], out posx) || !Validator.IsInt32(values[1], out posy))
                                Errors.Add(String.Format(WordDataErrors.SequencePositionInvalidError, fragment.OriginalWordData));
                            else
                                aWordData.Location = new Coordinate(posx, posy);
                        }
                    }
                }
                else
                    Errors.Add(String.Format(WordDataErrors.SeqOrLocFieldMissingError, fragment.OriginalWordData));
            }
            aWordData.OriginalWordData = 
                new string[] { (aWordData.Orientation != null) ? aWordData.Orientation.Direction : null,
                    (aWordData.Location != null) ? aWordData.Location.Row.ToString() : null,
                    (aWordData.Location != null) ? aWordData.Location.Column.ToString() : null,
                    (aWordData.Letters != null) ? aWordData.Letters : null};

            aWordData.Valid = Errors.Count == 0;
            return aWordData.Valid;
        }
        #endregion
    }
}