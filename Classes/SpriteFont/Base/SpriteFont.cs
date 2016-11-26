using UnityEngine;
using System.Collections;
using BitBison;

[ExecuteInEditMode]
public class SpriteFont : MonoBehaviour {

	private SFont _sFont;
	private bool _isVisible = false;

	/**
	 * READ-ONLY VALUES
	 * */
		
	[SerializeField]
	private Sprite _fontSprite = null;
	public Sprite fontSprite
	{
		get {
			return _fontSprite;
		}
	}

	[SerializeField]
	private float _pixelsToUnits = 100.0f;
	public float pixelsToUnits
	{
		get {
			return _pixelsToUnits;
		}
	}

	[SerializeField]
	private int _orderInLayer = 0;
	public int orderInLayer
	{
		get {
			return _orderInLayer;
		}
	}

	[SerializeField]
	private string _sortingLayer = "";
	public string sortingLayer
	{
		get {
			return _sortingLayer;
		}
	}

	[SerializeField]
	private string _text = "";
	public string text
	{
		get {
			return _text;
		}
	}

	[SerializeField]
	private string _fontSet = "";
	public string fontSet
	{
		get {
			return _fontSet;
		}
	}

	[SerializeField]
	private int _fontWidth = 0;
	public int fontWidth
	{
		get {
			return _fontWidth;
		}
	}

	[SerializeField]
	private int _fontHeight = 0;
	public int fontHeight
	{
		get {
			return _fontHeight;
		}
	}

	[SerializeField]
	private float _fontSpacing = 0.0f;
	public float fontSpacing
	{
		get {
			return _fontSpacing;
		}
	}

	[SerializeField]
	private SFont.HorzAlignment _horizontalAlignment = SFont.HorzAlignment.Left;
	public SFont.HorzAlignment horizontalAlignment
	{
		get {
			return _horizontalAlignment;
		}
	}

	[SerializeField]
	private SFont.VertAlignment _verticalAlignment = SFont.VertAlignment.Top;
	public SFont.VertAlignment verticalAlignment
	{
		get {
			return _verticalAlignment;
		}
	}

	[SerializeField]
	private int _fontSets = 0;
	public int fontSets
	{
		get {
			return _fontSets;
		}
	}

	[SerializeField]
	private Color _colorTint = Color.white;
	public Color colorTint
	{
		get {
			return _colorTint;
		}
	}

	public static GameObject Create(Sprite t_fontSprite = null, float t_pixelsToUnits = 100.0f, string t_text = "", string t_fontSet = "", int t_fontWidth = 0, int t_fontHeight = 0, float t_fontSpacing = 0.0f, SFont.HorzAlignment t_horizontalAlignment = SFont.HorzAlignment.Left, SFont.VertAlignment t_verticalAlignment = SFont.VertAlignment.Top, Color? t_colorTint = null, int t_orderInLayer = 0, string t_sortingLayer = "")
	{
		GameObject newSpriteFont = new GameObject("SpriteFont", typeof(SpriteFont));

		Color _tmpColor = t_colorTint.HasValue? t_colorTint.Value : Color.white;

		newSpriteFont.GetComponent<SpriteFont>().UpdateFont(t_fontSprite, t_pixelsToUnits, t_text, t_fontSet, t_fontWidth, t_fontHeight, t_fontSpacing, t_horizontalAlignment, t_verticalAlignment, _tmpColor, t_orderInLayer, t_sortingLayer, true, SFConstants.FontSetsArray.Length-1);
		return newSpriteFont;
	}

