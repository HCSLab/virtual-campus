using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.GameIntegration.Net;

public class ExtendedView : ExtendedViewBase {

    public ExtendedViewSettingsForInspector HeadOnlySettings = new ExtendedViewSettingsForInspector(new ExtendedViewSettings());

    public ExtendedViewSettingsForInspector GazeOnlySettings = new ExtendedViewSettingsForInspector(new ExtendedViewSettings());

    public ExtendedViewSettingsForInspector HeadAndGazeSettings = new ExtendedViewSettingsForInspector(new ExtendedViewSettings());

    public ExtendedViewSettingsUIEventsManager HeadOnlySettingsUiEventsManager;
    public ExtendedViewSettingsUIEventsManager GazeOnlySettingsUiEventsManager;
    public ExtendedViewSettingsUIEventsManager HeadAndGazeSettingsUiEventsManager;

    private ExtendedViewSettingsForInspector _lastHeadOnlySettings;

    private ExtendedViewSettingsForInspector _lastGazeOnlySettings;

    private ExtendedViewSettingsForInspector _lastHeadAndGazeSettings;

    protected override void UpdateAllChangedExtendedViewSettings()
    {
        if (!ExtendedViewSettingsForInspector.ShouldDeepEqual(GazeOnlySettings, _lastGazeOnlySettings))
        {
            TobiiGameIntegrationApi.UpdateExtendedViewGazeOnlySettings(GazeOnlySettings.ToExtendedViewSettings());
            _lastGazeOnlySettings = new ExtendedViewSettingsForInspector(GazeOnlySettings.ToExtendedViewSettings());
            if (GazeOnlySettingsUiEventsManager != null)
            {
                GazeOnlySettingsUiEventsManager.FillUiWithCurrentSettings();
            }
        }

        if (!ExtendedViewSettingsForInspector.ShouldDeepEqual(HeadOnlySettings, _lastHeadOnlySettings))
        {
            TobiiGameIntegrationApi.UpdateExtendedViewHeadOnlySettings(HeadOnlySettings.ToExtendedViewSettings());
            _lastHeadOnlySettings = new ExtendedViewSettingsForInspector(HeadOnlySettings.ToExtendedViewSettings());
            if (HeadOnlySettingsUiEventsManager != null)
            {
                HeadOnlySettingsUiEventsManager.FillUiWithCurrentSettings();
            }
        }

        if (!ExtendedViewSettingsForInspector.ShouldDeepEqual(HeadAndGazeSettings, _lastHeadAndGazeSettings))
        {
            TobiiGameIntegrationApi.UpdateExtendedViewSettings(HeadAndGazeSettings.ToExtendedViewSettings());
            _lastHeadAndGazeSettings = new ExtendedViewSettingsForInspector(HeadAndGazeSettings.ToExtendedViewSettings());
            if (HeadAndGazeSettingsUiEventsManager != null)
            {
                HeadAndGazeSettingsUiEventsManager.FillUiWithCurrentSettings();
            }
        }
    }

    public void UpdateAllExtendedViewSettings()
    {
        TobiiGameIntegrationApi.UpdateExtendedViewGazeOnlySettings(GazeOnlySettings.ToExtendedViewSettings());
        TobiiGameIntegrationApi.UpdateExtendedViewHeadOnlySettings(HeadOnlySettings.ToExtendedViewSettings());
        TobiiGameIntegrationApi.UpdateExtendedViewSettings(HeadAndGazeSettings.ToExtendedViewSettings());
    }
}
