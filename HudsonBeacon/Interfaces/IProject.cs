using System;

namespace HudsonBeacon.Interfaces
{
    public interface IProject
    {
        ProjectStateEnum ProjectState { get; }

        String Title { get; }
    }
}