	public void UpdateFont(Sprite t_fontSprite, float t_pixelsToUnits, string t_text, string t_fontSet, int t_fontWidth, int t_fontHeight, float t_fontSpacing, SFont.HorzAlignment t_horizontalAlignment, SFont.VertAlignment t_verticalAlignment, Color t_colorTint, int t_orderInLayer = 0, string t_sortingLayer = "", bool isNotPrefab = true, int t_fontSets = 0)
	{
		_fontSprite = t_fontSprite;
		_pixelsToUnits = t_pixelsToUnits;
		_text = t_text;
		_fontSet = t_fontSet;
		_fontWidth = t_fontWidth;
		_fontHeight = t_fontHeight;
		_fontSpacing = t_fontSpacing;
		_horizontalAlignment = t_horizontalAlignment;
		_verticalAlignment = t_verticalAlignment;
		_orderInLayer = t_orderInLayer;
		_sortingLayer = t_sortingLayer;
		_fontSets = t_fontSets;
		_colorTint = t_colorTint;

		if (SFConstants.FontSetsArray[_fontSets] != "Custom Value") {
			if (_fontSets < SFConstants.fsArray.Length) {
				_fontSet = SFConstants.fsArray[_fontSets];
			}
		}

		if (!(_fontSprite == null)) {
			//check if the fontsprite ends with the fontHeight and fontWidth
			int lastIndexOfX = _fontSprite.name.LastIndexOf("x");
			int lastIndexOf_ = _fontSprite.name.LastIndexOf("_");
			if (_fontHeight == 0) {
				if (lastIndexOfX > -1 && lastIndexOf_ > -1) {
					string heightStr = _fontSprite.name.Substring(lastIndexOfX+1, _fontSprite.name.Length - (lastIndexOfX+1));
					int iFontHeight = 0;
					if (int.TryParse(heightStr,out iFontHeight)) {
						_fontHeight = iFontHeight;
					}
				}
			}
			if (_fontWidth == 0) {
				if (lastIndexOfX > -1  && lastIndexOf_ > -1) {
					string widthStr = _fontSprite.name.Substring(lastIndexOf_+1, lastIndexOfX - (lastIndexOf_+1));
					int iFontWidth = 0;
					if (int.TryParse(widthStr,out iFontWidth)) {
						_fontWidth = iFontWidth;
					}
				}
			}
		}

		if (_pixelsToUnits < 0) {
			_pixelsToUnits*=-1.0f;
		} else if (Mathf.Approximately(_pixelsToUnits,0.0f)) {
			_pixelsToUnits = 1.0f;
		}
		_fontSet = _fontSet.Trim();
		if (_fontWidth < 0) {
			_fontWidth *= -1;
		}
		if (_fontHeight < 0) {
			_fontHeight *= -1;
		}

		if (_isVisible && isNotPrefab) {
			if (_sFont == null) {
				_sFont = ScriptableObject.CreateInstance<SFont>();
				_sFont.parentTransform = transform;
				_sFont.hideFlags = HideFlags.DontSave;
			}

			_sFont.UpdateFont(_fontSprite, _pixelsToUnits, _text, _fontSet, _fontWidth, _fontHeight, _fontSpacing, _horizontalAlignment, _verticalAlignment, _orderInLayer, _sortingLayer, _colorTint);
		}
	}

	public void SetText(string t_text)
	{
		_text = t_text;
		if (_isVisible && _sFont) {
			_sFont.SetText(_text);
		}
	}

	public void SetOpacity(float opacity)
	{
		_colorTint.a = opacity;
		if (_isVisible && _sFont) {
			if (opacity < 0) {
				opacity *= -1;
			} 
			if (opacity > 1.0f) {
				opacity = 1.0f;
			}
			//Color newColor = new Color(_colorTint.r, _colorTint.g, _colorTint.b, opacity);
			//_colorTint.
			_sFont.SetOpacity(opacity);
		}
	}

	public void SetColorTint(Color t_colorTint) {
		_colorTint = t_colorTint;
		if (_isVisible && _sFont) {
			_sFont.SetColorTint(t_colorTint);
		}
	}
	
	//can be called by the user to clear inactive digits
	public void ClearSpritePool()
	{
		if (_sFont) {
			_sFont.ClearSpritePool();
		}
	}

	void Update()
	{
		if (_sFont) {
			_sFont.Update();
		}
	}

	void OnEnable()
	{
		_isVisible = true;
		if (_sFont) {
			_sFont.Activate();
		}
		UpdateFont(_fontSprite, _pixelsToUnits, _text, _fontSet, _fontWidth, _fontHeight, _fontSpacing, _horizontalAlignment, _verticalAlignment, _colorTint, _orderInLayer, _sortingLayer, true, _fontSets);
	}

	void OnDisable()
	{
		_isVisible = false;
		if (_sFont) {
			_sFont.Deactivate();
		}
	}

	void Reset()
	{
		UpdateFont(_fontSprite, _pixelsToUnits, _text, _fontSet, _fontWidth, _fontHeight, _fontSpacing, _horizontalAlignment, _verticalAlignment, _colorTint, _orderInLayer, _sortingLayer, true, _fontSets);
	}

	void OnDestroy()
	{
		if (_sFont) {
			DestroyImmediate(_sFont);
		}
	}
}
