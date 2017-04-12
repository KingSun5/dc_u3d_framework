using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditorInternal;
using System.Reflection;
#endif

namespace BitBison
{
	public class SFont : ScriptableObject  
	{
		/**
		 * START OF ENUM DECLARATION
		 * */
		public enum HorzAlignment
		{
			Left,
			Center,
			Right
		}
		
		public enum VertAlignment
		{
			Top,
			Center,
			Bottom
		}
		
		/**
		 * END OF ENUM DECLARATION
		 * */
		
		private Sprite _fontSprite = null;
		private float _pixelsToUnits = 100.0f;
		private string _text = "";
		private int _orderInLayer = 0;
		private string _sortingLayer = "";
		private string _fontSet = "";
		private int _fontWidth = 0;
		private int _fontHeight = 0;
		private float _fontSpacing = 0.0f;
		private SFont.HorzAlignment _horizontalAlignment = SFont.HorzAlignment.Left;
		private SFont.VertAlignment _verticalAlignment = SFont.VertAlignment.Top;
		private Color _colorTint = Color.white;
		
		private Transform _parentTransform;
		public Transform parentTransform
		{
			get {
				return _parentTransform;
			}
			set {
				if (_parentTransform == null) {
					_parentTransform = value;
					_parentTransformRotation = _parentTransform.rotation;
					_parentTransformPosition = _parentTransform.position;
					_parentTransformScale = _parentTransform.localScale;
				} else {
					Debug.LogWarning("The value for parent transform cannot be changed once set.");
				}
			}
		}
		private GameObject _spriteFontObject;
		private GameObject _spriteFontRoot;
		private Vector3 _spriteFontOffset = Vector3.zero;
		private Vector3 _spriteFontOffsetPrev = Vector3.zero;
		private ArrayList _digits = new ArrayList();
		private Sprite[] _characterSprites;
		private bool _basicFontSettingSet = false;
		private float _textWidth = 0.0f;
		private bool _isTextWidthChanged = false;
		private Vector3 _centerAlignOffset = Vector3.zero;
		private Quaternion _parentTransformRotation = Quaternion.identity;
		private Vector3 _parentTransformPosition = Vector3.one;
		private Vector3 _parentTransformScale = Vector3.one;
		private bool _isAlignmentChanged = false;
		
