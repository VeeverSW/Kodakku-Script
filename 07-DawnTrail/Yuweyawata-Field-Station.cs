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

namespace Veever.DawnTrail.YuweyawataFieldStation;

[ScriptType(name: "LV.100 Yuweyawata Field Station", territorys: [1242], guid: "992e47a8-17d0-4379-891b-0762c0509257",
    version: "0.0.0.6", author: "Veever", note: noteStr)]

public class YuweyawataFieldStation
{
    public int RawElectropeCount, TelltaleTearsTTSCount, LightningStormTTSCount, DarkTwoCount, JaggedEdgeCount;
    private readonly object LeapingEarthLock = new object();
    private readonly object JaggedEdgeLock = new object();
    public int LeapingEarthResult;

    const string noteStr =
    """
    v0.0.0.6:
    1. 现在支持文字横幅/TTS开关/DR TTS开关（使用DR TTS开关之前请确保你已正确安装`DailyRoutines`插件）（请确保两个TTS开关不要同时打开）
    2. 以前的这几个脚本的底层扩展目前懒得重构（就能加啥随便加了）
    3. 删除顺时针地火判断
    4. v0.0.0.6，更新名字方便整理
    鸭门。
    """;
    [UserSetting("文字横幅提示开关")]
    public bool isText { get; set; } = true;

    [UserSetting("TTS开关")]
    public bool isTTS { get; set; } = true;

