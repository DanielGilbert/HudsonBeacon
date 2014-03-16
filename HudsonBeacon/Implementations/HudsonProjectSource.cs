using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HudsonBeacon.Interfaces;

namespace HudsonBeacon.Implementations
{
    public class HudsonProjectSource : IProjectSource
    {
        public List<IProject> GetProjectList(string projectSource)
        {
            List<IProject> projects = new List<IProject>();

            using (XmlReader reader = XmlReader.Create(projectSource))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                if (feed == null)
                    return projects;

                projects =
                    feed.Items.Select(
                        item => new HudsonProject(item.Title.Text))
                        .Cast<IProject>()
                        .ToList();
            }

            return projects;
        }
    }
}