		/**
		 * DEBUG SETTINGS
		 * */
		private bool _debug = false;
		
		
		public void UpdateFont(Sprite t_fontSprite, float t_pixelsToUnits, string t_text, string t_fontSet, int t_fontWidth, int t_fontHeight, float t_fontSpacing, SFont.HorzAlignment t_horizontalAlignment, SFont.VertAlignment t_verticalAlignment, int t_orderInLayer, string t_sortingLayer, Color t_colorTint)
		{
			if (_parentTransform) {
				//check basic requirements to make a font
				//check constraints on input
				if (t_pixelsToUnits < 0) {
					t_pixelsToUnits*=-1.0f;
				} else if (Mathf.Approximately(t_pixelsToUnits,0.0f)) {
					t_pixelsToUnits = 1.0f;
				}
				t_fontSet = t_fontSet.Trim();
				if (t_fontWidth < 0) {
					t_fontWidth *= -1;
				}
				if (t_fontHeight < 0) {
					t_fontHeight *= -1;
				}
				
				//check enums
				if (t_horizontalAlignment != SFont.HorzAlignment.Left && t_horizontalAlignment != SFont.HorzAlignment.Center && t_horizontalAlignment != SFont.HorzAlignment.Right) {
					t_horizontalAlignment = SFont.HorzAlignment.Left;
				}
				if (t_verticalAlignment != SFont.VertAlignment.Top && t_verticalAlignment != SFont.VertAlignment.Center && t_verticalAlignment != SFont.VertAlignment.Bottom) {
					t_verticalAlignment = SFont.VertAlignment.Top;
				}
				
				
				if (!(t_fontSprite == null) && t_pixelsToUnits > 0.0f && t_fontSet != "" && t_fontWidth > 0 && t_fontHeight > 0) {
					_basicFontSettingSet = true;
					
					//check if there are changes made to the font that will cause it to need to be recut
					if (t_fontSprite != _fontSprite || t_pixelsToUnits != _pixelsToUnits || t_fontSet != _fontSet || t_fontWidth != _fontWidth || t_fontHeight != _fontHeight || _spriteFontObject == null)
					{
						_fontSprite = t_fontSprite;
						_pixelsToUnits = t_pixelsToUnits;
						_fontSet = t_fontSet;
						_fontWidth = t_fontWidth;
						_fontHeight = t_fontHeight;
						_colorTint = t_colorTint;
						
						if (_spriteFontObject == null) {
							_spriteFontObject = new GameObject("_spriteFontObject");
							_spriteFontRoot = new GameObject("_spriteFontRoot");
							if (_debug) {
								_spriteFontObject.hideFlags = HideFlags.DontSave;
								_spriteFontRoot.hideFlags = HideFlags.DontSave;
							} else {
								_spriteFontObject.hideFlags = HideFlags.HideAndDontSave;
								_spriteFontRoot.hideFlags = HideFlags.HideAndDontSave;
							}
							_spriteFontObject.transform.position = _parentTransform.position;
							_spriteFontRoot.transform.position = _parentTransform.position;
							_spriteFontObject.transform.parent = _spriteFontRoot.transform;
							_spriteFontRoot.transform.rotation = _parentTransform.rotation;
							_spriteFontRoot.transform.localScale = _parentTransform.localScale;
						} else {
							ClearDigits();
							ClearSprites();
						}
						
						if (_characterSprites == null) {
							_characterSprites = new Sprite[_fontSet.Length];
						} else {
							System.Array.Resize(ref _characterSprites, _fontSet.Length);
						}
						
						CutSprites();
						
						_orderInLayer = t_orderInLayer;
						_sortingLayer = t_sortingLayer;
						
						_isAlignmentChanged = true;
						
						AlignAndSetText(t_text, t_fontSpacing, t_horizontalAlignment, t_verticalAlignment);
					}
					else if (_fontSpacing != t_fontSpacing || _horizontalAlignment != t_horizontalAlignment)
					{
						//only the alignment and text needs to be set or reset
						_isAlignmentChanged = true;
						
						ClearDigits();
						_orderInLayer = t_orderInLayer;
						_sortingLayer = t_sortingLayer;
						
						AlignAndSetText(t_text, t_fontSpacing, t_horizontalAlignment, t_verticalAlignment);
						
						if (t_colorTint != _colorTint) {
							_colorTint = t_colorTint;
							ColorSprites();
						}
					}
					else if (_text != t_text || _orderInLayer != t_orderInLayer || _sortingLayer != t_sortingLayer)
					{
						//only the text needs to be set or reset
						if (_orderInLayer != t_orderInLayer || _sortingLayer != t_sortingLayer) {
							_orderInLayer = t_orderInLayer;
							_sortingLayer = t_sortingLayer;
							SetText(t_text,true);
						} else {
							_orderInLayer = t_orderInLayer;
							_sortingLayer = t_sortingLayer;
							SetText(t_text);
						}
						if (t_colorTint != _colorTint) {
							_colorTint = t_colorTint;
							ColorSprites();
						}
					}
					else if (_verticalAlignment != t_verticalAlignment)
					{
						_isAlignmentChanged = true;
						if (t_colorTint != _colorTint) {
							_colorTint = t_colorTint;
							ColorSprites();
						}
						SetVerticalAligment(t_verticalAlignment);
					}
					else if (_colorTint != t_colorTint) {
						//change sprite colors
						_colorTint = t_colorTint;
						ColorSprites();
					}
				} else {
					ClearDigits();
					ClearSprites();
					
					_fontSprite = t_fontSprite;
					_pixelsToUnits = t_pixelsToUnits;
					_fontSet = t_fontSet;
					_fontWidth = t_fontWidth;
					_fontHeight = t_fontHeight;
					_text = t_text;
					_fontSpacing = t_fontSpacing;
					_horizontalAlignment = t_horizontalAlignment;
					_verticalAlignment = t_verticalAlignment;
					_orderInLayer = t_orderInLayer;
					_sortingLayer = t_sortingLayer;
					
					_basicFontSettingSet = false;
				}
			}
		}
		
