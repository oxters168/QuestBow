using UnityEngine;

public class OptionsPanelController : MonoBehaviour
{
    public static OptionsPanelController optionsPanelInScene { get; private set; }
    public GameObject showTrajectoryOption, showOrientationBoxOption, debugBowmanValuesOption, debugControllerValuesOption, hudDistanceOption, practiceTargetDistanceOption, practiceTargetHeightOption, randomTargetsIntervalOption;

    private void Awake()
    {
        optionsPanelInScene = this;
    }
    private void OnEnable()
    {
        ShowGeneralOptions(true);
    }
    private void OnDisable()
    {
        ShowGeneralOptions(false);
        ShowDebugOptions(false);
        ShowTargetsPracticeOption(false);
        ShowTargetsBlitzOptions(false);
    }

    public static void ShowGeneralOptions(bool onOff)
    {
        optionsPanelInScene.showTrajectoryOption.SetActive(onOff);
        optionsPanelInScene.showOrientationBoxOption.SetActive(onOff);
        optionsPanelInScene.hudDistanceOption.SetActive(onOff);
    }
    public static void ShowDebugOptions(bool onOff)
    {
        optionsPanelInScene.debugBowmanValuesOption.SetActive(onOff);
        optionsPanelInScene.debugControllerValuesOption.SetActive(onOff);
    }
    public static void ShowTargetsPracticeOption(bool onOff)
    {
        optionsPanelInScene.practiceTargetDistanceOption.SetActive(onOff);
        optionsPanelInScene.practiceTargetHeightOption.SetActive(onOff);
    }
    public static void ShowTargetsBlitzOptions(bool onOff)
    {
        optionsPanelInScene.randomTargetsIntervalOption.SetActive(onOff);
    }
}
