using System;
using System.Collections.Generic;
using System.Linq;

namespace CrozzleApplication
{
    public class FileFragment<T>
    {
        public String Name { get; set; }
        public List<T> Items { get; set; }

        public FileFragment()
        {
            Name = null;
            Items = new List<T>();
        }

        public void AddNewItem(T item)
        {
            Items.Add(item);
        }

        public override String ToString()
        {
            String aString = null;
            foreach (var item in Items)
            {
                /// Assuming the only classes that require a fragment are ConfigurationFileItem and CrozzleFileItem
                aString += (item is ConfigurationFileItem) ? 
                    String.Format("{0}={1}{2}", (item as ConfigurationFileItem).Name, (item as ConfigurationFileItem).KeyValue, Environment.NewLine) :
                    String.Format("{0}={1}{2}", (item as CrozzleFileItem).Name, (item as CrozzleFileItem).KeyValue, Environment.NewLine);
            }

            aString += String.Format("END-{0}", Name);

            return aString;
        }
    }

    /// Used as the alternative to KeyValuePair
    public class SequenceFragment
    {
        public String DirectionIdentifier { get; set; }
        public String OriginalWordData { get; set; }

        public SequenceFragment() { }

        public SequenceFragment(String identifier, string originalData)
        {
            DirectionIdentifier = identifier;
            OriginalWordData = originalData;
        }
    }
}
