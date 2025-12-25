using System;
using UnityEngine;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 预定义颜色枚举 - 包含所有常用颜色
    /// </summary>
    public enum PredefinedColor
    {
        // ========== 基础颜色 ==========
        Red,                // 红色
        Green,              // 绿色
        Blue,               // 蓝色
        Yellow,             // 黄色
        Cyan,               // 青色
        Magenta,            // 洋红
        White,              // 白色
        Black,              // 黑色
        Gray,               // 灰色
        Clear,              // 透明

        // ========== 红色系 ==========
        Crimson,            // 深红色
        DarkRed,            // 暗红色
        FireBrick,          // 砖红色
        IndianRed,          // 印度红
        LightCoral,         // 浅珊瑚色
        Salmon,             // 鲑鱼色
        DarkSalmon,         // 暗鲑鱼色
        LightSalmon,        // 浅鲑鱼色
        Tomato,             // 番茄红
        OrangeRed,          // 橙红色

        // ========== 粉色系 ==========
        HotPink,            // 热粉色
        DeepPink,           // 深粉色
        Pink,               // 粉色
        LightPink,          // 浅粉色
        PaleVioletRed,      // 淡紫红色
        MediumVioletRed,    // 中紫红色

        // ========== 橙色系 ==========
        DarkOrange,         // 暗橙色
        Orange,             // 橙色
        Gold,               // 金色
        Coral,              // 珊瑚色
        SandyBrown,         // 沙褐色

        // ========== 黄色系 ==========
        LightYellow,        // 浅黄色
        LemonChiffon,       // 柠檬绸色
        PaleGoldenrod,      // 淡金菊色
        Khaki,              // 卡其色
        DarkKhaki,          // 暗卡其色

        // ========== 绿色系 ==========
        Lime,               // 酸橙色（亮绿）
        LimeGreen,          // 酸橙绿
        SpringGreen,        // 春绿色
        MediumSpringGreen,  // 中春绿色
        LightGreen,         // 浅绿色
        PaleGreen,          // 淡绿色
        LawnGreen,          // 草坪绿
        Chartreuse,         // 查特酒绿
        GreenYellow,        // 绿黄色
        DarkGreen,          // 暗绿色
        ForestGreen,        // 森林绿
        SeaGreen,           // 海绿色
        MediumSeaGreen,     // 中海绿色
        DarkSeaGreen,       // 暗海绿色
        LightSeaGreen,      // 浅海绿色
        MediumAquamarine,   // 中碧绿色
        Olive,              // 橄榄色
        DarkOliveGreen,     // 暗橄榄绿
        OliveDrab,          // 橄榄军服绿
        YellowGreen,        // 黄绿色

        // ========== 青色系 ==========
        Aqua,               // 水绿色
        DarkTurquoise,      // 暗绿松石色
        Turquoise,          // 绿松石色
        MediumTurquoise,    // 中绿松石色
        PaleTurquoise,      // 淡绿松石色
        Aquamarine,         // 碧绿色
        PowderBlue,         // 粉蓝色
        CadetBlue,          // 军蓝色
        LightCyan,          // 浅青色
        DarkCyan,           // 暗青色
        Teal,               // 蓝绿色

        // ========== 蓝色系 ==========
        MediumBlue,         // 中蓝色
        DarkBlue,           // 暗蓝色
        Navy,               // 海军蓝
        MidnightBlue,       // 午夜蓝
        RoyalBlue,          // 皇家蓝
        CornflowerBlue,     // 矢车菊蓝
        DodgerBlue,         // 道奇蓝
        DeepSkyBlue,        // 深天蓝
        SkyBlue,            // 天蓝色
        LightSkyBlue,       // 浅天蓝色
        SteelBlue,          // 钢蓝色
        LightSteelBlue,     // 浅钢蓝色
        LightBlue,          // 浅蓝色
        SlateGray,          // 石板灰
        LightSlateGray,     // 浅石板灰

        // ========== 紫色系 ==========
        Purple,             // 紫色
        DarkMagenta,        // 暗洋红
        MediumPurple,       // 中紫色
        DarkOrchid,         // 暗兰花紫
        MediumOrchid,       // 中兰花紫
        Orchid,             // 兰花紫
        Plum,               // 梅红色
        Violet,             // 紫罗兰色
        Fuchsia,            // 紫红
        BlueViolet,         // 蓝紫色
        DarkViolet,         // 暗紫色
        Amethyst,           // 紫水晶色
        RebeccaPurple,      // 丽贝卡紫
        Indigo,             // 靛青色
        SlateBlue,          // 石板蓝
        MediumSlateBlue,    // 中石板蓝
        Thistle,            // 蓟色
        Lavender,           // 薰衣草色

        // ========== 褐色系 ==========
        Brown,              // 褐色
        SaddleBrown,        // 马鞍褐色
        Chocolate,          // 巧克力色
        Peru,               // 秘鲁色
        BurlyWood,          // 实木色
        Tan,                // 棕褐色
        RosyBrown,          // 玫瑰褐色
        Bisque,             // 陶瓷色
        NavajoWhite,        // 纳瓦白
        PapayaWhip,         // 番木瓜色
        Moccasin,           // 鹿皮色
        Cornsilk,           // 玉米丝色

        // ========== 灰色系 ==========
        DarkSlateGray,      // 暗石板灰
        DimGray,            // 暗淡灰
        DarkGray,           // 暗灰色
        Silver,             // 银色
        LightGray,          // 浅灰色
        Gainsboro,          // 庚斯博罗灰
        WhiteSmoke,         // 烟白色

        // ========== 白色系/其他 ==========
        Snow,               // 雪白色
        Honeydew,           // 蜜瓜色
        MintCream,          // 薄荷奶油色
        Azure,              // 天蓝色
        AliceBlue,          // 爱丽丝蓝
        GhostWhite,         // 幽灵白
        FloralWhite,        // 花白色
        SeaShell,           // 海贝色
        LavenderBlush,      // 淡紫红
        MistyRose,          // 雾玫瑰色
        Ivory,              // 象牙色
    }

    /// <summary>
    /// 类排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderAttribute : Attribute
    {
        public int Order;

        public OrderAttribute(int order)
        {
            this.Order = order;
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ButtonAttribute : Attribute
    {
        public string ButtonName { get; private set; }
    
        public ButtonAttribute()
        {
            ButtonName = string.Empty;
        }
    
        public ButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
    /// <summary>
    /// 菜单自定义名称
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class MenuNameAttribute : Attribute
    {
        public MenuNameAttribute(string name)
        {
            showName = name;
        }

        public string showName;
    }

    /// <summary>
    /// 关联某个类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionParamAttribute : Attribute
    {
        public Type classType;

        public OptionParamAttribute(Type type)
        {
            classType = type;
        }
    }

    /// <summary>
    /// 选项排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionSortAttribute : Attribute
    {
        public int sort;

        public OptionSortAttribute(int sort)
        {
            this.sort = sort;
        }
    }

    /// <summary>
    /// 关联某个字段的某个值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionRelateParamAttribute : Attribute
    {
        public string argsName;
        public object[] argsValue;

        public OptionRelateParamAttribute(string name, params object[] values)
        {
            argsName = name;
            argsValue = values;
        }
    }

    /// <summary>
    /// 关联某个 bool 字段的值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionRelateBoolAttribute : Attribute
    {
        public string boolFieldName;
        public bool[] boolValues;

        public OptionRelateBoolAttribute(string fieldName, params bool[] values)
        {
            boolFieldName = fieldName;
            boolValues = values.Length > 0 ? values : new bool[] { true };
        }
    }

    /// <summary>
    /// 选择对象路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectObjectPathAttribute : Attribute
    {
        public Type type;

        public SelectObjectPathAttribute(Type type)
        {
            this.type = type;
        }
    }

    /// <summary>
    /// 自定义检视面板
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInspectors : Attribute
    {
        public Type InspectedType;
        public bool _editorForChildClasses;

        public CustomInspectors(Type inspectedType)
        {
            InspectedType = inspectedType;
        }

        public CustomInspectors(Type inspectedType, bool editorForChildClasses)
        {
            InspectedType = inspectedType;
            _editorForChildClasses = editorForChildClasses;
        }
    }

    /// <summary>
    /// 自定义检视面板
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomHeader : Attribute
    {
        public Type InspectedType;

        public CustomHeader(Type inspectedType)
        {
            InspectedType = inspectedType;
        }

        public CustomHeader(Type inspectedType, bool editorForChildClasses)
        {
            InspectedType = inspectedType;
        }
    }

    /// <summary>
    /// 自定义名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public readonly string name;

        public NameAttribute(string name)
        {
            this.name = name;
        }
    }


    /// <summary>
    /// 指定类别
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute : Attribute
    {
        public readonly string category;

        public CategoryAttribute(string category)
        {
            this.category = category;
        }
    }

    /// <summary>
    /// 指定描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

    /// <summary>
    /// 指定类型的图标
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ShowIconAttribute : Attribute
    {
        public readonly string iconPath;
        public readonly Type fromType;
        public readonly Texture2D texture;

        public ShowIconAttribute(Texture2D texture)
        {
            this.texture = texture;
        }

        public ShowIconAttribute(string iconPath)
        {
            this.iconPath = iconPath;
        }

        public ShowIconAttribute(Type fromType)
        {
            this.fromType = fromType;
        }
    }

    /// <summary>
    /// 指定显示的颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ColorAttribute : Attribute
    {
        public readonly Color Color;

        /// <summary>
        /// 使用预定义颜色枚举
        /// </summary>
        public ColorAttribute(PredefinedColor predefinedColor)
        {
            this.Color = GetColor(predefinedColor);
        }

        /// <summary>
        /// 使用自定义 RGB 值
        /// </summary>
        public ColorAttribute(float r, float g, float b, float a = 1f)
        {
            this.Color = new Color(r, g, b, a);
        }

        private static Color GetColor(PredefinedColor color)
        {
            switch (color)
            {
                // ========== 基础颜色 ==========
                case PredefinedColor.Red: return new Color(1f, 0f, 0f);
                case PredefinedColor.Green: return new Color(0f, 1f, 0f);
                case PredefinedColor.Blue: return new Color(0f, 0f, 1f);
                case PredefinedColor.Yellow: return new Color(1f, 0.92f, 0.016f);
                case PredefinedColor.Cyan: return new Color(0f, 1f, 1f);
                case PredefinedColor.Magenta: return new Color(1f, 0f, 1f);
                case PredefinedColor.White: return new Color(1f, 1f, 1f);
                case PredefinedColor.Black: return new Color(0f, 0f, 0f);
                case PredefinedColor.Gray: return new Color(0.5f, 0.5f, 0.5f);
                case PredefinedColor.Clear: return new Color(0f, 0f, 0f, 0f);

                // ========== 红色系 ==========
                case PredefinedColor.Crimson: return new Color(0.863f, 0.078f, 0.235f);
                case PredefinedColor.DarkRed: return new Color(0.545f, 0f, 0f);
                case PredefinedColor.FireBrick: return new Color(0.698f, 0.133f, 0.133f);
                case PredefinedColor.IndianRed: return new Color(0.804f, 0.361f, 0.361f);
                case PredefinedColor.LightCoral: return new Color(0.941f, 0.502f, 0.502f);
                case PredefinedColor.Salmon: return new Color(0.980f, 0.502f, 0.447f);
                case PredefinedColor.DarkSalmon: return new Color(0.914f, 0.588f, 0.478f);
                case PredefinedColor.LightSalmon: return new Color(1f, 0.627f, 0.478f);
                case PredefinedColor.Tomato: return new Color(1f, 0.388f, 0.278f);
                case PredefinedColor.OrangeRed: return new Color(1f, 0.271f, 0f);

                // ========== 粉色系 ==========
                case PredefinedColor.HotPink: return new Color(1f, 0.412f, 0.706f);
                case PredefinedColor.DeepPink: return new Color(1f, 0.078f, 0.576f);
                case PredefinedColor.Pink: return new Color(1f, 0.753f, 0.796f);
                case PredefinedColor.LightPink: return new Color(1f, 0.714f, 0.757f);
                case PredefinedColor.PaleVioletRed: return new Color(0.859f, 0.439f, 0.576f);
                case PredefinedColor.MediumVioletRed: return new Color(0.780f, 0.082f, 0.522f);

                // ========== 橙色系 ==========
                case PredefinedColor.DarkOrange: return new Color(1f, 0.549f, 0f);
                case PredefinedColor.Orange: return new Color(1f, 0.647f, 0f);
                case PredefinedColor.Gold: return new Color(1f, 0.843f, 0f);
                case PredefinedColor.Coral: return new Color(1f, 0.498f, 0.314f);
                case PredefinedColor.SandyBrown: return new Color(0.957f, 0.643f, 0.376f);

                // ========== 黄色系 ==========
                case PredefinedColor.LightYellow: return new Color(1f, 1f, 0.878f);
                case PredefinedColor.LemonChiffon: return new Color(1f, 0.980f, 0.804f);
                case PredefinedColor.PaleGoldenrod: return new Color(0.933f, 0.910f, 0.667f);
                case PredefinedColor.Khaki: return new Color(0.941f, 0.902f, 0.549f);
                case PredefinedColor.DarkKhaki: return new Color(0.741f, 0.718f, 0.420f);

                // ========== 绿色系 ==========
                case PredefinedColor.Lime: return new Color(0f, 1f, 0f);
                case PredefinedColor.LimeGreen: return new Color(0.196f, 0.804f, 0.196f);
                case PredefinedColor.SpringGreen: return new Color(0f, 1f, 0.498f);
                case PredefinedColor.MediumSpringGreen: return new Color(0f, 0.980f, 0.604f);
                case PredefinedColor.LightGreen: return new Color(0.565f, 0.933f, 0.565f);
                case PredefinedColor.PaleGreen: return new Color(0.596f, 0.984f, 0.596f);
                case PredefinedColor.LawnGreen: return new Color(0.486f, 0.988f, 0f);
                case PredefinedColor.Chartreuse: return new Color(0.498f, 1f, 0f);
                case PredefinedColor.GreenYellow: return new Color(0.678f, 1f, 0.184f);
                case PredefinedColor.DarkGreen: return new Color(0f, 0.392f, 0f);
                case PredefinedColor.ForestGreen: return new Color(0.133f, 0.545f, 0.133f);
                case PredefinedColor.SeaGreen: return new Color(0.180f, 0.545f, 0.341f);
                case PredefinedColor.MediumSeaGreen: return new Color(0.235f, 0.702f, 0.443f);
                case PredefinedColor.DarkSeaGreen: return new Color(0.561f, 0.737f, 0.561f);
                case PredefinedColor.LightSeaGreen: return new Color(0.125f, 0.698f, 0.667f);
                case PredefinedColor.MediumAquamarine: return new Color(0.400f, 0.804f, 0.667f);
                case PredefinedColor.Olive: return new Color(0.502f, 0.502f, 0f);
                case PredefinedColor.DarkOliveGreen: return new Color(0.333f, 0.420f, 0.184f);
                case PredefinedColor.OliveDrab: return new Color(0.420f, 0.557f, 0.137f);
                case PredefinedColor.YellowGreen: return new Color(0.604f, 0.804f, 0.196f);

                // ========== 青色系 ==========
                case PredefinedColor.Aqua: return new Color(0f, 1f, 1f);
                case PredefinedColor.DarkTurquoise: return new Color(0f, 0.808f, 0.820f);
                case PredefinedColor.Turquoise: return new Color(0.251f, 0.878f, 0.816f);
                case PredefinedColor.MediumTurquoise: return new Color(0.282f, 0.820f, 0.800f);
                case PredefinedColor.PaleTurquoise: return new Color(0.686f, 0.933f, 0.933f);
                case PredefinedColor.Aquamarine: return new Color(0.498f, 1f, 0.831f);
                case PredefinedColor.PowderBlue: return new Color(0.690f, 0.878f, 0.902f);
                case PredefinedColor.CadetBlue: return new Color(0.373f, 0.620f, 0.627f);
                case PredefinedColor.LightCyan: return new Color(0.878f, 1f, 1f);
                case PredefinedColor.DarkCyan: return new Color(0f, 0.545f, 0.545f);
                case PredefinedColor.Teal: return new Color(0f, 0.502f, 0.502f);

                // ========== 蓝色系 ==========
                case PredefinedColor.MediumBlue: return new Color(0f, 0f, 0.804f);
                case PredefinedColor.DarkBlue: return new Color(0f, 0f, 0.545f);
                case PredefinedColor.Navy: return new Color(0f, 0f, 0.502f);
                case PredefinedColor.MidnightBlue: return new Color(0.098f, 0.098f, 0.439f);
                case PredefinedColor.RoyalBlue: return new Color(0.255f, 0.412f, 0.882f);
                case PredefinedColor.CornflowerBlue: return new Color(0.392f, 0.584f, 0.929f);
                case PredefinedColor.DodgerBlue: return new Color(0.118f, 0.565f, 1f);
                case PredefinedColor.DeepSkyBlue: return new Color(0f, 0.749f, 1f);
                case PredefinedColor.SkyBlue: return new Color(0.529f, 0.808f, 0.922f);
                case PredefinedColor.LightSkyBlue: return new Color(0.529f, 0.808f, 0.980f);
                case PredefinedColor.SteelBlue: return new Color(0.275f, 0.510f, 0.706f);
                case PredefinedColor.LightSteelBlue: return new Color(0.690f, 0.769f, 0.871f);
                case PredefinedColor.LightBlue: return new Color(0.678f, 0.847f, 0.902f);
                case PredefinedColor.SlateGray: return new Color(0.439f, 0.502f, 0.565f);
                case PredefinedColor.LightSlateGray: return new Color(0.467f, 0.533f, 0.600f);

                // ========== 紫色系 ==========
                case PredefinedColor.Purple: return new Color(0.502f, 0f, 0.502f);
                case PredefinedColor.DarkMagenta: return new Color(0.545f, 0f, 0.545f);
                case PredefinedColor.MediumPurple: return new Color(0.576f, 0.439f, 0.859f);
                case PredefinedColor.DarkOrchid: return new Color(0.600f, 0.196f, 0.800f);
                case PredefinedColor.MediumOrchid: return new Color(0.729f, 0.333f, 0.827f);
                case PredefinedColor.Orchid: return new Color(0.855f, 0.439f, 0.839f);
                case PredefinedColor.Plum: return new Color(0.867f, 0.627f, 0.867f);
                case PredefinedColor.Violet: return new Color(0.933f, 0.510f, 0.933f);
                case PredefinedColor.Fuchsia: return new Color(1f, 0f, 1f);
                case PredefinedColor.BlueViolet: return new Color(0.541f, 0.169f, 0.886f);
                case PredefinedColor.DarkViolet: return new Color(0.580f, 0f, 0.827f);
                case PredefinedColor.Amethyst: return new Color(0.600f, 0.400f, 0.800f);
                case PredefinedColor.RebeccaPurple: return new Color(0.400f, 0.200f, 0.600f);
                case PredefinedColor.Indigo: return new Color(0.294f, 0f, 0.510f);
                case PredefinedColor.SlateBlue: return new Color(0.416f, 0.353f, 0.804f);
                case PredefinedColor.MediumSlateBlue: return new Color(0.482f, 0.408f, 0.933f);
                case PredefinedColor.Thistle: return new Color(0.847f, 0.749f, 0.847f);
                case PredefinedColor.Lavender: return new Color(0.902f, 0.902f, 0.980f);

                // ========== 褐色系 ==========
                case PredefinedColor.Brown: return new Color(0.647f, 0.165f, 0.165f);
                case PredefinedColor.SaddleBrown: return new Color(0.545f, 0.271f, 0.075f);
                case PredefinedColor.Chocolate: return new Color(0.824f, 0.412f, 0.118f);
                case PredefinedColor.Peru: return new Color(0.804f, 0.522f, 0.247f);
                case PredefinedColor.BurlyWood: return new Color(0.871f, 0.722f, 0.529f);
                case PredefinedColor.Tan: return new Color(0.824f, 0.706f, 0.549f);
                case PredefinedColor.RosyBrown: return new Color(0.737f, 0.561f, 0.561f);
                case PredefinedColor.Bisque: return new Color(1f, 0.894f, 0.769f);
                case PredefinedColor.NavajoWhite: return new Color(1f, 0.871f, 0.678f);
                case PredefinedColor.PapayaWhip: return new Color(1f, 0.937f, 0.835f);
                case PredefinedColor.Moccasin: return new Color(1f, 0.894f, 0.710f);
                case PredefinedColor.Cornsilk: return new Color(1f, 0.973f, 0.863f);

                // ========== 灰色系 ==========
                case PredefinedColor.DarkSlateGray: return new Color(0.184f, 0.310f, 0.310f);
                case PredefinedColor.DimGray: return new Color(0.412f, 0.412f, 0.412f);
                case PredefinedColor.DarkGray: return new Color(0.663f, 0.663f, 0.663f);
                case PredefinedColor.Silver: return new Color(0.753f, 0.753f, 0.753f);
                case PredefinedColor.LightGray: return new Color(0.827f, 0.827f, 0.827f);
                case PredefinedColor.Gainsboro: return new Color(0.863f, 0.863f, 0.863f);
                case PredefinedColor.WhiteSmoke: return new Color(0.961f, 0.961f, 0.961f);

                // ========== 白色系/其他 ==========
                case PredefinedColor.Snow: return new Color(1f, 0.980f, 0.980f);
                case PredefinedColor.Honeydew: return new Color(0.941f, 1f, 0.941f);
                case PredefinedColor.MintCream: return new Color(0.961f, 1f, 0.980f);
                case PredefinedColor.Azure: return new Color(0.941f, 1f, 1f);
                case PredefinedColor.AliceBlue: return new Color(0.941f, 0.973f, 1f);
                case PredefinedColor.GhostWhite: return new Color(0.973f, 0.973f, 1f);
                case PredefinedColor.FloralWhite: return new Color(1f, 0.980f, 0.941f);
                case PredefinedColor.SeaShell: return new Color(1f, 0.961f, 0.933f);
                case PredefinedColor.LavenderBlush: return new Color(1f, 0.941f, 0.961f);
                case PredefinedColor.MistyRose: return new Color(1f, 0.894f, 0.882f);
                case PredefinedColor.Ivory: return new Color(1f, 1f, 0.941f);

                default: return Color.white;
            }
        }
    }

    /// <summary>
    /// 指定附加类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AttachableAttribute : Attribute
    {
        public readonly Type[] Types;

        public AttachableAttribute(params Type[] types)
        {
            this.Types = types;
        }
    }

    /// <summary>
    /// 组内唯一性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueAttribute : Attribute
    {
    }

    /// <summary>
    /// 自定义片段预览
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPreviewAttribute : Attribute
    {
        public Type PreviewType;

        public CustomPreviewAttribute(Type type)
        {
            PreviewType = type;
        }
    }
    
    /// <summary>
    /// 自定义片段预览
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawableAttribute : Attribute
    {
    }
}