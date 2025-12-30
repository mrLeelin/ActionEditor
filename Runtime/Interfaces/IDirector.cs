using System.Collections.Generic;
using UnityEngine;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 时间轴导演接口，管理整个时间轴资产
    /// </summary>
    public interface IDirector : IData
    {
        float Length { get; }

        public float ViewTimeMin { get; set; }
        public float ViewTimeMax { get; set; }

        public float ViewTime { get; }

        public float RangeMin { get; set; }
        public float RangeMax { get; set; }

        void DeleteGroup(Group group);

        /// <summary>
        /// 强制执行完整验证，重建树结构
        /// </summary>
        void Validate();

        /// <summary>
        /// 标记数据已修改，需要在下次 ValidateIfNeeded 时重新验证
        /// </summary>
        void MarkDirty();

        /// <summary>
        /// 条件验证，仅在 IsDirty 为 true 时执行验证
        /// </summary>
        void ValidateIfNeeded();

        /// <summary>
        /// 是否有待处理的验证
        /// </summary>
        bool IsDirty { get; }
    }
}