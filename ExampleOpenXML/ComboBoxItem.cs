using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExampleOpenXML
{
    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
        public static ComboboxItem DefaultItem
        {
            get
            {
                return new ComboboxItem()
                { Text = "---Select---", Value = -1 };
            }

        }
    }
}
