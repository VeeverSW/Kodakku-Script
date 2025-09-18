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
using System.Windows;

namespace Veever.A_Realm_Reborn.ShadowandClaw;

[ScriptType(name: "LV.35 注意无敌的眷属，讨伐大型妖异！", territorys: [223], guid: "70cf9a76-fc90-4f2b-9471-504472fb1b1e",
    version: "0.0.0.4", author: "Veever", note: noteStr)]

public class Shadow_and_Claw
{
    const string noteStr =
    """
    v0.0.0.4:
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
    public int drawEyeDelay;

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        attackCount = 0;
        drawEyeDelay = 0;
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

    [ScriptMethod(name: "notify", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:870", "SourceId:400111A2"])]
    public void notify(Event @event, ScriptAccessory accessory)
    {
        if (isDebug) accessory.Method.SendChat($"/e notifycount: {attackCount}");
        if (attackCount == 0)
        {
            if (isText) accessory.Method.TextInfo("集中攻击 暗影魔爪", duration: 2000, true);
            accessory.TTS("集中攻击 暗影魔爪", isTTS, isDRTTS);
            if (isMark) accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Attack1, LocalMark);
        }
        attackCount++;
    }

    [ScriptMethod(name: "暗影之眼范围", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:1802"])]
    public async void drawEye(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("不要攻击 暗影之眼, 注意远离钢铁", duration: 5000, true);
        accessory.TTS("不要攻击 暗影之眼, 注意远离钢铁", isTTS, isDRTTS);

        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    drawEyeDelay = 7900;
                    break;
                case 1:
                    drawEyeDelay = 18100;
                    break;
                case 2:
                    drawEyeDelay = 28400;
                    break;
                case 3:
                    drawEyeDelay = 38700;
                    break;
                case 4:
                    drawEyeDelay = 49000;
                    break;
            }

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"暗影之眼范围{i}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = @event.SourceId();
            dp.ScaleMode = ScaleMode.ByTime;
            dp.Scale = new Vector2(14f);
            dp.Delay = drawEyeDelay;
            dp.DestoryAt = 2500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

            var dp1 = accessory.Data.GetDefaultDrawProperties();
            dp1.Name = $"暗影之眼范围{i}描边";
            dp1.Scale = new(14f);
            dp1.InnerScale = new(13.9f);
            dp1.Radian = float.Pi * 2;
            dp1.Color = new Vector4(178 / 255.0f, 34 / 255.0f, 34 / 255.0f, 10.0f);
            dp1.Owner = @event.SourceId();
            dp1.Delay = drawEyeDelay;
            dp1.DestoryAt = 2500;
            dp1.Radian = 2 * float.Pi;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp1);
        }
        if (!LocalMark) await Task.Delay(1000);         // 拟人
        if (isMark) accessory.Method.Mark(@event.SourceId(), KodakkuAssist.Module.GameOperate.MarkType.Stop1, LocalMark);
    }

    [ScriptMethod(name: "三连爪", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:932"])]
    public void Triclip(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "三连爪";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(4f, 5f);
        dp.DestoryAt = 2500;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "咒言", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(585|586)$"])]
    public async void Curse(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(5);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"咒言范围";
        //dp.Color = accessory.Data.DefaultDangerColor;
        dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(3.8f);
        dp.DestoryAt = 2400;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "删除绘制", eventType: EventTypeEnum.Director, eventCondition: ["Command:40000002", "Instance:8003271B"])]
    public async void delDraw(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    [ScriptMethod(name: "骇人嚎叫", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:933"])]
    public async void FrightfulRoar(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = $"骇人嚎叫";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(8f);
        dp.DestoryAt = 2700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "删除骇人嚎叫", eventType: EventTypeEnum.CancelAction, eventCondition: ["ActionId:933"])]
    public async void delFrightfulRoar(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw("骇人嚎叫");
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