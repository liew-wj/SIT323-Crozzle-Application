using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;

namespace CrozzleApplication
{
    public class Configuration
    {
        #region constants
        public static String allowedCharacters = @"^[a-zA-Z]+$";
        public static String allowedBooleans = @"^(true|false)$";
        private static readonly Char[] PointSeparators = new Char[] { ',' };

        private static String DefaultLogFileName = "log.txt";
        #endregion

        #region properties - errors
        public static List<String> Errors { get; set; }

        public String FileErrorsTXT
        {
            get
            {
                int errorNumber = 1;
                String errors = "START PROCESSING FILE: " + ConfigurationFileName + "\r\n";

                foreach (String error in Configuration.Errors)
                    errors += "error " + errorNumber++ + ": " + error + "\r\n";
                errors += "END PROCESSING FILE: " + ConfigurationFileName + "\r\n";

                return (errors);
            }
        }

        public String FileErrorsHTML
        {
            get
            {
                int errorNumber = 1;
                String errors = "<p style=\"font-weight:bold\">START PROCESSING FILE: " + ConfigurationFileName + "</p>";

                foreach (String error in Configuration.Errors)
                    errors += "<p>error " + errorNumber++ + ": " + error + "</p>";
                errors += "<p style=\"font-weight:bold\">END PROCESSING FILE: " + ConfigurationFileName + "</p>";

                return (errors);
            }
        }
        #endregion

        #region properties - configuration file validity
        public Boolean Valid { get; set; } = false;
        #endregion

        #region properties - file names
        public String ConfigurationPath { get; set; }
        public String ConfigurationFileName { get; set; }
        public String ConfigurationDirectoryName { get; set; }
        public String LogFileName { get; set; }
        #endregion

        #region properties - word list configurations
        // Limits on the size of a word list.
        public int MinimumNumberOfUniqueWords { get; set; }
        public int MaximumNumberOfUniqueWords { get; set; }
        #endregion

        #region properties - crozzle output configurations
        public String InvalidCrozzleScore { get; set; } = "";
        public Boolean Uppercase { get; set; } = true;
        public String Style { get; set; } = @"<style></style>";
        public String BGcolourEmptyTD { get; set; } = @"#ffffff";
        public String BGcolourNonEmptyTD { get; set; } = @"#ffffff";
        #endregion

        #region properties - configurations keys
        private static Boolean[] ActualIntersectingKeys { get; set; }
        private static Boolean[] ActualNonIntersectingKeys { get; set; }
        private static List<string> ActualKeys { get; set; }
        private static readonly List<string> ExpectedKeys = new List<string>()
        {
            ConfigurationKeys.LOGFILE_FILENAME,
            ConfigurationKeys.UNIQUESEQ_OPENBRACKET + "-" + ConfigurationKeys.UNIQUESEQ_MINIMUM,
            ConfigurationKeys.UNIQUESEQ_OPENBRACKET + "-" + ConfigurationKeys.UNIQUESEQ_MAXIMUM,
            ConfigurationKeys.OUTPUT_INVALID,
            ConfigurationKeys.OUTPUT_UPPERCASE,
            ConfigurationKeys.OUTPUT_STYLE,
            ConfigurationKeys.OUTPUT_BGCOLOUR_EMPTY,
            ConfigurationKeys.OUTPUT_BGCOLOUR_NONEMPTY,
            ConfigurationKeys.SIZE_MINIMUMROWS,
            ConfigurationKeys.SIZE_MAXIMUMROWS,
            ConfigurationKeys.SIZE_MINIMUMCOLUMNS,
            ConfigurationKeys.SIZE_MAXIMUMCOLUMNS,
            ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALSEQ_MINIMUMHORZ,
            ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALSEQ_MAXIMUMHORZ,
            ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALSEQ_MINIMUMVERT,
            ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALSEQ_MAXIMUMVERT,
            ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALINTERSECT_MINIMUMHORZ,
            ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALINTERSECT_MAXIMUMHORZ,
            ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALINTERSECT_MINIMUMVERT,
            ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET + "-" + ConfigurationKeys.DIRECTIONALINTERSECT_MAXIMUMVERT,
            ConfigurationKeys.DUPE_OPENBRACKET + "-" + ConfigurationKeys.DUPE_MINIMUMVALID,
            ConfigurationKeys.DUPE_OPENBRACKET + "-" + ConfigurationKeys.DUPE_MAXIMUMVALID,
            ConfigurationKeys.VALIDGROUP_OPENBRACKET + "-" + ConfigurationKeys.VALIDGROUP_MINIMUM,
            ConfigurationKeys.VALIDGROUP_OPENBRACKET + "-" + ConfigurationKeys.VALIDGROUP_MAXIMUM
        };
        #endregion

