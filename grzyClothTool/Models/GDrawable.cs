﻿using grzyClothTool.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace grzyClothTool.Models;

public class DrawableAdditionalOptions
{
    public bool RenderFlags { get; set; } = false;
}

public class GReservedDrawable : GDrawable
{
    public override bool IsReserved => true;

    public GReservedDrawable(bool isMale, bool isProp, int compType, int count) : base(isMale, isProp, compType, count)
    {

        FilePath = Path.Combine(FileHelper.ReservedAssetsPath, "reservedDrawable.ydd");
        Textures = [new GTexture(Path.Combine(FileHelper.ReservedAssetsPath, "reservedTexture.ytd"), compType, count, 0, false, isProp)];
        TypeNumeric = compType;
        Number = count;
        Sex = isMale;
        IsProp = isProp;

        SetDrawableName();
    }
}


public class GDrawable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string FilePath { get; set; }

    private string _name;
    public string Name 
    { 
        get => _name; 
        set {
            _name = value;
            OnPropertyChanged();
        } 
    }

    public virtual bool IsReserved => false;

    public int TypeNumeric { get; set; }
    public string TypeName => EnumHelper.GetName(TypeNumeric, IsProp);

    /// <returns>
    /// true(1) = male ped, false(0) = female ped
    /// </returns>
    public bool Sex { get; set; }
    public bool IsProp { get; set; }
    public bool IsComponent => !IsProp;

    public int Number { get; set; }
    public string DisplayNumber => (Number % 128).ToString("D3");


    private bool _hasSkin;
    public bool HasSkin
    {
        get { return _hasSkin; }
        set
        {
            if (_hasSkin != value)
            {
                _hasSkin = value;

                foreach (var txt in Textures)
                {
                    txt.HasSkin = value;
                }
            }
        }
    }


    public DrawableAdditionalOptions AdditionalOptions { get; set; }


    private bool _enableKeepPreview;
    public bool EnableKeepPreview 
    { 
        get => _enableKeepPreview;
        set { _enableKeepPreview = value; OnPropertyChanged(); } 
    }

    public float HairScaleValue { get; set; } = 0.0f;


    private bool _enableHairScale;
    public bool EnableHairScale
    {
        get => _enableHairScale;
        set { _enableHairScale = value; OnPropertyChanged(); }
    }

    public float HighHeelsValue { get; set; } = 0.0f;
    private bool _enableHighHeels;
    public bool EnableHighHeels
    {
        get => _enableHighHeels;
        set { _enableHighHeels = value; OnPropertyChanged(); }
    }

    private string _audio;
    public string Audio
    {
        get => _audio;
        set {
            _audio = value;
            OnPropertyChanged();
        }
    }

    public List<string> AvailableAudioList => EnumHelper.GetAudioList(TypeNumeric);

    public string RenderFlag { get; set; } = ""; // "" is the default value
    public static List<string> AvailableRenderFlagList => ["","PRF_ALPHA","PRF_DECAL", "PRF_CUTOUT"];

    public ObservableCollection<GTexture> Textures { get; set; }

    public GDrawable(string drawablePath, bool isMale, bool isProp, int compType, int count, bool hasSkin, ObservableCollection<GTexture> textures)
    {
        FilePath = drawablePath;
        Textures = textures;
        TypeNumeric = compType;
        Number = count;
        HasSkin = hasSkin;
        Sex = isMale;
        IsProp = isProp;

        Audio = "none";
        AdditionalOptions = new DrawableAdditionalOptions() { RenderFlags = isProp };

        SetDrawableName();
    }

    protected GDrawable(bool isMale, bool isProp, int compType, int count) { /* Used in GReservedDrawable */ }

    public void SetDrawableName()
    {
        string name = $"{TypeName}_{DisplayNumber}";
        var finalName = IsProp ? name : $"{name}_{(HasSkin ? "r" : "u")}";

        Name = finalName;
        //texture number needs to be updated too
        foreach (var txt in Textures)
        {
            txt.Number = Number;
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
