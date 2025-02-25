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
using System.Timers;
using System.Reflection;

namespace Veever.Shadowbringers.theSeatofSacrifice;

[ScriptType(name: "LV.80 光之战士歼灭战", territorys: [922], guid: "864c6d7e-20bd-49b6-93ec-31b4d70e1afd",
    version: "0.0.0.5", author: "Veever", note: noteStr)]

public class theSeatofSacrifice
{
    const string noteStr =
    """
    v0.0.0.5:
    1. 现已支持几乎所有机制播报及绘图
    2. 支持DR 自动动态演练开关（默认为打开状态）
    3. 目前光明剑不确定是否只有两个情况，如果有更多的情况没有画出来的话请带arr回放在dc向我反馈
    4. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    5. v0.0.0.3，删除自动动态演练开关 关闭后会offload的方法
    6. 更新光之剑出现在东西侧的情况
    7. v0.0.0.4, 可能解决了光之剑东西侧画反的问题（回放太少所以不清楚是否解决，如果还是画错请dc@我）
    鸭门。
    """;

    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;
    [UserSetting("TTS开关(不要与DR TTS开关同时开启)")]
    public bool isTTS { get; set; } = false;
    [UserSetting("DR TTS开关(不要与TTS开关同时开启)")]
    public bool isDRTTS { get; set; } = true;
    [UserSetting("DR 自动动态演练开关")]
    public bool isDRQTE { get; set; } = true;

    private readonly object RadiantMeteorTTSLock = new object();
    private readonly object RadiantDesperadoLock = new object();
    private readonly object FlareBreathLock = new object();
    private readonly object RadiantBraverLock = new object();

