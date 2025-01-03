using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using ECommons.ExcelServices.TerritoryEnumeration;
using System.Reflection.Metadata;
using System.Net;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Generic;
using System.ComponentModel;
using ECommons.Reflection;
using System.Windows;

namespace Veever.A_Realm_Reborn.AnnoytheVoid;

[ScriptType(name: "LV.30 讨伐坑道中出现的妖异！", territorys: [222], guid: "9106a502-2bcb-41ac-8598-6c76bd1061ae",
    version: "0.0.0.1", author: "Veever", note: noteStr)]

public class Annoy_the_Void
{
    const string noteStr =
    """
    v0.0.0.1:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    2. 标点开关以及本地开关都在用户设置里面，可自行选择关闭或者开启（默认本地开启）
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = true;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = false;

    [UserSetting("标点开关")]
    public bool isMark { get; set; } = true;

    [UserSetting("本地标点开关(打开则为本地开关，关闭则为小队)")]
    public bool LocalMark { get; set; } = true;

    [UserSetting("Debug开关")]
    public bool isDebug { get; set; } = false;

    public int attackCount;

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        attackCount = 0;
    }


    [ScriptMethod(name: "定罪", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:912"])]
    public void Condemnation(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Condemnation";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(7.3f);
        dp.Radian = float.Pi / 180 * 90;
        dp.DestoryAt = 2500;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "notify", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:870", "SourceId:40011DA3"])]
    public void notify(Event @event, ScriptAccessory accessory)
    {
        if (isDebug) accessory.Method.SendChat($"/e notifycount: {attackCount}");
        if (attackCount == 1)
        {
            if (isText) accessory.Method.TextInfo("集中攻击 布索, 注意交互蓝色火焰削弱BOSS", duration: 6000, true);
            accessory.TTS("集中攻击 布索, 注意交互蓝色火焰削弱BOSS", isTTS, isDRTTS);
            if (isMark) accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack1, LocalMark);
        }
        attackCount++;
    }

    [ScriptMethod(name: "哀叫", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:602"])]
    public async void OvertoneShriek(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(50);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"哀叫范围";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(4.7f);
        dp.DestoryAt = 2800;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
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