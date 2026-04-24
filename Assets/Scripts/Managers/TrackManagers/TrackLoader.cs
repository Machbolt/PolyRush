using UnityEngine;

public class TrackLoader : MonoBehaviour
{
    public TrackRoot[] tracks;

    public static TrackRoot activeTrack;

    void Awake()
    {
        int index = TrackSelection.selectedTrackIndex;

        for (int i = 0; i < tracks.Length; i++)
            tracks[i].gameObject.SetActive(i == index);

        activeTrack = tracks[index];
    }
}
