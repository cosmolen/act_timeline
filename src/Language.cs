using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace ACTTimeline
{
    public class Translator
    {
        public static string Get(string resourcekey)
        {
            ResourceManager resm = null;
            if (CultureInfo.InstalledUICulture.Name == "ko-KR")
            {
                resm = resources.String_ko_KR.ResourceManager;
            }
            else
            {
                resm = resources.String_en_US.ResourceManager;
            }

            string value = resm.GetString(resourcekey);

            if (value == null) return resourcekey;
            else return value;
        }
    }
}
