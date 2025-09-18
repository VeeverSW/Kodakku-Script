using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.ComponentModel;

namespace Veever.A_Realm_Reborn.FlickingSticksandTakingNames;

[ScriptType(name: "LV.25 击溃哥布林炸弹军团！", territorys: [219], guid: "f63ef61a-5fe6-437a-afb4-513dafecbb54",
    version: "0.0.0.3", author: "Veever", note: noteStr)]

public class Flicking_Sticks_and_Taking_Names
{
    const string noteStr =
    """
    v0.0.0.3:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = false;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = true;

    [UserSetting("Debug开关")]
    public bool isDebug { get; set; } = false;

    public int showTargetCount;

    private readonly object showTargetLock = new object();

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        showTargetCount = 0;
    }


    [ScriptMethod(name: "目标显示", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:regex:^(531|530|587|533)$"])]
    public async void showTarget(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(2000);
        lock (showTargetLock)
        {
            if (@event.DataId() == 587)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = $"{@event.SourceId()}";
                //dp.Color = accessory.Data.DefaultDangerColor;
                dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 0 / 255.0f, 1.0f);
                dp.Owner = @event.SourceId();
                dp.ScaleMode = ScaleMode.ByTime;
                dp.Scale = new Vector2(0.5f);
                dp.DestoryAt = long.MaxValue;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
            }
            else if (@event.DataId() == 533)
            {
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = $"{@event.SourceId()}";
                dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 0 / 255.0f, 1.0f);
                dp.Owner = @event.SourceId();
                dp.ScaleMode = ScaleMode.ByTime;
                dp.Scale = new Vector2(1f);
                dp.DestoryAt = long.MaxValue;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
            }
            else
            {
                var dp1 = accessory.Data.GetDefaultDrawProperties();
                dp1.Name = $"{@event.SourceId()}";
                dp1.Color = accessory.Data.DefaultSafeColor;
                dp1.Owner = @event.SourceId();
                dp1.ScaleMode = ScaleMode.ByTime;
                dp1.Scale = new Vector2(0.5f);
                dp1.DestoryAt = long.MaxValue;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp1);
            }
            showTargetCount++;

        }
    }

    [ScriptMethod(name: "删除绘制", eventType: EventTypeEnum.RemoveCombatant, eventCondition: ["DataId:regex:^(531|530|587)$"])]
    public async void delDraw(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw($"{@event.SourceId()}");
    }

    [ScriptMethod(name: "bomb", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:596"])]
    public async void bomb(Event @event, ScriptAccessory accessory)
    {
        var dp1 = accessory.Data.GetDefaultDrawProperties();
        dp1.Name = $"{@event.SourceId()}";
        dp1.Color = accessory.Data.DefaultDangerColor;
        dp1.Owner = @event.SourceId();
        dp1.ScaleMode = ScaleMode.ByTime;
        dp1.Scale = new Vector2(3f);
        dp1.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp1);
    }

    [ScriptMethod(name: "删除绘制1", eventType: EventTypeEnum.Director, eventCondition: ["Command:40000002", "Instance:80032718"])]
    public async void delDraw1(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }
}





public static class EventExtensions
{
    private static bool ParseHexId(string? idStr, out uint id)
    {
        id = 0;
        if (string.IsNullOrEmpty(idStr)) return false;
        try
        {
            var idStr2 = idStr.Replace("0x", "");
            id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static uint ActionId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["ActionId"]);
    }

    public static uint SourceId(this Event @event)
    {
        return ParseHexId(@event["SourceId"], out var id) ? id : 0;
    }

    public static uint SourceDataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["SourceDataId"]);
    }

    public static uint DataId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["DataId"]);
    }

    public static uint Command(this Event @event)
    {
        return ParseHexId(@event["Command"], out var cid) ? cid : 0;
    }

    public static string DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<string>(@event["DurationMilliseconds"]) ?? string.Empty;
    }

    public static float SourceRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["SourceRotation"]);
    }

    public static float TargetRotation(this Event @event)
    {
        return JsonConvert.DeserializeObject<float>(@event["TargetRotation"]);
    }

    public static byte Index(this Event @event)
    {
        return (byte)(ParseHexId(@event["Index"], out var index) ? index : 0);
    }

    public static uint State(this Event @event)
    {
        return ParseHexId(@event["State"], out var state) ? state : 0;
    }

    public static string SourceName(this Event @event)
    {
        return @event["SourceName"];
    }

    public static string TargetName(this Event @event)
    {
        return @event["TargetName"];
    }

    public static uint TargetId(this Event @event)
    {
        return ParseHexId(@event["TargetId"], out var id) ? id : 0;
    }

    public static Vector3 SourcePosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["SourcePosition"]);
    }

    public static Vector3 TargetPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["TargetPosition"]);
    }

    public static Vector3 EffectPosition(this Event @event)
    {
        return JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
    }

    public static uint DirectorId(this Event @event)
    {
        return ParseHexId(@event["DirectorId"], out var id) ? id : 0;
    }

    public static uint StatusId(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StatusId"]);
    }

    public static uint StackCount(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["StackCount"]);
    }

    public static uint Param(this Event @event)
    {
        return JsonConvert.DeserializeObject<uint>(@event["Param"]);
    }
}


public static class Extensions
{
    public static void TTS(this ScriptAccessory accessory, string text, bool isTTS, bool isDRTTS)
    {
        if (isDRTTS)
        {
            accessory.Method.SendChat($"/pdr tts {text}");
        }
        else if (isTTS)
        {
            accessory.Method.TTS(text);
        }
    }
}