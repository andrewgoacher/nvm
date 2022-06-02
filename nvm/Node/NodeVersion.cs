using System.Text.RegularExpressions;

namespace nvm.Node
{
    internal class NodeVersion
    {
        const string VERSION_REGEX_PATTERN = @"v(\d+)\.(\d+)\.(\d+).*";
        const string DATE_REGEX_PATTERN = @".*(\d{4}-\d{2}-\d{2}).*";

        static readonly Regex versionRegex = new Regex(VERSION_REGEX_PATTERN);
        static readonly Regex dateRegex = new Regex(DATE_REGEX_PATTERN);

        private int _major;
        private int _minor;
        private int _patch;
        private bool _isLatest;

        private NodeVersion(int major, int minor, int patch, DateOnly releaseDate, bool islatest)
        {
            _major = major;
            _minor = minor;
            _patch = patch;
            ReleaseDate = releaseDate;
            _isLatest = islatest;
        }

        public bool IsLatest => _isLatest;

        public int Major => _major;
        public int Minor => _major;
        public int Patch => _major;

        public string Version
        {
            get
            {
                var latestString = IsLatest ? " (Latest)" : "";
                return $"v{_major}.{_minor}.{_patch}{latestString}";
            }
        }
        public DateOnly ReleaseDate { get; }

        public static NodeVersion? Parse(string version, string date, bool isLatest = false)
        {
            try
            {

                var (major, minor, patch, latest) = ParseVersion(version, isLatest);

                var parsedDate = ParseDate(date);
                if (parsedDate == null)
                {
                    return null;
                }
                return new NodeVersion(major, minor, patch, parsedDate.Value, latest);
            }
            catch
            {
                return null;
            }
        }

        private static DateOnly? ParseDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            var matches = dateRegex.Match(date);
            if (matches.Success)
            {
                return DateOnly.Parse(matches.Groups[1].Value);
            }

            return null;
        }

        private static (int major, int minor, int patch, bool latest) ParseVersion(string version, bool latest)
        {
            var matches = versionRegex.Match(version);

            if (matches.Success)
            {
                var major = Convert.ToInt32(matches.Groups[1].Value);
                var minor = Convert.ToInt32(matches.Groups[2].Value);
                var patch = Convert.ToInt32(matches.Groups[3].Value);

                return (major, minor, patch, latest);
            }

            throw new Exception("Error");
        }
    }
}