		public void SetText(string t_text, bool forceReset = false) {
			//Check if the basic requirement are met
			if (_basicFontSettingSet) {
				if (_text != t_text || forceReset) {
					Quaternion oldRotation = _spriteFontRoot.transform.rotation;
					Vector3 oldScale = _spriteFontRoot.transform.localScale;
					_spriteFontRoot.transform.rotation = Quaternion.identity;
					_spriteFontRoot.transform.localScale = Vector3.one;
					_text = t_text;
					int i = 0;
					
					float textWidth = (_text.Length*_fontWidth*1.0f)+((_text.Length-1)*_fontSpacing);
					if (textWidth != _textWidth) {
						_textWidth = textWidth;
						_isTextWidthChanged = true;
					}
					
					if (_horizontalAlignment == SFont.HorzAlignment.Center && _isTextWidthChanged) {
						_centerAlignOffset = new Vector3(-((_textWidth*0.5f)/_pixelsToUnits), 0.0f, 0.0f);
						_isTextWidthChanged = false;
						_spriteFontRoot.transform.position = _parentTransform.position;
						_spriteFontObject.transform.localPosition = _spriteFontOffset + _centerAlignOffset;
					}
					
					for (i=0; i < _text.Length; i++) {
						int indexOfCharacter = 0;
						
						if (_horizontalAlignment == SFont.HorzAlignment.Right) {
							indexOfCharacter = _fontSet.IndexOf(_text[_text.Length-(1+i)]);
						} else {
							indexOfCharacter = _fontSet.IndexOf(_text[i]);
						}
						
						if (i >= _digits.Count) {
							GameObject newDigit = new GameObject("_char"+i,typeof(SpriteRenderer));
							if (_debug) {
								newDigit.hideFlags = HideFlags.DontSave;
							} else {
								newDigit.hideFlags = HideFlags.HideAndDontSave;
							}
							if (_horizontalAlignment == SFont.HorzAlignment.Right) {
								newDigit.transform.position = new Vector3(_spriteFontObject.transform.position.x - ((i+0.5f)*(_fontWidth*1.0f)/_pixelsToUnits) - ((i*_fontSpacing)/_pixelsToUnits), _spriteFontObject.transform.position.y-((_fontHeight*0.5f)/_pixelsToUnits), 0);
							} else {
								newDigit.transform.position = new Vector3(_spriteFontObject.transform.position.x + ((i+0.5f)*(_fontWidth*1.0f)/_pixelsToUnits) + ((i*_fontSpacing)/_pixelsToUnits), _spriteFontObject.transform.position.y-((_fontHeight*0.5f)/_pixelsToUnits), 0);
							}
							newDigit.transform.parent = _spriteFontObject.transform;
							_digits.Add(newDigit);
						} else {
							((GameObject)_digits[i]).SetActive(true);
						}
						
						if (indexOfCharacter > -1 && indexOfCharacter < _characterSprites.Length) {
							if (((GameObject)_digits[i]).GetComponent<SpriteRenderer>().sprite != _characterSprites[indexOfCharacter])
							{
								SpriteRenderer srTemp = ((GameObject)_digits[i]).GetComponent<SpriteRenderer>();
								srTemp.sprite = _characterSprites[indexOfCharacter];
								srTemp.color = _colorTint;
							}
							if (((GameObject)_digits[i]).GetComponent<SpriteRenderer>().sortingLayerName != _sortingLayer) {
								//make sure that the sorting layer name is a valid layer
								#if UNITY_EDITOR
								_sortingLayer = CheckSortingLayerName(_sortingLayer);
								#endif
								SpriteRenderer srTemp = ((GameObject)_digits[i]).GetComponent<SpriteRenderer>();
								srTemp.sortingLayerName = _sortingLayer;
								srTemp.color = _colorTint;
							}
							if (((GameObject)_digits[i]).GetComponent<SpriteRenderer>().sortingOrder != _orderInLayer) {
								SpriteRenderer srTemp = ((GameObject)_digits[i]).GetComponent<SpriteRenderer>();
								srTemp.sortingOrder = _orderInLayer;
								srTemp.color = _colorTint;
							}
						} else {
							((GameObject)_digits[i]).GetComponent<SpriteRenderer>().sprite = null;
							((GameObject)_digits[i]).SetActive(false);
						}
					}
					
					
					
					//Clear the rest of the digits by setting their sprites to null
					//and disable the game object
					if (_text.Length < _digits.Count) {
						for (i=_text.Length; i < _digits.Count; i++) {
							if (_digits[i] != null) {
								((GameObject)_digits[i]).GetComponent<SpriteRenderer>().sprite = null;
								((GameObject)_digits[i]).SetActive(false);
							}
						}
					}
					
					_spriteFontRoot.transform.rotation = oldRotation;
					_spriteFontRoot.transform.localScale = oldScale;
					
				}
			}
		}
		
