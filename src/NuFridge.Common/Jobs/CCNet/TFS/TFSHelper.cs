using System.Configuration;
using System.Reflection;
using NuFridge.DataAccess.CCNetEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

namespace NuFridge.Common.Jobs.CCNet.TFS
{
    public static class TFSHelper
    {
        static ILog Log = LogManager.GetLogger(typeof(TFSHelper));

        internal static string GetChangesetFromBuildLabel(CCNetProject project, string buildLabel)
        {
            var pattern = new Regex(project.ChangeSetNumberRegex);
            Match match = pattern.Match(buildLabel);
            if (match.Groups.Count > 0)
            {
                string changeSetNumber = match.Groups["changesetnumber"].Value;

                return changeSetNumber;
            }

            return null;
        }

        private static string RunTFExe(string pathToTFExe, string tfsArguments)
        {
            var compiler = new Process
            {
                StartInfo =
                {
                    FileName = pathToTFExe,
                    WorkingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Jobs\CCNet\TFS\bin\"),
                    Arguments = tfsArguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            Log.Info("TF path: " + pathToTFExe);
            Log.Info("TF arguments: " + tfsArguments);

            compiler.Start();

            var processOutputStr = compiler.StandardOutput.ReadToEnd();

            Log.Info("TF output: " + processOutputStr);

            compiler.WaitForExit();

            return processOutputStr;
        }

        private static string FormatTFOutput(string processOutputStr)
        {
            var changeSets = processOutputStr.Split(new string[] { "-------------------------------------------------------------------------------" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (changeSets.Any())
            {
                var returnStr = "**Comments from TFS**" + Environment.NewLine + Environment.NewLine;
                foreach (var changeSet in changeSets)
                {
                    var startOfCommentIndex = changeSet.IndexOf(Environment.NewLine + "Comment:", System.StringComparison.Ordinal);
                    var startOfItemsIndex = changeSet.IndexOf(Environment.NewLine + "Items:", System.StringComparison.Ordinal);

                    var comment = changeSet.Substring(startOfCommentIndex + 12, startOfItemsIndex - startOfCommentIndex - 14).Trim();

                    if (!string.IsNullOrWhiteSpace(comment))
                    {
                        Log.Info("Found TFS comment: " + comment);
                        returnStr += " - " + comment + Environment.NewLine;
                    }
                }

                return returnStr;
            }
            return null;
        }

        public static string GetCommentsSinceLastChangeset(string TFSURL, string TFSPath, string changeSet)
        {
            var pathToTFExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Jobs\CCNet\TFS\bin\TF.exe");

            var username = ConfigurationManager.AppSettings["TFS_Username"];
            var password = ConfigurationManager.AppSettings["TFS_Password"];

            string returnStr = string.Empty;
            string processOutputStr = string.Empty;
            var tfsArguments = string.Format("history /server:{0} \"{1}\" /recursive /noprompt /login:{3},{4} /version:{2}~ /format:Detailed", TFSURL, TFSPath, changeSet, username, password);

            if (File.Exists(pathToTFExe))
            {
               processOutputStr = RunTFExe(pathToTFExe, tfsArguments);
            }
            else
            {
                Log.Error("Could not find TF exe: " + pathToTFExe);
            }

            if (!string.IsNullOrWhiteSpace(processOutputStr))
            {
                returnStr = FormatTFOutput(processOutputStr);
            }

            if (string.IsNullOrWhiteSpace(returnStr))
            {
                returnStr = "**Failed to find comments in TFS from changeset number '" + changeSet + "'. Please populate the release notes manually.**";
            }

            return returnStr;
        }
    }
}
