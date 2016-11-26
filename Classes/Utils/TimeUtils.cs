using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// 时间
/// @author hannibal
/// @time 2014-11-14
/// </summary>
public class TimeUtils
{
	static public UInt32 TimeSince1970
	{
		get 
		{
			System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.ToLocalTime();
			System.TimeSpan duration = System.DateTime.Now - baseDate;
			UInt32 currentTime = (UInt32)(duration.Seconds+duration.Minutes*60 + duration.Hours* 3600+duration.Days*86400);//duration.TotalSeconds
			return currentTime;
		}
	}
	static public UInt64 TimeSince2009
	{
		get 
		{
			System.DateTime baseDate = new System.DateTime(2009, 1, 1);
			System.TimeSpan duration = System.DateTime.Now - baseDate;
			UInt64 currentTime = (UInt64)(duration.Seconds+duration.Minutes*60 + duration.Hours* 3600+duration.Days*86400);//duration.TotalSeconds
			return currentTime;
		}
	}
	
	/**获取当前时间  “年” “月” “日” “时” “分”*/
	static public string GetNowTime(out UInt32 year, 
	                                out UInt32 month,
	                                out UInt32 day,
	                                out UInt32 hour,
	                                out UInt32 min)
	{
		System.DateTime baseDate = new System.DateTime(2009, 1, 1);
		System.TimeSpan duration = System.DateTime.Now - baseDate;
		return GetTimeSince2009((UInt32)duration.TotalSeconds,
		                        out year, 
		                        out month,
		                        out day,
		                        out hour,
		                        out min);
	}
	
	static public string GetNowTime()
	{
		System.DateTime baseDate = new System.DateTime(2009, 1, 1);
		System.TimeSpan duration = System.DateTime.Now - baseDate;
		return GetTimeSince2009((UInt32)duration.TotalSeconds);
	}
	
	static public string GetTimeSince2009(UInt32 second, 
	                                      out UInt32 year, 
	                                      out UInt32 month,
	                                      out UInt32 day,
	                                      out UInt32 hour,
	                                      out UInt32 min)
	{
		System.DateTime baseDate = new System.DateTime(2009, 1, 1);
		baseDate = baseDate.AddSeconds(second);
		year =(UInt32) baseDate.Year;
		month =(UInt32) baseDate.Month;
		day =(UInt32) baseDate.Day;
		hour =(UInt32) baseDate.Hour;
		min =(UInt32) baseDate.Minute;
		return baseDate.ToString("yyyy-MM-dd-hh:mm");
	}
	
	static public string GetTimeSince2009(UInt32 second)
	{
		System.DateTime baseDate = new System.DateTime(2009, 1, 1);
		baseDate = baseDate.AddSeconds(second);
        // 调整为当前系统时区
        baseDate = baseDate.ToLocalTime();
		return baseDate.ToString("yyyy-MM-dd-hh:mm");
	}

    static public string GetTimeSince1970(UInt32 second,
                                          out UInt32 year,
                                          out UInt32 month,
                                          out UInt32 day,
                                          out UInt32 hour,
                                          out UInt32 min)
    {
        System.DateTime baseDate = new System.DateTime(1970, 1, 1);
        baseDate = baseDate.AddSeconds(second);
        // 调整为当前系统时区
        baseDate = baseDate.ToLocalTime();
        year = (UInt32)baseDate.Year;
        month = (UInt32)baseDate.Month;
        day = (UInt32)baseDate.Day;
        hour = (UInt32)baseDate.Hour;
        min = (UInt32)baseDate.Minute;
        return baseDate.ToString("yyyy-MM-dd-hh:mm");
    }


    static public string GetTimeSince1970(UInt32 second)
    {
        System.DateTime baseDate = new System.DateTime(1970, 1, 1);
        baseDate = baseDate.AddSeconds(second);
        // 调整为当前系统时区
        baseDate = baseDate.ToLocalTime();
        return baseDate.ToString("yyyy-MM-dd-hh:mm");
    }

    static public DateTime GetTimeDateSince1970(UInt32 second)
    {
        System.DateTime baseDate = new System.DateTime(1970, 1, 1);
        baseDate = baseDate.AddSeconds(second);
        baseDate = baseDate.ToLocalTime();
        return baseDate;
    }
}
