using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;

namespace NuGet.Test.Utility
{
    public static class PlatformTestUtility
    {

        /// <summary>
        /// Returns a message to apply to the xunit attribute if it should be skipped.
        /// Null is returned if the test should run.
        /// </summary>
        public static string GetSkipMessageOrNull(params string[] platforms)
        {
            if (platforms.Length < 0)
            {
                throw new ArgumentException("No platforms provided.");
            }

            var current = CurrentPlatform;

            var skip = platforms.Any(s => StringComparer.OrdinalIgnoreCase.Equals(current, s));

            if (skip)
            {
                return $"Test does not apply to: {current}. Target platforms: {String.Join(", ", platforms)}";
            }

            return null;
        }

        /// <summary>
        /// Current platform, windows, darwin, linux
        /// </summary>
        public static string CurrentPlatform
        {
            get
            {
                return _currentPlatform.Value;
            }
        }


        private static readonly Lazy<string> _currentPlatform = new Lazy<string>(GetCurrentPlatform);

        private static string GetCurrentPlatform()
        {
            if (RuntimeEnvironmentHelper.IsWindows)
            {
                return Platform.Windows;
            }

            if (RuntimeEnvironmentHelper.IsLinux)
            {
                return Platform.Linux;
            }

            if (RuntimeEnvironmentHelper.IsMacOSX)
            {
                return Platform.Darwin;
            }

            return "UNKNOWN";
        }
    }
}