		public void SetOpacity(float opacity) {
			if (opacity < 0) {
				opacity *= -1;
			} 
			if (opacity > 1.0f) {
				opacity = 1.0f;
			}
			_colorTint.a = opacity;
			ColorSprites();
		}
		
		public void SetColorTint(Color t_colorTint) {
			_colorTint = t_colorTint;
			ColorSprites();
		}
		
		public void Activate() {
			if (_spriteFontObject) {
				SetPosition();
				_spriteFontObject.SetActive(true);
			}
		}
		
		public void Deactivate() {
			if (_spriteFontObject) {
				_spriteFontObject.SetActive(false);
			}
		}
		
		public void Update()
		{
			if (_spriteFontObject) {
				SetPosition();
			}
		}
		
		void SetPosition() {
			if (_parentTransformScale != _parentTransform.localScale || _isAlignmentChanged == true) {
				_spriteFontRoot.transform.localScale = _parentTransform.localScale;
				_parentTransformScale = _parentTransform.localScale;
			}
			if (_parentTransformRotation != _parentTransform.rotation || _isAlignmentChanged == true) {
				_spriteFontRoot.transform.rotation = _parentTransform.rotation;
				_parentTransformRotation = _parentTransform.rotation;
			}
			if (_parentTransformPosition != _parentTransform.position || _isAlignmentChanged == true || _spriteFontOffsetPrev != _spriteFontOffset) {
				if (_horizontalAlignment != SFont.HorzAlignment.Center) {
					_spriteFontRoot.transform.position = _parentTransform.position;
					_spriteFontObject.transform.localPosition = _spriteFontOffset;
				} else {
					_spriteFontRoot.transform.position = _parentTransform.position;
					_spriteFontObject.transform.localPosition = _spriteFontOffset + _centerAlignOffset;
				}
				_parentTransformPosition = _parentTransform.position;
				_spriteFontOffsetPrev = _spriteFontOffset;
				_isAlignmentChanged = false;
			}
		}
		
		void OnDestroy() {
			ClearDigits();
			_digits = null;
			ClearSprites();
			_characterSprites = null;
			
			if (_spriteFontObject) {
				GameObject.DestroyImmediate(_spriteFontObject);
				GameObject.DestroyImmediate(_spriteFontRoot);
			}
		}
		
		/**
		 * Utility functions ---------------------------------------------------------------------------------------------------------
		 * */
		public void ClearSpritePool()
		{
			//can be called by the user to clear inactive sprites
			//remove all the inactive digits from back to front till you meet one that is active
			if (_digits.Count > 0) {
				for (int i=_digits.Count-1; i > -1; i--) {
					if (((GameObject) _digits[i]).activeSelf == false) {
						GameObject tmpDigit = (GameObject) _digits[i];
						_digits.RemoveAt(i);
						GameObject.DestroyImmediate(tmpDigit);
					} else {
						break;
					}
				}
			}
		}
		
		private void AlignAndSetText(string t_text, float t_fontSpacing, HorzAlignment t_horizontalAlignment, VertAlignment t_verticalAlignment)
		{
			_horizontalAlignment = t_horizontalAlignment;
			_verticalAlignment = t_verticalAlignment;
			SetVerticalAligment(t_verticalAlignment);
			_fontSpacing = t_fontSpacing;
			
			SetText(t_text, true);
		}
		
