using System;
using System.Collections.Generic;
using System.Text;

namespace Yesolm.DevOps.Models
{
    public class OptionsConfig
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public string[] Aliases { get; set; }
    }
}
