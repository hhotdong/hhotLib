﻿using System;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

namespace SleekRender
{
    [CustomEditor(typeof(SleekRenderSettings))]
    public class SleekRenderSettingsInspector : Editor
    {
        private SerializedProperty _isBloomGroupExpandedProperty;
        private SerializedProperty _bloomEnabledProperty;
        private SerializedProperty _bloomThresholdProperty;
        private SerializedProperty _bloomIntensityProperty;
        private SerializedProperty _bloomTintProperty;
        private SerializedProperty _bloomPreserveAspectRatioProperty;
        private SerializedProperty _bloomWidthProperty;
        private SerializedProperty _bloomHeightProperty;
        private SerializedProperty _bloomLumaVectorProperty;
        private SerializedProperty _bloomSelectedLumaVectorTypeProperty;

        private string[] _bloomSizeVariants = new[] { "32", "64", "128" };
        private int[] _bloomSizeVariantInts = new[] { 32, 64, 128 };
        private int _selectedBloomWidthIndex = -1;
        private int _selectedBloomHeightIndex = -1;

        private LumaVectorType _selectedLumaVectorType;

        private SerializedProperty _isColorizeGroupExpandedProperty;
        private SerializedProperty _colorizeEnabledProperty;
        private SerializedProperty _colorizeProperty;

        private SerializedProperty _isVignetteExpandedProperty;
        private SerializedProperty _vignetteEnabledProperty;
        private SerializedProperty _vignetteBeginRadiusProperty;
        private SerializedProperty _vignetteExpandRadiusProperty;
        private SerializedProperty _vignetteColorProperty;

        private SerializedProperty _isContrastAndBrightnessEditorExpandedProperty;
        private SerializedProperty _contrastAndBrightnessEnabledProperty;
        private SerializedProperty _contrasteIntensity;
        private SerializedProperty _brightnesseIntensity;

        private SerializedProperty _isScreenBlurExpandedProperty;
        private SerializedProperty _screenBlurEnabledProperty;
        private SerializedProperty _kernelSizeProperty;
        private SerializedProperty _interpolationProperty;
        private SerializedProperty _downsampleProperty;
        private SerializedProperty _iterationsProperty;
        private SerializedProperty _gammaCorrectionProperty;
        private BlurKernelSize _selectedBlurKernelSize;

        private SerializedProperty _isHDRControlExpandedProperty;
        private SerializedProperty _HDREnabledProperty;
        private SerializedProperty _HDRtonemapperProperty;
        private SerializedProperty _HDRexposureProperty;
        private SerializedProperty _HDRlinearWhitePointProperty;
        private SerializedProperty _HDRuseDitheringProperty;
        private TonemappingSetting _selectedToneMapping;

        private SerializedProperty _isColorGradingExpandedProperty;
        private SerializedProperty _colorGradingEnabledProperty;
        private SerializedProperty _colorGradingNowBlendingProperty;
        private SerializedProperty _colorGradingQualityLevelProperty;
        private SerializedProperty _colorGradingblendAmountProperty;
        private SerializedProperty _colorGradinglutTextureProperty;
        private SerializedProperty _colorGradinglutBlendTextureProperty;
        private ColorGradingQuality _selectedColorGradingQuality;

