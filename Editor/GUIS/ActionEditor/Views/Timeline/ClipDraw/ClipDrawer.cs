using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class ClipDrawer
    {
        // private static Dictionary<Type, ClipDrawBase> _clipDrawDictionary = new Dictionary<Type, ClipDrawBase>();

        private static Dictionary<Clip, ClipDrawBase> _clipDraws = new Dictionary<Clip, ClipDrawBase>();

        static ClipDrawer()
        {
            // _clipDrawDictionary.Add(typeof(SimpleClipDraw), new SimpleClipDraw());
            // _clipDrawDictionary.Add(typeof(BasicClipDraw), new BasicClipDraw());
            // _clipDrawDictionary.Add(typeof(SignalClipDraw), new SignalClipDraw());
        }

        #region Get

        public static List<IDirectable> GetClips(Rect rect)
        {
            List<IDirectable> list = new List<IDirectable>();
            foreach (var clip in _clipDraws.Keys)
            {
                var draw = _clipDraws[clip];
                if (rect.Overlaps(draw.ClipRealRect))
                {
                    list.Add(clip);
                }
            }

            return list;
        }


        public static bool ClipContainsByMergeRect(IDirectable[] clips, Vector2 pos)
        {
            var combineRect = CombineRects(clips);
            return combineRect.Contains(pos);
        }

        public static Rect CombineRects(IDirectable[] clips)
        {
            bool hasRect = false;
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var directable in clips)
            {
                if (directable is Clip clip)
                {
                    var rect = _clipDraws[clip].ClipRealRect;

                    if (!hasRect)
                        hasRect = true;

                    minX = Mathf.Min(minX, rect.xMin);
                    minY = Mathf.Min(minY, rect.yMin);
                    maxX = Mathf.Max(maxX, rect.xMax);
                    maxY = Mathf.Max(maxY, rect.yMax);
                }
            }

            if (!hasRect)
                return Rect.zero;

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        public static bool ClipContainsByRealRect(Vector2 pos)
        {
            foreach (var clip in _clipDraws.Keys)
            {
                var draw = _clipDraws[clip];
                if (draw.ClipRealRect.Contains(pos))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ClipYContainsByRealRect(Vector2 pos)
        {
            foreach (var clip in _clipDraws.Keys)
            {
                var draw = _clipDraws[clip];
                var posY = pos.y;
                if (posY >= draw.ClipRealRect.y && posY <= draw.ClipRealRect.y + draw.ClipRealRect.height)
                {
                    return true;
                }
            }

            return false;
        }

        public static Clip GetClipByTrackPosition(IDirectable track, Vector2 mousePosition)
        {
            foreach (var clip in _clipDraws.Keys)
            {
                if (clip.Parent != track) continue;
                var draw = _clipDraws[clip];
                if (draw.ClipRect.Contains(mousePosition))
                {
                    return clip;
                }
            }

            return null;
        }

        #endregion

        #region Draw

        public static void Reset()
        {
            List<Clip> clips = new List<Clip>();
            var asset = App.AssetData;
            if (asset == null) return;
            foreach (var group in asset.groups)
            {
                foreach (var track in group.Tracks)
                {
                    clips.AddRange(track.Clips);
                }
            }

            List<Clip> invalidClips = _clipDraws.Keys.Where(clip => !clips.Contains(clip)).ToList();
            foreach (var clip in invalidClips)
            {
                _clipDraws.Remove(clip);
            }

            foreach (var clip in clips)
            {
                if (!_clipDraws.ContainsKey(clip))
                {
                    var type = typeof(BasicClipDraw);
                    if (clip is ClipSignal)
                    {
                        type = typeof(SignalClipDraw);
                    }

                    _clipDraws[clip] = Activator.CreateInstance(type) as BasicClipDraw;
                }
            }
        }

        public static ClipDrawBase GetDraw<T>(T clip) where T : Clip
        {
            return _clipDraws.GetValueOrDefault(clip);
        }

        // public static ClipDrawBase Draw<T>(EditorWindow window, Rect trackRect, Rect trackRightRect, T clip,
        //     bool select,
        //     Type drawType = null) where T : Clip
        // {
        //     if (_clipDraws.TryGetValue(clip, out var draw))
        //     {
        //         draw.Draw(window, trackRect, trackRightRect, clip, select);
        //         return draw;
        //     }
        //
        //     return default;
        // }

        #endregion
    }
}