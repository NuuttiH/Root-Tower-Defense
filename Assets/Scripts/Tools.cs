using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static Treasure GetClosestFromSet(Vector3 position, HashSet<Treasure> set)
    {
        float distance = 1000000;
        Treasure result = null;
        foreach (Treasure t in set){
            float newDistance = Vector3.Distance(position, t.gameObject.transform.position);
            if(newDistance < distance)
            {
                distance = newDistance;
                result = t;
            } 
        }
        return result;
    }

    public static Mole GetClosestFromSet(Vector3 position, HashSet<Mole> set)
    {
        float distance = 1000000;
        Mole result = null;
        foreach (Mole m in set){
            float newDistance = Vector3.Distance(position, m.gameObject.transform.position);
            if(newDistance < distance)
            {
                distance = newDistance;
                result = m;
            } 
        }
        return result;
    }

    public static void PlayAudio(GameObject parent, AudioEvent audio)
    {
        var audioPlayer = new GameObject("AudioPlayer", typeof (AudioSource)).GetComponent<AudioSource>();
        audioPlayer.transform.position = parent.transform.position;
        audio.Play(audioPlayer);
        UnityEngine.Object.Destroy(audioPlayer.gameObject, audioPlayer.clip.length*audioPlayer.pitch);
    }
}
