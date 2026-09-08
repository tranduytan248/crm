using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFramework.Libs.Enums
{
    public enum EnumTypeSupport
    {
        // Call
        [Description("call")] call,
        // Redirect
        [Description("redirect")] redirect,
        // Map
        [Description("map")] map
    }
}
