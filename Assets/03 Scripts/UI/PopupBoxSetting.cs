using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PopupBoxSetting : MonoBehaviour
{
    [Serializable]
    public class SettingTap
    {
        public SettingType settingType;
        public Button btTap;
        public Sprite sptNormal;
        public Sprite sptSelected;

        [NonSerialized]
        public RectTransform rectContent;
    }

    const string BGM_VOLUME_KEY = "Settings.BgmVolume";
    const string SFX_VOLUME_KEY = "Settings.SfxVolume";
    const string FRAME_RATE_KEY = "Settings.FrameRate";
    const int DEFAULT_FRAME_RATE = 60;

    static readonly Color TEXT_COLOR = new Color32( 80, 47, 29, 255 );
    static readonly Color ROW_COLOR = new Color32( 255, 247, 230, 238 );
    static readonly Color BUTTON_COLOR = new Color32( 255, 211, 127, 255 );
    static readonly Color TRACK_COLOR = new Color32( 223, 190, 152, 255 );
    static readonly Color FILL_COLOR = new Color32( 52, 132, 166, 255 );

    [Header( "설정 팝업" )]
    [SerializeField]
    RectTransform rectPanel;
    [SerializeField]
    TMP_FontAsset font;
    [SerializeField]
    Sprite sptCloseIcon;
    [SerializeField]
    List<SettingTap> settingTaps = new List<SettingTap>();

    [Header( "오디오 믹서" )]
    [SerializeField]
    AudioMixer audioMixer;

    [Header( "서비스 주소" )]
    [SerializeField]
    string termsUrl;
    [SerializeField]
    string privacyUrl;
    [SerializeField]
    string contactUrl;

    TextMeshProUGUI txNickname;
    TextMeshProUGUI txAccountId;
    Button btCopyId;

    public SettingType CurrentSettingType { get; private set; } = SettingType.Settings;

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    static void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = PlayerPrefs.GetInt( FRAME_RATE_KEY, DEFAULT_FRAME_RATE );
    }

    private void Awake()
    {
        if( rectPanel == null )
            return;

        CreateText( rectPanel, "Title", "설정", new Vector2( 0.25f, 0.86f ), new Vector2( 0.75f, 0.95f ), 46 );
        var btClose = CreateButton( rectPanel, "Close", sptCloseIcon == null ? "×" : string.Empty, new Vector2( 0.78f, 0.865f ), new Vector2( 0.89f, 0.935f ), BUTTON_COLOR );
        if( sptCloseIcon != null )
        {
            var imgClose = CreateImage( btClose.image.rectTransform, "CloseIcon", new Vector2( 0.14f, 0.14f ), new Vector2( 0.86f, 0.86f ), Color.white );
            imgClose.sprite = sptCloseIcon;
            imgClose.preserveAspect = true;
        }

        btClose.onClick.AddListener( () => gameObject.SetActive( false ) );

        for( int i = 0; i < settingTaps.Count; i++ )
        {
            var tap = settingTaps[ i ];
            if( tap == null )
                continue;

            float tabLeft = 0.18f + i * 0.16f;
            if( tap.btTap == null )
            {
                tap.btTap = CreateButton( rectPanel, tap.settingType.ToString(), string.Empty, new Vector2( tabLeft, 0.745f ), new Vector2( tabLeft + 0.16f, 0.83f ), Color.white );
                tap.btTap.image.preserveAspect = true;
            }
            tap.btTap.transition = Selectable.Transition.None;

            tap.rectContent = CreateRect( rectPanel, tap.settingType + "Content", new Vector2( 0.155f, 0.22f ), new Vector2( 0.845f, 0.725f ) );
            InitContent( tap );

            tap.btTap.OnClickAsObservable().Subscribe( _ =>
            {
                ReflashTap( tap.settingType );
            } ).AddTo( this );
        }
    }

    private void OnEnable()
    {
        transform.SetAsLastSibling();
        ReflashTap( CurrentSettingType );
    }

    public void ReflashTap( SettingType settingType )
    {
        CurrentSettingType = settingType;

        foreach( var tap in settingTaps )
        {
            if( tap == null )
                continue;

            bool isSelected = tap.settingType == settingType;
            if( tap.btTap != null )
            {
                tap.btTap.image.sprite = isSelected ? tap.sptSelected : tap.sptNormal;
                tap.btTap.interactable = isSelected == false;
            }

            if( tap.rectContent != null )
                tap.rectContent.gameObject.SetActive( isSelected );
        }

        if( settingType == SettingType.Account && txAccountId != null )
        {
            var userManager = UserManager.Ins;
            string accountId = userManager?.userGamerID;
            txNickname.text = string.IsNullOrEmpty( userManager?.userNickName ) ? "닉네임 없음" : userManager.userNickName;
            txAccountId.text = string.IsNullOrEmpty( accountId ) ? "계정 ID 없음" : accountId;
            btCopyId.interactable = string.IsNullOrEmpty( accountId ) == false;
        }
    }

    void InitContent( SettingTap tap )
    {
        switch( tap.settingType )
        {
            case SettingType.Settings:
            {
                CreateVolumeRow( tap.rectContent, 0, "배경음", BGM_VOLUME_KEY, "BGMVolume" );
                CreateVolumeRow( tap.rectContent, 1, "효과음", SFX_VOLUME_KEY, "SFXVolume" );
                break;
            }
            case SettingType.Graphics:
            {
                var rectRow = CreateRow( tap.rectContent, 0, "프레임 제한" );
                var bt30 = CreateButton( rectRow, "30FPS", "30 FPS", new Vector2( 0.46f, 0.2f ), new Vector2( 0.69f, 0.8f ), BUTTON_COLOR );
                var bt60 = CreateButton( rectRow, "60FPS", "60 FPS", new Vector2( 0.71f, 0.2f ), new Vector2( 0.94f, 0.8f ), BUTTON_COLOR );

                void ReflashFrameRate()
                {
                    int frameRate = PlayerPrefs.GetInt( FRAME_RATE_KEY, DEFAULT_FRAME_RATE );
                    bt30.interactable = frameRate != 30;
                    bt60.interactable = frameRate != 60;
                }

                bt30.onClick.AddListener( () => SetFrameRate( 30 ) );
                bt60.onClick.AddListener( () => SetFrameRate( 60 ) );
                ReflashFrameRate();

                void SetFrameRate( int frameRate )
                {
                    PlayerPrefs.SetInt( FRAME_RATE_KEY, frameRate );
                    PlayerPrefs.Save();
                    ApplyFrameRate();
                    ReflashFrameRate();
                }

                break;
            }
            case SettingType.Service:
            {
                CreateServiceRow( tap.rectContent, 0, "이용약관", termsUrl );
                CreateServiceRow( tap.rectContent, 1, "개인정보 처리방침", privacyUrl );
                CreateServiceRow( tap.rectContent, 2, "고객 문의", contactUrl );
                var rectVersion = CreateRow( tap.rectContent, 3, "게임 버전" );
                CreateText( rectVersion, "Version", Application.version, new Vector2( 0.62f, 0 ), new Vector2( 0.94f, 1 ), 27, TextAlignmentOptions.MidlineRight );
                break;
            }
            case SettingType.Account:
            {
                var rectNickname = CreateRow( tap.rectContent, 0, "닉네임" );
                txNickname = CreateText( rectNickname, "Nickname", string.Empty, new Vector2( 0.4f, 0 ), new Vector2( 0.94f, 1 ), 31, TextAlignmentOptions.MidlineRight );
                var rectId = CreateRow( tap.rectContent, 1, "계정 ID" );
                txAccountId = CreateText( rectId, "AccountId", string.Empty, new Vector2( 0.06f, 0.02f ), new Vector2( 0.68f, 0.46f ), 24, TextAlignmentOptions.MidlineLeft );
                var txLabel = rectId.GetComponentInChildren<TextMeshProUGUI>();
                txLabel.rectTransform.anchorMin = new Vector2( 0.06f, 0.46f );
                btCopyId = CreateButton( rectId, "Copy", "복사", new Vector2( 0.72f, 0.18f ), new Vector2( 0.94f, 0.82f ), BUTTON_COLOR );
                btCopyId.onClick.AddListener( () => GUIUtility.systemCopyBuffer = txAccountId.text );
                break;
            }
        }
    }

    void CreateVolumeRow( RectTransform parent, int index, string label, string key, string mixerParameter )
    {
        var rectRow = CreateRow( parent, index, label );
        rectRow.GetComponentInChildren<TextMeshProUGUI>().rectTransform.anchorMin = new Vector2( 0.06f, 0.47f );
        var btMute = CreateButton( rectRow, "Mute", "음소거", new Vector2( 0.7f, 0.53f ), new Vector2( 0.94f, 0.92f ), BUTTON_COLOR );
        var txMute = btMute.GetComponentInChildren<TextMeshProUGUI>();
        var txValue = CreateText( rectRow, "Volume", string.Empty, new Vector2( 0.81f, 0.07f ), new Vector2( 0.94f, 0.5f ), 29, TextAlignmentOptions.MidlineRight );
        var rectSlider = CreateRect( rectRow, "Slider", new Vector2( 0.06f, 0.11f ), new Vector2( 0.76f, 0.46f ) );
        var slider = rectSlider.gameObject.AddComponent<Slider>();
        var imgTrack = CreateImage( rectSlider, "Track", new Vector2( 0, 0.34f ), new Vector2( 1, 0.66f ), TRACK_COLOR );
        var imgFill = CreateImage( imgTrack.rectTransform, "Fill", Vector2.zero, Vector2.one, FILL_COLOR );
        var rectHandle = CreateRect( rectSlider, "HandleArea", new Vector2( 0.04f, 0 ), new Vector2( 0.96f, 1 ) );
        var imgHandle = CreateImage( rectHandle, "Handle", new Vector2( 0, 0.05f ), new Vector2( 0, 0.95f ), FILL_COLOR );
        imgHandle.rectTransform.sizeDelta = new Vector2( 20, 0 );
        imgTrack.raycastTarget = true;
        imgHandle.raycastTarget = true;
        slider.targetGraphic = imgHandle;
        slider.fillRect = imgFill.rectTransform;
        slider.handleRect = imgHandle.rectTransform;
        slider.SetValueWithoutNotify( Mathf.Clamp01( PlayerPrefs.GetFloat( key, 1f ) ) );
        bool isMuted = PlayerPrefs.GetInt( key + ".Muted", 0 ) != 0;

        void ReflashVolume()
        {
            txValue.text = string.Format( "{0:0}%", slider.value * 100f );
            txMute.text = isMuted ? "음 켜기" : "음소거";
            ApplyVolume( mixerParameter, isMuted ? 0f : slider.value );
        }

        slider.onValueChanged.AddListener( value =>
        {
            PlayerPrefs.SetFloat( key, value );
            ReflashVolume();
        } );

        btMute.onClick.AddListener( () =>
        {
            isMuted = !isMuted;
            PlayerPrefs.SetInt( key + ".Muted", isMuted ? 1 : 0 );
            ReflashVolume();
        } );

        ReflashVolume();
    }

    void ApplyVolume( string mixerParameter, float volume )
    {
        if( audioMixer != null )
            audioMixer.SetFloat( mixerParameter, 20f * Mathf.Log10( Mathf.Max( 0.0001f, volume ) ) );
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    void CreateServiceRow( RectTransform parent, int index, string label, string url )
    {
        var rectRow = CreateRow( parent, index, label );
        var button = CreateButton( rectRow, "Open", "열기", new Vector2( 0.73f, 0.18f ), new Vector2( 0.94f, 0.82f ), BUTTON_COLOR );
        button.interactable = Uri.TryCreate( url, UriKind.Absolute, out Uri address ) && ( address.Scheme == Uri.UriSchemeHttps || address.Scheme == Uri.UriSchemeHttp );
        if( button.interactable )
            button.onClick.AddListener( () => Application.OpenURL( address.AbsoluteUri ) );
    }

    RectTransform CreateRow( RectTransform parent, int index, string label )
    {
        float top = 1f - index * 0.245f;
        var row = CreateImage( parent, label, new Vector2( 0, top - 0.216f ), new Vector2( 1, top ), ROW_COLOR );
        CreateText( row.rectTransform, "Label", label, new Vector2( 0.06f, 0 ), new Vector2( 0.69f, 1 ), 34, TextAlignmentOptions.MidlineLeft );
        return row.rectTransform;
    }

    Button CreateButton( RectTransform parent, string name, string label, Vector2 min, Vector2 max, Color color )
    {
        var image = CreateImage( parent, name, min, max, color );
        image.raycastTarget = true;
        var bt = image.gameObject.AddComponent<Button>();
        bt.targetGraphic = image;
        var colors = bt.colors;
        colors.disabledColor = new Color( 1f, 1f, 1f, 0.55f );
        bt.colors = colors;
        if( string.IsNullOrEmpty( label ) == false )
            CreateText( image.rectTransform, "Label", label, new Vector2( 0.04f, 0.02f ), new Vector2( 0.96f, 0.98f ), 32 );
        return bt;
    }

    TextMeshProUGUI CreateText( RectTransform parent, string name, string value, Vector2 min, Vector2 max, float maxSize, TextAlignmentOptions alignment = TextAlignmentOptions.Center )
    {
        var rect = CreateRect( parent, name, min, max );
        var tx = rect.gameObject.AddComponent<TextMeshProUGUI>();
        tx.font = font;
        tx.text = value;
        tx.color = TEXT_COLOR;
        tx.alignment = alignment;
        tx.enableAutoSizing = true;
        tx.fontSizeMin = 14;
        tx.fontSizeMax = maxSize;
        tx.enableWordWrapping = false;
        tx.overflowMode = TextOverflowModes.Ellipsis;
        tx.raycastTarget = false;
        return tx;
    }

    static Image CreateImage( RectTransform parent, string name, Vector2 min, Vector2 max, Color color )
    {
        var rect = CreateRect( parent, name, min, max );
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    static RectTransform CreateRect( RectTransform parent, string name, Vector2 min, Vector2 max )
    {
        var rect = new GameObject( name, typeof( RectTransform ) ).GetComponent<RectTransform>();
        rect.gameObject.layer = parent.gameObject.layer;
        rect.SetParent( parent, false );
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }
}