		private void SetVerticalAligment(SFont.VertAlignment t_verticalAlignment) {
			_verticalAlignment = t_verticalAlignment;
			
			Vector2 pivotMultiplier = new Vector2(0.0f,0.0f);
			
			switch (_verticalAlignment)
			{
			case VertAlignment.Top:
				pivotMultiplier = new Vector2(0.0f,0.0f);
				break;
			case VertAlignment.Center:
				pivotMultiplier = new Vector2(0.0f,0.5f);
				break;
			case VertAlignment.Bottom:
				pivotMultiplier = new Vector2(0.0f,1.0f);
				break;
			}
			
			_spriteFontOffset = new Vector3(0, (_fontHeight*pivotMultiplier.y)/_pixelsToUnits, 0);
		}
		
		private void CutSprites() {
			//Debug.Log("Called Cut-Sprite");

			int spriteX = 0;
			int spriteY = _fontHeight;
			int fontSetIndex = 0;
			
			while (fontSetIndex < _fontSet.Length) {
				bool isCharCreated = false;
				
				if (spriteX + _fontWidth <= _fontSprite.rect.width && spriteY <= _fontSprite.rect.height) {
					_characterSprites[fontSetIndex] = Sprite.Create(_fontSprite.texture, new Rect(_fontSprite.textureRect.xMin + spriteX, _fontSprite.textureRect.yMax - spriteY, _fontWidth*1.0f, _fontHeight*1.0f), new Vector2(0.5f, 0.5f), _pixelsToUnits);
					isCharCreated = true;
				} else if (spriteY > _fontSprite.rect.height) {
					_characterSprites[fontSetIndex] = Sprite.Create(_fontSprite.texture, new Rect(_fontSprite.textureRect.xMin, _fontSprite.textureRect.yMax, 0.0f, 0.0f), new Vector2(0.0f, 0.0f), _pixelsToUnits);
					isCharCreated = true;
				}
				
				if (isCharCreated) {
					if (_debug) {
						_characterSprites[fontSetIndex].hideFlags = HideFlags.DontSave;
					} else {
						_characterSprites[fontSetIndex].hideFlags = HideFlags.HideAndDontSave;
					}
					_characterSprites[fontSetIndex].name = _fontSprite.name + "_" + _fontSet[fontSetIndex];
					fontSetIndex++;
				}
				
				spriteX += _fontWidth;
				if (spriteX >= _fontSprite.textureRect.width) {
					spriteY += _fontHeight;
					spriteX = 0;
				}
				
			}
			
		}
		
		private void ColorSprites() {
			if (_digits.Count > 0) {
				for (int i=0; i < _digits.Count; i++) {
					((GameObject) _digits[i]).GetComponent<SpriteRenderer>().color = _colorTint;
				}
			}
		}
		
		private void ClearDigits() {
			if (_digits.Count > 0) {
				for (int i=0; i < _digits.Count; i++) {
					GameObject.DestroyImmediate((GameObject) _digits[i]);
				}
				_digits.Clear();
				_textWidth = 0.0f;
				_isTextWidthChanged = true;
			}
		}
		
		private void ClearSprites() {
			if (_characterSprites != null) {
				for (int i=0; i < _characterSprites.Length; i++) {
					if (_characterSprites[i] != null) {
						GameObject.DestroyImmediate(_characterSprites[i]);
					}
				}
			}
		}

#if UNITY_EDITOR
		private string[] GetSortingLayerNames()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
		
		private string CheckSortingLayerName(string sortingLayerName) {
			string[] sortingLayerNames = GetSortingLayerNames();
			int selectedSortingLayer = 0;
			
			for (int i = 0; i<sortingLayerNames.Length;i++)
			{
				if (sortingLayerNames[i] == sortingLayerName)
				{
					selectedSortingLayer = i;
				}
			}
			return sortingLayerNames[selectedSortingLayer];
		}
#endif
	}
}