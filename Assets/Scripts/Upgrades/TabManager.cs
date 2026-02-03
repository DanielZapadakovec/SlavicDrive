using UnityEngine;

public class TabManager : MonoBehaviour
{
    public TabButton[] tabs;
    public AudioSource municipalUISource;
    public AudioClip buttonClick;
    public void OpenTab(int index)
    {
        if (!municipalUISource.isPlaying)
        {
            municipalUISource.PlayOneShot(buttonClick);
        }
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == index)
                tabs[i].Select();
            else
                tabs[i].Deselect();
        }
    }
}
