using System.ComponentModel;

namespace PngToICO
{
    public enum IconSize
    {
        [Description("32p")]
        VerySmall = 32,
        [Description("64p")]
        Small = 64,
        [Description("128p")]
        Medium = 128,
        [Description("256p")]
        Large = 256
    }
}
