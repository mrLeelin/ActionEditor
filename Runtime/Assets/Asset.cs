using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    public abstract class Asset : IDirector
    {
        [HideInInspector] public List<Group> groups = new();
        
      
        
        [SerializeField] private float length;
        /*
        [SerializeField] private float viewTimeMin;
        [SerializeField] private float viewTimeMax;
        */

        [SerializeField] private float rangeMin;
        [SerializeField] private float rangeMax ;
        

        private float _previewLength = 5f;
        private float _previewTimeMin;
        private float _previewTimeMax = 5f;
        
        private float _previewRangeMin;
        private float _previewRangeMax = 5f;
        
        public Asset()
        {
            Init();
        }


        [fsIgnore] public List<IDirectable> directables { get; private set; }

        public float Length
        {
            get => _previewLength;
            set => _previewLength = Mathf.Max(value, 0.1f);
        }

        public float ViewTimeMin
        {
            get => _previewTimeMin;
            set
            {
                if (ViewTimeMax > 0) _previewTimeMin = Mathf.Min(value, ViewTimeMax - 0.25f);
                _previewTimeMin = Math.Max(0, value);
            }
        }

        public float ViewTimeMax
        {
            get => _previewTimeMax;
            set => _previewTimeMax = Mathf.Max(value, ViewTimeMin + 0.25f, 0);
        }


        public float ViewTime => ViewTimeMax - ViewTimeMin;

        public float RangeMin
        {
            get => _previewRangeMin;
            set
            {
                _previewRangeMin = value;
                if (_previewRangeMin < 0) _previewRangeMin = 0;
            }
        }

        public float RangeMax
        {
            get => _previewRangeMax;
            set
            {
                _previewRangeMax = value;
                if (_previewRangeMax < _previewLength) _previewRangeMax = _previewLength;
            }
        }


        public void UpdateMaxTime()
        {
            var t = 0f;
            foreach (var group in groups)
            {
                if (!group.IsActive) continue;
                foreach (var track in group.Tracks)
                {
                    if (!track.IsActive) continue;
                    foreach (var clip in track.Clips)
                        if (clip.EndTime > t)
                            t = clip.EndTime;
                }
            }

            Length = t;
        }

        public void DeleteGroup(Group group)
        {
            groups.Remove(group);
            Validate();
        }

        public void Validate()
        {
            directables = new List<IDirectable>();
            foreach (IDirectable group in groups.AsEnumerable().Reverse())
            {
                directables.Add(group);
                try
                {
                    group.Validate(this, null);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                foreach (var track in group.Children.Reverse())
                {
                    directables.Add(track);
                    try
                    {
                        track.Validate(this, group);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    foreach (var clip in track.Children)
                    {
                        directables.Add(clip);
                        try
                        {
                            clip.Validate(this, track);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }

            if (directables != null)
                foreach (var d in directables)
                    d.OnAfterDeserialize();

            UpdateMaxTime();
        }

        public Group AddGroup(Type type)
        {
            if (!typeof(Group).IsAssignableFrom(type)) return null;
            var newGroup = Activator.CreateInstance(type) as Group;
            if (newGroup != null)
            {
                newGroup.Name = "New Group";
                groups.Add(newGroup);
                Validate();
            }

            return newGroup;
        }

        public T AddGroup<T>(string name = "") where T : Group, new()
        {
            var newGroup = new T();
            if (string.IsNullOrEmpty(name))
            {
                name = newGroup.GetType().Name;
            }

            newGroup.Name = name;
            groups.Add(newGroup);
            Validate();
            return newGroup;
        }


        public void Init()
        {
            Validate();
        }

        public void OnBeforeSerialize()
        {
            if (directables != null)
                foreach (var d in directables)
                    d.OnBeforeSerialize();

            if (Prefs.timeStepMode == Prefs.TimeStepMode.Seconds)
            {
                length = Length;
                /*
                viewTimeMin = ViewTimeMin;
                viewTimeMax = ViewTimeMax;
                */
                rangeMin = RangeMin;
                rangeMax = RangeMax;
            }
            else
            {
                length = Mathf.FloorToInt(Length * Prefs.FrameRate);
                /*
                viewTimeMin = Mathf.FloorToInt(ViewTimeMin * Prefs.FrameRate);
                viewTimeMax = Mathf.FloorToInt(ViewTimeMax * Prefs.FrameRate);
                */
                rangeMin = Mathf.FloorToInt(RangeMin * Prefs.FrameRate);
                rangeMax = Mathf.FloorToInt(RangeMax * Prefs.FrameRate);
            }
            // groupStr = FullSerializerExtensions.Serialize(typeof(List<Group>), groups);
        }

        public void OnAfterDeserialize()
        {
            // if (!string.IsNullOrEmpty(groupStr))
            // {
            //     var obj = FullSerializerExtensions.Deserialize(typeof(List<Group>), groupStr);
            //     if (obj is List<Group> list)
            //     {
            //         groups = list;
            //     }
            // }
        }
    }
}