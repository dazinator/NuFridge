using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Platform.Model;

namespace NuFridge.Common.Jobs.CCNet.Octo
{
    public class OctoPackageVersionResolver : IPackageVersionResolver
    {

        private readonly IDictionary<string, string> stepNameToVersion = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        private string defaultVersion;

        public void Add(string stepNameAndVersion)
        {
            var index = new[] { ':', '=' }.Select(s => stepNameAndVersion.IndexOf(s)).Where(i => i >= 0).OrderBy(i => i).FirstOrDefault();
            if (index <= 0)
                throw new Exception("The package argument '" + stepNameAndVersion + "' does not use expected format of : {PackageId}:{Version}");

            var key = stepNameAndVersion.Substring(0, index);
            var value = (index >= stepNameAndVersion.Length - 1) ? string.Empty : stepNameAndVersion.Substring(index + 1);

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("The package argument '" + stepNameAndVersion + "' does not use expected format of : {PackageId}:{Version}");
            }

            SemanticVersion version;
            if (!SemanticVersion.TryParse(value, out version))
            {
                throw new Exception("The version portion of the package constraint '" + stepNameAndVersion + "' is not a valid semantic version number.");
            }

            Add(key, value);
        }

        public void Add(string stepName, string packageVersion)
        {
            string current;
            if (stepNameToVersion.TryGetValue(stepName, out current))
            {
                var newVersion = SemanticVersion.Parse(packageVersion);
                var currentVersion = SemanticVersion.Parse(current);
                if (newVersion < currentVersion)
                {
                    return;
                }
            }

            stepNameToVersion[stepName] = packageVersion;
        }

        public void Default(string packageVersion)
        {
            defaultVersion = packageVersion;
        }

        public string ResolveVersion(string stepName)
        {
            string version;
            return stepNameToVersion.TryGetValue(stepName, out version)
                       ? version
                       : defaultVersion;
        }
    }
    public interface IPackageVersionResolver
    {

        void Add(string stepNameAndVersion);
        void Add(string stepName, string packageVersion);
        void Default(string packageVersion);
        string ResolveVersion(string stepName);
    }
}
