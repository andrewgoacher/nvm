using System.Text.RegularExpressions;

namespace nvm
{
    internal class NodeVersion
    {
        const string VERSION_REGEX_PATTERN = @"v(\d+)\.(\d+)\.(\d+)\/.*";
        const string DATE_REGEX_PATTERN = @".*(\d{2}-\w{3}-\d{4}).*";

        static readonly Regex versionRegex = new Regex(VERSION_REGEX_PATTERN);
        static readonly Regex dateRegex = new Regex(DATE_REGEX_PATTERN);

        private int? _major;
        private int? _minor;
        private int? _patch;

        private NodeVersion(int? major, int? minor, int? patch, DateOnly releaseDate)
        {
            _major = major;
            _minor = minor;
            _patch = patch;
            ReleaseDate = releaseDate;
        }

        public bool IsLatest => _major == null;

        public int Major => _major.HasValue ? _major.Value : int.MaxValue;
        public int Minor => _major.HasValue ? _major.Value : int.MaxValue;
        public int Patch => _major.HasValue ? _major.Value : int.MaxValue;

        public string Version
        {
            get
            {
                if (_minor==null)
                {
                    return "latest";
                }

                return $"v{_major}.{_minor}.{_patch}";
            }
        }
        public DateOnly ReleaseDate { get; }

        public static NodeVersion? Parse(string version, string date)
        {
            var (major, minor, patch, latest) = ParseVersion(version);

            if (major == null && !latest)
            {
                return null;
            }

            var parsedDate = ParseDate(date);
            if (parsedDate == null)
            {
                return null;
            }
            return new NodeVersion(major, minor, patch, parsedDate.Value);
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

        private static (int? major, int? minor, int? patch, bool latest) ParseVersion(string version)
        {
            if (version.Equals("latest/", StringComparison.OrdinalIgnoreCase))
            {
                return (null, null, null, true);
            }

            var matches = versionRegex.Match(version);

            if (matches.Success)
            {
                var major = Convert.ToInt32(matches.Groups[1].Value);
                var minor = Convert.ToInt32(matches.Groups[2].Value);
                var patch = Convert.ToInt32(matches.Groups[3].Value);

                return (major, minor, patch, false);
            }

            return (null, null, null, false);
        }
    }
}
