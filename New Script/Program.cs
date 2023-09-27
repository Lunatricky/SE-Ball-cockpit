﻿using Sandbox.Game.EntityComponents;
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
         * Throttle System
         * Smart thrust cut off
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

        ShipOrientation shipOrientation = new ShipOrientation();

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
            GetShipOrientation();
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

        
    public struct ShipOrientation
        {
            public Base6Directions.Direction ShipUp;
            public Base6Directions.Direction ShipLeft;
            public Base6Directions.Direction ShipForward;
        }

    void GetShipOrientation()
        {
            if (Controller != null)
            {
                IMyShipController reference;
                reference = Controller;
                shipOrientation.ShipUp = reference.Orientation.TransformDirection(Base6Directions.Direction.Up);//4
                shipOrientation.ShipLeft = reference.Orientation.TransformDirection(Base6Directions.Direction.Left);//2
                shipOrientation.ShipForward = reference.Orientation.TransformDirection(Base6Directions.Direction.Forward);//0
            }
        }

        //Reference: ZerothAngel
        public class GyroscopeControl
        {
            public const int Yaw = 0;
            public const int Pitch = 1;
            public const int Roll = 2;

            public readonly string[] AxisNames = new string[] { "Yaw", "Pitch", "Roll" };
            public struct GyroscopeAxis
            {


                public int LocalAxis;
                public int Sign;

                public GyroscopeAxis(int localAxis, int sign)
                {
                    LocalAxis = localAxis;
                    Sign = sign;
                }
            }
            public struct Gyroscope
            {
                public IMyGyro Gyro;
                public GyroscopeAxis[] AxisInfo;

                public Gyroscope(IMyGyro gyro, Base6Directions.Direction shipUp, Base6Directions.Direction shipLeft, Base6Directions.Direction shipForward)
                {
                    Gyro = gyro;
                    AxisInfo = new GyroscopeAxis[3];

                    // Determine yaw axis
                    switch (gyro.Orientation.TransformDirectionInverse(shipUp))
                    {
                        case Base6Directions.Direction.Up:
                            AxisInfo[Yaw] = new GyroscopeAxis(Yaw, -1);
                            break;
                        case Base6Directions.Direction.Down:
                            AxisInfo[Yaw] = new GyroscopeAxis(Yaw, 1);
                            break;
                        case Base6Directions.Direction.Left:
                            AxisInfo[Yaw] = new GyroscopeAxis(Pitch, 1);
                            break;
                        case Base6Directions.Direction.Right:
                            AxisInfo[Yaw] = new GyroscopeAxis(Pitch, -1);
                            break;
                        case Base6Directions.Direction.Forward:
                            AxisInfo[Yaw] = new GyroscopeAxis(Roll, 1);
                            break;
                        case Base6Directions.Direction.Backward:
                            AxisInfo[Yaw] = new GyroscopeAxis(Roll, -1);
                            break;
                    }
                    // Determine pitch axis
                    switch (gyro.Orientation.TransformDirectionInverse(shipLeft))
                    {
                        case Base6Directions.Direction.Up:
                            AxisInfo[Pitch] = new GyroscopeAxis(Yaw, -1);
                            break;
                        case Base6Directions.Direction.Down:
                            AxisInfo[Pitch] = new GyroscopeAxis(Yaw, 1);
                            break;
                        case Base6Directions.Direction.Left:
                            AxisInfo[Pitch] = new GyroscopeAxis(Pitch, -1);
                            break;
                        case Base6Directions.Direction.Right:
                            AxisInfo[Pitch] = new GyroscopeAxis(Pitch, 1);
                            break;
                        case Base6Directions.Direction.Forward:
                            AxisInfo[Pitch] = new GyroscopeAxis(Roll, 1);
                            break;
                        case Base6Directions.Direction.Backward:
                            AxisInfo[Pitch] = new GyroscopeAxis(Roll, -1);
                            break;
                    }

                    // Determine roll axis
                    switch (gyro.Orientation.TransformDirectionInverse(shipForward))
                    {
                        case Base6Directions.Direction.Up:
                            AxisInfo[Roll] = new GyroscopeAxis(Yaw, -1);
                            break;
                        case Base6Directions.Direction.Down:
                            AxisInfo[Roll] = new GyroscopeAxis(Yaw, 1);
                            break;
                        case Base6Directions.Direction.Left:
                            AxisInfo[Roll] = new GyroscopeAxis(Pitch, -1);
                            break;
                        case Base6Directions.Direction.Right:
                            AxisInfo[Roll] = new GyroscopeAxis(Pitch, 1);
                            break;
                        case Base6Directions.Direction.Forward:
                            AxisInfo[Roll] = new GyroscopeAxis(Roll, 1);
                            break;
                        case Base6Directions.Direction.Backward:
                            AxisInfo[Roll] = new GyroscopeAxis(Roll, -1);
                            break;
                    }
                }
            }
            private readonly List<Gyroscope> controlledGyroscopes = new List<Gyroscope>();

            //Link Credit:
            public void Init(IEnumerable<IMyTerminalBlock> blocks, Func<IMyGyro, bool> collect = null, Base6Directions.Direction shipUp = Base6Directions.Direction.Up, Base6Directions.Direction shipLeft = Base6Directions.Direction.Left, Base6Directions.Direction shipForward = Base6Directions.Direction.Forward)
            {
                controlledGyroscopes.Clear();
                for (var e = blocks.GetEnumerator(); e.MoveNext();)
                {
                    var gyro = e.Current as IMyGyro;
                    if (gyro != null && gyro.IsFunctional && gyro.IsWorking && gyro.Enabled && (collect == null || collect(gyro)))
                    {
                        var details = new Gyroscope(gyro, shipUp, shipLeft, shipForward);
                        controlledGyroscopes.Add(details);
                    }
                }
            }
            public void EnableOverride(bool enable)
            {
                foreach(Gyroscope gyro in controlledGyroscopes)
                {
                    gyro.Gyro.SetValue<bool>("Override", enable);
                }
            }

            public void SetAxisVelocity(int axis, float velocity)
            {
                foreach(Gyroscope gyro in controlledGyroscopes)
                {
                    gyro.Gyro.SetValue<float>(AxisNames[gyro.AxisInfo[axis].LocalAxis], gyro.AxisInfo[axis].Sign * velocity);
                }
            }

            public void SetAxisVelocityRPM(int axis, float rpmVelocity)
            {
                SetAxisVelocity(axis, rpmVelocity * MathHelper.RPMToRadiansPerSecond);
            }

            public void Reset()
            {
                foreach(Gyroscope gyro in controlledGyroscopes)
                {
                    gyro.Gyro.SetValue<float>("Yaw", 0.0f);
                    gyro.Gyro.SetValue<float>("Pitch", 0.0f);
                    gyro.Gyro.SetValue<float>("Roll", 0.0f);
                } 
            }
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
