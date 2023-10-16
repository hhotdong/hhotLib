// Original source code : 2016 Spyblood Games(DayAndNightControl.cs)

using UnityEngine;
using System.Collections;

[System.Serializable]
public struct DayColor
{
    public Color skyColor;
    public Color equatorColor;
    public Color horizonColor;
    public Color fogColor;
}

public class DayNightManager : MonoBehaviour
{
    public static Light MainDirLight { get; private set; }

    [SerializeField] private bool m_IsStartDay;
    [SerializeField] private float m_StartingTime = 0.1F;
    [SerializeField] private bool m_IsTransitioning;
    [SerializeField] private float m_CurrentTime = 0;
    [SerializeField] private DayColor m_NightColors;
    [SerializeField] private DayColor m_DawnColors;
    [SerializeField] private DayColor m_DayColors;

    private float m_DirectionalLightIntensity_Init;
    private readonly float NIGHT_TIME = 0.1F;
    private readonly float DAWN_TIME = 0.3F;
    private readonly float DAY_TIME = 0.5F;
    private readonly float TRANSITION_SPEED = 0.25F;
    private readonly float INTENSITY_MULTIPLIER_OFFSET = 1.0F / 0.02F;

    private void Awake()
    {
        MainDirLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    public void Start()
    {
        if (m_IsStartDay)
        {
            m_CurrentTime = m_StartingTime;
        }
        
        m_IsTransitioning = false;
        m_DirectionalLightIntensity_Init = MainDirLight.intensity;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        TimeOfDayTransition(true);
    }

    public void TimeOfDayTransition(bool shouldBrighten)
    {
        if (m_IsTransitioning)
        {
            Debug.Log("TimeOfDay is already transitioning.");
            return;
        }
        else
        {
            if ((shouldBrighten == true && m_CurrentTime > DAY_TIME) || (shouldBrighten == false && m_CurrentTime < NIGHT_TIME))
            {
                Debug.Log("CurrentTime can't transition because it's already in that state.");
                return;
            }

            m_IsTransitioning = true;
            StartCoroutine(TimeOfDayTransitionProcess(shouldBrighten));
        }
    }

    private IEnumerator TimeOfDayTransitionProcess(bool shouldBrighten)
    {
        Transform tempLightTr = MainDirLight.transform;
        Color currSkyColor;
        Color currEquatorColor;
        Color currHorizonColor;
        Color currFogColor;
        float intensityMultiplier = 1.0F;
        float currLerp = 0.0F;

        while (true)
        {
            m_CurrentTime = shouldBrighten ? m_CurrentTime + Time.deltaTime * TRANSITION_SPEED : m_CurrentTime - Time.deltaTime * TRANSITION_SPEED;

            if (m_CurrentTime <= NIGHT_TIME)
            {
                intensityMultiplier = 0.0F;
                currSkyColor = m_NightColors.skyColor;
                currEquatorColor = m_NightColors.equatorColor;
                currHorizonColor = m_NightColors.horizonColor;
                currFogColor = m_NightColors.fogColor;
            }
            else if (m_CurrentTime <= DAWN_TIME)
            {
                intensityMultiplier = Mathf.Clamp01((m_CurrentTime - DAWN_TIME) * INTENSITY_MULTIPLIER_OFFSET);
                currLerp = Mathf.InverseLerp(NIGHT_TIME, DAWN_TIME, m_CurrentTime);
                currSkyColor = Color.Lerp(m_NightColors.skyColor, m_DawnColors.skyColor, currLerp);
                currEquatorColor = Color.Lerp(m_NightColors.equatorColor, m_DawnColors.equatorColor, currLerp);
                currHorizonColor = Color.Lerp(m_NightColors.horizonColor, m_DawnColors.horizonColor, currLerp);
                currFogColor = Color.Lerp(m_NightColors.fogColor, m_DawnColors.fogColor, currLerp);
            }
            else if (m_CurrentTime <= DAY_TIME)
            {
                intensityMultiplier = Mathf.Clamp01(1.0F - ((m_CurrentTime - DAY_TIME) * INTENSITY_MULTIPLIER_OFFSET));
                currLerp = Mathf.InverseLerp(DAWN_TIME, DAY_TIME, m_CurrentTime);
                currSkyColor = Color.Lerp(m_DawnColors.skyColor, m_DayColors.skyColor, currLerp);
                currEquatorColor = Color.Lerp(m_DawnColors.equatorColor, m_DayColors.equatorColor, currLerp);
                currHorizonColor = Color.Lerp(m_DawnColors.horizonColor, m_DayColors.horizonColor, currLerp);
                currFogColor = Color.Lerp(m_DawnColors.fogColor, m_DayColors.fogColor, currLerp);
            }
            else
            {
                currSkyColor = m_DayColors.skyColor;
                currEquatorColor = m_DayColors.equatorColor;
                currHorizonColor = m_DayColors.horizonColor;
                currFogColor = m_DayColors.fogColor;
            }

            MainDirLight.intensity = m_DirectionalLightIntensity_Init * intensityMultiplier;
            //tempLightTr.localRotation = Quaternion.Euler((m_CurrentTime * 360.0F) - 90.0F, 170.0F, 0.0F);
            tempLightTr.localRotation = Quaternion.Euler(55.0F, 54.6F, 0.0F);
            RenderSettings.ambientSkyColor = currSkyColor;
            RenderSettings.ambientEquatorColor = currEquatorColor;
            RenderSettings.ambientGroundColor = currHorizonColor;
            RenderSettings.fogColor = currFogColor;

            if (shouldBrighten == true && m_CurrentTime >= DAY_TIME)
            {
                break;
            }
            else if (shouldBrighten == false && m_CurrentTime <= NIGHT_TIME)
            {
                break;
            }

            yield return null;
        }

        m_IsTransitioning = false;
    }
}
