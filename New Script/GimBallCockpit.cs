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
    partial class GimBallCockpit : MyGridProgram
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
        const string _YawTag = "[Ball - Yaw]";
        const string _PitchTag = "[Ball - Pitch]";
        const string _RollTag = "[Ball - Roll]";

        const float _DeadZone = 5;
        float acceleration = 5;

        float YawDemand;
        float PitchDemand;
        float RollDemand;


        //List<IMyTerminalBlock> Group = new List<IMyTerminalBlock>();
        //IMyBlockGroup Group;
        IMyShipController Controller;
        List<IMyTerminalBlock> Gyros = new List<IMyTerminalBlock>();
            GyroscopeControl ControlledGyros = null;
        List<IMyTerminalBlock> Rotors = new List<IMyTerminalBlock>();

        //IMyTerminalBlock ShipController;
        //IMyTerminalBlock YawRotor;
        //IMyTerminalBlock PitchRotor;
        //IMyTerminalBlock RollRotor;

        IMyMotorStator YawRotor;
        IMyMotorStator PitchRotor;
        IMyMotorStator RollRotor;

        ShipOrientation shipOrientation = new ShipOrientation();

        //Rotor BankRotorDetails = new Rotor();
        //Rotor PitchRotorDetails = new Rotor();
        //Rotor RollRotorDetails = new Rotor();


        public GimBallCockpit()
        {
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, PopulateLists);

            Setup();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string arg, UpdateType updateSource)
        {
            switch (arg.ToUpperInvariant())
            {

            }
            GyroDemand();
        }

        void Setup()
        {  
            
            GetRotors();
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
                Controller = (IMyShipController)block;
            }
            return false;
        }

        //The gyros have to inherit the class and then be initialized 
        public GyroscopeControl InitializedGyroscopeControl
        {
            get
            {
                if (ControlledGyros == null)
                {
                    ControlledGyros = new GyroscopeControl();
                    ControlledGyros.Init(Gyros, shipUp: shipOrientation.ShipUp, shipLeft: shipOrientation.ShipLeft, shipForward: shipOrientation.ShipForward);
                }
                return ControlledGyros;
            }
        }

        #region Ship Orientation
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
        #endregion Ship Orientation

        #region Get Rotors
        void GetRotors()
        {
            var group = GridTerminalSystem.GetBlockGroupWithName(_GroupName);
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
                if (StringContains(block.CustomName, _YawTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    YawRotor = v;
                    //BankRotorDetails.rotor = YawRotor;
                    //BankRotorDetails.angle = YawRotor.Angle;
                }
                if (StringContains(block.CustomName, _PitchTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    PitchRotor = v;
                    //PitchRotorDetails.rotor = PitchRotor;
                    //PitchRotorDetails.angle = PitchRotor.Angle;
                }
                if (StringContains(block.CustomName, _RollTag))
                {
                    AddRotor(block, Rotors);
                    var v = (IMyMotorStator)block;
                    RollRotor = v;
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

        public void GyroDemand()
        {
            /*
            public const int Yaw = 0;
            public const int Pitch = 1;
            public const int Roll = 2;

            SetAxisVelocityRPM(int axis, float rpmVelocity)
            */
            GyroYawDemand();
            GyroPitchDemand();
            GyroRollDemand();

        }
        public void GyroYawDemand()
        {
            float angle = RotorAngle();
            if (angle > _DeadZone)
            {
                YawDemand = MathHelper.Clamp(YawDemand + acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(0, YawDemand);
            }
            if (angle < -_DeadZone)
            {
                YawDemand = MathHelper.Clamp(YawDemand - acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(0, YawDemand);
            }
        }

        public void GyroPitchDemand()
        {
            float angle = RotorAngle();
            if (angle > _DeadZone)
            {
                PitchDemand = MathHelper.Clamp(PitchDemand + acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(1, PitchDemand);
            }
            if (angle < -_DeadZone)
            {
                PitchDemand = MathHelper.Clamp(PitchDemand - acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(1, PitchDemand);
            }
        }

        public void GyroRollDemand()
        {
            float angle = RotorAngle();
            if (angle > _DeadZone)
            {
                RollDemand = MathHelper.Clamp(RollDemand + acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(2, RollDemand);
            }
            if (angle < -_DeadZone)
            {
                RollDemand = MathHelper.Clamp(RollDemand - acceleration, -30, 30);
                InitializedGyroscopeControl.SetAxisVelocityRPM(2, RollDemand);
            }
        }

        /**
         *  returns only the difference from 0º to the rotor current angle
         *  in case rotor is at negative degrees or above 360º
         */
        private float RotorAngle()
        {
            float relativeRotorAngle = Math.Abs(RollRotor.Angle / 360);
            float singleTurnAngle = (relativeRotorAngle - ((int)relativeRotorAngle)) * 360;
            float angle = singleTurnAngle * 180 / MathHelper.Pi;
            return angle;
        }


        struct ControllerDirection
        {
            public IMyShipController controller;
            public float yaw;
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

            #region GyroscopeControl Class
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
                foreach (Gyroscope gyro in controlledGyroscopes)
                {
                    gyro.Gyro.SetValue<bool>("Override", enable);
                }
            }

            public void SetAxisVelocity(int axis, float velocity)
            {
                foreach (Gyroscope gyro in controlledGyroscopes)
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
                foreach (Gyroscope gyro in controlledGyroscopes)
                {
                    gyro.Gyro.SetValue<float>("Yaw", 0.0f);
                    gyro.Gyro.SetValue<float>("Pitch", 0.0f);
                    gyro.Gyro.SetValue<float>("Roll", 0.0f);
                }
            }
        }
        #endregion GyroscopeControl Class


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
