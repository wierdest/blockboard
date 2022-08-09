using UnityEngine;
using UnityEngine.UI;

public class SoundPlayer : MonoBehaviour
{
    // plays audio sources in a sequence.
    public AudioSource[] tracks; // there are

    // plays audio sources as single shots;
    public AudioSource[] singleShots;
    [SerializeField] private AudioSource currentTrack;
    [SerializeField] private int playCount;
    [SerializeField] private float volume;
    [SerializeField] private Image buttonImage; // sound button image;
    [SerializeField] private Sprite buttonOn, buttonOff;
    private int numberOfTracks;
    private bool hasUpdatedTrack;
    private bool isOn;

    private void Awake()
    {
        tracks = GetComponentsInChildren<AudioSource>();
        numberOfTracks = tracks.Length;
        isOn = true;
        setSoundTrackVolume(volume);
    }

    private void Update()
    {
        if(!isOn)
        {
            return;
        }

        // here's the deal with this player:
        // we let each track finish to change to the next one.
        //  playCount is the currentTrack index in AudioSource[] tracks.
        // if we turn off sound, we currentTrack.stop, 
        // causing it to update next time we turn it on

        if(!currentTrack.isPlaying && !hasUpdatedTrack)
        {
            playCount++;
            if(playCount >= numberOfTracks)
            {
                playCount = 0;
            }
            currentTrack = tracks[playCount];
            hasUpdatedTrack = true;
            currentTrack.Play();

            Debug.LogFormat("SoundPlayer: PlayCount {0} Now playin'  {1}!", playCount, currentTrack.clip.name);

        }
        if(hasUpdatedTrack && currentTrack.isPlaying)
        {
            hasUpdatedTrack = false;
        }
    }
    private void setSoundTrackVolume(float value)
    {
        volume = value;
        foreach(AudioSource track in tracks)
        {
            track.volume = volume;
        }
    }

    public void OnClickSoundButton()
    {
        isOn = !isOn;
        buttonImage.sprite = isOn ? buttonOn : buttonOff;
        if(!isOn)
        {
            currentTrack.Stop();
        }
    }

    public void PlayNextSingleShot()
    {

    }

}
