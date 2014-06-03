using NuFridge.DataAccess.CCNetEntity;
using NuFridge.DataAccess.Repositories;
using NuFridge.Common.Jobs.CCNet.Octo;
using NuFridge.Common.Jobs.CCNet.TFS;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;
using log4net;

namespace NuFridge.Common.Jobs.CCNet
{
    [DisallowConcurrentExecution]
   public class CCNetJob : IJob
    {
        protected ILog Log = LogManager.GetLogger(typeof(CCNetJob));

        private void ProcessMatchedProjects(MongoDbRepository<CCNetProject> repository, Dictionary<ProjectStatus, CCNetProject> dictionary)
        {
            var grouped = dictionary.GroupBy(pj => pj.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList());

            foreach (var groupedProject in grouped)
            {
                var mongoProject = groupedProject.Key;
                var lastProjectStatus = groupedProject.Value.LastOrDefault();
                if (lastProjectStatus != null)
                {
                    if (lastProjectStatus.LastBuildLabel != mongoProject.LastSuccessfulBuildLabel && lastProjectStatus.BuildStatus == IntegrationStatus.Success && lastProjectStatus.Status == ProjectIntegratorState.Unknown)
                    {
                        Log.Info("Detected new build for " + lastProjectStatus.Name + ": " + lastProjectStatus.LastBuildLabel);
                        CreateReleaseForProject(repository, mongoProject, lastProjectStatus);
                    }
                }
            }
        }

        private void CreateReleaseForProject(MongoDbRepository<CCNetProject> repository, CCNetProject mongoProject, ProjectStatus projectStatus)
        {
            mongoProject.LastSuccessfulReleaseLabel = projectStatus.Name;
            mongoProject.LastSuccessfulBuildLabel = projectStatus.LastBuildLabel;
            repository.Update(mongoProject);

            Log.Info("Updated database so this release is not picked up again");

            OctoHelper octoHelper = new OctoHelper(ConfigurationManager.AppSettings["Octopus_APIURl"], ConfigurationManager.AppSettings["Octopus_APIKey"]);

            var packageVersion = octoHelper.GetPackageVersionFromLastRelease(mongoProject);

            Log.Info("Last Octopus package version: " + packageVersion);

            var changeSetNumber = TFSHelper.GetChangesetFromBuildLabel(mongoProject, packageVersion);

            Log.Info("Last changeset number: " + changeSetNumber);

            int intChangeSetNumber;
            if (!int.TryParse(changeSetNumber, out intChangeSetNumber))
            {
                Log.Error("Change set number is not an integer... stopping");
                return;
            }

            //We don't want to include the comments from the last release..
            intChangeSetNumber++;

            var comments = TFSHelper.GetCommentsSinceLastChangeset(mongoProject.TFSUrl, mongoProject.TFSPath, intChangeSetNumber.ToString());

            Log.Info("TFS Comments: " + comments);

            octoHelper.CreateReleaseInOctopusDeploy(mongoProject, comments);
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                MongoDbRepository<CCNetProject> repository = new MongoDbRepository<CCNetProject>();


                var ipAddressOrHostNameOfCCServer = ConfigurationManager.AppSettings["CCNet_HostName"];
                var client = new CruiseServerHttpClient(string.Format("http://{0}/ccnet/", ipAddressOrHostNameOfCCServer));

                var mongoProjects = repository.GetAll();

                var ccNetProjects = client.GetProjectStatus().OrderBy(ps => ps.Name);

                Dictionary<ProjectStatus, CCNetProject> dictionary = new Dictionary<ProjectStatus, CCNetProject>();

                foreach (var ccProject in ccNetProjects)
                {
                    var mongoProject = mongoProjects.FirstOrDefault(mnpj => ccProject.Name.StartsWith(mnpj.Name));
                    if (mongoProject != null)
                    {
                        dictionary.Add(ccProject, mongoProject);
                    }
                }

                ProcessMatchedProjects(repository, dictionary);
            }
            catch (Exception ex)
            {
               Log.Error("An error occurring in the CCNetMontior", ex);
            }
        }
    }
}