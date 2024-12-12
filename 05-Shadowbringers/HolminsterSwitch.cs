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
namespace Veever.Shadowbringers.HolminsterSwitch;

[ScriptType(name: "遇袭集落水滩村", territorys: [837], guid: "a407d364-b2bd-4e12-9332-70ca3829ece7",
    version:"0.0.0.1", author: "Veever")]

public class HolminsterSwitch
{

    public void Init(ScriptAccessory accessory)
    {
        accessory.Method.RemoveDraw(".*");
    }

    private static bool ParseObjectId(string? idStr, out uint id)
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

    #region Boss1

    [ScriptMethod(name: "Boss1AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15813"])]
    public void Boss1AOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 4000, true);
        accessory.Method.TTS("AOE");
    }

    [ScriptMethod(name: "Boss1钢铁", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15816"])]
    public void Boss1GibbetCage(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("钢铁，远离Boss", duration: 3000, true);

        var dp = accessory.Data.GetDefaultDrawProperties();
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;

        dp.Name = "GibbetCage";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = tid;
        dp.Scale = new Vector2(8);
        dp.DestoryAt = 3000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        accessory.Method.TTS("钢铁，远离Boss");
    }

    [ScriptMethod(name: "Boss1螺旋突刺", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15814"])]
    public void Boss1Thumbscrew(Event @event, ScriptAccessory accessory)
    {
        var tarp = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosion"]);
        var dp = accessory.Data.GetDefaultDrawProperties();
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;

        dp.Name = "Thumbscrew";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Owner = tid;
        dp.TargetPosition = tarp;
        dp.Scale = new Vector2(10, 5);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
    }

    [ScriptMethod(name: "Boss1死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15812"])]
    public void Boss1Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑准备", duration: 4000, true);
        accessory.Method.TTS("死刑准备");
    }



    #endregion


    #region Boss2

    [ScriptMethod(name: "Boss2死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15823"])]
    public void Boss2Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑准备", duration: 4000, true);
        accessory.Method.TTS("死刑准备");
    }


    [ScriptMethod(name: "Boss2AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15824"])]
    public void Boss2AOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 4000, true);
        accessory.Method.TTS("AOE");
    }

    [ScriptMethod(name: "Boss2麻将点名", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15829"])]
    public void Boss2FeveredFlagellation(Event @event, ScriptAccessory accessory)
    {
        
        //var bossPosition = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosion"] );

        //if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        
        //for (int i = 0; i < 4; i++)
        //{
        //    var dp = accessory.Data.GetDefaultDrawProperties();
        //    dp.Name = "connect";
        //    dp.Scale = new Vector2(5, 5);
        //    dp.Color = accessory.Data.DefaultDangerColor;
        //    dp.Owner = accessory.Data.PartyList[i];
        //    dp.TargetPosition = bossPosition;
        //    dp.DestoryAt = 16000;
        //    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        //}

        accessory.Method.TextInfo("分散，不要重叠", duration: 8000, true);
        accessory.Method.TTS("分散，不要重叠");

    }

    [ScriptMethod(name: "Boss2分摊", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15826"])]
    public void Boss2Exorcise(Event @event, ScriptAccessory accessory)
    {
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";

        accessory.Method.TextInfo($"躲避圆圈并与 {tname} 分摊", duration: 4000, true);

        var dp = accessory.Data.GetDefaultDrawProperties();
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;

        dp.Name = "Exorcise";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = tid;
        dp.Scale = new Vector2(6);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);

        accessory.Method.TTS($"躲避圆圈并与 {tname} 分摊");
    }

    #endregion

    #region Boss3
    [ScriptMethod(name: "Boss3死刑", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15831"])]
    public void Boss3Tankbuster(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("死刑准备", duration: 4000, true);
        accessory.Method.TTS("死刑准备");
    }

    [ScriptMethod(name: "Boss3AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15832"])]
    public void Boss3AOE(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("AOE", duration: 4000, true);

        accessory.Method.TTS("AOE");
    }

    [ScriptMethod(name: "Boss3钟摆", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:16777"])]
    public void Boss3Pendulum(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("远离坦克和场中", duration: 4000, true);
        accessory.Method.TTS("远离坦克和场中");
        var midPoint = new Vector3(134, 23, -465);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "远离场中";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 8000;
        dp.Position = midPoint;
        dp.Scale = new Vector2(16);
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Boss3核爆", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0057"])]
    public void Boss3PendulumTank(Event @event, ScriptAccessory accessory)
    {
        var dp = accessory.Data.GetDefaultDrawProperties();
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;
        dp.Name = "Boss3核爆";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.Delay = 2000;
        dp.DestoryAt = 6000;
        dp.Owner = tid;
        dp.Scale = new Vector2(22);
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "Boss3束缚", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:1849"])]
    public void Boss3ChainDown(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;
        //accessory.Method.Mark(tid, KodakkuAssist.Module.GameOperate.MarkType.Attack1, true);

        accessory.Method.TextInfo("攻击锁链", duration: 4000, true);
        accessory.Method.TTS("攻击锁链");
    }

    [ScriptMethod(name: "Boss3左鞭打", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15847"])]
    public void Boss3LeftKnout(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("去Boss右面", duration: 4000, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        float.TryParse(@event["SourceRotation"], out var rotation);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "左鞭打";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5500;
        dp.Owner = sid;
        dp.Scale = new Vector2(22);
        dp.Radian = float.Pi / 180 * 210;
        float offsetAngle = 115 * (float)Math.PI / 180;
        dp.Rotation = rotation + offsetAngle;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }


    [ScriptMethod(name: "Boss3右鞭打", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15846"])]
    public void Boss3RightKnout(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("去Boss左面", duration: 4000, true);

        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        float.TryParse(@event["SourceRotation"], out var rotation);

        var dp = accessory.Data.GetDefaultDrawProperties();
        dp.Name = "右鞭打";
        dp.Color = accessory.Data.DefaultDangerColor;
        dp.DestoryAt = 5500;
        dp.Owner = sid;
        dp.Scale = new Vector2(22);
        dp.Radian = float.Pi / 180 * 210;
        float offsetAngle = -90 * (float)Math.PI / 180;
        dp.Rotation = rotation + offsetAngle;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
    }


    [ScriptMethod(name: "Boss3分摊", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:15844"])]
    public void Boss3IntoTheLight(Event @event, ScriptAccessory accessory)
    {
        if (!ParseObjectId(@event["TargetId"], out var tid)) return;
        if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        string tname = @event["TargetName"]?.ToString() ?? "未知目标";

        accessory.Method.TextInfo($"与 {tname} 分摊", duration: 4000, true);

        var dp = accessory.Data.GetDefaultDrawProperties();

        dp.Name = "IntoTheLight";
        dp.Color = accessory.Data.DefaultSafeColor;
        dp.Owner = sid;
        dp.TargetObject = tid;
        dp.Scale = new Vector2(4, 20);
        dp.DestoryAt = 5000;
        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);

        accessory.Method.TTS($"与 {tname} 分摊");
    }


    [ScriptMethod(name: "激烈捶打", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(1583(6|7|9))$"])]
    public void Boss3FierceBeating(Event @event, ScriptAccessory accessory)
    {
        accessory.Method.TextInfo("跟随身后扇形躲避", duration: 4000, true);
        accessory.Method.TTS("跟随身后扇形躲避");
        //var dp = accessory.Data.GetDefaultDrawProperties();
        //if (!ParseObjectId(@event["SourceId"], out var sid)) return;
        //var efp = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);

        //var aid = JsonConvert.DeserializeObject<uint>(@event["ActionId"]);

        //dp.Name = "激烈捶打";
        //dp.Color = accessory.Data.DefaultDangerColor;
        //dp.DestoryAt = aid == 15836 ? 3000 : 7000;
        //dp.Position = efp;
        //dp.Scale = new Vector2(6);
        //dp.DestoryAt = 5000;
        //accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
    }

    [ScriptMethod(name: "九尾猫", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:15840"])]
    public void Boss3NineTails(Event @event, ScriptAccessory accessory)
    {
        //var dp = accessory.Data.GetDefaultDrawProperties();
        //if (!ParseObjectId(@event["SourceId"], out var sid)) return;

        //dp.Name = "九尾猫";
        //dp.Color = accessory.Data.DefaultDangerColor;
        //dp.DestoryAt = 6000;
        //dp.Owner = sid;
        //dp.Scale = new Vector2(20);
        //dp.Radian = float.Pi / 180 * 120;
        //dp.Rotation = float.Pi;
        //accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);

    }
    #endregion
}