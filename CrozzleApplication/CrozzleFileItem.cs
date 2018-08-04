using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class CrozzleFileItem
    {
        /// ASS1 - Modify or add constants to work with the new data files.
        #region constant - new data symbols
        enum DataKeys
        {
            UNDEFINED = 0,
            DEPENDENCIES,
            SIZE,
            HORZSEQUENCE,
            VERTSEQUENCE
        }

        struct FileDependenciesKeys
        {
            public const String OpenBracket = "FILE-DEPENDENCIES";
            public const String EndBracket = "END-FILE-DEPENDENCIES";

            public const String ConfigFileName = "CONFIG-DATA";
            public const String SequenceFileName = "SEQUENCE-DATA";
        }

        struct CrozzleSizeKeys
        {
            public const String OpenBracket = "CROZZLE-SIZE";
            public const String EndBracket = "END-CROZZLE-SIZE";

            public const String Size = "SIZE";
        }

        struct HorizontalSequenceKeys
        {
            public const String OpenBracket = "HORIZONTAL-SEQUENCES";
            public const String EndBracket = "END-HORIZONTAL-SEQUENCES";

            public const String Sequence = "SEQUENCE";
            public const String Location = "LOCATION";
        }

        struct VerticalSequenceKeys
        {
            public const String OpenBracket = "VERTICAL-SEQUENCES";
            public const String EndBracket = "END-VERTICAL-SEQUENCES";

            public const String Sequence = "SEQUENCE";
            public const String Location = "LOCATION";
        }

        /// For reference, see https://stackoverflow.com/questions/29948341/regex-latin-characters-filter-and-non-latin-character-filer
        public const String ForeignCharacters = @"[^\u0000-\u007F]+";
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        private String OriginalItem { get; set; }
        public Boolean Valid { get; set; }
        public String Name { get; set; }
        public KeyValue KeyValue { get; set; }
        #endregion

        #region constructors
        public CrozzleFileItem(String originalItemData)
        {
            OriginalItem = originalItemData;
            Valid = false;
            Name = null;
            KeyValue = null;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(List<string> crozzleFileFragment, out FileFragment<CrozzleFileItem> aCrozzleFileFragment)
        {
            Errors = new List<String>();
            crozzleFileFragment.RemoveAll(s => String.IsNullOrEmpty(s));
            aCrozzleFileFragment = new FileFragment<CrozzleFileItem>();

            String formattedLine = String.Empty;
            DataKeys flag = DataKeys.UNDEFINED;

            foreach (String line in crozzleFileFragment)
            {
                /// Discard comment and trim possible whitespace.
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//");
                    formattedLine = line.Remove(index);
                }
                else
                    formattedLine = line;
                formattedLine = formattedLine.Trim();

                CrozzleFileItem aCrozzleFileItem = new CrozzleFileItem(formattedLine);

                if (Regex.IsMatch(formattedLine, @"^\s*$"))
                    continue;
                else if (Regex.IsMatch(formattedLine, ForeignCharacters))
                    Errors.Add(String.Format(ConfigurationFileItemErrors.SymbolError, aCrozzleFileItem));
                else
                {
                    /// TODO: switch case with flag
                    switch (flag)
                    {
                        case DataKeys.UNDEFINED:
                            /// fragment specification
                            if (Regex.IsMatch(formattedLine, @"^" + FileDependenciesKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.DEPENDENCIES;
                                aCrozzleFileFragment.Name = FileDependenciesKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.SIZE;
                                aCrozzleFileFragment.Name = CrozzleSizeKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + HorizontalSequenceKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.HORZSEQUENCE;
                                aCrozzleFileFragment.Name = HorizontalSequenceKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + VerticalSequenceKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.VERTSEQUENCE;
                                aCrozzleFileFragment.Name = VerticalSequenceKeys.OpenBracket;
                            }
                            break;

                        case DataKeys.DEPENDENCIES:
                            if (Regex.IsMatch(formattedLine, @"^" + FileDependenciesKeys.ConfigFileName + @".*"))
                            {
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, FileDependenciesKeys.ConfigFileName, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aCrozzleFileItem.Name = FileDependenciesKeys.ConfigFileName;
                                aCrozzleFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + FileDependenciesKeys.SequenceFileName + @".*"))
                            {
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, FileDependenciesKeys.SequenceFileName, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aCrozzleFileItem.Name = FileDependenciesKeys.SequenceFileName;
                                aCrozzleFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + FileDependenciesKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.SIZE:
                            if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.Size + @".*"))
                            {
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleSizeKeys.Size, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aCrozzleFileItem.Name = CrozzleSizeKeys.Size;
                                aCrozzleFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.HORZSEQUENCE:
                            if (Regex.IsMatch(formattedLine, @"^" + HorizontalSequenceKeys.Sequence + @".*"))
                            {
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, HorizontalSequenceKeys.Sequence, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aCrozzleFileItem.Name = HorizontalSequenceKeys.Sequence;
                                aCrozzleFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + HorizontalSequenceKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.VERTSEQUENCE:
                            if (Regex.IsMatch(formattedLine, @"^" + VerticalSequenceKeys.Sequence + @".*"))
                            {
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, VerticalSequenceKeys.Sequence, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aCrozzleFileItem.Name = VerticalSequenceKeys.Sequence;
                                aCrozzleFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + VerticalSequenceKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;
                    }

                    aCrozzleFileItem.Valid = Errors.Count == 0;
                    /// If the current line is the opening bracket to a fragment, do not add as a new item. Else, do so.
                    if (aCrozzleFileItem.Name != null)
                        aCrozzleFileFragment.AddNewItem(aCrozzleFileItem);
                }
            }
            /// FALSE TRUE FALSE
            /// FALSE FALSE TRUE
            return (!aCrozzleFileFragment.Items.Exists(item => item.Valid == false));
        }
        #endregion
    }
}