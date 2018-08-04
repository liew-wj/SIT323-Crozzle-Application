using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrozzleApplication
{
    class ConfigurationFileItem
    {
        /// ASS1 - Modify or add new constants to work with the new data files
        #region constants - new data symbols
        enum DataKeys
        {
            UNDEFINED = 0,
            LOGFILE,
            SEQUENCES,
            OUTPUT,
            SIZE,
            LIMITSEQUENCE,
            LIMITINTERSECT,
            DUPLICATE,
            VALIDGROUP,
            INTERSECT,
            NONINTERSECT
        }

        struct LogFileKeys
        {
            public const String OpenBracket = "LOGFILE";
            public const String EndBracket = "END-LOGFILE";

            public const String FileName = "DEFAULT";
        }

        struct UniqueSequenceKeys
        {
            public const String OpenBracket = "SEQUENCES-IN-FILE";
            public const String EndBracket = "END-SEQUENCES-IN-FILE";

            public const String MinUnique = "MINIMUM";
            public const String MaxUnique = "MAXIMUM";
        }

        public struct CrozzleOutputKeys
        {
            public const String OpenBracket = "CROZZLE-OUTPUT";
            public const String EndBracket = "END-CROZZLE-OUTPUT";

            public const String InvalidScore = "INVALID-CROZZLE-SCORE";
            public const String Uppercase = "UPPERCASE";
            public const String Style = "STYLE";
            public const String BGColourEmptyTD = "BGCOLOUR-EMPTY-TD";
            public const String BGColourNonEmptyTD = "BGCOLOUR-NON-EMPTY-TD";
        }

        struct CrozzleSizeKeys
        {
            public const String OpenBracket = "CROZZLE-SIZE";
            public const String EndBracket = "END-CROZZLE-SIZE";

            public const String MinRows = "MINIMUM-ROWS";
            public const String MaxRows = "MAXIMUM-ROWS";
            public const String MinCols = "MINIMUM-COLUMNS";
            public const String MaxCols = "MAXIMUM-COLUMNS";
        }

        struct DirectionalSequenceKeys
        {
            public const String OpenBracket = "SEQUENCES-IN-CROZZLE";
            public const String EndBracket = "END-SEQUENCES-IN-CROZZLE";

            public const String MinHorz = "MINIMUM-HORIZONTAL";
            public const String MaxHorz = "MAXIMUM-HORIZONTAL";
            public const String MinVert = "MINIMUM-VERTICAL";
            public const String MaxVert = "MAXIMUM-VERTICAL";
        }

        struct DirectionalIntersectionKeys
        {
            public const String OpenBracket = "INTERSECTIONS-IN-SEQUENCES";
            public const String EndBracket = "END-INTERSECTIONS-IN-SEQUENCES";

            public const String MinHorz = "MINIMUM-HORIZONTAL";
            public const String MaxHorz = "MAXIMUM-HORIZONTAL";
            public const String MinVert = "MINIMUM-VERTICAL";
            public const String MaxVert = "MAXIMUM-VERTICAL";
        }

        struct DuplicateSequenceKeys
        {
            public const String OpenBracket = "DUPLICATE-SEQUENCES";
            public const String EndBracket = "END-DUPLICATE-SEQUENCES";

            public const String MinDupe = "MINIMUM";
            public const String MaxDupe = "MAXIMUM";
        }

        struct ValidGroupKeys
        {
            public const String OpenBracket = "VALID-GROUPS";
            public const String EndBracket = "END-VALID-GROUPS";

            public const String MinValidGroup = "MINIMUM";
            public const String MaxValidGroup = "MAXIMUM";
        }

        struct IntersectionKeys
        {
            public const String OpenBracket = "INTERSECTING-POINTS";
            public const String EndBracket = "END-INTERSECTING-POINTS";
        }

        struct NonIntersectionKeys
        {
            public const String OpenBracket = "NON-INTERSECTING-POINTS";
            public const String EndBracket = "END-NON-INTERSECTING-POINTS";
        }

        public const String AtoZ = @"^([A-Z]=|[A-Z]\b).*";

        /// For reference, see https://stackoverflow.com/questions/29948341/regex-latin-characters-filter-and-non-latin-character-filer
        public const String ForeignCharacters = @"[^\u0000-\u007F]+";
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }
        #endregion

        #region properties
        public String OriginalItem { get; set; }
        public Boolean Valid { get; set; }
        public String Name { get; set; }
        public KeyValue KeyValue { get; set; }
        #endregion
        
        #region constructors
        public ConfigurationFileItem(String originalItemData)
        {
            OriginalItem = originalItemData;
            Valid = false;
            Name = null;
            KeyValue = null;
        }
        #endregion

        #region parsing
        public static Boolean TryParse(List<String> configurationFileFragment, out FileFragment<ConfigurationFileItem> aConfigurationFileFragment)
        {
            Errors = new List<String>();
            configurationFileFragment.RemoveAll(s => String.IsNullOrEmpty(s));
            aConfigurationFileFragment = new FileFragment<ConfigurationFileItem>();

            String formattedLine = String.Empty;
            DataKeys flag = DataKeys.UNDEFINED;

            foreach (String line in configurationFileFragment)
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

                ConfigurationFileItem aConfigurationFileItem = new ConfigurationFileItem(formattedLine);

                if (Regex.IsMatch(formattedLine, @"^\s*$"))
                    continue;
                else if (Regex.IsMatch(formattedLine, ForeignCharacters))
                    Errors.Add(String.Format(ConfigurationFileItemErrors.SymbolError, aConfigurationFileItem)); /// Checks if the line contains foreign characters
                else
                {
                    switch (flag)
                    {
                        case DataKeys.UNDEFINED:
                            /// fragment specification.
                            if (Regex.IsMatch(formattedLine, @"^" + LogFileKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.LOGFILE;
                                aConfigurationFileFragment.Name =  LogFileKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + UniqueSequenceKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.SEQUENCES;
                                aConfigurationFileFragment.Name = UniqueSequenceKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.OUTPUT;
                                aConfigurationFileFragment.Name = CrozzleOutputKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.SIZE;
                                aConfigurationFileFragment.Name = CrozzleSizeKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.LIMITSEQUENCE;
                                aConfigurationFileFragment.Name = DirectionalSequenceKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.LIMITINTERSECT;
                                aConfigurationFileFragment.Name = DirectionalIntersectionKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DuplicateSequenceKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.DUPLICATE;
                                aConfigurationFileFragment.Name = DuplicateSequenceKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + ValidGroupKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.VALIDGROUP;
                                aConfigurationFileFragment.Name = ValidGroupKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + IntersectionKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.INTERSECT;
                                aConfigurationFileFragment.Name = IntersectionKeys.OpenBracket;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + NonIntersectionKeys.OpenBracket + @".*"))
                            {
                                flag = DataKeys.NONINTERSECT;
                                aConfigurationFileFragment.Name = NonIntersectionKeys.OpenBracket;
                            }
                            break;

                        case DataKeys.LOGFILE:
                            if (Regex.IsMatch(formattedLine, @"^" + LogFileKeys.FileName + @".*"))
                            {
                                /// get the LogFile key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, LogFileKeys.FileName, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = LogFileKeys.FileName;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + LogFileKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.SEQUENCES:
                            if (Regex.IsMatch(formattedLine, @"^" + UniqueSequenceKeys.MinUnique + @".*"))
                            {
                                /// get the MinimumUniqueLetterSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, UniqueSequenceKeys.MinUnique, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = UniqueSequenceKeys.MinUnique;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + UniqueSequenceKeys.MaxUnique + @".*"))
                            {
                                /// get the MaximumUniqueLetterSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, UniqueSequenceKeys.MaxUnique, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = UniqueSequenceKeys.MaxUnique;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + UniqueSequenceKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.OUTPUT:
                            if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.InvalidScore + @".*"))
                            {
                                /// get the InvalidCrozzleScore key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleOutputKeys.InvalidScore, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleOutputKeys.InvalidScore;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.Uppercase + @".*"))
                            {
                                /// get the Uppercase key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleOutputKeys.Uppercase, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleOutputKeys.Uppercase;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.Style + @".*"))
                            {
                                /// get the Style key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleOutputKeys.Style, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleOutputKeys.Style;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.BGColourEmptyTD + @".*"))
                            {
                                /// get the BGColourEmptyTD key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleOutputKeys.BGColourEmptyTD, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleOutputKeys.BGColourEmptyTD;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.BGColourNonEmptyTD + @".*"))
                            {
                                /// get the BGColourNonEmptyTD key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleOutputKeys.BGColourNonEmptyTD, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleOutputKeys.BGColourNonEmptyTD;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleOutputKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.SIZE:
                            if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.MinRows + @".*"))
                            {
                                /// get the MinimumCrozzleSizeRows key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleSizeKeys.MinRows, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleSizeKeys.MinRows;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.MaxRows + @".*"))
                            {
                                /// get the MaximumCrozzleSizeRows key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleSizeKeys.MaxRows, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleSizeKeys.MaxRows;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.MinCols + @".*"))
                            {
                                /// get the MinimumCrozzleSizeColumns key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleSizeKeys.MinCols, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleSizeKeys.MinCols;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.MaxCols + @".*"))
                            {
                                /// get the MaximumCrozzleSizeColumns key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, CrozzleSizeKeys.MaxCols, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = CrozzleSizeKeys.MaxCols;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + CrozzleSizeKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.LIMITSEQUENCE:
                            if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.MinHorz + @".*"))
                            {
                                /// get the MinimumHorizontalSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalSequenceKeys.MinHorz, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalSequenceKeys.MinHorz;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.MaxHorz + @".*"))
                            {
                                /// get the MaximumHorizontalSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalSequenceKeys.MaxHorz, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalSequenceKeys.MaxHorz;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.MinVert + @".*"))
                            {
                                /// get the MinimumVerticalSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalSequenceKeys.MinVert, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalSequenceKeys.MinVert;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.MaxVert + @".*"))
                            {
                                /// get the MaximumVerticalSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalSequenceKeys.MaxVert, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalSequenceKeys.MaxVert;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalSequenceKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.LIMITINTERSECT:
                            if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.MinHorz + @".*"))
                            {
                                /// get the MinimumHorizontalIntersections key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalIntersectionKeys.MinHorz, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalIntersectionKeys.MinHorz;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.MaxHorz + @".*"))
                            {
                                /// get the MaximumHorizontalIntersections key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalIntersectionKeys.MaxHorz, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalIntersectionKeys.MaxHorz;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.MinVert + @".*"))
                            {
                                /// get the MinimumVerticalIntersections key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalIntersectionKeys.MinVert, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalIntersectionKeys.MinVert;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.MaxVert + @".*"))
                            {
                                /// get the MaximumVerticalIntersections key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DirectionalIntersectionKeys.MaxVert, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DirectionalIntersectionKeys.MaxVert;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DirectionalIntersectionKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.DUPLICATE:
                            if (Regex.IsMatch(formattedLine, @"^" + DuplicateSequenceKeys.MinDupe + @".*"))
                            {
                                /// get the MinimumDuplicateSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DuplicateSequenceKeys.MinDupe, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DuplicateSequenceKeys.MinDupe;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DuplicateSequenceKeys.MaxDupe + @".*"))
                            {
                                /// get the MaximumDuplicateSequences key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, DuplicateSequenceKeys.MaxDupe, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = DuplicateSequenceKeys.MaxDupe;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + DuplicateSequenceKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.VALIDGROUP:
                            if (Regex.IsMatch(formattedLine, @"^" + ValidGroupKeys.MinValidGroup + @".*"))
                            {
                                /// get the MinimumValidGroups key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, ValidGroupKeys.MinValidGroup, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = ValidGroupKeys.MinValidGroup;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + ValidGroupKeys.MaxValidGroup + @".*"))
                            {
                                /// get the MaximumValidGroups key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, ValidGroupKeys.MaxValidGroup, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = ValidGroupKeys.MaxValidGroup;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + ValidGroupKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.INTERSECT:
                            if (Regex.IsMatch(formattedLine, AtoZ))
                            {
                                /// get the AtoZ key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, AtoZ, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = aKeyValue.Key;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + IntersectionKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;

                        case DataKeys.NONINTERSECT:
                            if (Regex.IsMatch(formattedLine, AtoZ))
                            {
                                /// get the AtoZ key-value pair.
                                KeyValue aKeyValue;
                                if (!KeyValue.TryParse(formattedLine, AtoZ, out aKeyValue))
                                    Errors.AddRange(KeyValue.Errors);
                                aConfigurationFileItem.Name = aKeyValue.Key;
                                aConfigurationFileItem.KeyValue = aKeyValue;
                            }
                            else if (Regex.IsMatch(formattedLine, @"^" + NonIntersectionKeys.EndBracket + @".*"))
                                flag = DataKeys.UNDEFINED;
                            break;
                    }
                }

                aConfigurationFileItem.Valid = Errors.Count == 0;
                /// If the current line is the opening bracket to a fragment, do not add as a new item. Else, do so.
                if (aConfigurationFileItem.Name != null)
                    aConfigurationFileFragment.AddNewItem(aConfigurationFileItem);
            }
            /// FALSE TRUE FALSE
            /// FALSE FALSE TRUE
            return (!aConfigurationFileFragment.Items.Exists(item => item.Valid == false));
        }
        #endregion
    }
}