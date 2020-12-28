using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace PngToICO
{
    public static class Utility
    {
        public static string ToSentenceCase(string Input)
        {
            return new string(Input.SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
        }

        public static string GetDescription(IconSize size)
        {
            return size.GetType().GetMember(size.ToString())[0].GetCustomAttribute<DescriptionAttribute>().Description;
        }

        public static Key GetKey(KeyEventArgs args)
        {
            return args.Key == Key.System ? args.SystemKey : args.Key;
        }

        public static string GetString(IconSize size)
        {
            return $"{GetDescription(size)}: {ToSentenceCase(size.ToString())}";
        }
    }
}
