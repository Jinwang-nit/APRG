using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : SingletonMono<PostProcessManager>
{
    public PostProcessVolume PostProcessVolume;
    private ChromaticAberration ChromaticAberration;
    private float value;
    [SerializeField]private float speed;
    private void Start()
    {
        ChromaticAberration = PostProcessVolume.profile.GetSetting<ChromaticAberration>();
    }

    public void ChromaticAberrationEF(float value)
    {
        StopAllCoroutines(); // ��ֹ��δ���
        this.value = value;
        StartCoroutine(StartChromaticAberrationEF());
    }

    IEnumerator StartChromaticAberrationEF()
    {
        // ������value
        while (ChromaticAberration.intensity < value)
        {
            yield return null;

            ChromaticAberration.intensity.value += Time.deltaTime * speed; 
        }

        // �ݼ���value
        while (ChromaticAberration.intensity > 0)
        {
            yield return null;

            ChromaticAberration.intensity.value -= Time.deltaTime * speed;
        }
    }
}
