using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndgameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpro;
    [SerializeField] private Button _button;


    void Start()
    {
        int minutes = 0;
        float seconds = Time.time - GameManager.StartTime;
        while(seconds > 60f)
        {
            seconds -= 60f;
            minutes ++;
        }
        _tmpro.text = "You survived for " + minutes.ToString("0") + ":" + seconds.ToString("00") 
                + " with a score of " + GameManager.Score;
        _button.onClick.AddListener( delegate{ SceneLoadingManager.LoadLevel("GameScene"); } );
    }
}
