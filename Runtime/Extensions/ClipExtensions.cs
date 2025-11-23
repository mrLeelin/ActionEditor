namespace NBC.ActionEditor
{
    public static class ClipExtensions
    {
        /// <summary>
        /// 计算循环动画采样时间
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public static float CalculateAnimationSampleTime(this Clip clip, float currentTime)
        {
            // 将当前时间转换为剪辑内的本地时间（0到Length）
            var localTime = clip.ToPercentage(currentTime) * clip.Length;

            // 获取 ISubClipContainable 接口
            // 如果不是 ISubClipContainable，使用默认计算
            if (clip is not ISubClipContainable subClip) return localTime;
            var clipOffset = subClip.SubClipOffset;
            var clipSpeed = subClip.SubClipSpeed;
            var clipLength = subClip.SubClipLength;

            // 如果动画资源为空或长度为0，返回0
            if (clipLength <= 0)
                return 0;

            // 应用速度：localTime * speed
            var speedAdjustedTime = localTime * clipSpeed;

            // 应用偏移量并处理循环
            var animationTime = (clipOffset + speedAdjustedTime) % clipLength;

            return animationTime;

         
        }
    }
}