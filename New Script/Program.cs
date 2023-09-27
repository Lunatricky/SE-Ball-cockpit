using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public Program()
        {
            Setup();
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string arg, UpdateType updateSource)
        {
            switch (arg.ToUpperInvariant())
            {

            }
        }

        void Setup()
        {
            if (true)
            {

            }
            if (false)
            {
                Runtime.UpdateFrequency |= UpdateFrequency.None;
            }
        }

        struct 

        #region Tools

        #region StringContains
        public static bool StringContains(string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
        #endregion

        #region FilterThis
        bool FilterThis(IMyTerminalBlock block)
        {
            return block.CubeGrid == Me.CubeGrid;
        }
        #endregion FilterThis

        #endregion Tool
    }
}