        #region properties - crozzle configurations
        // Limits on the size of the crozzle grid.
        public int MinimumNumberOfRows { get; set; }
        public int MaximumNumberOfRows { get; set; }
        public int MinimumNumberOfColumns { get; set; }
        public int MaximumNumberOfColumns { get; set; }

        // Limits on the number of horizontal and vertical words in a crozzle.
        public int MinimumHorizontalWords { get; set; }
        public int MaximumHorizontalWords { get; set; }
        public int MinimumVerticalWords { get; set; }
        public int MaximumVerticalWords { get; set; }

        // Limits on the number of 
        // intersecting vertical words for each horizontal word, and
        // intersecting horizontal words for each vertical word.
        public int MinimumIntersectionsInHorizontalWords { get; set; }
        public int MaximumIntersectionsInHorizontalWords { get; set; }
        public int MinimumIntersectionsInVerticalWords { get; set; }
        public int MaximumIntersectionsInVerticalWords { get; set; }

        // Limits on duplicate words in the crozzle.
        public int MinimumNumberOfTheSameWord { get; set; }
        public int MaximumNumberOfTheSameWord { get; set; }

        // Limits on the number of valid word groups.
        public int MinimumNumberOfGroups { get; set; }
        public int MaximumNumberOfGroups { get; set; }
        #endregion

        #region properties - scoring configurations
        // The number of points per word within the crozzle.
        public int PointsPerWord { get; set; }

        // Points per letter that is at the intersection of a horizontal and vertical word within the crozzle.
        public int[] IntersectingPointsPerLetter { get; set; } = new int[26];

        // Points per letter that is not at the intersection of a horizontal and vertical word within the crozzle.
        public int[] NonIntersectingPointsPerLetter { get; set; } = new int[26];
        #endregion

        #region constructors
        public Configuration(String path)
        {
            ConfigurationPath = path;
            ConfigurationFileName = Path.GetFileName(path);
            ConfigurationDirectoryName = Path.GetDirectoryName(path);
        }
        #endregion