    [UserSetting("DR TTS开关")]
    public bool isDRTTS { get; set; } = false;


    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
        LightningStormTTSCount = 0;
        RawElectropeCount = 0;
        TelltaleTearsTTSCount = 0;
        DarkTwoCount = 0;
        JaggedEdgeCount = 0;
        LeapingEarthResult = 0;
    }

    #region 小怪
    [ScriptMethod(name: "Sweeping Gouge", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40668"])]
    public void SweepingGouge(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Sweeping Gouge";
        //dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(9);
        dp.Radian = float.Pi / 180 * 90;
        dp.DestoryAt = 3700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "Thunderball", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40666"])]
    public void Thunderball(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Sweeping Gouge";
        //dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(8);
        dp.DestoryAt = 3700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Catapult", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40672"])]
    public void Catapult(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Catapult";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(6);
        dp.DestoryAt = 3750;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Glass Punch", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40671"])]
    public void GlassPunch(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Glass Punch";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(7);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 3750;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "Landslip", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41118"])]
    public void Landslip(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Landslip";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(12);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 3700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "Plummet", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40676"])]
    public void Plummet(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Plummet";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(10);
        dp.DestoryAt = 3700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Wild Horn", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40675"])]
    public void WildHorn(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Wild Horn";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Scale = new Vector2(15);
        dp.Radian = float.Pi / 180 * 120;
        dp.DestoryAt = 3700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }


    #endregion


    #region Boss1
    [ScriptMethod(name: "Electrical Overload", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40635"])]
    public void ElectricalOverload(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("AOE");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts AOE");
    }

    [ScriptMethod(name: "CellShock", eventType: EventTypeEnum.EnvControl, eventCondition: ["State:regex:^(00020001|00200010)$"])]
    public void CellShock(Event @event, ScriptAccessory accessory)
    {
        var EffectPositions = new Dictionary<byte, Vector3>
        {
            { 0x0D, new Vector3(81.132f, -0.75f, 268.868f) },
            { 0x0E, new Vector3(81.132f, -0.75f, 285.132f) },
            { 0x0F, new Vector3(64.868f, -0.75f, 268.868f) },
            { 0x10, new Vector3(64.868f, -0.75f, 285.132f) }
        };
        var EffectPositionsPairs = new Dictionary<byte, byte>
        {
            { 0x0D, 0x10 },
            { 0x0E, 0x0F },
            { 0x0F, 0x0E },
            { 0x10, 0x0D }
        };

        var index = @event.Index();
        var state = @event.State();

        if (state == 0x00200010 && EffectPositionsPairs.TryGetValue(index, out var remappedIndex))
            index = remappedIndex;

        if (EffectPositions.TryGetValue(index, out var position))
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "CellShock";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Position = position;
            dp.ScaleMode = ScaleMode.ByTime;
            dp.Scale = new Vector2(26);
            dp.DestoryAt = 8000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(name: "Lightning Storm", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40637"])]
    public void LightningStorm(Event @event, ScriptAccessory accessory)
    {
        if (LightningStormTTSCount == 0 || LightningStormTTSCount == 4 || LightningStormTTSCount == 7 || LightningStormTTSCount == 12)
        {
            if (isText) accessory.Method.TextInfo("全体分散", duration: 4700, true);
            if (isTTS) accessory.Method.TTS("全体分散");
            if (isDRTTS) accessory.Method.SendChat("/pdr tts 全体分散");
        }
        LightningStormTTSCount++;
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Lightning Storm";
        dp.Color = new Vector4(138 / 255.0f, 43 / 255.0f, 251 / 226.0f, 1.0f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(5);
        dp.DestoryAt = 4800;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Raw Electrope", eventType: EventTypeEnum.Targetable, eventCondition: ["SourceId:40002641"])]
    public void RawElectrope(Event @event, ScriptAccessory accessory)
    {
        if (RawElectropeCount == 0)
        {
            if (isText) accessory.Method.TextInfo("优先攻击小怪", duration: 4700, true);
            if (isTTS) accessory.Method.TTS("优先攻击小怪");
            if (isDRTTS) accessory.Method.SendChat("/pdr tts 优先攻击小怪");
            RawElectropeCount++;
        }
    }


    [ScriptMethod(name: "Sparking Fissure", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41258"])]
    public async void SparkingFissure(Event @event, ScriptAccessory accessory)
    {
        await Task.Delay(8700);
        if (isText) accessory.Method.TextInfo("AOE", duration: 3700, true);
        if (isTTS) accessory.Method.TTS("AOE");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts AOE");
    }


    [ScriptMethod(name: "Clear", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:17998"])]
    public void Boss1Clear(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }
    #endregion


    #region Boss2
    [ScriptMethod(name: "Boss2死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40658"])]
    public void Boss2Tankbuster(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("死刑准备", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("死刑准备");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 死刑准备");
    }

    [ScriptMethod(name: "Phantom Flood", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40644"])]
    public void PhantomFlood(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("去Boss脚下", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("去Boss脚下");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 去Boss脚下");

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Phantom Flood";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(5);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Dark II", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4065[67])$"])]
    public void DarkTwo(Event @event, ScriptAccessory accessory)
    {
        var isFirstAction = @event.ActionId() == 40656;
        var duration = @event.DurationMilliseconds();
        var delay = isFirstAction ? 0 : 4700;
        var destroyAt = isFirstAction ? 4700 : 3800;

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Phantom Flood";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = @event.TargetId();
        dp.Delay = delay;
        dp.Scale = DarkTwoCount < 12 ? new Vector2(5) : new Vector2(35);
        dp.Radian = float.Pi / 180 * 30;
        dp.Rotation = float.Pi / 180 * @event.SourceRotation() + float.Pi / 180 * 30;
        dp.DestoryAt = destroyAt;

        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

        DarkTwoCount++;
    }


    [ScriptMethod(name: "Telltale Tears", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40649"])]
    public void TelltaleTears(Event @event, ScriptAccessory accessory)
    {
        if (TelltaleTearsTTSCount == 0)
        {
            if (isText) accessory.Method.TextInfo("分散, 不要重叠", duration: 4700, true);
            if (isTTS) accessory.Method.TTS("分散, 不要重叠");
            if (isDRTTS) accessory.Method.SendChat("/pdr tts 分散, 不要重叠");
            TelltaleTearsTTSCount++;
        }
        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Telltale Tears";
        dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 251 / 255.0f, 1.0f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(5);
        dp.DestoryAt = 4800;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Necrohazard", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40646"])]
    public void Necrohazard(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("远离场中, 注意目压", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("远离场中, 注意目压");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 远离场中, 注意目压");

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Telltale Tears";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = new Vector3(115.94f, 12.50f, -66.04f);
        dp.Scale = new Vector2(18);
        dp.DestoryAt = 14700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Bloodburst", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40647"])]
    public async void Bloodburst(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("AOE");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts AOE");
    }

    [ScriptMethod(name: "Soul Douse", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40651"])]
    public void SoulDouse(Event @event, ScriptAccessory accessory)
    {
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";
        if (isText) accessory.Method.TextInfo($"与{tname}分摊", duration: 4700, true);
        if (isTTS) accessory.Method.TTS($"与{tname}分摊");
        if (isDRTTS) accessory.Method.SendChat($"/pdr tts 与{tname}分摊");

        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Telltale Tears";
        dp.Color = new Vector4(0 / 255.0f, 255 / 255.0f, 255 / 255.0f, 1.0f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        dp.DestoryAt = 4700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }



    #endregion


    #region Boss3
    [ScriptMethod(name: "Raging Claw", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40613"])]
    public void RagingClaw(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("去Boss身后", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("去Boss身后");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 去Boss身后");

        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Raging Claw";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = @event.SourceId();
        dp.Scale = new Vector2(45);
        dp.Radian = float.Pi / 180 * 180;
        dp.DestoryAt = 5400;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }

    [ScriptMethod(name: "Boulder Dance", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4060[78])$"])]
    public void BoulderDance(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Boulder Dance";
        dp.Color = accessory.Data.DefaultDangerColor;
        //dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(7);
        dp.DestoryAt = 7300;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }


    //[ScriptMethod(name: "LeapingEarth", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40662"])]
    //public void LeapingEarth(Event @event, ScriptAccessory accessory)
    //{
    //    List<uint> LeapingEarthCheck0 = new List<uint> { 0x400029F4, 0x400029F5, 0x400029F6, 0x400029F7 };
    //    List<uint> LeapingEarthCheck1 = new List<uint> { 0x400029F0, 0x400029F1, 0x400029F2, 0x400029F3 };
    //    lock (LeapingEarthLock)
    //    {
    //        if (LeapingEarthTargetList.Count == 4)
    //        {
    //            if (LeapingEarthTargetList.All(value => LeapingEarthCheck0.Contains(value)))
    //            {
    //                LeapingEarthResult = 0;
    //            }
    //            if (LeapingEarthTargetList.All(value => LeapingEarthCheck1.Contains(value)))
    //            {
    //                LeapingEarthResult = 1;
    //            }

    //            if (LeapingEarthResult == 0)
    //            {
    //                var dp = accessory.Data.GetDefaultDrawProperties();
    //                dp.Name = $"{LeapingEarthResult}LeapingEarth";
    //                dp.Owner = accessory.Data.Me;
    //                dp.Color = accessory.Data.DefaultSafeColor;
    //                dp.ScaleMode |= ScaleMode.YByDistance;
    //                dp.TargetPosition = new Vector3(41.77f, -87.90f, -707.98f);
    //                dp.Scale = new(2);
    //                dp.DestoryAt = 4000;
    //                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    //            }
    //            if (LeapingEarthResult == 1)
    //            {
    //                var dp = accessory.Data.GetDefaultDrawProperties();
    //                dp.Name = $"{LeapingEarthResult}LeapingEarth";
    //                dp.Owner = accessory.Data.Me;
    //                dp.Color = accessory.Data.DefaultSafeColor;
    //                dp.ScaleMode |= ScaleMode.YByDistance;
    //                dp.TargetPosition = new Vector3(34.28f, -87.90f, -716.22f);
    //                dp.Scale = new(2);
    //                dp.DestoryAt = 4000;
    //                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    //            }
    //            LeapingEarthTargetList.Clear();
    //        }

    //        LeapingEarthTargetList.Add(@event.TargetId());
    //    }
    //}
    //[ScriptMethod(name: "LeapingEarth", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40662"])]
    //public void LeapingEarth(Event @event, ScriptAccessory accessory)
    //{
    //    lock (LeapingEarthLock)
    //    {

    //        if (LeapingEarthResult == 0)
    //        {
    //            var dp = accessory.Data.GetDefaultDrawProperties();
    //            dp.Name = $"{LeapingEarthResult}LeapingEarth";
    //            dp.Owner = accessory.Data.Me;
    //            dp.Color = accessory.Data.DefaultSafeColor;
    //            dp.ScaleMode |= ScaleMode.YByDistance;
    //            dp.TargetPosition = new Vector3(41.77f, -87.90f, -707.98f);
    //            dp.Scale = new(2);
    //            dp.DestoryAt = 4000;
    //            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    //        }
    //        if (LeapingEarthResult == 4)
    //        {
    //            var dp = accessory.Data.GetDefaultDrawProperties();
    //            dp.Name = $"{LeapingEarthResult}LeapingEarth";
    //            dp.Owner = accessory.Data.Me;
    //            dp.Color = accessory.Data.DefaultSafeColor;
    //            dp.ScaleMode |= ScaleMode.YByDistance;
    //            dp.TargetPosition = new Vector3(34.28f, -87.90f, -716.22f);
    //            dp.Scale = new(2);
    //            dp.DestoryAt = 4000;
    //            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
    //        }
    //        LeapingEarthResult++;
    //    }
    //}

    [ScriptMethod(name: "Jagged Edge", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40615"])]
    public void JaggedEdge(Event @event, ScriptAccessory accessory)
    {
        lock (JaggedEdgeLock)
        {
            Task.Delay(50).ContinueWith(t =>
            {
                if (JaggedEdgeCount == 0 || JaggedEdgeCount == 4 || JaggedEdgeCount == 7)
                {
                    if (isText) accessory.Method.TextInfo("分散, 不要重叠", duration: 4700, true);
                    if (isTTS) accessory.Method.TTS("分散, 不要重叠");
                    if (isDRTTS) accessory.Method.SendChat("/pdr tts 分散, 不要重叠");
                }
                JaggedEdgeCount++;
            });
            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = "Jagged Edge";
            dp.Color = new Vector4(255 / 255.0f, 0 / 255.0f, 251 / 255.0f, 1.0f);
            dp.ScaleMode = ScaleMode.ByTime;
            dp.Owner = @event.TargetId();
            dp.Scale = new Vector2(6);
            dp.DestoryAt = 4700;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
    }

    [ScriptMethod(name: "Crater Carve", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40605"])]
    public void CraterCarve(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Crater Carve";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(11);
        dp.DestoryAt = 9200;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Beastly Roar", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40610"])]
    public void BeastlyRoar(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("远离Boss", duration: 7000, true);
        if (isTTS) accessory.Method.TTS("远离Boss");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 远离Boss");

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "Beastly Roar";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Position = @event.EffectPosition();
        dp.Scale = new Vector2(25);
        dp.DestoryAt = 8000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Turali Stone IV", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40616"])]
    public void TuraliStoneIV(Event @event, ScriptAccessory accessory)
    {
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";
        if (isText) accessory.Method.TextInfo($"与{tname}分摊", duration: 4700, true);
        if (isTTS) accessory.Method.TTS($"与{tname}分摊");
        if (isDRTTS) accessory.Method.SendChat($"/pdr tts 与{tname}分摊");

        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "Turali Stone IV";
        dp.Color = new Vector4(0 / 255.0f, 255 / 255.0f, 255 / 255.0f, 1.0f);
        dp.ScaleMode = ScaleMode.ByTime;
        dp.Owner = @event.TargetId();
        dp.Scale = new Vector2(6);
        dp.DestoryAt = 4700;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Sonic Howl", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40618"])]
    public async void SonicHowl(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("AOE", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("AOE");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts AOE");
    }

    [ScriptMethod(name: "Boss3死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40619"])]
    public void Boss3Tankbuster(Event @event, ScriptAccessory accessory)
    {
        if (isText) accessory.Method.TextInfo("死刑准备", duration: 4700, true);
        if (isTTS) accessory.Method.TTS("死刑准备");
        if (isDRTTS) accessory.Method.SendChat("/pdr tts 死刑准备");
    }

    #endregion
    //[ScriptMethod(name: "Rock Blast", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40611"])]
    //public void RockBlast(Event @event, ScriptAccessory accessory)
    //{
    //    var clockwise = new Vector3(33.98f, -87.91f, -723.51f);
    //    //var antiClockwise = new Vector3(33.98f, -87.91f, -723.51f);

    //    Task.Delay(50).ContinueWith(t =>
    //    {
    //        if (@event.EffectPosition() == clockwise)
    //        {
    //            accessory.Method.TextInfo("顺时针地火", duration: 4700, true);
    //            if (isTTS) accessory.Method.TTS("顺时针弟火");
    //            var pos = @event.EffectPosition();
    //            pos.X -= 1.5f;
    //            var dp = accessory.Data.GetDefaultDrawProperties();
    //            dp.Name = "Clockwise Safe Area";
    //            dp.Color = accessory.Data.DefaultSafeColor;
    //            dp.Position = pos;
    //            dp.Scale = new Vector2(3);
    //            dp.DestoryAt = 14000;
    //            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    //        }
           
    //    });
    //}

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

