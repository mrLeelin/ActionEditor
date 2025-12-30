using UnityEngine;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 时间转换工具类，统一处理帧/秒之间的转换
    /// </summary>
    /// <remarks>
    /// 将分散在各处的时间转换逻辑统一收口，提高代码可维护性
    /// </remarks>
    public static class TimeConverter
    {
#if UNITY_EDITOR
        /// <summary>
        /// 当前是否为帧模式
        /// </summary>
        public static bool IsFrameMode => Prefs.timeStepMode == Prefs.TimeStepMode.Frames;

        /// <summary>
        /// 当前帧率
        /// </summary>
        public static int FrameRate => Prefs.FrameRate;

        /// <summary>
        /// 一帧的时长（秒）
        /// </summary>
        public static float FrameDuration => 1f / Prefs.FrameRate;

        /// <summary>
        /// 根据当前模式获取最小时间增量
        /// </summary>
        /// <remarks>
        /// 帧模式下返回一帧时长，秒模式下返回 0.1 秒
        /// </remarks>
        public static float MinTimeDelta => IsFrameMode ? FrameDuration : 0.1f;

        /// <summary>
        /// 秒转帧（向下取整）
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>帧数（整数）</returns>
        public static int SecondsToFrames(float seconds)
        {
            return Mathf.FloorToInt(seconds * Prefs.FrameRate);
        }

        /// <summary>
        /// 秒转帧（保留浮点精度）
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>帧数（浮点）</returns>
        public static float SecondsToFramesFloat(float seconds)
        {
            return seconds * Prefs.FrameRate;
        }

        /// <summary>
        /// 帧转秒
        /// </summary>
        /// <param name="frames">帧数（整数）</param>
        /// <returns>秒数</returns>
        public static float FramesToSeconds(int frames)
        {
            return frames / (float)Prefs.FrameRate;
        }

        /// <summary>
        /// 帧转秒
        /// </summary>
        /// <param name="frames">帧数（浮点）</param>
        /// <returns>秒数</returns>
        public static float FramesToSeconds(float frames)
        {
            return frames / Prefs.FrameRate;
        }

        /// <summary>
        /// 将秒数转换为当前模式的显示值
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>帧模式返回帧数，秒模式返回秒数</returns>
        public static float ToDisplayValue(float seconds)
        {
            return IsFrameMode ? SecondsToFrames(seconds) : seconds;
        }

        /// <summary>
        /// 将显示值转换为秒数
        /// </summary>
        /// <param name="displayValue">显示值（帧模式为帧数，秒模式为秒数）</param>
        /// <returns>秒数</returns>
        public static float FromDisplayValue(float displayValue)
        {
            return IsFrameMode ? FramesToSeconds(displayValue) : displayValue;
        }

        /// <summary>
        /// 向上取整的秒转帧
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>帧数（整数）</returns>
        public static int SecondsToFramesCeil(float seconds)
        {
            return Mathf.CeilToInt(seconds * Prefs.FrameRate);
        }

        /// <summary>
        /// 四舍五入的秒转帧
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <returns>帧数（整数）</returns>
        public static int SecondsToFramesRound(float seconds)
        {
            return Mathf.RoundToInt(seconds * Prefs.FrameRate);
        }
#endif
    }
}
