using System;
using System.Reflection;
using Tobii.GameIntegration.Net;
using UnityEngine;

[Serializable]
public class ExtendedViewSettingsForInspector {

    [SerializeField]
    private HeadViewType _headViewType;
    [SerializeField]
    private float _aspectRatioCorrectionFactor;
    [SerializeField]
    private float _headViewPitchCorrectionFactor;
    [SerializeField]
    private SensitivityGradientSettingsForInspector _gazeViewSensitivityGradientSettings;
    [SerializeField]
    private float _gazeViewResponsiveness;
    [SerializeField]
    private float _normalizedGazeViewMinimumExtensionAngle;
    [SerializeField]
    private float _normalizedGazeViewExtensionAngle;
    [SerializeField]
    private SensitivityGradientSettingsForInspector _headViewSensitivityGradientSettings;
    [SerializeField]
    private float _headViewResponsiveness;
    [SerializeField]
    private HeadViewAutoCenterSettingsForInspector _headViewAutoCenter;

    public ExtendedViewSettingsForInspector(ExtendedViewSettings source)
    {
        _headViewType = source.HeadViewType;
        _aspectRatioCorrectionFactor = source.AspectRatioCorrectionFactor;
        _headViewPitchCorrectionFactor = source.HeadViewPitchCorrectionFactor;
        _gazeViewSensitivityGradientSettings = new SensitivityGradientSettingsForInspector(source.GazeViewSensitivityGradientSettings);
        _gazeViewResponsiveness = source.GazeViewResponsiveness;
        _normalizedGazeViewMinimumExtensionAngle = source.NormalizedGazeViewMinimumExtensionAngle;
        _normalizedGazeViewExtensionAngle = source.NormalizedGazeViewExtensionAngle;
        _headViewSensitivityGradientSettings = new SensitivityGradientSettingsForInspector(source.HeadViewSensitivityGradientSettings);
        _headViewResponsiveness = source.HeadViewResponsiveness;
        _headViewAutoCenter = new HeadViewAutoCenterSettingsForInspector(source.HeadViewAutoCenter);
    }

    public ExtendedViewSettings ToExtendedViewSettings()
    {
        var result = new ExtendedViewSettings();

        result.HeadViewType = _headViewType;
        result.AspectRatioCorrectionFactor = _aspectRatioCorrectionFactor;
        result.HeadViewPitchCorrectionFactor = _headViewPitchCorrectionFactor;
        result.GazeViewSensitivityGradientSettings =
            _gazeViewSensitivityGradientSettings.ToSensitivityGradientSettings();
        result.GazeViewResponsiveness = _gazeViewResponsiveness;
        result.NormalizedGazeViewMinimumExtensionAngle = _normalizedGazeViewMinimumExtensionAngle;
        result.NormalizedGazeViewExtensionAngle = _normalizedGazeViewExtensionAngle;
        result.HeadViewSensitivityGradientSettings = _headViewSensitivityGradientSettings.ToSensitivityGradientSettings();
        result.HeadViewResponsiveness = _headViewResponsiveness;
        result.HeadViewAutoCenter = _headViewAutoCenter.ToHeadViewAutoCenterSettings();

        return result;
    }

    public static bool ShouldDeepEqual(
        object expected,
        object actual,
        BindingFlags bindingFlags = BindingFlags.NonPublic |
                                    BindingFlags.Public |
                                    BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly)
    {
        if (expected == null && actual == null)
            return true;

        if (expected == null || actual == null)
            return false;
        
        var result = true;

        var type = expected.GetType();

        foreach (var field in type.GetFields(bindingFlags))
        {
            var fieldType = field.FieldType;
            var expectedValue = field.GetValue(expected);
            var acctualValue = field.GetValue(actual);

            if (fieldType.IsValueType)
            {
                var expectedStringValue = expectedValue.ToString();
                var actualStringlValue = acctualValue.ToString();

                result = expectedStringValue == actualStringlValue;
            }

            if (fieldType.IsClass)
            {
                result = result && ShouldDeepEqual(expectedValue, acctualValue);
            }

            if (!result)
            {
                break;
            }
        }

        return result;
    }


    public HeadViewType HeadViewType
    {
        get
        {
            return _headViewType;
        }
        set
        {
            _headViewType = value;
        }
    }

    public float AspectRatioCorrectionFactor
    {
        get
        {
            return _aspectRatioCorrectionFactor;
        }
        set
        {
            _aspectRatioCorrectionFactor = value;
        }
    }

    public float HeadViewPitchCorrectionFactor
    {
        get
        {
            return _headViewPitchCorrectionFactor;
        }
        set
        {
            _headViewPitchCorrectionFactor = value;
        }
    }

    public SensitivityGradientSettingsForInspector GazeViewSensitivityGradientSettings
    {
        get
        {
            return _gazeViewSensitivityGradientSettings;
        }
        set
        {
            _gazeViewSensitivityGradientSettings = value;
        }
    }

    public float GazeViewResponsiveness
    {
        get
        {
            return _gazeViewResponsiveness;
        }
        set
        {
            _gazeViewResponsiveness = value;
        }
    }

    public float NormalizedGazeViewMinimumExtensionAngle
    {
        get
        {
            return _normalizedGazeViewMinimumExtensionAngle;
        }
        set
        {
            _normalizedGazeViewMinimumExtensionAngle = value;
        }
    }

    public float NormalizedGazeViewExtensionAngle
    {
        get
        {
            return _normalizedGazeViewExtensionAngle;
        }
        set
        {
            _normalizedGazeViewExtensionAngle = value;
        }
    }

    public SensitivityGradientSettingsForInspector HeadViewSensitivityGradientSettings
    {
        get
        {
            return _headViewSensitivityGradientSettings;
        }
        set
        {
            _headViewSensitivityGradientSettings = value;
        }
    }