    public bool fireRecord;
    public bool iceRecord;
    public string DelugeofDeathName;
    public int RadiantMeteorTTS;
    public bool To00A1TTS;
    public int RadiantDesperadoTTS;
    public int FlareBreathTTS;
    public int RadiantBraverTTS;

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        fireRecord = false;
        iceRecord = false;
        DelugeofDeathName = "未知目标";
        RadiantMeteorTTS = 0;
        To00A1TTS = false;
        RadiantDesperadoTTS = 0;
        FlareBreathTTS = 0;
        RadiantBraverTTS = 0;
    }

    [ScriptMethod(name: "恐惧释放", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20263"])]
    public void TerrorUnleashed(Event @event, ScriptAccessory accessory)
    {
        if(isText) accessory.Method.TextInfo("全员1血，奶妈注意回复至满血", duration: 4700, true);
        accessory.TTS("全员1血，奶妈注意回复至满血", isTTS, isDRTTS);
    }

    [ScriptMethod(name: "绝对爆炎", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00E3"])]
    public async void AbsoluteFireIII(Event @event, ScriptAccessory accessory)
    {
        if (@event.TargetId() == accessory.Data.Me)
        {
            if (isText) accessory.Method.TextInfo("准备停止移动", duration: 2000, true);
            accessory.TTS("准备停止移动", isTTS, isDRTTS);
            await Task.Delay(4700);
            if (isText) accessory.Method.TextInfo("停止移动", duration: 4000, true);
            accessory.TTS("停止移动", isTTS, isDRTTS);
        }
    }

    [ScriptMethod(name: "绝对冰封", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00E1"])]
    public async void AbsoluteBlizzardIII(Event @event, ScriptAccessory accessory)
    {
        if (@event.TargetId() == accessory.Data.Me)
        {
            if (isText) accessory.Method.TextInfo("准备持续移动", duration: 2000, true);
            accessory.TTS("准备持续移动", isTTS, isDRTTS);
            await Task.Delay(4700);
            if (isText) accessory.Method.TextInfo("持续移动，不要停", duration: 2500, true);
            accessory.TTS("持续移动，不要停", isTTS, isDRTTS);
        }
    }

    [ScriptMethod(name: "魔法剑·绝对爆炎记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20242"])]
    public async void ImbuedAbsoluteFireIII(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("一会儿准备停止移动", duration: 3000, true);
        accessory.TTS("一会儿准备停止移动", isTTS, isDRTTS);
        fireRecord = true;
    }

    [ScriptMethod(name: "魔法剑·绝对冰封记录", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20243"])]
    public async void ImbuedAbsoluteBlizzardIII(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("一会儿准备持续移动", duration: 3000, true);
        accessory.TTS("一会儿准备持续移动", isTTS, isDRTTS);
        iceRecord = true;
    }

    [ScriptMethod(name: "光明利剑(钢铁)&魔法剑技·光明利剑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(20240|20299)$"])]
    public async void CoruscantSaberOutside(Event @event, ScriptAccessory accessory)
    {
        if (@event.ActionId() == 20299)
        {
            if (isText) accessory.Method.TextInfo($"钢铁，远离Boss，准备{(fireRecord ? "停止移动" : "持续移动")}", duration: 5500, true);
            accessory.TTS($"钢铁，远离Boss，准备{(fireRecord ? "停止移动" : "持续移动")}", isTTS, isDRTTS);
        }
        else
        {
            if (isText) accessory.Method.TextInfo("钢铁，远离Boss", duration: 6700, true);
            accessory.TTS("钢铁，远离Boss", isTTS, isDRTTS);
        }

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "光明利剑(钢铁)&魔法剑技·光明利剑";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(10);
        dp.DestoryAt = 7000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        if (fireRecord)
        {
            await Task.Delay(7000);
            if (isText) accessory.Method.TextInfo("停止移动", duration: 3000, true);
            accessory.TTS("停止移动", isTTS, isDRTTS);
            fireRecord = false;
        }
        if (iceRecord)
        {
            await Task.Delay(7000);
            if (isText) accessory.Method.TextInfo("持续移动", duration: 2500, true);
            accessory.TTS("持续移动", isTTS, isDRTTS);
            iceRecord = false;
        }
    }
    [ScriptMethod(name: "光明利剑(月环)&魔法剑技·光明利剑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(20241|20300)$"])]
    public async void ImbuedCoruscance(Event @event, ScriptAccessory accessory)
    {
        if (@event.ActionId() == 20300)
        {
            if (isText) accessory.Method.TextInfo($"月环，去Boss脚下，准备{(fireRecord ? "停止移动" : "持续移动")}", duration: 5500, true);
            accessory.TTS($"月环，去Boss脚下，准备{(fireRecord ? "停止移动" : "持续移动")}", isTTS, isDRTTS);
        } else
        {
            if (isText) accessory.Method.TextInfo("月环，去Boss脚下", duration: 6500, true);
            accessory.TTS("月环，去Boss脚下", isTTS, isDRTTS);
        }


        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "光明利剑(月环)&魔法剑技·光明利剑";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(5);
        dp.DestoryAt = 7000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        var dp1 = accessory.Data.GetDefaultDrawProperties();
        dp1.Name = "月环指路";
        dp1.Owner = accessory.Data.Me;
        dp1.Color = accessory.Data.DefaultSafeColor;
        dp1.ScaleMode |= ScaleMode.YByDistance;
        dp1.TargetPosition = @event.SourcePosition();
        dp1.Scale = new(2);
        dp1.DestoryAt = 7000;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp1);

        if (fireRecord)
        {
            await Task.Delay(7000);
            if (isText) accessory.Method.TextInfo("停止移动", duration: 2500, true);
            accessory.TTS("停止移动", isTTS, isDRTTS);
            fireRecord = false;
        }
        if (iceRecord)
        {
            await Task.Delay(7000);
            if (isText) accessory.Method.TextInfo("一会儿准备持续移动", duration: 2500, true);
            accessory.TTS("一会儿准备持续移动", isTTS, isDRTTS);
            iceRecord = false;
        }
    }

    [ScriptMethod(name: "光之剑", eventType: EventTypeEnum.EnvControl, eventCondition: ["Id:01000800"])]
    public void SwordofLight(Event @event, ScriptAccessory accessory)
    {
        if(@event.Index() == 0x14)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "光之剑南北1";
            dp.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            dp.Position = new Vector3(100.00f, 0.00f, 80.00f);
            dp.TargetPosition = new Vector3(100.00f, 0.00f, 100.00f);
            dp.Radian = float.Pi / 180 * 53;
            dp.Scale = new(60);
            dp.DestoryAt = 11000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
        if (@event.Index() == 0x16)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "光之剑南北2";
            dp.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            dp.Position = new Vector3(100.00f, 0.00f, 120.00f);
            dp.TargetPosition = new Vector3(100.00f, 0.00f, 100.00f);
            dp.Radian = float.Pi / 180 * 53;
            dp.Scale = new(60);
            dp.DestoryAt = 11000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        if (@event.Index() == 0x15)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "光之剑东西1";
            dp.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            
            dp.Position = new Vector3(120.00f, 0.00f, 100.00f);
            dp.TargetPosition = new Vector3(100.00f, 0.00f, 100.00f);
            dp.Radian = float.Pi / 180 * 53;
            dp.Scale = new(60);
            dp.DestoryAt = 11000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        if (@event.Index() == 0x17)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "光之剑东西2";
            dp.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            dp.Position = new Vector3(80.00f, 0.00f, 100.00f);
            dp.TargetPosition = new Vector3(100.00f, 0.00f, 100.00f);
            dp.Radian = float.Pi / 180 * 53;
            dp.Scale = new(60);
            dp.DestoryAt = 11000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

    }

    [ScriptMethod(name: "俯冲", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20261"])]
    public void Cauterize(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "俯冲";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(20, 40);
        dp.DestoryAt = 4000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "生辰星位", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:21297"])]
    public void Ascendance(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        accessory.TTS("AOE", isTTS, isDRTTS);
        if (isDRQTE) accessory.Method.SendChat("/pdr load autoQTE");
    }

    [ScriptMethod(name: "究极·交汇", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:21627"])]
    public async void UltimateCrossover(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(3400);
        if (isText) accessory.Method.TextInfo("坦克LB", duration: 6000, true);
        accessory.TTS("坦克LB", isTTS, isDRTTS);
    }

    [ScriptMethod(name: "踩塔提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20284"])]
    public async void SpecterofLight(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(10000);
        if (isText) accessory.Method.TextInfo("踩塔，每个塔至少两人", duration: 6000, true);
        accessory.TTS("踩塔，每个塔至少两人", isTTS, isDRTTS);
    }
    [ScriptMethod(name: "忍者击退提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20252"])]
    public async void SuitonSan(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("击退", duration: 5700, true);
        accessory.TTS("击退", isTTS, isDRTTS);

        var dp1 = accessory.Data.GetDefaultDrawProperties();
        dp1.Name = $"忍者击退提示";
        dp1.Scale = new(1.5f, 26);
        dp1.Color = accessory.Data.DefaultSafeColor;
        dp1.Owner = accessory.Data.Me;
        dp1.TargetPosition = @event.TargetPosition();
        dp1.Rotation = float.Pi;
        dp1.DestoryAt = 5700;
        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp1);
    }

    [ScriptMethod(name: "远古龙炎冲", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20265"])]
    public void ElddragonDive(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        accessory.TTS("AOE", isTTS, isDRTTS);
    }

    [ScriptMethod(name: "狱火大地", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20254"])]
    public void BrimstoneEarth(Event @event, ScriptAccessory accessory)
    {
        //accessory.Method.TextInfo("AOE", duration: 4700, true);
        //accessory.TTS("AOE", isTTS, isDRTTS);
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "狱火大地";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Position = @event.EffectPosition();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Delay = 8000;
        dp.Scale = new Vector2(15);
        dp.DestoryAt = 14000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }


    [ScriptMethod(name: "核爆", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20256"])]
    public void DelugeofDeath(Event @event, ScriptAccessory accessory)
    {
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";
        DelugeofDeathName = tname;
    }

    [ScriptMethod(name: "分摊", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00A1"])]
    public async void To00A1(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(350);
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";

        if (To00A1TTS == false)
        {
            if (isText) accessory.Method.TextInfo($"与{tname}分摊, 核爆{DelugeofDeathName}远离", duration: 4700, true);
            accessory.TTS($"与{tname}分摊, 核爆{DelugeofDeathName}远离", isTTS, isDRTTS);
        } else
        {
            if (isText) accessory.Method.TextInfo($"与{tname}分摊", duration: 4700, true);
            accessory.TTS($"与{tname}分摊", isTTS, isDRTTS);
        }
        To00A1TTS = true;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "分摊";
        dp.Color = new Vector4(0 / 255.0f, 255 / 255.0f, 255 / 255.0f, 1.0f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20264"])]
    public void TheBitterEnd(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("死刑准备", duration: 4700, true);
        accessory.TTS($"死刑准备", isTTS, isDRTTS);
    }

    [ScriptMethod(name: "LB1顺劈", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00EA"])]
    public async void RadiantBraver(Event @event, ScriptAccessory accessory)
    {
        lock(RadiantBraverLock)
        {
            if (RadiantBraverTTS == 0)
            {
                if (isText) accessory.Method.TextInfo("扇形顺劈, 不要重叠", duration: 4700, true);
                accessory.TTS($"扇形顺劈, 不要重叠", isTTS, isDRTTS);
            }
            RadiantBraverTTS++;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "LB1顺劈";
            dp.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            dp.Position = new Vector3(100.00f, 0.00f, 100.00f);
            dp.TargetObject = @event.TargetId();
            dp.Radian = float.Pi / 180 * 90;
            dp.Scale = new(60);
            dp.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
    }

    [ScriptMethod(name: "LB2分组分摊", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:20294"])]
    public async void RadiantDesperado(Event @event, ScriptAccessory accessory)
    {
        lock(RadiantDesperadoLock)
        {
            if (RadiantDesperadoTTS == 0)
            {
                if (isText) accessory.Method.TextInfo($"分组44分摊，不要重叠", duration: 8000, true);
                accessory.TTS("分组四四分摊，不要重叠", isTTS, isDRTTS);
            }
            RadiantDesperadoTTS++;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "RadiantDesperado";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Owner = @event.SourceId();
            dp.TargetObject = @event.TargetId();
            dp.Scale = new Vector2(6, 20);
            dp.DestoryAt = 10000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
    }

    [ScriptMethod(name: "LB3陨石点名提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:20251"])]
    public async void RadiantMeteor(Event @event, ScriptAccessory accessory)
    {
        lock(RadiantMeteorTTSLock)
        {
            if (RadiantMeteorTTS == 0)
            {
                if (isText) accessory.Method.TextInfo($"陨石点名, 让出中间位置 非点名玩家去中间", duration: 8000, true);
                accessory.TTS("陨石点名, 让出中间位置 非点名玩家去中间", isTTS, isDRTTS);
            }
            RadiantMeteorTTS++;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "LB3陨石点名提示";
            dp.Color = new Vector4(138 / 255.0f, 43 / 255.0f, 251 / 226.0f, 1.0f);
            dp.ScaleMode = ScaleMode.ByTime;
            dp.Owner = @event.TargetId();
            dp.Scale = new Vector2(20);
            dp.DestoryAt = 8000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(name: "核爆吐息(连线处理)", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0011"])]
    public async void FlareBreath(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(1000);
        lock(FlareBreathLock)
        {
            List<Vector3> vectorList = new List<Vector3>
        {
            new Vector3(83.71f, 0.00f, 83.80f),
            new Vector3(115.75f, 0.00f, 84.96f),
            new Vector3(116.72f, 0.00f, 116.73f),
            new Vector3(84.40f, 0.00f, 115.74f),
        };
            if (FlareBreathTTS == 0)
            {
                if (isText) accessory.Method.TextInfo("将线拉到对应的小怪的角落安全范围", duration: 4700, true);
                accessory.TTS($"将线拉到对应的小怪的角落安全范围", isTTS, isDRTTS);
                for (int i = 0; i < 4; i++)
                {
                    var dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = $"核爆吐息安全点{i}";
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Position = vectorList[i];
                    dp.Scale = new Vector2(4);
                    dp.DestoryAt = 9000;
                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                }
            }
            FlareBreathTTS++;

            var dp1 = accessory.Data.GetDefaultDrawProperties();
            dp1.Name = "核爆吐息(连线处理)";
            dp1.Color = new Vector4(253 / 255.0f, 223 / 255.0f, 196 / 255.0f, 1.0f);
            dp1.Owner = @event.SourceId();
            dp1.TargetObject = @event.TargetId();
            dp1.Radian = float.Pi / 180 * 90;
            dp1.Scale = new(60);
            dp1.DestoryAt = 9000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp1);

            if (@event.TargetId() == accessory.Data.Me)
            {
                var dp2 = accessory.Data.GetDefaultDrawProperties();
                dp2.Name = "核爆吐息指路";
                dp2.Owner = accessory.Data.Me;
                dp2.Color = accessory.Data.DefaultSafeColor;
                dp2.ScaleMode |= ScaleMode.YByDistance;
                dp2.TargetPosition = @event.SourcePosition();
                dp2.Scale = new(2);
                dp2.DestoryAt = 9000;
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp2);
            }
        }
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


    public static string DurationMilliseconds(this Event @event)
    {
        return JsonConvert.DeserializeObject<string>(@event["DurationMilliseconds"]) ?? string.Empty;
    }

    public static uint SourceRotation(this Event @event)
    {
        return ParseHexId(@event["SourceRotation"], out var sourceRotation) ? sourceRotation : 0;
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
        return JsonConvert.DeserializeObject<string>(@event["SourceName"]) ?? string.Empty;
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