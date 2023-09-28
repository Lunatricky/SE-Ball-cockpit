//x2,0,Rotor\6roll,47,8:&gt;=:15[11]%3,0,#Control\6Surface,21[5]3:true#6:30[7]%2,0,Rotor\6roll,47,8:&lt;=:-15[11]%3,0,#Control\6Surface,21[5]3:true#6:-30[7]%2,0,Rotor\6roll,47,8:&gt;:-15[11]%3,0,#Control\6Surface,21[5]6:0[7]%,y,,1.1.1
// Above is your LOAD LINE. Copy it into Visual Script Builder to load your script.
// dco.pe/vsb

void Main(string argument)
{
    // block declarations
    string ERR_TXT = "";
    List<IMyTerminalBlock> l0 = new List<IMyTerminalBlock>();
    IMyMotorStator v0 = null;
    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(l0, filterThis);
    if (l0.Count == 0)
    {
        ERR_TXT += "no Rotor blocks found\n";
    }
    else
    {
        for (int i = 0; i < l0.Count; i++)
        {
            if (l0[i].CustomName == "Rotor roll")
            {
                v0 = (IMyMotorStator)l0[i];
                break;
            }
        }
        if (v0 == null)
        {
            ERR_TXT += "no Rotor block named Rotor roll found\n";
        }
    }
    List<IMyTerminalBlock> l1 = new List<IMyTerminalBlock>();
    IMyGyro v1 = null;
    if (GridTerminalSystem.GetBlockGroupWithName("Control Surface") != null)
    {
        GridTerminalSystem.GetBlockGroupWithName("Control Surface").GetBlocksOfType<IMyGyro>(l1, filterThis);
        if (l1.Count == 0)
        {
            ERR_TXT += "group Control Surface has no Gyroscope blocks\n";
        }
        else
        {
            for (int i = 0; i < l1.Count; i++)
            {
                v1 = (IMyGyro)l1[i];
            }
            if (v1 == null)
            {
                ERR_TXT += "group Control Surface has no Gyroscope block named Gyroscope\n";
            }
        }
    }
    else
    {
        ERR_TXT += "group Control Surface not found\n";
    }

    // display errors
    if (ERR_TXT != "")
    {
        Echo("Script Errors:\n" + ERR_TXT + "(make sure block ownership is set correctly)");
    }
    else { Echo(""); }

    // logic
    if (((IMyMotorStator)v0).Angle >= 200)
    {
        v1.GyroOverride = true;
        v1.Roll = (float)30;
        Echo("Roll 30rpm\n");
    }
    if (((IMyMotorStator)v0).Angle <= 160)
    {
        v1.GyroOverride = true;
        v1.Roll = (float)-30;
        Echo("Roll -30rpm\n");
    }
    if (((IMyMotorStator)v0).Angle > 160 && ((IMyMotorStator)v0).Angle < 200)
    {
        v1.Roll = (float)0;
        Echo("Roll 0rpm\n");
    }
}

bool filterThis(IMyTerminalBlock block)
{
    return block.CubeGrid == Me.CubeGrid;
}