    public float HeadViewResponsiveness
    {
        get
        {
            return _headViewResponsiveness;
        }
        set
        {
            _headViewResponsiveness = value;
        }
    }

    public HeadViewAutoCenterSettingsForInspector HeadViewAutoCenter
    {
        get
        {
            return _headViewAutoCenter;
        }
        set
        {
            _headViewAutoCenter = value;
        }
    }
}

[Serializable]
public class SensitivityGradientSettingsForInspector
{
    [SerializeField]
    private float _scale;
    [SerializeField]
    private float _exponent;
    [SerializeField]
    private float _inflectionPoint;
    [SerializeField]
    private float _startPoint;
    [SerializeField]
    private float _endPoint;

    public SensitivityGradientSettingsForInspector(SensitivityGradientSettings source)
    {
        _scale = source.Scale;
        _exponent = source.Exponent;
        _inflectionPoint = source.InflectionPoint;
        _startPoint = source.StartPoint;
        _endPoint = source.EndPoint;
    }

    public SensitivityGradientSettings ToSensitivityGradientSettings()
    {
        var result = new SensitivityGradientSettings
        {
            Scale = _scale,
            Exponent = _exponent,
            InflectionPoint = _inflectionPoint,
            StartPoint = _startPoint,
            EndPoint = _endPoint
        };

        return result;
    }

    public float Scale
    {
        get
        {
            return _scale;
        }
        set
        {
            _scale = value;
        }
    }

    public float Exponent
    {
        get
        {
            return _exponent;
        }
        set
        {
            _exponent = value;
        }
    }

    public float InflectionPoint
    {
        get
        {
            return _inflectionPoint;
        }
        set
        {
            _inflectionPoint = value;
        }
    }

    public float StartPoint
    {
        get
        {
            return _startPoint;
        }
        set
        {
            _startPoint = value;
        }
    }

    public float EndPoint
    {
        get
        {
            return _endPoint;
        }
        set
        {
            _endPoint = value;
        }
    }
}

[Serializable]
public class HeadViewAutoCenterSettingsForInspector
{
    [SerializeField]
    private bool _isEnabled;
    [SerializeField]
    private float _normalizeFasterGazeDeadZoneNormalized;
    [SerializeField]
    private float _extendedViewAngleFasterDeadZoneDegrees;
    [SerializeField]
    private float _maxDistanceFromMasterCm;
    [SerializeField]
    private float _maxAngularDistanceDegrees;
    [SerializeField]
    private float _fasterNormalizationFactor;
    [SerializeField]
    private float _positionCompensationSpeed;
    [SerializeField]
    private float _rotationCompensationSpeed;

    public HeadViewAutoCenterSettingsForInspector(HeadViewAutoCenterSettings source)
    {
        _isEnabled = source.IsEnabled;
        _normalizeFasterGazeDeadZoneNormalized = source.NormalizeFasterGazeDeadZoneNormalized;
        _extendedViewAngleFasterDeadZoneDegrees = source.ExtendedViewAngleFasterDeadZoneDegrees;
        _maxDistanceFromMasterCm = source.MaxDistanceFromMasterCm;
        _maxAngularDistanceDegrees = source.MaxAngularDistanceDegrees;
        _fasterNormalizationFactor = source.FasterNormalizationFactor;
        _positionCompensationSpeed = source.PositionCompensationSpeed;
        _rotationCompensationSpeed = source.RotationCompensationSpeed;
    }

    public HeadViewAutoCenterSettings ToHeadViewAutoCenterSettings()
    {
        var result = new HeadViewAutoCenterSettings
        {
            IsEnabled = _isEnabled,
            NormalizeFasterGazeDeadZoneNormalized = _normalizeFasterGazeDeadZoneNormalized,
            ExtendedViewAngleFasterDeadZoneDegrees = _extendedViewAngleFasterDeadZoneDegrees,
            MaxDistanceFromMasterCm = _maxDistanceFromMasterCm,
            MaxAngularDistanceDegrees = _maxAngularDistanceDegrees,
            FasterNormalizationFactor = _fasterNormalizationFactor,
            PositionCompensationSpeed = _positionCompensationSpeed,
            RotationCompensationSpeed = _rotationCompensationSpeed
        };

        return result;
    }

    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            _isEnabled = value;
        }
    }

    public float NormalizeFasterGazeDeadZoneNormalized
    {
        get
        {
            return _normalizeFasterGazeDeadZoneNormalized;
        }
        set
        {
            _normalizeFasterGazeDeadZoneNormalized = value;
        }
    }

    public float ExtendedViewAngleFasterDeadZoneDegrees
    {
        get
        {
            return _extendedViewAngleFasterDeadZoneDegrees;
        }
        set
        {
            _extendedViewAngleFasterDeadZoneDegrees = value;
        }
    }

    public float MaxDistanceFromMasterCm
    {
        get
        {
            return _maxDistanceFromMasterCm;
        }
        set
        {
            _maxDistanceFromMasterCm = value;
        }
    }

    public float MaxAngularDistanceDegrees
    {
        get
        {
            return _maxAngularDistanceDegrees;
        }
        set
        {
            _maxAngularDistanceDegrees = value;
        }
    }

    public float FasterNormalizationFactor
    {
        get
        {
            return _fasterNormalizationFactor;
        }
        set
        {
            _fasterNormalizationFactor = value;
        }
    }

    public float PositionCompensationSpeed
    {
        get
        {
            return _positionCompensationSpeed;
        }
        set
        {
            _positionCompensationSpeed = value;
        }
    }

    public float RotationCompensationSpeed
    {
        get
        {
            return _rotationCompensationSpeed;
        }
        set
        {
            _rotationCompensationSpeed = value;
        }
    }
}
