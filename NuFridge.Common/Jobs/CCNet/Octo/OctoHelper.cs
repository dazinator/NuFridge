using System.Reflection;
using NuFridge.DataAccess.CCNetEntity;
using Octopus.Client;
using Octopus.Client.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace NuFridge.Common.Jobs.CCNet.Octo
{
    public class OctoHelper : IDisposable
    {
        protected ILog Log = LogManager.GetLogger(typeof(OctoHelper));

        private OctopusClient client { get; set; }
        private OctopusServerEndpoint endpoint { get; set; }
        private OctopusRepository repo { get; set; }

        private string CreateTempFileForReleaseNotes(string projectName, string releaseNotes)
        {
            var tempPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Temp");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            var tempFileName = Path.Combine(tempPath, projectName + ".temp");

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }

            File.WriteAllText(tempFileName, releaseNotes);

            return tempFileName;
        }

        public OctoHelper(string octopusUrl, string octopusAPIKey)
        {
            Connect(octopusUrl, octopusAPIKey);
        }

        private void Connect(string octopusUrl, string apiKey)
        {
            endpoint = new OctopusServerEndpoint(octopusUrl, apiKey);
            client = new OctopusClient(endpoint);
            repo = new OctopusRepository(client);
        }

        private void ResolveStepsOnReleasePlan(OctoReleasePlan plan)
        {
            foreach (var unresolved in plan.UnresolvedSteps)
            {
                if (!unresolved.IsResolveable)
                {
                    Log.Info(string.Format("The version number for step '{0}' cannot be automatically resolved because the feed or package ID is dynamic.", unresolved.StepName));
                    continue;
                }

                var feed = repo.Feeds.Get(unresolved.NuGetFeedId);
                if (feed == null)
                {
                    Log.Error(string.Format(
                            "Could not find a feed with ID {0}, which is used by step: " + unresolved.StepName,
                            unresolved.NuGetFeedId));
                    throw new Exception(
                        string.Format(
                            "Could not find a feed with ID {0}, which is used by step: " + unresolved.StepName,
                            unresolved.NuGetFeedId));
                }

                var packages = repo.Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), new { packageIds = new[] { unresolved.PackageId } });
                var version = packages.FirstOrDefault();
                if (version == null)
                {
                    Log.Error(string.Format("Could not find any packages with ID '{0}' in the feed '{1}'", unresolved.PackageId, feed.FeedUri));
                    throw new Exception(string.Format("Could not find any packages with ID '{0}' in the feed '{1}'", unresolved.PackageId, feed.FeedUri));
                }

                unresolved.SetVersionFromLatest(version.Version);
            }
        }

        internal string GetPackageVersionFromLastRelease(CCNetProject project)
        {
            var octopusProject = repo.Projects.FindByName(project.OctopusProjectName);
            if (octopusProject != null)
            {
                var release = repo.Releases.FindOne(rl => rl.ProjectId == octopusProject.Id);
                    if (release != null)
                    {
                        var deploymentProcess = repo.DeploymentProcesses.Get(release.ProjectDeploymentProcessSnapshotId);
                        if (deploymentProcess != null)
                        {
                            var releaseTemplate = repo.DeploymentProcesses.GetTemplate(deploymentProcess);
                            if (releaseTemplate != null)
                            {
                                var package = releaseTemplate.Packages.FirstOrDefault(pk => pk.NuGetPackageId == project.OctopusNuGetPacakgeId);
                                if (package != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(package.VersionSelectedLastRelease))
                                    {
                                        return package.VersionSelectedLastRelease;
                                    }
                                }
                            }
                        }
                    }
            }
            return null;
        }

        internal void CreateReleaseInOctopusDeploy(CCNetProject project, string releaseNotes)
        {
            Log.Info("Checking last release in octopus");

            var octopusProject = repo.Projects.FindByName(project.OctopusProjectName);
            if (octopusProject != null)
            {
                Log.Info("Getting previous releases");
                //TODO dont get all releases.... why....
                var releases = repo.Releases.FindMany(rl => rl.ProjectId == octopusProject.Id); //According to the API this will be ordered for us so it will always be the latest
                if (releases != null && releases.Any())
                {
                    var release = releases.FirstOrDefault();
                    if (release != null)
                    {
                        Log.Info("Creating relase plan");
                        var deploymentProcess = repo.DeploymentProcesses.Get(release.ProjectDeploymentProcessSnapshotId);
                        var releaseTemplate = repo.DeploymentProcesses.GetTemplate(deploymentProcess);
                        var plan = new OctoReleasePlan(releaseTemplate, new OctoPackageVersionResolver());

                        if (plan.UnresolvedSteps.Count > 0)
                        {
                            Log.Info("Resolving steps");
                            ResolveStepsOnReleasePlan(plan);
                        }

                        var stepItem = plan.Steps.FirstOrDefault(stp => stp.PackageId == project.OctopusNuGetPacakgeId);
                        var newPackage = releaseTemplate.Packages.FirstOrDefault(pk => pk.NuGetPackageId == project.OctopusNuGetPacakgeId);

                        if (stepItem != null)
                        {
                            if (newPackage == null)
                            {
                                Log.Error("Could not find the package in the release template packages: " + project.OctopusNuGetPacakgeId);
                            }
                            else
                            {

                                if (newPackage.VersionSelectedLastRelease == stepItem.Version)
                                {
                                    Log.Warn("Did not detect a new version for " + newPackage.NuGetPackageId + " (" +
                                             stepItem.Version + ")");
                                    return;
                                }

                                StartOctoExe(project, releaseNotes);
                            }
                        }
                        else
                        {
                            Log.Error("Could not find a project process steps: " + project.OctopusNuGetPacakgeId);
                        }
                    }
                }


            }


        }

        private void StartOctoExe(CCNetProject project, string releaseNotes)
        {


            var octoPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Jobs\CCNet\Octo\bin\Octo.exe");

            if (File.Exists(octoPath))
            {

                var releaseNotesFilePath = CreateTempFileForReleaseNotes(project.OctopusProjectName, releaseNotes);

                var arguments = "create-release --project=\"" + project.OctopusProjectName + "\" --server=" +
                                  endpoint.OctopusServer + " --apiKey=" +
                                  endpoint.ApiKey + " --releasenotesfile=\"" +
                                  releaseNotesFilePath + "\"";


                var compiler = new Process
                {
                    StartInfo =
                    {
                        FileName = octoPath,
                        WorkingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Jobs\CCNet\Octo\bin\"),
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };
                compiler.Start();

                compiler.StandardOutput.ReadToEnd();

                compiler.WaitForExit();
            }
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }
    }
}
