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

namespace Veever.A_Realm_Reborn.MorethanaFeeler;

[ScriptType(name: "LV.30 讨伐污染源头魔界花！", territorys: [221], guid: "3113eeb5-96d8-48a8-9069-e1e3e5232ee1",
    version: "0.0.0.3", author: "Veever", note: noteStr)]

public class More_than_a_Feeler
{
    const string noteStr =
    """
    v0.0.0.3:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    2. 标点开关以及本地开关都在用户设置里面，可自行选择关闭或者开启（默认本地开启）
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = false;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = true;

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

    [ScriptMethod(name: "指路", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:1126"])]
    public async void Navi(Event @event, ScriptAccessory accessory)
    {
        if (isDebug) accessory.Method.SendChat("/e im in Navi");
        await Task.Delay(1500);
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "指路";
        dp.Owner = accessory.Data.Me;
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.ScaleMode |= ScaleMode.YByDistance;
        dp.TargetObject = @event.SourceId();
        dp.Scale = new(1);
        dp.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    }

    [ScriptMethod(name: "删除指路", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:872"])]
    public void delNavi(Event @event, ScriptAccessory accessory)
    {
        if (isDebug) accessory.Method.SendChat("/e im in delNavi");
        if (attackCount == 1)
        {
            accessory.Method.RemoveDraw("指路");
            if (isText) accessory.Method.TextInfo("集中攻击 剧毒魔花谭琳", duration: 6000, true);
            accessory.TTS("集中攻击 剧毒魔花谭琳", isTTS, isDRTTS);
            if (isMark) accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack1, LocalMark);
        }
        attackCount++;
    }

    [ScriptMethod(name: "臭气", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:604"])]
    public void BadBreath(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "BadBreath";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(15);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 3000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "腐坏泡泡", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:1027"])]
    public void stalebubble(Event @event, ScriptAccessory accessory)
    {
        var dp1 = accessory.Data.GetDefaultDrawProperties();
        dp1.Name = "stalebubble";
        dp1.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 0 / 225.0f, 1f);
        dp1.Owner = @event.SourceId();
        dp1.ScaleMode = ScaleMode.ByTime;
        dp1.Scale = new Vector2(0.5f);
        dp1.DestoryAt = long.MaxValue;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp1);
    }

    [ScriptMethod(name: "腐坏泡泡销毁", eventType: EventTypeEnum.RemoveCombatant, eventCondition: ["DataId:1027"])]
    public void Delstalebubble(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw($"{@event.SourceId()}");
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