        private void OnEnable()
        {
            _isBloomGroupExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomExpanded));
            _bloomEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomEnabled));
            _bloomThresholdProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomThreshold));
            _bloomIntensityProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomIntensity));
            _bloomTintProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomTint));
            _bloomPreserveAspectRatioProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.preserveAspectRatio));
            _bloomWidthProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomTextureWidth));
            _selectedBloomWidthIndex = Array.IndexOf(_bloomSizeVariantInts, _bloomWidthProperty.intValue);
            _bloomHeightProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomTextureHeight));
            _selectedBloomHeightIndex = Array.IndexOf(_bloomSizeVariantInts, _bloomHeightProperty.intValue);
            _bloomLumaVectorProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomLumaVector));
            _bloomSelectedLumaVectorTypeProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.bloomLumaCalculationType));
            _selectedLumaVectorType = (LumaVectorType)_bloomSelectedLumaVectorTypeProperty.enumValueIndex;

            _isColorizeGroupExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorizeExpanded));
            _colorizeEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorizeEnabled));
            _colorizeProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorize));

            _isVignetteExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.vignetteExpanded));
            _vignetteEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.vignetteEnabled));
            _vignetteBeginRadiusProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.vignetteBeginRadius));
            _vignetteExpandRadiusProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.vignetteExpandRadius));
            _vignetteColorProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.vignetteColor));

            _isContrastAndBrightnessEditorExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.brightnessContrastExpanded));
            _contrastAndBrightnessEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.brightnessContrastEnabled));
            _contrasteIntensity = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.contrast));
            _brightnesseIntensity = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.brightness));

            _isScreenBlurExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.screenBlurExpanded));
            _screenBlurEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.screenBlurEnabled));
            _kernelSizeProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.kernelSize));
            _interpolationProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.interpolation));
            _downsampleProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.downsample));
            _iterationsProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.iterations));
            _gammaCorrectionProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.gammaCorrection));
            _selectedBlurKernelSize = (BlurKernelSize)_kernelSizeProperty.enumValueIndex;

            _isHDRControlExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrControlExpanded));
            _HDREnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrControlEnabled));
            _HDRtonemapperProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrTonemapper));
            _HDRexposureProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrExposure));
            _HDRlinearWhitePointProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrLinearWhitePoint));
            _HDRuseDitheringProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.hdrApplyDithering));
            _selectedToneMapping = (TonemappingSetting)_HDRtonemapperProperty.enumValueIndex;

            _isColorGradingExpandedProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorGradingExpanded));
            _colorGradingEnabledProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorGradingEnabled));
            _colorGradingNowBlendingProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.nowBlending));
            _colorGradingQualityLevelProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.colorGradingQualityLevel));
            _colorGradingblendAmountProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.ColorGradingBlendAmount));
            _colorGradinglutTextureProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.LutTexture));
            _colorGradinglutBlendTextureProperty = serializedObject.FindProperty(GetMemberName((SleekRenderSettings s) => s.LutBlendTexture));
            _selectedColorGradingQuality = (ColorGradingQuality)_colorGradingQualityLevelProperty.enumValueIndex;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int indent = EditorGUI.indentLevel;

            DrawBloomEditor();
            EditorGUILayout.Space();

            DrawColorizeEditor();
            EditorGUILayout.Space();

            DrawVignetteEditor();
            EditorGUILayout.Space();

            DrawContrastAndBrightnessEditor();
            EditorGUILayout.Space();

            DrawScreenBlurEditor();
            EditorGUILayout.Space();

            DrawHDRControlEditor();
            EditorGUILayout.Space();

            DrawColorGradingEditor();
            EditorGUILayout.Space();

            //DrawTotalCost();

            EditorGUI.indentLevel = indent;
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBloomEditor()
        {
            Header("Bloom", _isBloomGroupExpandedProperty, _bloomEnabledProperty);

            if (_isBloomGroupExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Bloom threshold");
                EditorGUILayout.Slider(_bloomThresholdProperty, 0f, 1f, "");
                EditorGUILayout.LabelField("Bloom intensity");
                EditorGUILayout.Slider(_bloomIntensityProperty, 0f, 15f, "");
                EditorGUILayout.LabelField("Bloom tint");
                _bloomTintProperty.colorValue = EditorGUILayout.ColorField("", _bloomTintProperty.colorValue);

                DrawBloomWidthProperties();
                DisplayLumaVectorProperties();

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawColorizeEditor()
        {
            Header("Colorize", _isColorizeGroupExpandedProperty, _colorizeEnabledProperty);

            if (_isColorizeGroupExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.LabelField("Color");
                _colorizeProperty.colorValue = EditorGUILayout.ColorField("", _colorizeProperty.colorValue);
                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawVignetteEditor()
        {
            Header("Vignette", _isVignetteExpandedProperty, _vignetteEnabledProperty);

            if (_isVignetteExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Begin radius");
                EditorGUILayout.Slider(_vignetteBeginRadiusProperty, 0f, 1f, "");

                EditorGUILayout.LabelField("Expand radius");
                EditorGUILayout.Slider(_vignetteExpandRadiusProperty, 0f, 3f, "");

                EditorGUILayout.LabelField("Color");
                _vignetteColorProperty.colorValue = EditorGUILayout.ColorField("", _vignetteColorProperty.colorValue);

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawContrastAndBrightnessEditor()
        {
            Header("Brightness / Contrast", _isContrastAndBrightnessEditorExpandedProperty, _contrastAndBrightnessEnabledProperty);

            if (_isContrastAndBrightnessEditorExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Contrast Intensity");
                EditorGUILayout.Slider(_contrasteIntensity, -1f, 1f, "");

                EditorGUILayout.LabelField("Brightness Intensity");
                EditorGUILayout.Slider(_brightnesseIntensity, -1f, 1f, "");

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawScreenBlurEditor()
        {
            Header("Screen Blur", _isScreenBlurExpandedProperty, _screenBlurEnabledProperty);

            if (_isScreenBlurExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Kernel Size");
                _selectedBlurKernelSize = (BlurKernelSize)EditorGUILayout.EnumPopup(_selectedBlurKernelSize);
                _kernelSizeProperty.enumValueIndex = (int)_selectedBlurKernelSize;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Interpolation");
                EditorGUILayout.Slider(_interpolationProperty, 0.0f, 1.0f, "");

                EditorGUILayout.LabelField("Downsample");
                EditorGUILayout.IntSlider(_downsampleProperty, 0, 4, "");

                EditorGUILayout.LabelField("Iterations");
                EditorGUILayout.IntSlider(_iterationsProperty, 1, 8, "");

                _gammaCorrectionProperty.boolValue = EditorGUILayout.ToggleLeft("Gamma Correction", _gammaCorrectionProperty.boolValue);

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawHDRControlEditor()
        {
            Header("HDR", _isHDRControlExpandedProperty, _HDREnabledProperty);

            if (_isHDRControlExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Tonemapping");
                _selectedToneMapping = (TonemappingSetting)EditorGUILayout.EnumPopup(_selectedToneMapping);
                _HDRtonemapperProperty.enumValueIndex = (int)_selectedToneMapping;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Exposure");
                EditorGUILayout.Slider(_HDRexposureProperty, 0.0f, 1.0f, "");

                EditorGUILayout.LabelField("Linear White Point");
                EditorGUILayout.Slider(_HDRlinearWhitePointProperty, 11.2f, 15.0f, "");

                _HDRuseDitheringProperty.boolValue = EditorGUILayout.ToggleLeft("Use Dithering", _HDRuseDitheringProperty.boolValue);

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawColorGradingEditor()
        {
            Header("Color Grading", _isColorGradingExpandedProperty, _colorGradingEnabledProperty);

            if (_isColorGradingExpandedProperty.boolValue)
            {
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Color Grading Quality");
                _selectedColorGradingQuality = (ColorGradingQuality)EditorGUILayout.EnumPopup(_selectedColorGradingQuality);
                _colorGradingQualityLevelProperty.enumValueIndex = (int)_selectedColorGradingQuality;

                EditorGUILayout.Space();
                _colorGradingNowBlendingProperty.boolValue = EditorGUILayout.ToggleLeft("Now Blending", _colorGradingNowBlendingProperty.boolValue);

                EditorGUILayout.LabelField("Blend Amount");
                EditorGUILayout.Slider(_colorGradingblendAmountProperty, 0.0f, 1.0f, "");

                EditorGUILayout.BeginHorizontal();
                _colorGradinglutTextureProperty.objectReferenceValue = TextureField("LUT Texture", (Texture2D)_colorGradinglutTextureProperty.objectReferenceValue);
                _colorGradinglutBlendTextureProperty.objectReferenceValue = TextureField("LUT BlendTexture", (Texture2D)_colorGradinglutBlendTextureProperty.objectReferenceValue);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawTotalCost()
        {
            // Skipping control rect, ignore any statical analisys warnings
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.HelpBox(SleekRenderCostCalculator.GetTotalCostStringFor(target as SleekRenderSettings),
                MessageType.Info);
        }

        private void DisplayLumaVectorProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Brightpass Luma calculation");

            _selectedLumaVectorType = (LumaVectorType)EditorGUILayout.EnumPopup(_selectedLumaVectorType);
            _bloomSelectedLumaVectorTypeProperty.enumValueIndex = (int)_selectedLumaVectorType;
            switch (_selectedLumaVectorType)
            {
                case LumaVectorType.Custom:
                    EditorGUILayout.PropertyField(_bloomLumaVectorProperty, new GUIContent(""));
                    break;
                case LumaVectorType.Uniform:
                    var oneOverThree = 1f / 3f;
                    _bloomLumaVectorProperty.vector3Value = new Vector3(oneOverThree, oneOverThree, oneOverThree);
                    break;
                case LumaVectorType.sRGB:
                    _bloomLumaVectorProperty.vector3Value = new Vector3(0.2126f, 0.7152f, 0.0722f);
                    break;
            }

            var vector = _bloomLumaVectorProperty.vector3Value;
            if (!Mathf.Approximately(vector.x + vector.y + vector.z, 1f))
            {
                EditorGUILayout.HelpBox("Luma vector is not normalized.\nVector values should sum up to 1.",
                    MessageType.Warning);
            }
        }

        private void DrawBloomWidthProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bloom texture size");

            _bloomPreserveAspectRatioProperty.boolValue = EditorGUILayout.ToggleLeft("Preserve aspect ratio", _bloomPreserveAspectRatioProperty.boolValue);

            var rect = EditorGUILayout.GetControlRect();
            var oneFourthOfWidth = rect.width * 0.25f;
            var xLabelRect = new Rect(rect.x, rect.y, oneFourthOfWidth, rect.height);
            var widthRect = new Rect(rect.x + oneFourthOfWidth, rect.y, oneFourthOfWidth, rect.height);
            var yLabelRect = new Rect(rect.x + oneFourthOfWidth * 2.0f, rect.y, oneFourthOfWidth, rect.height);
            var heightRect = new Rect(rect.x + oneFourthOfWidth * 3.0f, rect.y, oneFourthOfWidth, rect.height);

            if (!_bloomPreserveAspectRatioProperty.boolValue)
            {
                EditorGUI.LabelField(xLabelRect, "X");
                _selectedBloomWidthIndex = _selectedBloomWidthIndex != -1 ? _selectedBloomWidthIndex : 2;
                _selectedBloomWidthIndex = EditorGUI.Popup(widthRect, _selectedBloomWidthIndex, _bloomSizeVariants);
                _bloomWidthProperty.intValue = _bloomSizeVariantInts[_selectedBloomWidthIndex];
            }

            EditorGUI.LabelField(yLabelRect, "Y");
            _selectedBloomHeightIndex = _selectedBloomHeightIndex != -1 ? _selectedBloomHeightIndex : 2;
            _selectedBloomHeightIndex = EditorGUI.Popup(heightRect, _selectedBloomHeightIndex, _bloomSizeVariants);
            _bloomHeightProperty.intValue = _bloomSizeVariantInts[_selectedBloomHeightIndex];
        }

        public static bool Header(string title, SerializedProperty isExpanded, SerializedProperty enabledField)
        {
            var display = isExpanded == null || isExpanded.boolValue;
            var enabled = enabledField.boolValue;
            var rect = GUILayoutUtility.GetRect(16f, 22f, FxStyles.header);
            GUI.Box(rect, title, FxStyles.header);

            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            var e = Event.current;

            if (e.type == EventType.Repaint)
            {
                FxStyles.headerCheckbox.Draw(toggleRect, false, false, enabled, false);
            }

            if (e.type == EventType.MouseDown)
            {
                const float kOffset = 2f;
                toggleRect.x -= kOffset;
                toggleRect.y -= kOffset;
                toggleRect.width += kOffset * 2f;
                toggleRect.height += kOffset * 2f;

                if (toggleRect.Contains(e.mousePosition))
                {
                    enabledField.boolValue = !enabledField.boolValue;
                    e.Use();
                }
                else if (rect.Contains(e.mousePosition) && isExpanded != null)
                {
                    display = !display;
                    isExpanded.boolValue = !isExpanded.boolValue;
                    e.Use();
                }
            }

            return display;
        }

        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

        private static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperLeft;
            style.fixedWidth = 120;
            GUILayout.Label(name, style);
            var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(75), GUILayout.Height(75));
            GUILayout.EndVertical();
            return result;
        }
    }
}