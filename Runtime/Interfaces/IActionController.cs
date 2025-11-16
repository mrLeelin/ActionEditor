using UnityEngine;

namespace NBC.ActionEditor
{
    public interface IActionController
    {
        /// <summary>
        /// 动作Animator
        /// </summary>
        Animator Animator { get; }
    }
}