        #region parsing
        public static Boolean TryParse(String path, out Configuration aConfiguration)
        {
            Errors = new List<String>();
            ActualIntersectingKeys = new Boolean[26];
            ActualNonIntersectingKeys = new Boolean[26];
            ActualKeys = new List<string>();
            aConfiguration = new Configuration(path);

            if (aConfiguration.ConfigurationFileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
                Errors.Add(String.Format(ConfigurationErrors.FilenameError, path));
            else
            {
                StreamReader fileIn = new StreamReader(path);

                // Validate file.
                while (!fileIn.EndOfStream)
                {
                    List<String> fragment = new List<String>();
                    do
                    {
                        /// Add multiple lines as part of a text fragment until the last element is empty or null.
                        fragment.Add(fileIn.ReadLine());
                    } while (!String.IsNullOrEmpty(fragment.Last()));

                    // Parse a configuration fragment.
                    FileFragment<ConfigurationFileItem> aConfigurationFileFragment;
                    if (ConfigurationFileItem.TryParse(fragment, out aConfigurationFileFragment))
                    {
                        List<ConfigurationFileItem> aItemsWithErrors;
                        if ((aItemsWithErrors = aConfigurationFileFragment.Items.Where(item => item.KeyValue != null && ActualKeys.Contains(item.KeyValue.Key)).ToList()).Any())
                        {
                            foreach (ConfigurationFileItem error in aItemsWithErrors)
                                Errors.Add(String.Format(ConfigurationErrors.DuplicateKeyError, error.KeyValue.OriginalKeyValue));
                        }
                        else
                        {
                            /// TODO: Process key record and key-value validation.
                            String aFragmentKey = aConfigurationFileFragment.Name;

                            foreach (ConfigurationFileItem aItem in aConfigurationFileFragment.Items)
                            {
                                /// Record that this key has been found.
                                if (aItem.KeyValue.Key != null)
                                {
                                    /// Identified fragments contain keys that have the same string. To minimise duplicate key error, lead the key with the fragment's name.
                                    if (new string[] { ConfigurationKeys.UNIQUESEQ_OPENBRACKET, ConfigurationKeys.DUPE_OPENBRACKET,
                                        ConfigurationKeys.VALIDGROUP_OPENBRACKET, ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET,
                                        ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET }.Contains(aFragmentKey))
                                        ActualKeys.Add(aFragmentKey + "-" + aItem.KeyValue.Key);
                                    else if (!(new string[] { ConfigurationKeys.INTERSECT_OPENBRACKET, ConfigurationKeys.NONINTERSECT_OPENBRACKET }.Contains(aFragmentKey)))
                                        ActualKeys.Add(aItem.KeyValue.Key);
                                }

                                /// Process the key-value, given the current fragment key.
                                if (aFragmentKey.Equals(ConfigurationKeys.LOGFILE_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.LOGFILE_FILENAME)
                                    {
                                        aConfiguration.LogFileName = aItem.KeyValue.Value.Trim();
                                        if (Validator.IsDelimited(aConfiguration.LogFileName, Crozzle.StringDelimiters))
                                        {
                                            String value = aConfiguration.LogFileName.Trim(Crozzle.StringDelimiters);
                                            aConfiguration.LogFileName = (!String.IsNullOrEmpty(value)) ? value : DefaultLogFileName;
                                            if (!Validator.IsFilename(aConfiguration.LogFileName))
                                                Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.UNIQUESEQ_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.UNIQUESEQ_MINIMUM)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfUniqueWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.UNIQUESEQ_MAXIMUM)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfUniqueWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));

                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.OUTPUT_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.OUTPUT_INVALID)
                                    {
                                        aConfiguration.InvalidCrozzleScore = aItem.KeyValue.Value.Trim();
                                        if (Validator.IsDelimited(aConfiguration.InvalidCrozzleScore, Crozzle.StringDelimiters))
                                            aConfiguration.InvalidCrozzleScore = aConfiguration.InvalidCrozzleScore.Trim(Crozzle.StringDelimiters);
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.OUTPUT_UPPERCASE)
                                    {
                                        Boolean uppercase = true;
                                        if (!Validator.IsMatch(aItem.KeyValue.Value, allowedBooleans))
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        aConfiguration.Uppercase = uppercase;
                                    }
                                    else if (aItem.Name == ConfigurationKeys.OUTPUT_STYLE)
                                    {
                                        aConfiguration.Style = aItem.KeyValue.Value.Trim();
                                        if (Validator.IsDelimited(aConfiguration.Style, Crozzle.StringDelimiters))
                                        {
                                            aConfiguration.Style = aConfiguration.Style.Trim(Crozzle.StringDelimiters);
                                            if (!Validator.IsStyleTag(aConfiguration.Style))
                                                Errors.Add(String.Format(ConfigurationErrors.StyleError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.OUTPUT_BGCOLOUR_EMPTY)
                                    {
                                        aConfiguration.BGcolourEmptyTD = aItem.KeyValue.Value.Trim();
                                        if (!Validator.IsHexColourCode(aConfiguration.BGcolourEmptyTD))
                                            Errors.Add(String.Format(ConfigurationErrors.ColourError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.OUTPUT_BGCOLOUR_NONEMPTY)
                                    {
                                        aConfiguration.BGcolourNonEmptyTD = aItem.KeyValue.Value.Trim();
                                        if (!Validator.IsHexColourCode(aConfiguration.BGcolourNonEmptyTD))
                                            Errors.Add(String.Format(ConfigurationErrors.ColourError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.SIZE_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.SIZE_MINIMUMROWS)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfRows = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.SIZE_MAXIMUMROWS)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfRows = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.SIZE_MINIMUMCOLUMNS)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfColumns = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.SIZE_MAXIMUMCOLUMNS)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfColumns = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.DIRECTIONALSEQ_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.DIRECTIONALSEQ_MINIMUMHORZ)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumHorizontalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALSEQ_MAXIMUMHORZ)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumHorizontalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALSEQ_MINIMUMVERT)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumVerticalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALSEQ_MAXIMUMVERT)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumVerticalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.DIRECTIONALINTERSECT_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.DIRECTIONALINTERSECT_MINIMUMHORZ)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumIntersectionsInHorizontalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALINTERSECT_MAXIMUMHORZ)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumIntersectionsInHorizontalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALINTERSECT_MINIMUMVERT)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumIntersectionsInVerticalWords = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DIRECTIONALINTERSECT_MAXIMUMVERT)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumIntersectionsInVerticalWords = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.DUPE_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.DUPE_MINIMUMVALID)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfTheSameWord = minimum;
                                            if (!Validator.TryRange(minimum, 0, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.DUPE_MAXIMUMVALID)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfTheSameWord = maximum;
                                            if (!Validator.TryRange(maximum, 0, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.VALIDGROUP_OPENBRACKET))
                                {
                                    if (aItem.Name == ConfigurationKeys.VALIDGROUP_MINIMUM)
                                    {
                                        int minimum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out minimum))
                                        {
                                            aConfiguration.MinimumNumberOfGroups = minimum;
                                            if (!Validator.TryRange(minimum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else if (aItem.Name == ConfigurationKeys.VALIDGROUP_MAXIMUM)
                                    {
                                        int maximum;
                                        if (Validator.IsInt32(aItem.KeyValue.Value.Trim(), out maximum))
                                        {
                                            aConfiguration.MaximumNumberOfGroups = maximum;
                                            if (!Validator.TryRange(maximum, 1, Int32.MaxValue))
                                                Errors.Add(String.Format(ConfigurationErrors.IntegerError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.INTERSECT_OPENBRACKET))
                                {
                                    if (aItem.Name.Length == 1) /// ConfigurationFileItem.cs defines the key as the letter used in the CFG
                                    {
                                        String originalValues = aItem.KeyValue.Value.Trim();

                                        int points;
                                        if (Validator.IsInt32(aItem.KeyValue.Value, out points))
                                        {
                                            int index = (int)aItem.KeyValue.Key[0] - (int)'A';
                                            aConfiguration.IntersectingPointsPerLetter[index] = points;
                                            ActualIntersectingKeys[index] = true;
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else
                                        Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                }
                                else if (aFragmentKey.Equals(ConfigurationKeys.NONINTERSECT_OPENBRACKET))
                                {

                                    if (aItem.Name.Length == 1) /// ConfigurationFileItem.cs defines the key as the letter used in the CFG
                                    {
                                        String originalValues = aItem.KeyValue.Value.Trim();

                                        int points;
                                        if (Validator.IsInt32(aItem.KeyValue.Value, out points))
                                        {
                                            int index = (int)aItem.KeyValue.Key[0] - (int)'A';
                                            aConfiguration.NonIntersectingPointsPerLetter[index] = points;
                                            ActualNonIntersectingKeys[index] = true;
                                        }
                                        else
                                            Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                    }
                                    else
                                        Errors.Add(String.Format(ConfigurationErrors.ValueError, aItem.KeyValue.OriginalKeyValue, Validator.Errors[0]));
                                }
                                else
                                    Errors.AddRange(ConfigurationFileItem.Errors);
                            }
                        }
                    }
                }

                // Close files.
                fileIn.Close();

                // Check which keys are missing from the configuration file.
                foreach (string expectedKey in ExpectedKeys)
                    if (!ActualKeys.Contains(expectedKey))
                        Errors.Add(String.Format(ConfigurationErrors.MissingKeyError, expectedKey));
                for (char ch = 'A'; ch <= 'Z'; ch++)
                    if (!ActualIntersectingKeys[(int)ch - (int)'A'])
                        Errors.Add(String.Format(ConfigurationErrors.MissingIntersectionKeyError, ch.ToString()));
                for (char ch = 'A'; ch <= 'Z'; ch++)
                    if (!ActualNonIntersectingKeys[(int)ch - (int)'A'])
                        Errors.Add(String.Format(ConfigurationErrors.MissingNonIntersectionKeyError, ch.ToString()));

                // Check that minimum values are <= to their maximmum counterpart values.
                if (ActualKeys.Contains("SEQUENCES-IN-FILE-MINIMUM") && ActualKeys.Contains("SEQUENCES-IN-FILE-MAXIMUM"))
                    if (aConfiguration.MinimumNumberOfUniqueWords > aConfiguration.MaximumNumberOfUniqueWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "SEQUENCES-IN-FILE-MINIMUM",
                            aConfiguration.MinimumNumberOfUniqueWords, aConfiguration.MaximumNumberOfUniqueWords));

                if (ActualKeys.Contains("MINIMUM-ROWS") && ActualKeys.Contains("MAXIMUM-ROWS"))
                    if (aConfiguration.MinimumNumberOfRows > aConfiguration.MaximumNumberOfRows)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM-ROWS",
                            aConfiguration.MinimumNumberOfRows, aConfiguration.MaximumNumberOfRows));

                if (ActualKeys.Contains("MINIMUM-COLUMNS") && ActualKeys.Contains("MAXIMUM-COLUMNS"))
                    if (aConfiguration.MinimumNumberOfColumns > aConfiguration.MaximumNumberOfColumns)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "MINIMUM-COLUMNS",
                            aConfiguration.MinimumNumberOfColumns, aConfiguration.MaximumNumberOfColumns));

