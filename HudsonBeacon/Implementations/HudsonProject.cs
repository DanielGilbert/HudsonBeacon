using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HudsonBeacon.Interfaces;

namespace HudsonBeacon.Implementations
{
    public class HudsonProject : IProject
    {
        private ProjectStateEnum _projectState;
        private string _title;

        private static String[] _successStates = new String[]{"SUCCESS", "stable", "back to normal", "Stabil", "Wieder normal"};

        public ProjectStateEnum ProjectState
        {
            get
            {
                return _projectState;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public HudsonProject(string title)
        {
            _title = title;

            _projectState = ParseHudsonTitleToState(title);
        }

        private ProjectStateEnum ParseHudsonTitleToState(string title)
        {
            ProjectStateEnum projectState = ProjectStateEnum.Failed;
            Match regMatch = Regex.Match(title, "\\(([^)]+)\\)$");

            if (regMatch.Success)
            {
                string val = regMatch.Groups[0].Value;

                foreach (string successState in _successStates)
                {
                    if (val.Contains(successState))
                    {
                        projectState = ProjectStateEnum.Success;
                        break;
                    }
                }
            }
            else
            {
                projectState = ProjectStateEnum.Failed;
            }

            return projectState;
        }
    }
}
