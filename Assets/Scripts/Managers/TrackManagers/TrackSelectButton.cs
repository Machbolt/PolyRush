using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelectButton : MonoBehaviour
{
    public void SelectTrack(int trackIndex)
    {
        TrackSelection.selectedTrackIndex = trackIndex;
        SceneManager.LoadScene("RaceScene");
    }
}
