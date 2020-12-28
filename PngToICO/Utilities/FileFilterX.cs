public static class FileFilterX
{
    public static string FileFilterBuilder(bool includeAllFileTypes, params Filter[] filters)
    {
        if (filters == null) return string.Empty;

        var filterString = string.Empty;

        var first = true;
        foreach (var filter in filters)
        {
            if (ValidFilterName(filter.FileName) && filter.Extension != null && filter.Extension.Length > 0)
            {
                filterString += $"{(first ? string.Empty : "|")}{filter.FileName} (";

                first = false;

                var e1 = string.Empty;
                var e2 = string.Empty;
                foreach (var extension in filter.Extension)
                {
                    if (ValidExtension(extension, out string ext))
                    {
                        e1 += $"{ext};";
                        e2 += $"{ext};";
                    }
                }

                filterString += $"{e1.Substring(0, e1.Length - 1)})|{e2}";
                filterString = filterString.Substring(0, filterString.Length - 1);
            }
        }

        if (includeAllFileTypes)
        {
            filterString += "|All files (*.*)|*.*";
        }

        return filterString;
    }

    public static bool ValidFilterName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;

        if (System.Text.RegularExpressions.Regex.IsMatch(fileName, "/[a-zA-Z]+/g"))
        {
            return false;
        }

        return true;
    }

    public static bool ValidExtension(string extension, out string corrected)
    {
        corrected = string.Empty;
        if (string.IsNullOrWhiteSpace(extension)) return false;

        var ext = System.Text.RegularExpressions.Regex.Replace(extension, @"[^0-9a-zA-Z:,]+", string.Empty);

        if (string.IsNullOrWhiteSpace(ext))
        {
            return false;
        }

        corrected = $"*.{ext}";

        return true;
    }
}

public class Filter
{
    public string[] Extension { get; set; }
    public string FileName { get; set; } = string.Empty;
}
