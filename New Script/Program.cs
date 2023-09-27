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



        /*TODO
         * 
         * Set up gyro controller
         * Set up rotor angle interpretation
         * 
         * 
         */




        const string VERSION = "0.1";
        const string _GroupName = "[Ball Cockpit]";
        const string _ShipControllerTag = "[Reference]";
        const string _BankTag = "[Ball - Bank]";
        const string _PitchTag = "[Ball - Pitch]";
        const string _RollTag = "[Ball - Roll]";

        //List<IMyTerminalBlock> Group = new List<IMyTerminalBlock>();
        IMyBlockGroup Group;
        IMyShipController Controller;
        List<IMyTerminalBlock> Gyros = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> Rotors = new List<IMyTerminalBlock>();

        IMyTerminalBlock ShipController;
        //IMyTerminalBlock BankRotor;
        //IMyTerminalBlock PitchRotor;
        //IMyTerminalBlock RollRotor;

        IMyMotorStator BankRotor;
        IMyMotorStator PitchRotor;
        IMyMotorStator RollRotor;

        //Rotor BankRotorDetails = new Rotor();
        //Rotor PitchRotorDetails = new Rotor();
        //Rotor RollRotorDetails = new Rotor();
      

        public Program()
        {
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, PopulateLists);

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
            
            GetRotors();
            //GetShipController();
            GetGyros();
            if (true)
            {

            }
            if (false)
            {
                Runtime.UpdateFrequency |= UpdateFrequency.None;
            }
        }

        
        bool PopulateLists(IMyTerminalBlock block)
        {
            if (!block.IsSameConstructAs(Me))
                return false;
            if (StringContains(block.CustomName, _ShipControllerTag))
            {
                block = Controller;
            }
            return false;
        }

        #region Get Rotors
        void GetRotors()
        {
            var group = GridTerminalSystem.GetBlockGroupWithName("_GroupName");
            if (group == null)
            {
                return;
            }
            group.GetBlocks(null, CollectRotors);
        }
        bool CollectRotors(IMyTerminalBlock block)
        {
            if (!block.IsSameConstructAs(Me))
                return false;
            if (block is IMyMotorStator)
            {
                if (StringContains(block.CustomName, _BankTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    v = BankRotor;
                    //BankRotorDetails.rotor = BankRotor;
                    //BankRotorDetails.angle = BankRotor.Angle;
                }
                if (StringContains(block.CustomName, _PitchTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    v = PitchRotor;
                    //PitchRotorDetails.rotor = PitchRotor;
                    //PitchRotorDetails.angle = PitchRotor.Angle;
                }
                if (StringContains(block.CustomName, _RollTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    v = RollRotor;
                    //RollRotorDetails.rotor = RollRotor;
                    //RollRotorDetails.angle = RollRotor.Angle;
                }

            }
            return false;
        }

        void AddRotor(IMyTerminalBlock block, List<IMyTerminalBlock> rotors)
        {
            if(block != null)
            {
                rotors.Add(block);
            }
        }
        #endregion Get Rotors

        #region Get Gyros
        void GetGyros()
        {
            var group = GridTerminalSystem.GetBlockGroupWithName("_GroupName");
            if (group == null)
            {
                return;
            }
            group.GetBlocks(null, CollectGyros);
        }

        bool CollectGyros(IMyTerminalBlock block)
        {
            if (block.CubeGrid != Me)
                return false;
            
            if (block is IMyGyro)
            {
                AddGyro(block, Gyros);
            }
            return false;
        }
    
        void AddGyro(IMyTerminalBlock block, List<IMyTerminalBlock> gyros)
        {
            if(block != null)
            {
                gyros.Add(block);
            }
        }
        #endregion Get Gyros

        public float GetBank()
        {
            return BankRotor.Angle;
        }

        public float GetPitch()
        {
            return PitchRotor.Angle;
        }

        public float GetRoll()
        {
            return RollRotor.Angle;
        }


        struct ControllerDirection
        {
            public IMyShipController controller;
            public float bank;
            public float pitch;
            public float roll;
        }

        /*
        struct Rotor
        {
            public IMyMotorStator rotor;
            //public string name;
            public float angle;
        }
        */

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
            if (block.IsSameConstructAs(Me))
            {
                return true;
            }
            return false;
        }
        #endregion FilterThis

        #endregion Tool
    }
}