                if (ActualKeys.Contains("SEQUENCES-IN-CROZZLE-MINIMUM-HORIZONTAL") && ActualKeys.Contains("SEQUENCES-IN-CROZZLE-MAXIMUM-HORIZONTAL"))
                    if (aConfiguration.MinimumHorizontalWords > aConfiguration.MaximumHorizontalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "SEQUENCES-IN-CROZZLE-MINIMUM-HORIZONTAL",
                            aConfiguration.MinimumHorizontalWords, aConfiguration.MaximumHorizontalWords));

                if (ActualKeys.Contains("SEQUENCES-IN-CROZZLE-MINIMUM-VERTICAL") && ActualKeys.Contains("SEQUENCES-IN-CROZZLE-MAXIMUM-VERTICAL"))
                    if (aConfiguration.MinimumVerticalWords > aConfiguration.MaximumVerticalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "SEQUENCES-IN-CROZZLE-MINIMUM-VERTICAL",
                            aConfiguration.MinimumVerticalWords, aConfiguration.MaximumVerticalWords));

                if (ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCES-MINIMUM-HORIZONTAL") && ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCES-MINIMUM-HORIZONTAL"))
                    if (aConfiguration.MinimumIntersectionsInHorizontalWords > aConfiguration.MaximumIntersectionsInHorizontalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "INTERSECTIONS-IN-SEQUENCES-MINIMUM-VERTICAL",
                            aConfiguration.MinimumIntersectionsInHorizontalWords, aConfiguration.MaximumIntersectionsInHorizontalWords));

                if (ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCES-MINIMUM-VERTICAL") && ActualKeys.Contains("INTERSECTIONS-IN-SEQUENCES-MINIMUM-VERTICAL"))
                    if (aConfiguration.MinimumIntersectionsInVerticalWords > aConfiguration.MaximumIntersectionsInVerticalWords)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "INTERSECTIONS-IN-SEQUENCES-MINIMUM-VERTICAL",
                            aConfiguration.MinimumIntersectionsInVerticalWords, aConfiguration.MaximumIntersectionsInVerticalWords));

                if (ActualKeys.Contains("DUPLICATE-SEQUENCES-MINIMUM") && ActualKeys.Contains("DUPLICATE-SEQUENCES-MAXIMUM"))
                    if (aConfiguration.MaximumNumberOfTheSameWord < aConfiguration.MinimumNumberOfTheSameWord)
                        Errors.Add(String.Format(ConfigurationErrors.MaxDupeLowerThanMinError, "DUPLICATE-SEQUENCES-MAXIMUM",
                            aConfiguration.MaximumNumberOfTheSameWord, aConfiguration.MaximumNumberOfTheSameWord));

                if (ActualKeys.Contains("VALID-GROUPS-MINIMUM") && ActualKeys.Contains("VALID-GROUPS-MAXIMUM"))
                    if (aConfiguration.MinimumNumberOfGroups > aConfiguration.MaximumNumberOfGroups)
                        Errors.Add(String.Format(ConfigurationErrors.MinGreaterThanMaxError, "VALID-GROUPS-MINIMUM",
                            aConfiguration.MinimumNumberOfGroups, aConfiguration.MaximumNumberOfGroups));
            }

            // Store validity.
            aConfiguration.Valid = Errors.Count == 0;
            return (aConfiguration.Valid);
        }
        #endregion    
    }
}