using UnityEngine;

/// <summary>
/// Various utility functions related to ROS or ROS-Unity type conversions.
/// </summary>
public static class ROSUtils
{
    /// <summary>
    /// Convert Unity time to a ROS time message format 
    /// </summary>
    public static RosMessageTypes.BuiltinInterfaces.TimeMsg UnityTimeToROSTime(float time)
    {
        int secs = (int)Mathf.FloorToInt(time);
        uint nanoSecs = (uint)Mathf.FloorToInt((time - (float)secs) * 1e9f);

        return new RosMessageTypes.BuiltinInterfaces.TimeMsg(secs, nanoSecs);
    }

    /// <summary>
    /// Build a time message with the current Unity time.
    /// </summary>
    public static RosMessageTypes.BuiltinInterfaces.TimeMsg ROSTimeNow()
    {
        return UnityTimeToROSTime(Time.time);
    }
}
